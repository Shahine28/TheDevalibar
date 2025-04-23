using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

public class DialogueGraph : EditorWindow
{
        private string _fileName = "New Narrative";
        private string _filePath = "Assets/_NarrativeProject/Prog/Scripts/DialogueGraph/Resources";
        private TextField _filePathTextField;

        private DialogueGraphView _graphView;
        private DialogueContainer _dialogueContainer;

        [MenuItem("Graph/Narrative Graph")]
        public static void CreateGraphViewWindow()
        {
            var window = GetWindow<DialogueGraph>();
            window.titleContent = new GUIContent("Narrative Graph");
        }

        private void ConstructGraphView()
        {
            _graphView = new DialogueGraphView(this)
            {
                name = "Narrative Graph",
            };
            _graphView.StretchToParentSize();
            rootVisualElement.Add(_graphView);
        }

        private void GenerateToolbar()
        {
            Toolbar toolbar = new Toolbar();

            toolbar.Add(new Button(ChooseFilePath)
            {
                text = "Choose File Path",
                tooltip = $"Choose File Path where to save or load {_fileName}\n" +
                          $"The Current file path is:\n{_filePath}"
            });
            
            _filePathTextField = new TextField("Selected Folder:")
            {
                isReadOnly = true,
                tooltip = $"Folder path: {_filePath}"
            };
            _filePathTextField.SetValueWithoutNotify( new DirectoryInfo(_filePath).Name);
            toolbar.Add(_filePathTextField);
            
            
            TextField fileNameTextField = new TextField("File Name:");
            fileNameTextField.SetValueWithoutNotify(_fileName);
            fileNameTextField.MarkDirtyRepaint();
            fileNameTextField.RegisterValueChangedCallback(evt => _fileName = evt.newValue);
            toolbar.Add(fileNameTextField);

            toolbar.Add(new Button(() => RequestDataOperation(true)) {text = "Save Data"});

            toolbar.Add(new Button(() => RequestDataOperation(false)) {text = "Load Data"});
            // toolbar.Add(new Button(() => _graphView.CreateNewDialogueNode("Dialogue Node")) {text = "New Node",});
            rootVisualElement.Add(toolbar);
        }
        
        private void ChooseFilePath()
        {
            string path = EditorUtility.OpenFolderPanel("Choose Folder", _filePath, "");
            if (!string.IsNullOrEmpty(path))
            {
                string projectPath = Application.dataPath; // 🔹 Chemin absolu du dossier "Assets"
                projectPath = projectPath.Replace("\\", "/"); // 🔹 Normalise les slashs
                path = path.Replace("\\", "/"); // 🔹 Normalise les slashs

                // Debug.Log($" Absolute Path Selected: {path}");
                // Debug.Log($" Project Path: {projectPath}");

                string lastFolder = new DirectoryInfo(projectPath).Name; // 🔹 Récupère le dernier dossier (ex: "Assets")

                if (path.StartsWith(projectPath))
                {
                    path = path.Substring(projectPath.Length).TrimStart('/'); // 🔹 Supprime la partie avant "Assets"
                }

                _filePath = $"{lastFolder}/{path}".TrimEnd('/'); // 🔹 Construit le chemin final
                _filePathTextField.SetValueWithoutNotify( new DirectoryInfo(_filePath).Name);
                Debug.Log($"Selected Path: {_filePath}");
            }
        }



        private void RequestDataOperation(bool save)
        {
            if (!string.IsNullOrEmpty(_fileName))
            {
                var saveUtility = GraphSaveUtility.GetInstance(_graphView);
                if (save)
                    saveUtility.SaveGraph(_fileName, _filePath);
                else
                    saveUtility.LoadGraph(_fileName, _filePath);
            }
            else
            {
                EditorUtility.DisplayDialog("Invalid File name", "Please Enter a valid filename", "OK");
            }
        }

        private void OnEnable()
        {
            ConstructGraphView();
            GenerateToolbar();
            GenerateMiniMap();
            // GenerateBlackBoard(); // Pas besoin du blackboard
        }

        private void GenerateMiniMap()
        {
            var miniMap = new MiniMap {anchored = true};
            var cords = _graphView.contentViewContainer.WorldToLocal(new Vector2(this.maxSize.x - 10, 30));
            miniMap.SetPosition(new Rect(cords.x, cords.y, 200, 140));
            _graphView.Add(miniMap);
        }
        
        private void OnGUI()
        {
            if (Event.current.rawType == EventType.Layout)
            {
                MiniMap _miniMap = _graphView.contentContainer.Q<MiniMap>();
                _miniMap.SetPosition(new Rect(this.position.width - 210, 30, 200, 140));
            }
        }

        private void GenerateBlackBoard()
        {
            var blackboard = new Blackboard(_graphView);
            blackboard.Add(new BlackboardSection {title = "Exposed Variables"});
            blackboard.addItemRequested = _blackboard =>
            {
                _graphView.AddPropertyToBlackBoard(ExposedProperty.CreateInstance(), false);
            };
            blackboard.editTextRequested = (_blackboard, element, newValue) =>
            {
                var oldPropertyName = ((BlackboardField) element).text;
                if (_graphView.ExposedProperties.Any(x => x.PropertyName == newValue))
                {
                    EditorUtility.DisplayDialog("Error", "This property name already exists, please chose another one.",
                        "OK");
                    return;
                }

                var targetIndex = _graphView.ExposedProperties.FindIndex(x => x.PropertyName == oldPropertyName);
                _graphView.ExposedProperties[targetIndex].PropertyName = newValue;
                ((BlackboardField) element).text = newValue;
            };
            blackboard.SetPosition(new Rect(10,30,200,300));
            _graphView.Add(blackboard);
            _graphView.Blackboard = blackboard;
        }

        private void OnDisable()
        {
            rootVisualElement.Remove(_graphView);
        }
}
