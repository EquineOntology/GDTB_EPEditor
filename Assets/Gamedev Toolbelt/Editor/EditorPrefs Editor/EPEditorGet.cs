using UnityEngine;
using UnityEditor;

namespace GDTB.EditorPrefsEditor
{
    public class EPEditorGet : EditorWindow
    {
        private string _key = "";
        private int _type = 0;
        private string[] _prefTypes = { "Bool", "Int", "Float", "String" };

        private bool _boolValue = false;
        private int _intValue = 0;
        private float _floatValue = 0.0f;
        private string _stringValue = "";

        private GUISkin _EPEditorSkin;
        private GUISkin _defaultSkin;

        public static void Init()
        {
            EPEditorGet window = (EPEditorGet)EditorWindow.GetWindow(typeof(EPEditorGet));
            window.minSize = new Vector2(200, 150);
            window.titleContent = new GUIContent("Get EditorPref");
            window.ShowUtility();
        }

        public void OnEnable()
        {
            _defaultSkin = GUI.skin;
            _EPEditorSkin = Resources.Load(EPConstants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        }

        public void OnGUI()
        {
            DrawTypePopup();
            DrawKeyField();
            DrawButton();
        }


        /// Draw key input field.
        private void DrawKeyField()
        {
            var labelRect = new Rect(10, 10, Mathf.Clamp(position.width - 20, 80, 500), 16);
            EditorGUI.LabelField(labelRect, "Write the key:", EditorStyles.boldLabel);

            var keyRect = new Rect(10, 29, Mathf.Clamp(position.width - 20, 80, 500), 32);
            _key = EditorGUI.TextField(keyRect, _key);
        }


        /// Draw type popup.
        private void DrawTypePopup()
        {
            var labelRect = new Rect(10, 71, Mathf.Clamp(position.width - 20, 80, 500), 16);
            EditorGUI.LabelField(labelRect, "Choose a type:", EditorStyles.boldLabel);

            var priorityRect = new Rect(10, 90, 70, 16);
            _type = EditorGUI.Popup(priorityRect, _type, _prefTypes);
        }


        /// Draw "Add pref" button.
        private void DrawButton()
        {
            GUI.skin = _EPEditorSkin;

            var buttonRect = new Rect(Mathf.Clamp((Screen.width / 2) - 60, 0, 190), 116, 120, 20);

            if (GUI.Button(buttonRect, "Get EditorPref"))
            {
                if (_key == "")
                {
                    EditorUtility.DisplayDialog("No key to look for", "Please add a key.", "Ok");
                }
                else
                {
                    if (EditorUtility.DisplayDialog("Get editor preference?", "Are you sure you want to get this key from EditorPrefs?\nIf it's not found, we'll tell you and no key will be added to the interface, no worries.", "Add key", "Cancel"))
                    {

                        // Check that pref was added correctly.
                        if (EditorPrefs.HasKey(_key))
                        {
                            switch (_type)
                            {
                                case 0:
                                    _boolValue = EditorPrefs.GetBool(_key, false);
                                    break;
                                case 1:
                                    _intValue = EditorPrefs.GetInt(_key, 0);
                                    break;
                                case 2:
                                    _floatValue = EditorPrefs.GetFloat(_key, 0.0f);
                                    break;
                                case 3:
                                    _stringValue = EditorPrefs.GetString(_key, "");
                                    break;
                            }
                            AddEditorPref();
                            EPEditorIO.WritePrefsToFile();
                            EditorWindow.GetWindow(typeof(EPEditorGet)).Close();
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("No key found", "The key you inserted was not found in EditorPrefs.\nPlease check that the spelling is correct (it's case sensitive).", "Ok");
                        }
                    }
                }
                GUI.skin = _defaultSkin;
            }
        }

        private void AddEditorPref()
        {
            switch (_type)
            {
                case 0:
                    EPEditor.Prefs.Add(new EditorPref(EditorPrefType.BOOL, _key, _boolValue.ToString()));
                    break;
                case 1:
                    EPEditor.Prefs.Add(new EditorPref(EditorPrefType.INT, _key, _intValue.ToString()));
                    break;
                case 2:
                    EPEditor.Prefs.Add(new EditorPref(EditorPrefType.FLOAT, _key, _floatValue.ToString()));
                    break;
                case 3:
                    EPEditor.Prefs.Add(new EditorPref(EditorPrefType.STRING, _key, _stringValue));
                    break;
            }
        }
    }
}