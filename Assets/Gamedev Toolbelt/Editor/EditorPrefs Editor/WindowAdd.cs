using UnityEngine;
using UnityEditor;
namespace com.immortalyhydra.gdtb.epeditor
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
        private string pref_key = "";
        private int pref_typeIndex = 0;
        private string[] arr_prefTypes = { "Bool", "Int", "Float", "String" };

        private bool pref_boolValue = false;
        private int idx_bool = 0;
        private string[] arr_boolValues = { "False", "True" };
        private int pref_intValue = 0;
        private float pref_floatValue = 0.0f;
        private string pref_stringValue = "";

        //============================ Editor GUI =============================
        private GUISkin skin_custom;
        private GUIStyle style_bold, style_customGrid, style_buttonText;

        public static void Init()
        {
            WindowAdd window = (WindowAdd)EditorWindow.GetWindow(typeof(WindowAdd));
            window.minSize = new Vector2(275, 209);

            #if UNITY_5_3_OR_NEWER || UNITY_5_1 || UNITY_5_2
                window.titleContent = new GUIContent("Add EditorPref");
            #else
                window.title = "Add EditorPref";
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
            GUI.skin = skin_custom;

            DrawWindowBackground();
            DrawKey();
            DrawType();
            DrawValue();
            DrawAdd();
        }


        /// Draw the background texture.
        private void DrawWindowBackground()
        {
            EditorGUI.DrawRect(new Rect(0,0, position.width, position.height), Preferences.Color_Primary);
        }


        /// Draw key input field.
        private void DrawKey()
        {
            rect_key_label = new Rect(10, 10, position.width - 20, 16);
            EditorGUI.LabelField(rect_key_label, "Key:", style_bold);

            rect_key = new Rect(10, 29, position.width - 20, 32);
            pref_key = EditorGUI.TextField(rect_key, pref_key);
        }


        /// Draw type popup.
        private void DrawType()
        {
            rect_type_label = new Rect(10, 71, Mathf.Clamp(position.width - 20, 80, 500), 16);
            EditorGUI.LabelField(rect_type_label, "Type:", style_bold);

            rect_type = new Rect(10, 90, position.width - 20, 20);
            pref_typeIndex = GUI.SelectionGrid(rect_type, pref_typeIndex, arr_prefTypes, arr_prefTypes.Length, style_customGrid);
            DrawingUtils.DrawSelectionGrid(rect_type, arr_prefTypes, pref_typeIndex, 60, 5, style_buttonText, style_customGrid);
        }


        /// Draw value input field.
        private void DrawValue()
        {
            rect_value_label = new Rect(10, 118, Mathf.Clamp(position.width - 20, 80, 500), 16);
            EditorGUI.LabelField(rect_value_label, "Value:", style_bold);

            switch (pref_typeIndex)
            {
                case 0:
                    var boolRect = new Rect(10, 137, 130, 20);
                    idx_bool = GUI.SelectionGrid(boolRect, idx_bool, arr_boolValues, arr_boolValues.Length, style_customGrid);
                    DrawingUtils.DrawSelectionGrid(boolRect, arr_boolValues, idx_bool, 60, 5, style_buttonText, style_customGrid);
                    pref_boolValue = idx_bool == 0 ? false : true;
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


        /// Draw Add button based of preferences.
        private void DrawAdd()
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
                AddButtonPressed(currentPref); // In another function to help readability of this one.
            }

            // We draw our button above Unity's one (because ours looks cooler ;D )
            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                DrawingUtils.DrawTextureButton(rect_add, DrawingUtils.Texture_Add);
            }
            else
            {
                DrawingUtils.DrawTextButton(rect_add, addContent.text, style_buttonText);
            }
        }

        private void Button_Add_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(Mathf.Clamp((Screen.width / 2) - ButtonWidth/2, 0, position.width), 179, ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Add", "Add this new EditorPref");
        }
        private void Button_Add_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(Mathf.Clamp((Screen.width / 2) - IconSize/2, 0, position.width), 179, IconSize, IconSize);
            aContent = new GUIContent("", "Add this new EditorPref");
        }


        /// Load custom styles and apply colors from preferences.
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

        private void AddButtonPressed(Pref currentPref)
        {
            // A: we want a key.
            if (pref_key == "")
            {
                EditorUtility.DisplayDialog("No key to add", "Please add a key.", "Ok");
            }
            // B: The key exists in EditorPrefs:
            else if (NewEditorPrefs.HasKey(pref_key))
            {
                var InPrefs = false;

                // B-a: is the key in the Prefs list already?
                foreach (var pref in WindowMain.Prefs)
                {
                    if (pref.Key == pref_key)
                    {
                        InPrefs = true;
                        currentPref = pref;
                        break;
                    }
                }

                // B-b: the key is not in the list (but it exists in EditorPrefs).
                if (InPrefs == false)
                {
                    switch (pref_typeIndex) // Get the key's value based on the type given.
                    {
                        case 0:
                            pref_boolValue = NewEditorPrefs.GetBool(pref_key); //idx_bool == 0 ? false : true;
                            currentPref = new Pref(PrefType.BOOL, pref_key, pref_boolValue.ToString());
                            break;
                        case 1:
                            pref_intValue = NewEditorPrefs.GetInt(pref_key);
                            currentPref = new Pref(PrefType.INT, pref_key, pref_intValue.ToString());
                            break;
                        case 2:
                            pref_floatValue = NewEditorPrefs.GetFloat(pref_key);
                            currentPref = new Pref(PrefType.FLOAT, pref_key, pref_floatValue.ToString());
                            break;
                        case 3:
                            pref_stringValue = NewEditorPrefs.GetString(pref_key);
                            currentPref = new Pref(PrefType.STRING, pref_key, pref_stringValue);
                            break;
                    }
                }

                // B-c: does the user want to edit the already existing key before adding it?
                if (currentPref != null)
                {
                    if (EditorUtility.DisplayDialog("Pref already exists.", "The key you're trying to use already exists.\nDo you want to edit it before adding it to the list?", "Edit", "Add"))
                    {
                        WindowEdit.Init(currentPref);
                    }
                    else
                    {
                        PrefOps.AddPref(currentPref);
                    }
                    EditorWindow.GetWindow(typeof(WindowAdd)).Close();
                }
            }
            // C: the pref doesn't exist in EditorPrefs.
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

                if (canExecute == true)
                {
                    switch (pref_typeIndex)
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
                    EditorWindow.GetWindow(typeof(WindowAdd)).Close();
                }
            }
        }
    }
}