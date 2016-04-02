using UnityEngine;
using UnityEditor;

namespace GDTB.EditorPrefsEditor
{
    public class WindowGet : EditorWindow
    {
        public static WindowGet Instance { get; private set; }
        public static bool IsOpen {
            get { return Instance != null; }
        }

        private string _key = "";
        private int _type = 0;
        private string[] _prefTypes = { "Bool", "Int", "Float", "String" };

        private bool _boolValue = false;
        private int _intValue = 0;
        private float _floatValue = 0.0f;
        private string _stringValue = "";

        private GUISkin _skin;
        private GUIStyle _boldStyle, _gridStyle;

        public static void Init()
        {
            WindowGet window = (WindowGet)EditorWindow.GetWindow(typeof(WindowGet));
            window.minSize = new Vector2(275, 162);
            window.titleContent = new GUIContent("Get EditorPref");
            window.ShowUtility();
        }

        public void OnEnable()
        {
            Instance = this;
            _skin = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
            _boldStyle = _skin.GetStyle("GDTB_EPEditor_key");
            _gridStyle = _skin.GetStyle("GDTB_EPEditor_selectionGrid");
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
            EditorGUI.LabelField(labelRect, "Key:", _boldStyle);

            var keyRect = new Rect(10, 29, position.width - 20, 32);
            _key = EditorGUI.TextField(keyRect, _key);
        }


        /// Draw type popup.
        private void DrawTypePopup()
        {
            var labelRect = new Rect(10, 71, position.width - 20, 16);
            EditorGUI.LabelField(labelRect, "Type:", _boldStyle);

            var typeRect = new Rect(10, 90, position.width - 20, 20);
            _type = GUI.SelectionGrid(typeRect, _type, _prefTypes, _prefTypes.Length, _gridStyle);
        }


        /// Draw "Add pref" button.
        private void DrawButton()
        {
            GUI.skin = _skin;

            var buttonRect = new Rect((Screen.width / 2) - 60, 126, 120, 20);

            if (GUI.Button(buttonRect, "Get EditorPref"))
            {
                if (_key == "")
                {
                    EditorUtility.DisplayDialog("No key to look for", "Please add a key.", "Ok");
                }
                else
                {
                    // Get confirmation through dialog (or not if the user doesn't want to).
                    var canExecute = false;
                    if (Preferences.ShowConfirmationDialogs == true)
                    {
                        if (EditorUtility.DisplayDialog("Get editor preference?", "Are you sure you want to get this key from EditorPrefs?\nIf it's not found, we'll tell you and no key will be added to the interface, no worries.", "Add key", "Cancel"))
                        {
                            canExecute = true;
                        }
                    }
                    else
                    {
                        canExecute = true;
                    }

                    // Actually do the thing.
                    if (canExecute == true)
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
                    PrefOps.AddPref(_key, _boolValue);
                    break;
                case 1:
                    PrefOps.AddPref(_key, _intValue);
                    break;
                case 2:
                    PrefOps.AddPref(_key, _floatValue);
                    break;
                case 3:
                    PrefOps.AddPref(_key, _stringValue);
                    break;
            }
        }
    }
}