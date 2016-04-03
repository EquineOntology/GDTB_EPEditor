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

        private const int IconSize = 16;
        private const int ButtonWidth = 60;
        private const int ButtonHeight = 18;

        private string _key = "";
        private int _type = 0;
        private string[] _prefTypes = { "Bool", "Int", "Float", "String" };

        private bool _boolValue = false;
        private int _intValue = 0;
        private float _floatValue = 0.0f;
        private string _stringValue = "";

        private GUISkin _customSkin, _defaultSkin;
        private GUIStyle _boldStyle, _gridStyle;

        public static void Init()
        {
            WindowGet window = (WindowGet)EditorWindow.GetWindow(typeof(WindowGet));
            window.minSize = new Vector2(275, 154);
            window.titleContent = new GUIContent("Get EditorPref");
            window.ShowUtility();
        }

        public void OnEnable()
        {
            Instance = this;
            _customSkin = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
            _boldStyle = _customSkin.GetStyle("GDTB_EPEditor_key");
            _gridStyle = _customSkin.GetStyle("GDTB_EPEditor_selectionGrid");
        }

        public void OnGUI()
        {
            if (_defaultSkin == null)
            {
                _defaultSkin = GUI.skin;
            }
            GUI.skin = _customSkin;

            DrawBG();
            DrawType();
            DrawKeyField();
            DrawGet();
        }


        /// Draw the background texture.
        private void DrawBG()
        {
            EditorGUI.DrawRect(new Rect(0,0, position.width, position.height), Constants.COLOR_UI_BG);
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
        private void DrawType()
        {
            var labelRect = new Rect(10, 71, position.width - 20, 16);
            EditorGUI.LabelField(labelRect, "Type:", _boldStyle);

            var typeRect = new Rect(10, 90, position.width - 20, 20);
            if(Preferences.ButtonsDisplay == ButtonsDisplayFormat.REGULAR_BUTTONS)
            {
                GUI.skin = _defaultSkin;
                _type = GUI.SelectionGrid(typeRect, _type, _prefTypes, _prefTypes.Length);
            }
            else
            {
                GUI.skin = _customSkin;
                _type = GUI.SelectionGrid(typeRect, _type, _prefTypes, _prefTypes.Length, _gridStyle);
            }

        }


        /// Draw Get button based on preferences.
        private void DrawGet()
        {
            Rect getRect;
            GUIContent getContent;
            switch (Preferences.ButtonsDisplay)
            {
                case ButtonsDisplayFormat.REGULAR_BUTTONS:
                    GUI.skin = _defaultSkin;
                    CreateGetButton_Default(out getRect, out getContent);
                    break;
                case ButtonsDisplayFormat.COOL_ICONS:
                default:
                    GUI.skin = _customSkin;
                    CreateGetButton_Icon(out getRect, out getContent);
                    break;
            }

            if (GUI.Button(getRect, getContent))
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

                            if (WindowMain.IsOpen)
                            {
                                EditorWindow.GetWindow(typeof(WindowMain)).Repaint();
                            }
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


        /// Create rect and content for default Get.
        private void CreateGetButton_Default(out Rect aRect, out GUIContent aContent)
        {
            GUI.skin = _defaultSkin;
            aRect = new Rect((Screen.width / 2) - ButtonWidth/2, 126, ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Get key", "Add existing key");
        }


        /// Create rect and content for icon Get.
        private void CreateGetButton_Icon(out Rect aRect, out GUIContent aContent)
        {
            GUI.skin = _customSkin;
            aRect = new Rect((Screen.width / 2) - IconSize/2, 126, IconSize, IconSize);
            aContent = new GUIContent(Resources.Load(Constants.FILE_GDTB_GET, typeof(Texture2D)) as Texture2D, "Add existing key");
        }


        /// Add EditorPref to list.
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