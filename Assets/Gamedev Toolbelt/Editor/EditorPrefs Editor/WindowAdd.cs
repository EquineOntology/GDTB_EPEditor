using UnityEngine;
using UnityEditor;
namespace GDTB.EditorPrefsEditor
{
    public class WindowAdd : EditorWindow
    {
        public static WindowAdd Instance { get; private set; }
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
        private Rect rect_add;

        // ======================= Class functionality ========================
        private string _key = "";
        private int idx_prefType = 0;
        private string[] arr_prefTypes = { "Bool", "Int", "Float", "String" };

        private bool val_bool = false;
        private int idx_bool = 0;
        private string[] arr_boolValues = { "False", "True" };
        private int val_int = 0;
        private float val_float = 0.0f;
        private string val_string = "";

        //============================ Editor GUI =============================
        private GUISkin skin_custom, skin_default;
        private GUIStyle style_bold, style_customGrid, style_buttonText;

        public static void Init()
        {
            WindowAdd window = (WindowAdd)EditorWindow.GetWindow(typeof(WindowAdd));
            window.minSize = new Vector2(275, 209);
            window.titleContent = new GUIContent("Add EditorPref");
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
            if (skin_default == null)
            {
                skin_default = GUI.skin;
            }
            GUI.skin = skin_custom;

            DrawBG();
            DrawKeyField();
            DrawType();
            DrawValueField();
            DrawAddButton();
        }


        /// Draw the background texture.
        private void DrawBG()
        {
            EditorGUI.DrawRect(new Rect(0,0, position.width, position.height), Preferences.Color_Primary);
        }


        /// Draw key input field.
        private void DrawKeyField()
        {
            rect_key_label = new Rect(10, 10, position.width - 20, 16);
            EditorGUI.LabelField(rect_key_label, "Key:", style_bold);

            rect_key = new Rect(10, 29, position.width - 20, 32);
            _key = EditorGUI.TextField(rect_key, _key);
        }


        /// Draw type popup.
        private void DrawType()
        {
            rect_type_label = new Rect(10, 71, Mathf.Clamp(position.width - 20, 80, 500), 16);
            EditorGUI.LabelField(rect_type_label, "Type:", style_bold);

            rect_type = new Rect(10, 90, position.width - 20, 20);
            idx_prefType = GUI.SelectionGrid(rect_type, idx_prefType, arr_prefTypes, arr_prefTypes.Length, style_customGrid);
            DrawingUtils.DrawSelectionGrid(rect_type, arr_prefTypes, idx_prefType, 60, 5, style_buttonText, style_customGrid);
        }


        /// Draw value input field.
        private void DrawValueField()
        {
            rect_value_label = new Rect(10, 118, Mathf.Clamp(position.width - 20, 80, 500), 16);
            EditorGUI.LabelField(rect_value_label, "Value:", style_bold);

            switch (idx_prefType)
            {
                case 0:
                    var boolRect = new Rect(10, 137, 130, 20);
                    idx_bool = GUI.SelectionGrid(boolRect, idx_bool, arr_boolValues, arr_boolValues.Length, style_customGrid);
                    DrawingUtils.DrawSelectionGrid(boolRect, arr_boolValues, idx_bool, 60, 5, style_buttonText, style_customGrid);
                    val_bool = idx_bool == 0 ? false : true;
                    break;
                case 1:
                    var intRect = new Rect(10, 137, position.width - 20, 16);
                    val_int = EditorGUI.IntField(intRect, val_int);
                    break;
                case 2:
                    var floatRect = new Rect(10, 137, position.width - 20, 16);
                    val_float = EditorGUI.FloatField(floatRect, val_float);
                    break;
                case 3:
                    var stringRect = new Rect(10, 137, position.width - 20, 32);
                    val_string = EditorGUI.TextField(stringRect, val_string);
                    break;
            }
        }


