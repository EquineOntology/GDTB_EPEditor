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

        // =========================== Editor GUI =============================
        private GUISkin skin_custom, skin_default;
        private GUIStyle style_bold, style_customGrid;
        private Texture2D t_get;

        // ========================= Editor layouting =========================
        private const int IconSize = Constants.ICON_SIZE;
        private const int ButtonWidth = 60;
        private const int ButtonHeight = 18;

        // ========================= Class functionality =========================
        private string _key = "";
        private int _type = 0;
        private string[] _prefTypes = { "Bool", "Int", "Float", "String" };

        private bool val_bool = false;
        private int val_int = 0;
        private float val_float = 0.0f;
        private string val_string = "";


        public static void Init()
        {
            WindowGet window = (WindowGet)EditorWindow.GetWindow(typeof(WindowGet));
            window.minSize = new Vector2(275, 154);
            window.titleContent = new GUIContent("Get EditorPref");
            //window.InitButtonTextures();

            window.ShowUtility();
        }

        public void OnEnable()
        {
            Instance = this;
            skin_custom = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
            style_bold = skin_custom.GetStyle("GDTB_EPEditor_key");
            style_customGrid = skin_custom.GetStyle("GDTB_EPEditor_selectionGrid");
        }

        public void OnGUI()
        {
            if (skin_default == null)
            {
                skin_default = GUI.skin;
            }
            GUI.skin = skin_custom;

            DrawBG();
            DrawType();
            DrawKeyField();
            DrawGet();
        }


        /// Draw the background texture.
        private void DrawBG()
        {
            EditorGUI.DrawRect(new Rect(0,0, position.width, position.height), Preferences.Color_Primary);
        }


        /// Draw key input field.
        private void DrawKeyField()
        {
            var labelRect = new Rect(10, 10, position.width - 20, 16);
            EditorGUI.LabelField(labelRect, "Key:", style_bold);

            var keyRect = new Rect(10, 29, position.width - 20, 32);
            _key = EditorGUI.TextField(keyRect, _key);
        }


        /// Draw type popup.
        private void DrawType()
        {
            var labelRect = new Rect(10, 71, position.width - 20, 16);
            EditorGUI.LabelField(labelRect, "Type:", style_bold);

            var typeRect = new Rect(10, 90, position.width - 20, 20);
            if(Preferences.ButtonsDisplay == ButtonsDisplayFormat.REGULAR_BUTTONS)
            {
                GUI.skin = skin_default;
                _type = GUI.SelectionGrid(typeRect, _type, _prefTypes, _prefTypes.Length);
            }
            else
            {
                GUI.skin = skin_custom;
                _type = GUI.SelectionGrid(typeRect, _type, _prefTypes, _prefTypes.Length, style_customGrid);
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
                    GUI.skin = skin_default;
                    Button_Get_default(out getRect, out getContent);
                    break;
                case ButtonsDisplayFormat.COOL_ICONS:
                default:
                    GUI.skin = skin_custom;
                    Button_Get_icon(out getRect, out getContent);
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
                                    val_bool = NewEditorPrefs.GetBool(_key, false);
                                    break;
                                case 1:
                                    val_int = NewEditorPrefs.GetInt(_key, 0);
                                    break;
                                case 2:
                                    val_float = NewEditorPrefs.GetFloat(_key, 0.0f);
                                    break;
                                case 3:
                                    val_string = NewEditorPrefs.GetString(_key, "");
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
            DrawingUtils.DrawButtonTexture(getRect, DrawingUtils.Texture_Get);
        }


        /// Create rect and content for default Get.
        private void Button_Get_default(out Rect aRect, out GUIContent aContent)
        {
            GUI.skin = skin_default;
            aRect = new Rect((Screen.width / 2) - ButtonWidth/2, 126, ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Get key", "Add existing key");
        }


        /// Create rect and content for icon Get.
        private void Button_Get_icon(out Rect aRect, out GUIContent aContent)
        {
            GUI.skin = skin_custom;
            aRect = new Rect((Screen.width / 2) - IconSize/2, 126, IconSize, IconSize);
            aContent = new GUIContent("", "Add existing key");
        }


        /// Add EditorPref to list.
        private void AddEditorPref()
        {
            switch (_type)
            {
                case 0:
                    PrefOps.AddPref(_key, val_bool);
                    break;
                case 1:
                    PrefOps.AddPref(_key, val_int);
                    break;
                case 2:
                    PrefOps.AddPref(_key, val_float);
                    break;
                case 3:
                    PrefOps.AddPref(_key, val_string);
                    break;
            }
        }

        /// Initialize textures (so that they're not instantiated every frame in OnGUI).
        private void InitButtonTextures()
        {
            t_get = Resources.Load(Constants.FILE_GDTB_GET, typeof(Texture2D)) as Texture2D;
        }


        /// Remove textures from memory when not needed anymore.
        private void OnDestroy()
        {
            Resources.UnloadUnusedAssets();
        }
    }
}