using UnityEngine;
using UnityEditor;
namespace com.immortalyhydra.gdtb.epeditor
{
    public class WindowEdit : EditorWindow
    {
        public static WindowEdit Instance { get; private set; }
        public static bool IsOpen {
            get { return Instance != null; }
        }

        // ========================= Editor layouting =========================
        private const int IconSize = Constants.ICON_SIZE;
        private const int ButtonWidth = 60;
        private const int ButtonHeight = 18;

        private Rect rect_key_label, rect_key;
        private Rect rect_type_label, rect_type;
        private Rect rect_value_label;
        private Rect rect_edit;

        // ======================= Class functionality ========================
        private Pref _originalPref;
        private string pref_key = "";
        private int idx_prefType = 0;
        private string[] arr_prefTypes = { "Bool", "Int", "Float", "String" };

        private bool pref_boolValue = false;
        private int idx_boolValues = 0;
        private string[] arr_boolValues = { "False", "True" };
        private int pref_intValue = 0;
        private float pref_floatValue = 0.0f;
        private string pref_stringValue = "";

        //============================ Editor GUI =============================
        private GUISkin skin_custom;
        private GUIStyle style_bold, style_selectedGridButton, style_textButton;

        public static void Init(Pref aPref)
        {
            WindowEdit window = (WindowEdit)EditorWindow.GetWindow(typeof(WindowEdit));
            window.minSize = new Vector2(275, 209);

            #if UNITY_5_3_OR_NEWER || UNITY_5_1 || UNITY_5_2
                window.titleContent = new GUIContent("Edit EditorPref");
            #else
                window.title = "Edit EditorPref";
            #endif

            window.InitInputValues(aPref);
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
            DrawKey();
            DrawType();
            DrawValue();
            DrawEdit();
        }


        /// Draw the background texture.
        private void DrawWindowBackground()
        {
            EditorGUI.DrawRect(new Rect(0,0, position.width, position.height), Preferences.Color_Primary);
        }


        /// Draw key input field.
        private void DrawKey()
        {
            rect_key_label = new Rect(10, 10, Mathf.Clamp(position.width - 20, 80, position.width), 16);
            EditorGUI.LabelField(rect_key_label, "Key:", style_bold);

            rect_key = new Rect(10, 29, Mathf.Clamp(position.width - 20, 80, position.width), 32);
            pref_key = EditorGUI.TextField(rect_key, pref_key);
        }


        /// Draw type selection.
        private void DrawType()
        {
            rect_type_label = new Rect(10, 71, Mathf.Clamp(position.width - 20, 80, position.width), 16);
            EditorGUI.LabelField(rect_type_label, "Type:", style_bold);

            rect_type = new Rect(10, 90, position.width - 20, 20);
            idx_prefType = GUI.SelectionGrid(rect_type, idx_prefType, arr_prefTypes, arr_prefTypes.Length, style_selectedGridButton);
            DrawingUtils.DrawSelectionGrid(rect_type, arr_prefTypes, idx_prefType, 60, 5, style_textButton, style_selectedGridButton);
        }


        /// Draw value input field.
        private void DrawValue()
        {
            rect_value_label = new Rect(10, 118, Mathf.Clamp(position.width - 20, 80, position.width), 16);
            EditorGUI.LabelField(rect_value_label, "Value:", style_bold);

            switch (idx_prefType)
            {
                case 0:
                    var boolRect = new Rect(10, 137, 130, 20);
                    idx_boolValues = GUI.SelectionGrid(boolRect, idx_boolValues, arr_boolValues, arr_boolValues.Length, style_selectedGridButton);
                    DrawingUtils.DrawSelectionGrid(boolRect, arr_boolValues, idx_boolValues, 60, 5, style_textButton, style_selectedGridButton);
                    pref_boolValue = idx_boolValues == 0 ? false : true;
                    break;
                case 1:
                    var intRect = new Rect(10, 137, position.width - 20, 16);
                    pref_intValue = EditorGUI.IntField(intRect, pref_intValue);
                    break;
                case 2:
                    var floatRect = new Rect(10, 137, position.width - 20, 16);
                    pref_floatValue = EditorGUI.FloatField(floatRect, pref_floatValue);
                    break;
                case 3:
                    var stringRect = new Rect(10, 137, position.width - 20, 32);
                    pref_stringValue = EditorGUI.TextField(stringRect, pref_stringValue);
                    break;
            }
        }


