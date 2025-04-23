using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Search;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;
using ObjectField = UnityEditor.UIElements.ObjectField;

public class DialogueGraphView : GraphView
    {
        public readonly Vector2 DefaultNodeSize = new Vector2(200, 150);
        public readonly Vector2 DefaultCommentBlockSize = new Vector2(300, 200);
        public DialogueNode EntryPointNode;
        public Blackboard Blackboard = new Blackboard();
        public List<ExposedProperty> ExposedProperties { get; private set; } = new List<ExposedProperty>();
        private NodeSearchWindow _searchWindow;

        public DialogueGraphView(DialogueGraph editorWindow)
        {
            styleSheets.Add(Resources.Load<StyleSheet>("DialogueGraph"));
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new FreehandSelector());

            var grid = new GridBackground();
            Insert(0, grid);
            grid.StretchToParentSize();

            AddElement(GetEntryPointNodeInstance());

            AddSearchWindow(editorWindow);
        }


        private void AddSearchWindow(DialogueGraph editorWindow)
        {
            _searchWindow = ScriptableObject.CreateInstance<NodeSearchWindow>();
            _searchWindow.Configure(editorWindow, this);
            nodeCreationRequest = context =>
                SearchWindow.Open(new SearchWindowContext(context.screenMousePosition), _searchWindow);
        }


        public void ClearBlackBoardAndExposedProperties()
        {
            ExposedProperties.Clear();
            Blackboard.Clear();
        }

        public Group CreateCommentBlock(Rect rect, CommentBlockData commentBlockData = null)
        {
            if(commentBlockData==null)
                commentBlockData = new CommentBlockData();
            var group = new Group
            {
                autoUpdateGeometry = true,
                title = commentBlockData.Title
            };
            AddElement(group);
            group.SetPosition(rect);
            return group;
        }

        public void AddPropertyToBlackBoard(ExposedProperty property, bool loadMode = false)
        {
            var localPropertyName = property.PropertyName;
            var localPropertyValue = property.PropertyValue;
            if (!loadMode)
            {
                while (ExposedProperties.Any(x => x.PropertyName == localPropertyName))
                    localPropertyName = $"{localPropertyName}(1)";
            }

            var item = ExposedProperty.CreateInstance();
            item.PropertyName = localPropertyName;
            item.PropertyValue = localPropertyValue;
            ExposedProperties.Add(item);

            var container = new VisualElement();
            var field = new BlackboardField {text = localPropertyName, typeText = "string"};
            container.Add(field);

            var propertyValueTextField = new TextField("Value:")
            {
                value = localPropertyValue
            };
            propertyValueTextField.RegisterValueChangedCallback(evt =>
            {
                var index = ExposedProperties.FindIndex(x => x.PropertyName == item.PropertyName);
                ExposedProperties[index].PropertyValue = evt.newValue;
            });
            var sa = new BlackboardRow(field, propertyValueTextField);
            container.Add(sa);
            Blackboard.Add(container);
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            var compatiblePorts = new List<Port>();
            var startPortView = startPort;

            ports.ForEach((port) =>
            {
                var portView = port;
                if (startPortView != portView && startPortView.node != portView.node)
                    compatiblePorts.Add(port);
            });

            return compatiblePorts;
        }
        
        private DialogueNode GetEntryPointNodeInstance()
        {
            var nodeCache = new DialogueNode()
            {
                title = "START",
                GUID = Guid.NewGuid().ToString(),
                DialogueText = "ENTRYPOINT",
                EntryPoint = true,
            };

            var generatedPort = GetPortInstance(nodeCache, Direction.Output);
            generatedPort.portName = "StartPointNode";
            nodeCache.outputContainer.Add(generatedPort);

            nodeCache.capabilities &= ~Capabilities.Movable;
            nodeCache.capabilities &= ~Capabilities.Deletable;

            nodeCache.RefreshExpandedState();
            nodeCache.RefreshPorts();
            nodeCache.SetPosition(new Rect(100, 200, 100, 150));
            return nodeCache;
        }
        
        public void CreateNewDialogueNode(string nodeName, Vector2 position, int statModifier, Sprite characterMoodSprite)
        {
            AddElement(CreateNode(nodeName, position, statModifier, characterMoodSprite));
        }

        public DialogueNode CreateNode(string nodeName, Vector2 position, int statModifier, Sprite characterMoodSprite)
        {
            var tempDialogueNode = new DialogueNode()
            {
                title = nodeName,
                DialogueText = nodeName,
                GUID = Guid.NewGuid().ToString(),
                StatModifier = statModifier,
                CharacterMoodSprite = characterMoodSprite
                
            };
            
            tempDialogueNode.styleSheets.Add(Resources.Load<StyleSheet>("Node"));
            
            Port inputPort = GetPortInstance(tempDialogueNode, Direction.Input, Port.Capacity.Multi);
            inputPort.portName = "Input";
            tempDialogueNode.inputContainer.Add(inputPort);
            
            // IntegerField pour StatsModifier
            IntegerField statsField = new IntegerField("Stat Modifier")
            {
                value = tempDialogueNode.StatModifier 
            };
            statsField.RegisterValueChangedCallback(evt =>
            {
                tempDialogueNode.StatModifier = evt.newValue; // Met à jour StatsModifier
            });
            tempDialogueNode.inputContainer.Add(statsField); // Ajoute à l'inputContainer
            
            
            
            // Ajout d'un champ ObjectField pour CharacterMoodSprite
            var spriteField = new ObjectField("Character Mood Sprite")
            {
                objectType = typeof(Sprite),
                value = tempDialogueNode.CharacterMoodSprite,
                allowSceneObjects = false
            };
            
            spriteField.RegisterValueChangedCallback(evt =>
            {
                tempDialogueNode.CharacterMoodSprite = (Sprite)evt.newValue; 
            });

            tempDialogueNode.inputContainer.Add(spriteField);
            
            tempDialogueNode.RefreshExpandedState();
            tempDialogueNode.RefreshPorts();
            tempDialogueNode.SetPosition(new Rect(position,
                DefaultNodeSize)); //To-Do: implement screen center instantiation positioning

            var textField = new TextField("");
            textField.RegisterValueChangedCallback(evt =>
            {
                tempDialogueNode.DialogueText = evt.newValue;
                tempDialogueNode.title = evt.newValue;
            });
            textField.SetValueWithoutNotify(tempDialogueNode.title);
            tempDialogueNode.mainContainer.Add(textField);

            var buttonNeutral = new Button(() => { AddNullPort(tempDialogueNode); })
            {
                text = "Add Null",
            };
            tempDialogueNode.titleButtonContainer.Add(buttonNeutral);
            
            var button = new Button(() => { AddChoicePort(tempDialogueNode); })
            {
                text = "Add Choice"
            };
            tempDialogueNode.titleButtonContainer.Add(button);
            return tempDialogueNode;
        }

        public void AddNullPort(DialogueNode dialogueNode)
        {
            // Crée un port de sortie sans fonctionnalité spécifique
            var nullPort = GetPortInstance(dialogueNode, Direction.Output);

            // Donne un nom par défaut au port
            nullPort.portName = "Null Port";
            nullPort.portColor = Color.red;

            // Crée un bouton "X" pour supprimer le port
            var deleteButton = new Button(() => RemoveNullPort(dialogueNode, nullPort))
            {
                text = "X"
            };

            // Ajoute le bouton "X" au conteneur du port
            nullPort.contentContainer.Add(deleteButton);

            // Ajoute le port au conteneur de sortie du noeud
            dialogueNode.outputContainer.Add(nullPort);

            // Rafraîchit l'affichage du noeud pour inclure le nouveau port
            dialogueNode.RefreshPorts();
            dialogueNode.RefreshExpandedState();
        }

        public void RemoveNullPort(DialogueNode dialogueNode, Port nullPort)
        {
            // Vérifie et déconnecte les liens associés au port s'ils existent
            var targetEdge = edges.ToList()
                .Where(x => x.output == nullPort);
            if (targetEdge.Any())
            {
                foreach (var edge in targetEdge)
                {
                    edge.input.Disconnect(edge);
                    edge.output.Disconnect(edge);
                    RemoveElement(edge);
                }
            }

            // Retire le port du conteneur de sortie du noeud
            dialogueNode.outputContainer.Remove(nullPort);

            // Rafraîchit l'affichage du noeud pour refléter les modifications
            dialogueNode.RefreshPorts();
            dialogueNode.RefreshExpandedState();
        }

        public void AddChoicePort(DialogueNode dialogueNode, string overriddenPortName = "")
        {
            var generatedPort = GetPortInstance(dialogueNode, Direction.Output);
            
            var portLabel = generatedPort.contentContainer.Q<Label>("type");
            generatedPort.Q<Label>("type").style.display = DisplayStyle.None; 

            var outputPortCount = dialogueNode.outputContainer.Query("connector").ToList().Count();
            
            var choicePortName = string.IsNullOrEmpty(overriddenPortName)
                ? $"Option {outputPortCount + 1}"
                : overriddenPortName;


            var textField = new TextField()
            {
                name = string.Empty,
                value = choicePortName
            };
            textField.RegisterValueChangedCallback(evt =>
            {
                if (evt.newValue != "StartPointNode")
                {
                    generatedPort.portName = evt.newValue;
                }
                else
                {
                    EditorUtility.DisplayDialog("New Port Name not valid", "Please choose a port name that is not 'StartPointNode'.", "OK");
                    
                    textField.value = evt.previousValue;
                    generatedPort.portName = evt.previousValue;
                    // Bug mineur d'affichage à corriger //
                   
                }
                
            });
            generatedPort.contentContainer.Add(new Label("  "));
            generatedPort.contentContainer.Add(textField);
            var deleteButton = new Button(() => RemovePort(dialogueNode, generatedPort))
            {
                text = "X"
            };
            generatedPort.contentContainer.Add(deleteButton);
            generatedPort.portName = choicePortName;
            dialogueNode.outputContainer.Add(generatedPort);
            dialogueNode.RefreshPorts();
            dialogueNode.RefreshExpandedState();
        }

        private void RemovePort(Node node, Port socket)
        {
            var targetEdge = edges.ToList()
                .Where(x => x.output.portName == socket.portName && x.output.node == socket.node);
            if (targetEdge.Any())
            {
                var edge = targetEdge.First();
                edge.input.Disconnect(edge);
                RemoveElement(targetEdge.First());
            }

            node.outputContainer.Remove(socket);
            node.RefreshPorts();
            node.RefreshExpandedState();
        }

        private Port GetPortInstance(DialogueNode node, Direction nodeDirection,
            Port.Capacity capacity = Port.Capacity.Single)
        {
            return node.InstantiatePort(Orientation.Horizontal, nodeDirection, capacity, typeof(float));
        }
    }
