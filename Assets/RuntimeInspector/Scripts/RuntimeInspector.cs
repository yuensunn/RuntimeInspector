using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;
using System.Linq;
using RI.UI;
using UnityEngine.Events;




namespace RI
{

    [AddComponentMenu("SunPlugin/Runtime Inspector")]
    public class RuntimeInspector : MonoBehaviour
    {
        DataForm dataForm;
        CreateData createData;
        FileHierarchy fileHierarchy;
        void Awake()
        {
            string[] options = Helper.GetTypesWithHelpAttribute().Select(x => x.ToString()).ToArray();
            dataForm = new DataForm("Data Form", options[0], Rect.MinMaxRect(Screen.width * 0.05f, Screen.height * 0.05f, Screen.width * 0.75f, Screen.height * 0.95f), 0);
            createData = new CreateData(options, "Create Data", Rect.MinMaxRect(Screen.width * 0.77f, Screen.height * 0.05f, Screen.width * 0.95f, Screen.height * 0.5f), 1);
            fileHierarchy = new FileHierarchy("File Hierarchy", Rect.MinMaxRect(Screen.width * 0.77f, Screen.height * 0.52f, Screen.width * 0.95f, Screen.height * 0.95f), 2);
            fileHierarchy.SetOption(dataForm.SetField);
            createData.onCreateDataFile.AddListener((index, name) =>
            {
                DataFile.Create(System.Activator.CreateInstance(System.Type.GetType(name)));
                fileHierarchy.Refresh();

            });

            fileHierarchy.Refresh();

            if (fileHierarchy.files.Length > 0)
                fileHierarchy.selectionGrid.SetSelection(0);



        }
        void OnGUI()
        {
            dataForm.Draw();
            createData.Draw();
            fileHierarchy.Draw();
        }


    }


    public class CreateData : DRWindow
    {
        //private static Texture2D blueTexture = new Texture2D
        public ScrollView scrollView { get; private set; }
        public SelectionGrid selectionGrid { get; private set; }
        public OnCreateButton onCreateDataFile { get; private set; }
        public CreateData(string[] options, string windowName, Rect rect, int index) : base(windowName, rect, index)
        {
            scrollView = new ScrollView();
            selectionGrid = new SelectionGrid();
            onCreateDataFile = new OnCreateButton();
            selectionGrid.options = options;
        }

        protected override void WindowFunction(int id)
        {
            scrollView.Begin("Box");
            selectionGrid.Draw();
            scrollView.End();
            if (GUILayout.Button("Create"))
            {
                onCreateDataFile.Invoke(selectionGrid.selected, selectionGrid.options[selectionGrid.selected]);
            }
            base.WindowFunction(id);
        }

        public class OnCreateButton : UnityEvent<int, string> { }
    }


    public class FileHierarchy : DRWindow
    {
        public DataFile[] files { get; private set; }
        public ScrollView scrollView { get; private set; }
        public SelectionGrid selectionGrid { get; private set; }
        public FileHierarchy(string windowName, Rect rect, int index) : base(windowName, rect, index)
        {
            scrollView = new ScrollView();
            selectionGrid = new SelectionGrid();
        }

        protected override void WindowFunction(int id)
        {
            GUILayout.BeginHorizontal("Box");
            if (GUILayout.Button("↻", GUILayout.MaxWidth(25f))) Refresh();
            GUILayout.EndHorizontal();
            scrollView.Begin("Box");
            selectionGrid.Draw();
            scrollView.End();
            base.WindowFunction(id);
        }


        public void Refresh()
        {
            files = DataFile.LoadAll();
            selectionGrid.options = files.Select(x => System.IO.Path.GetFileNameWithoutExtension(x.fileInfo.Name)).ToArray();
        }

        public void SetOption(System.Action<DataFile> option)
        {
            selectionGrid.OnOptionChange = (w, x) =>
            {
                option(files[w]);
            };
        }
    }

    public class DataForm : DRWindow
    {
        BaseField targetData;
        ScrollView scrollView;
        object serializedObject;
        DataFile current;

        public DataForm(string windowName, string name, Rect rect, int index) : base(windowName, rect, index)
        {
            scrollView = new ScrollView();
        }

        public void SetField(DataFile data)
        {
            current = data;
            serializedObject = data.data;
            System.Type type = System.Type.GetType(data.fileInfo.Directory.Name);
            targetData = new ClassField(new SerializedObject("serializedObject", type, this));
            //   targetData = new ClassField(new SerializedObject(serializedObject));
        }

        protected override void WindowFunction(int id)
        {
            GUILayout.BeginHorizontal();
            // GUILayout.Label("File Name", GUILayout.MaxWidth(100));
            // GUILayout.TextField(current.fileInfo.Name);
            GUILayout.EndHorizontal();
            scrollView.Begin();
            if (targetData != null) targetData.Draw();
            scrollView.End();
            GUILayout.BeginHorizontal("Box");
            if (GUILayout.Button("Undo"))
            {
                Undo.current.Use();
            }
            if (GUILayout.Button("Save"))
            {
                if (current != null)
                {
                    DataFile.Ammend(current);
                }

            }
            GUILayout.EndHorizontal();
            base.WindowFunction(id);
        }
    }


}