        /// Draw Edit button based on preferences.
        private void DrawEdit()
        {
            GUIContent editContent;

            switch (Preferences.ButtonsDisplay)
            {
                case ButtonsDisplayFormat.REGULAR_BUTTONS:
                    Button_Edit_default(out rect_edit, out editContent);
                    break;
                case ButtonsDisplayFormat.COOL_ICONS:
                default:
                    Button_Edit_icon(out rect_edit, out editContent);
                    break;
            }

            if (GUI.Button(rect_edit, editContent))
            {
                DrawingUtils.DrawButtonPressed(rect_edit, Preferences.ButtonsDisplay, DrawingUtils.Texture_Edit, editContent.text, style_textButton);
                if (pref_key == "") // We definitely want a key.
                {
                    EditorUtility.DisplayDialog("No key to use", "Please add a key.", "Ok");
                }
                else // What we do when editing is basically removing the old key and creating a new, updated one.
                {
                    // Get confirmation through dialog (or not if the user doesn't want to).
                    var canExecute = false;
                    if (Preferences.ShowConfirmationDialogs == true)
                    {
                        if (EditorUtility.DisplayDialog("Save edited Pref?", "Are you sure you want to save the changes to this EditorPref?", "Save", "Cancel"))
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
                        NewEditorPrefs.DeleteKey(_originalPref.Key);

                        switch (idx_prefType)
                        {
                            case 0:
                                NewEditorPrefs.SetBool(pref_key, pref_boolValue);
                                break;
                            case 1:
                                NewEditorPrefs.SetInt(pref_key, pref_intValue);
                                break;
                            case 2:
                                NewEditorPrefs.SetFloat(pref_key, pref_floatValue);
                                break;
                            case 3:
                                NewEditorPrefs.SetString(pref_key, pref_stringValue);
                                break;
                        }
                        if (WindowMain.IsOpen)
                        {
                            EditorWindow.GetWindow(typeof(WindowMain)).Repaint();
                        }
                        EditorWindow.GetWindow(typeof(WindowEdit)).Close();
                    }
                }
            }
            else
            {
                DrawingUtils.DrawButton(rect_edit, Preferences.ButtonsDisplay, DrawingUtils.Texture_Edit, editContent.text, style_textButton);
            }

        }


        private void Button_Edit_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(Mathf.Clamp((Screen.width / 2) - ButtonWidth/2, 0, position.width), 179, ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Save", "Save changes");
        }
        private void Button_Edit_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(Mathf.Clamp((Screen.width / 2) - IconSize/2, 0, position.width), 179, IconSize, IconSize);
            aContent = new GUIContent("", "Save changes");
        }


        /// Set the values of the original pref for input fields.
        private void InitInputValues(Pref aPref)
        {
            _originalPref = aPref;
            pref_key = _originalPref.Key;
            idx_prefType = (int)_originalPref.Type;
            switch (idx_prefType)
            {
                case 0:
                    pref_boolValue = aPref.Value == "True" ? true : false;
                    idx_boolValues = pref_boolValue == true ? 1 : 0;
                    break;
                case 1:
                    pref_intValue = int.Parse(aPref.Value);
                    break;
                case 2:
                    pref_floatValue = float.Parse(aPref.Value);
                    break;
                case 3:
                    pref_stringValue = aPref.Value;
                    break;
            }
        }


        /// Load custom styles and apply colors in preferences.
        public void LoadStyles ()
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
            if (WindowGet.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowGet)).Close();
            }
        }
    }
}