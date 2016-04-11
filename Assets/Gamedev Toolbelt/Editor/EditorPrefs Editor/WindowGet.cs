using UnityEngine;
using UnityEditor;

namespace com.immortalyhydra.gdtb.epeditor
{
    public class WindowGet : EditorWindow
    {
        public static WindowGet Instance { get; private set; }
        public static bool IsOpen {
            get { return Instance != null; }
        }

        // =========================== Editor GUI =============================
        private GUISkin skin_custom;
        private GUIStyle style_bold, style_pressedButton, style_normalButton;

        // ========================= Editor layouting =========================
        private const int IconSize = Constants.ICON_SIZE;
        private const int ButtonWidth = 60;
        private const int ButtonHeight = 18;

        private Rect rect_get;
        private Rect rect_key_label, rect_key;
        private Rect rect_type_label, rect_type;

        // ========================= Class functionality =========================
        private string pref_key = "";
        private int idx_prefType = 0;
        private string[] arr_prefTypes = { "Bool", "Int", "Float", "String" };

        private bool pref_value_bool = false;
        private int pref_value_int = 0;
        private float pref_value_float = 0.0f;
        private string prefe_value_string = "";


        public static void Init()
        {
            WindowGet window = (WindowGet)EditorWindow.GetWindow(typeof(WindowGet));
            window.minSize = new Vector2(275, 154);
            window.titleContent = new GUIContent("Get EditorPref");
            window.CloseOtherWindows();

            window.ShowUtility();
        }

        public void OnEnable()
        {
            Instance = this;
            skin_custom = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
            LoadStyles();
        }

        public void OnGUI()
        {
            DrawWindowBackground();
            DrawType();
            DrawKeyField();
            DrawGet();
        }


        /// Draw the background rectangle.
        private void DrawWindowBackground()
        {
            EditorGUI.DrawRect(new Rect(0,0, position.width, position.height), Preferences.Color_Primary);
        }


        /// Draw key input field.
        private void DrawKeyField()
        {
            rect_key_label = new Rect(10, 10, position.width - 20, 16);
            EditorGUI.LabelField(rect_key_label, "Key:", style_bold);

            rect_key = new Rect(10, 29, position.width - 20, 32);
            pref_key = EditorGUI.TextField(rect_key, pref_key);
        }


        /// Draw type selector.
        private void DrawType()
        {
            rect_type_label = new Rect(10, 71, position.width - 20, 16);
            EditorGUI.LabelField(rect_type_label, "Type:", style_bold);

            rect_type = new Rect(10, 90, position.width - 20, 20);
            idx_prefType = GUI.SelectionGrid(rect_type, idx_prefType, arr_prefTypes, arr_prefTypes.Length, style_pressedButton);
            DrawingUtils.DrawSelectionGrid(rect_type, arr_prefTypes, idx_prefType, 60, 5, style_normalButton, style_pressedButton); // Draw our selectionGrid above Unity's one.

        }


        /// Draw Get button based on preferences.
        private void DrawGet()
        {
            GUIContent getContent;
            switch (Preferences.ButtonsDisplay)
            {
                case ButtonsDisplayFormat.REGULAR_BUTTONS:
                    Button_Get_default(out rect_get, out getContent);
                    break;
                case ButtonsDisplayFormat.COOL_ICONS:
                default:
                    Button_Get_icon(out rect_get, out getContent);
                    break;
            }

            if (GUI.Button(rect_get, getContent))
            {
                if (pref_key == "")
                {
                    EditorUtility.DisplayDialog("No key to look for", "Please add a key.", "Ok");
                }
                else
                {
                    // Get confirmation through dialog (or not if the user doesn't want to).
                    var canExecute = false;
                    if (Preferences.ShowConfirmationDialogs == true)
                    {
                        if (EditorUtility.DisplayDialog("Get editor preference?", "Are you sure you want to get this key from EditorPrefs?\nIf the key is not found, we'll tell you.\nIf the type is wrong, a default key will be added.", "Add key", "Cancel"))
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
                        if (NewEditorPrefs.HasKey(pref_key))
                        {
                            switch (idx_prefType)
                            {
                                case 0:
                                    pref_value_bool = NewEditorPrefs.GetBool(pref_key);
                                    PrefOps.GetPref(pref_key, PrefType.BOOL);
                                    break;
                                case 1:
                                    pref_value_int = NewEditorPrefs.GetInt(pref_key);
                                    PrefOps.GetPref(pref_key, PrefType.INT);
                                    break;
                                case 2:
                                    pref_value_float = NewEditorPrefs.GetFloat(pref_key);
                                    PrefOps.GetPref(pref_key, PrefType.FLOAT);
                                    break;
                                case 3:
                                    prefe_value_string = NewEditorPrefs.GetString(pref_key);
                                    PrefOps.GetPref(pref_key, PrefType.STRING);
                                    break;
                            }

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
            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                DrawingUtils.DrawTextureButton(rect_get, DrawingUtils.Texture_Get);
            }
            else
            {
                DrawingUtils.DrawTextButton(rect_get, getContent.text, style_normalButton);
            }
        }

        private void Button_Get_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((Screen.width / 2) - ButtonWidth/2, 126, ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Get key", "Add existing key");
        }
        private void Button_Get_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((Screen.width / 2) - IconSize/2, 126, IconSize, IconSize);
            aContent = new GUIContent("", "Add existing key");
        }


        /// Add EditorPref to list.
        private void AddEditorPref()
        {
            switch (idx_prefType)
            {
                case 0:
                    PrefOps.AddPref(pref_key, pref_value_bool);
                    break;
                case 1:
                    PrefOps.AddPref(pref_key, pref_value_int);
                    break;
                case 2:
                    PrefOps.AddPref(pref_key, pref_value_float);
                    break;
                case 3:
                    PrefOps.AddPref(pref_key, prefe_value_string);
                    break;
            }
        }


        /// Load styles and apply color preferences to them.
        public void LoadStyles()
        {
            style_bold = skin_custom.GetStyle("GDTB_EPEditor_key");
            style_bold.normal.textColor = Preferences.Color_Secondary;
            style_bold.active.textColor = Preferences.Color_Secondary;
            style_pressedButton = skin_custom.GetStyle("GDTB_EPEditor_selectionGrid");
            style_normalButton = skin_custom.GetStyle("GDTB_EPEditor_buttonText");
            style_normalButton.active.textColor = Preferences.Color_Tertiary;
            style_normalButton.normal.textColor = Preferences.Color_Tertiary;
        }


        /// Close other sub-windows when this one is opened.
        private void CloseOtherWindows()
        {
            if (WindowAdd.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowAdd)).Close();
            }
            if (WindowEdit.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowEdit)).Close();
            }
        }


        /// Remove textures from memory when not needed anymore.
        private void OnDestroy()
        {
            Resources.UnloadUnusedAssets();
        }
    }
}