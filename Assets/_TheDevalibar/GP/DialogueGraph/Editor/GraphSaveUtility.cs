using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Unity.Collections;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

public class GraphSaveUtility
    {
        private DialogueGraphView _targetGraphView;
        private DialogueContainer _containerCache;
        private List<Edge> Edges => _targetGraphView.edges.ToList();
        private List<DialogueNode> Nodes => _targetGraphView.nodes.ToList().Cast<DialogueNode>().ToList();

        private List<Group> CommentBlocks =>
            _targetGraphView.graphElements.ToList().Where(x => x is Group).Cast<Group>().ToList();
        

        public static GraphSaveUtility GetInstance(DialogueGraphView graphView)
        {
            return new GraphSaveUtility
            {
                _targetGraphView = graphView
            };
        }

        private void ValidateAndCreateFilePath(string filePath)
        {
            // Vérifie si le dossier existe
            if (!AssetDatabase.IsValidFolder(filePath))
            {
                Debug.LogWarning($"Le dossier {filePath} n'existe pas. Création en cours...");

                // Sépare le chemin en parties
                string[] folders = filePath.Split('/');
                string currentPath = "";

                // Reconstruit le chemin et crée les dossiers s'ils n'existent pas
                for (int i = 0; i < folders.Length; i++)
                {
                    string folder = folders[i];
                    if (i == 0)
                    {
                        currentPath = folder; 
                    }
                    else
                    {
                        string newPath = $"{currentPath}/{folder}";
                        if (!AssetDatabase.IsValidFolder(newPath))
                        {
                            AssetDatabase.CreateFolder(currentPath, folder);
                        }
                        currentPath = newPath;
                    }
                }

                Debug.Log($"Dossier {filePath} créé avec succès !");
            }
            
            string parentDirectory = Path.GetDirectoryName(filePath);
            string folderName = Path.GetFileName(filePath); 

            Debug.Log($"Chemin parent : {parentDirectory}");
            Debug.Log($"Nom du dossier cible : {folderName}");
        }
        
        public void SaveGraph(string fileName, string filePath)
        {
            var dialogueContainerObject = ScriptableObject.CreateInstance<DialogueContainer>();
            if (!SaveNodes(fileName, dialogueContainerObject)) return;
            SaveExposedProperties(dialogueContainerObject);
            SaveCommentBlocks(dialogueContainerObject);

            ValidateAndCreateFilePath(filePath);

            UnityEngine.Object loadedAsset = AssetDatabase.LoadAssetAtPath($"{filePath}/{fileName}.asset", typeof(DialogueContainer));

            if (loadedAsset == null || !AssetDatabase.Contains(loadedAsset)) 
			{
                AssetDatabase.CreateAsset(dialogueContainerObject, $"{filePath}/{fileName}.asset");
            }
            else 
			{
                DialogueContainer container = loadedAsset as DialogueContainer;
                container.NodeLinks = dialogueContainerObject.NodeLinks;
                container.DialogueNodeData = dialogueContainerObject.DialogueNodeData;
                container.ExposedProperties = dialogueContainerObject.ExposedProperties;
                container.CommentBlockData = dialogueContainerObject.CommentBlockData;
                EditorUtility.SetDirty(container);
            }

            AssetDatabase.SaveAssets();
        }

        private bool SaveNodes(string fileName, DialogueContainer dialogueContainerObject)
        {
            if (Nodes.Where(node => !node.EntryPoint).ToList().Count == 0) return false;
            var connectedPorts = Edges.Where(x => x.input.node != null).ToArray();
            for (var i = 0; i < connectedPorts.Count(); i++)
            {
                var outputNode = (connectedPorts[i].output.node as DialogueNode);
                var inputNode = (connectedPorts[i].input.node as DialogueNode);

                if (outputNode != null && inputNode != null)
                {
                    dialogueContainerObject.NodeLinks.Add(new NodeLinkData
                    {
                        BaseNodeGuid = outputNode.GUID,
                        PortName = connectedPorts[i].output.portName,
                        TargetNodeGuid = inputNode.GUID,
                        IsNullNode =  connectedPorts[i].output.portColor == Color.red
                    });
                }
            }
            
            foreach (DialogueNode node in Nodes.Where(node => !node.EntryPoint))
            {
               

                var nodeData = new DialogueNodeData
                {
                    NodeGUID = node.GUID,
                    DialogueText = node.DialogueText,
                    Position = node.GetPosition().position,
                    StatModifier = node.StatModifier,
                    
                    
                    Ports = node.outputContainer.Children()
                        .OfType<Port>()
                        .Select(port => new SerializablePort(port.portName, port.direction == Direction.Input ? PortDirection.Input :PortDirection.Output , port.capacity == Port.Capacity.Single ? PortCapacity.Single : PortCapacity.Multi, port.portColor == Color.red))
                        .ToList(),
                    CharacterMoodSprite = node.CharacterMoodSprite,
                };
                dialogueContainerObject.DialogueNodeData.Add(nodeData);
            }

            return true;
        }


        private void SaveExposedProperties(DialogueContainer dialogueContainer)
        {
            dialogueContainer.ExposedProperties.Clear();
            dialogueContainer.ExposedProperties.AddRange(_targetGraphView.ExposedProperties);
        }

        private void SaveCommentBlocks(DialogueContainer dialogueContainer)
        {
            foreach (var block in CommentBlocks)
            {
                var nodes = block.containedElements.Where(x => x is DialogueNode).Cast<DialogueNode>().Select(x => x.GUID)
                    .ToList();

                dialogueContainer.CommentBlockData.Add(new CommentBlockData
                {
                    ChildNodes = nodes,
                    Title = block.title,
                    Position = block.GetPosition().position
                });
            }
        }

        public void LoadGraph(string fileName, string filePath)
        {
            UnityEngine.Object loadedAsset = AssetDatabase.LoadAssetAtPath($"{filePath}/{fileName}.asset", typeof(DialogueContainer));
             _containerCache = loadedAsset as DialogueContainer;
            // _containerCache = Resources.Load<DialogueContainer>(fileName);
            if (_containerCache == null)
            {
                EditorUtility.DisplayDialog("File Not Found", "Target Narrative Data does not exist!", "OK");
                return;
            }

            ClearGraph();
            CreateNodes();
            ConnectNodes();
            AddExposedProperties();
            GenerateCommentBlocks();
        }

        /// <summary>
        /// Set Entry point GUID then Get All Nodes, remove all and their edges. Leave only the entrypoint node. (Remove its edge too)
        /// </summary>
        private void ClearGraph()
        {
            // Nodes.Find(x => x.EntryPoint).GUID =_containerCache.NodeLinks.Find(x => x.PortName == "Next")?.BaseNodeGuid;
            var entryPointNode = Nodes.Find(x => x.EntryPoint);
            if (entryPointNode != null)
            {
                // Supprime toutes les connexions sortantes du EntryPoint
                foreach (var outputPort in entryPointNode.outputContainer.Children().OfType<Port>())
                {
                    var connectedEdges = outputPort.connections.ToList();
                    foreach (var edge in connectedEdges)
                    {
                        edge.input.Disconnect(edge);
                        edge.output.Disconnect(edge);
                        _targetGraphView.RemoveElement(edge);
                    }
                }
            }
            foreach (DialogueNode  node in Nodes)
            {
                if (node.EntryPoint) continue;
                Edges.Where(x => x.input.node == node).ToList()
                    .ForEach(edge => _targetGraphView.RemoveElement(edge));
                _targetGraphView.RemoveElement(node);
            }
        }

        /// <summary>
        /// Create All serialized nodes and assign their guid and dialogue text to them
        /// </summary>
        private void CreateNodes()
        {
            foreach (DialogueNodeData nodeData in _containerCache.DialogueNodeData)
            {
                DialogueNode tempNode = _targetGraphView.CreateNode(nodeData.DialogueText, nodeData.Position, nodeData.StatModifier, nodeData.CharacterMoodSprite);
                tempNode.GUID = nodeData.NodeGUID;
                _targetGraphView.AddElement(tempNode);

                if (nodeData.Ports == null) continue;
                foreach (var nodePort in nodeData.Ports)
                {
                    if (!nodePort.IsPortNull) _targetGraphView.AddChoicePort(tempNode, nodePort.PortName);
                    else _targetGraphView.AddNullPort(tempNode);
                }
            }
        }

        private void ConnectNodes()
        {
            for (int i = 0; i < Nodes.Count; i++) // 
            {
                List<NodeLinkData> connections =
                    _containerCache.NodeLinks.Where(x=> x.BaseNodeGuid == Nodes[i].GUID || x.PortName == "StartPointNode").ToList();  // Je récupère toutes les connections du nodes mais également le startNode
                for (var j = 0; j < connections.Count; j++)
                {
                    string portName = connections[j].PortName;
                    string targetNodeGuid = connections[j].TargetNodeGuid;
                    DialogueNode targetNode = Nodes.First(x => x.GUID == targetNodeGuid);
                    
                    int PortIndex = Nodes[i].outputContainer.Children()
                        .OfType<Port>() // Filtre uniquement les Ports
                        .ToList()       // Convertit les Ports en une liste
                        .FindIndex(port => port.portName == portName); // Trouve l'index correspondant

                    if (PortIndex != -1)
                    {
                        LinkNodesTogether(Nodes[i].outputContainer[PortIndex].Q<Port>(), (Port) targetNode.inputContainer[0]);
                    }
                    
                    targetNode.SetPosition(new Rect(
                        _containerCache.DialogueNodeData.First(x => x.NodeGUID == targetNodeGuid).Position, _targetGraphView.DefaultNodeSize));
                }
            }
        }


        private void LinkNodesTogether(Port outputSocket, Port inputSocket)
        {
            var tempEdge = new Edge()
            {
                output = outputSocket,
                input = inputSocket
            };
            tempEdge?.input.Connect(tempEdge);
            tempEdge?.output.Connect(tempEdge);
            _targetGraphView.Add(tempEdge);
        }

        private void AddExposedProperties()
        {
            _targetGraphView.ClearBlackBoardAndExposedProperties();
            foreach (var exposedProperty in _containerCache.ExposedProperties)
            {
                _targetGraphView.AddPropertyToBlackBoard(exposedProperty);
            }
        }

        private void GenerateCommentBlocks()
        {
            foreach (var commentBlock in CommentBlocks)
            {
                _targetGraphView.RemoveElement(commentBlock);
            }

            foreach (var commentBlockData in _containerCache.CommentBlockData)
            {
               var block = _targetGraphView.CreateCommentBlock(new Rect(commentBlockData.Position, _targetGraphView.DefaultCommentBlockSize),
                    commentBlockData);
               block.AddElements(Nodes.Where(x=>commentBlockData.ChildNodes.Contains(x.GUID)));
            }
        }
    }