        /// Draw Add button based of preferences.
        private void DrawAddButton()
        {
            Pref currentPref = null;
            GUIContent addContent;

            switch (Preferences.ButtonsDisplay)
            {
                case ButtonsDisplayFormat.REGULAR_BUTTONS:
                    Button_Add_default(out rect_add, out addContent);
                    break;
                case ButtonsDisplayFormat.COOL_ICONS:
                default:
                    Button_Add_icon(out rect_add, out addContent);
                    break;
            }
            if (GUI.Button(rect_add, addContent))
            {
                if (_key == "")
                {
                    EditorUtility.DisplayDialog("No key to add", "Please add a key.", "Ok");
                }

                // If the key exists.
                else if (NewEditorPrefs.HasKey(_key))
                {
                    var shouldAddPref = true;

                    // If the key is already in prefs.
                    foreach (var pref in WindowMain.Prefs)
                    {
                        if (pref.Key == _key)
                        {
                            shouldAddPref = false;
                            currentPref = pref;
                            break;
                        }
                    }

                    // If the key is not in prefs.
                    if (shouldAddPref)
                    {
                        bool bValue;
                        int iValue;
                        float fValue;
                        string sValue;
                        try
                        {
                            bValue = NewEditorPrefs.GetBool(_key);
                            currentPref = new Pref(PrefType.BOOL, _key, bValue.ToString());
                        }
                        catch (System.Exception) { }

                        try
                        {
                            iValue = NewEditorPrefs.GetInt(_key);
                            currentPref = new Pref(PrefType.INT, _key, iValue.ToString());
                        }
                        catch (System.Exception) { }

                        try
                        {
                            fValue = NewEditorPrefs.GetFloat(_key);
                            currentPref = new Pref(PrefType.FLOAT, _key, fValue.ToString());
                        }
                        catch (System.Exception) { }

                        try
                        {
                            sValue = NewEditorPrefs.GetString(_key);
                            currentPref = new Pref(PrefType.STRING, _key, sValue);
                        }
                        catch (System.Exception) { }
                    }

                    // Does the user want to edit the already existing key?
                    if (currentPref != null)
                    {
                        if (EditorUtility.DisplayDialog("Pref already exists.", "The key you're trying to use already exists.\nDo you want to edit it?", "Edit", "Cancel"))
                        {
                            WindowEdit.Init(currentPref);
                            EditorWindow.GetWindow(typeof(WindowAdd)).Close();
                        }
                        else
                        {
                            PrefOps.AddPref(currentPref);
                        }
                    }
                }
                else
                {
                    // Get confirmation through dialog (or not if the user doesn't want to).
                    var canExecute = false;
                    if (Preferences.ShowConfirmationDialogs == true)
                    {
                        if (EditorUtility.DisplayDialog("Add editor preference?", "Are you sure you want to add this key to EditorPrefs?", "Add key", "Cancel"))
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
                        switch (idx_prefType)
                        {
                            case 0:
                                NewEditorPrefs.SetBool(_key, val_bool);
                                break;
                            case 1:
                                NewEditorPrefs.SetInt(_key, val_int);
                                break;
                            case 2:
                                NewEditorPrefs.SetFloat(_key, val_float);
                                break;
                            case 3:
                                NewEditorPrefs.SetString(_key, val_string);
                                break;
                        }
                        EditorWindow.GetWindow(typeof(WindowAdd)).Close();
                    }
                }
            }
            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                DrawingUtils.DrawTextureButton(rect_add, DrawingUtils.Texture_Add);
            }
            else
            {
                DrawingUtils.DrawTextButton(rect_add, addContent.text, style_buttonText);
            }
        }


        /// Create rect and content for default Add.
        private void Button_Add_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(Mathf.Clamp((Screen.width / 2) - ButtonWidth/2, 0, position.width), 179, ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Add", "Add this new EditorPref");
        }


        /// Create rect and content for icon Add.
        private void Button_Add_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(Mathf.Clamp((Screen.width / 2) - IconSize/2, 0, position.width), 179, IconSize, IconSize);
            aContent = new GUIContent("", "Add this new EditorPref");
        }


        /// Load custom styles.
        public void LoadStyles()
        {
            style_bold = skin_custom.GetStyle("GDTB_EPEditor_key");
            style_bold.normal.textColor = Preferences.Color_Secondary;
            style_bold.active.textColor = Preferences.Color_Secondary;
            style_customGrid = skin_custom.GetStyle("GDTB_EPEditor_selectionGrid");
            style_buttonText = skin_custom.GetStyle("GDTB_EPEditor_buttonText");
            style_buttonText.active.textColor = Preferences.Color_Tertiary;
            style_buttonText.normal.textColor = Preferences.Color_Tertiary;
        }


        /// Close other sub-windows when this one is opened.
        private void CloseOtherWindows()
        {
            if (WindowEdit.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowEdit)).Close();
            }
            if (WindowGet.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowGet)).Close();
            }
        }


        /// Remove textures from memory when not needed anymore.
        private void OnDestroy()
        {
            Resources.UnloadUnusedAssets();
        }
    }
}