using UnityEngine;
using UnityEditor;

namespace GDTB.EditorPrefsEditor
{
    public class WindowGet : EditorWindow
    {
        private string _key = "";
        private int _type = 0;
        private string[] _prefTypes = { "Bool", "Int", "Float", "String" };

        private bool _boolValue = false;
        private int _intValue = 0;
        private float _floatValue = 0.0f;
        private string _stringValue = "";

        private GUISkin _skin;
        private GUIStyle _boldStyle;

        public static void Init()
        {
            WindowGet window = (WindowGet)EditorWindow.GetWindow(typeof(WindowGet));
            window.minSize = new Vector2(200, 150);
            window.titleContent = new GUIContent("Get EditorPref");
            window.ShowUtility();
        }

        public void OnEnable()
        {
            _skin = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
            _boldStyle = _skin.GetStyle("GDTB_EPEditor_key");
        }

        public void OnGUI()
        {
            DrawBG();
            DrawTypePopup();
            DrawKeyField();
            DrawButton();
        }


        /// Draw the background texture.
        private void DrawBG()
        {
            EditorGUI.DrawRect(new Rect(0,0, position.width, position.height), Constants.COLOR_UI_ACCENT);
        }


        /// Draw key input field.
        private void DrawKeyField()
        {
            var labelRect = new Rect(10, 10, position.width - 20, 16);
            EditorGUI.LabelField(labelRect, "Write the key:", _boldStyle);

            var keyRect = new Rect(10, 29, position.width - 20, 32);
            _key = EditorGUI.TextField(keyRect, _key);
        }


        /// Draw type popup.
        private void DrawTypePopup()
        {
            var labelRect = new Rect(10, 71, position.width - 20, 16);
            EditorGUI.LabelField(labelRect, "Choose a type:", _boldStyle);

            var priorityRect = new Rect(10, 90, 70, 16);
            _type = EditorGUI.Popup(priorityRect, _type, _prefTypes);
        }


        /// Draw "Add pref" button.
        private void DrawButton()
        {
            GUI.skin = _skin;

            var buttonRect = new Rect((Screen.width / 2) - 60, 116, 120, 20);

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
                        if (NewEditorPrefs.HasKey(_key))
                        {
                            switch (_type)
                            {
                                case 0:
                                    _boolValue = NewEditorPrefs.GetBool(_key, false);
                                    break;
                                case 1:
                                    _intValue = NewEditorPrefs.GetInt(_key, 0);
                                    break;
                                case 2:
                                    _floatValue = NewEditorPrefs.GetFloat(_key, 0.0f);
                                    break;
                                case 3:
                                    _stringValue = NewEditorPrefs.GetString(_key, "");
                                    break;
                            }
                            AddEditorPref();
                            EditorWindow.GetWindow(typeof(WindowGet)).Close();
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("No key found", "The key you inserted was not found in EditorPrefs.\nPlease check that the spelling is correct (it's case sensitive).", "Ok");
                        }
                    }
                }
            }
        }

        private void AddEditorPref()
        {
            switch (_type)
            {
                case 0:
                    PrefManager.AddPref(_key, _boolValue);
                    break;
                case 1:
                    PrefManager.AddPref(_key, _intValue);
                    break;
                case 2:
                    PrefManager.AddPref(_key, _floatValue);
                    break;
                case 3:
                    PrefManager.AddPref(_key, _stringValue);
                    break;
            }
        }
    }
}