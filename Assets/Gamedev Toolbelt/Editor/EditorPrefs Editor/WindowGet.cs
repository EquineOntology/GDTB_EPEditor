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
        private GUIStyle style_bold, style_selectedGridButton, style_textButton;
        private bool clicked_get;

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


        public static void Init()
        {
            WindowGet window = (WindowGet)EditorWindow.GetWindow(typeof(WindowGet));
            window.minSize = new Vector2(275, 154);

            #if UNITY_5_3_OR_NEWER || UNITY_5_1 || UNITY_5_2
                window.titleContent = new GUIContent("Get EditorPref");
            #else
                window.title = "Get EditorPref";
            #endif

            window.CloseOtherWindows();
            window.ShowUtility();
        }

        public void OnEnable()
        {
            Instance = this;
            skin_custom = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
            LoadStyles();
        }

        private void OnDestroy()
        {
            Resources.UnloadUnusedAssets();
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
            idx_prefType = GUI.SelectionGrid(rect_type, idx_prefType, arr_prefTypes, arr_prefTypes.Length, style_selectedGridButton);
            DrawingUtils.DrawSelectionGrid(rect_type, arr_prefTypes, idx_prefType, 60, 5, style_textButton, style_selectedGridButton); // Draw our selectionGrid above Unity's one.

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
                clicked_get = true;
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
                        if (NewEditorPrefs.HasKey(pref_key))
                        {
                            AddEditorPref(idx_prefType, pref_key);

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
            else
            {
                clicked_get = false;
            }

            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                DrawingUtils.DrawIconButton(rect_get, DrawingUtils.Texture_Get, clicked_get);
            }
            else
            {
                DrawingUtils.DrawTextButton(rect_get, getContent.text, style_textButton, clicked_get);
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
        private void AddEditorPref(int aType, string aKey)
        {
            switch (aType)
            {
                case 0:
                    PrefOps.GetPref(aKey, PrefType.BOOL);
                    break;
                case 1:
                    PrefOps.GetPref(aKey, PrefType.INT);
                    break;
                case 2:
                    PrefOps.GetPref(aKey, PrefType.FLOAT);
                    break;
                case 3:
                    PrefOps.GetPref(aKey, PrefType.STRING);
                    break;
            }
        }


        /// Load styles and apply color preferences to them.
        public void LoadStyles()
        {
            style_bold = skin_custom.GetStyle("GDTB_EPEditor_key");
            style_bold.normal.textColor = Preferences.Color_Secondary;
            style_bold.active.textColor = Preferences.Color_Secondary;
            style_selectedGridButton = skin_custom.GetStyle("GDTB_EPEditor_selectionGrid");
            style_selectedGridButton.active.textColor = Preferences.Color_Primary;
            style_selectedGridButton.normal.textColor = Preferences.Color_Primary;
            style_textButton = skin_custom.GetStyle("GDTB_EPEditor_buttonText");
            style_textButton.active.textColor = Preferences.Color_Primary;
            style_textButton.normal.textColor = Preferences.Color_Tertiary;
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
    }
}