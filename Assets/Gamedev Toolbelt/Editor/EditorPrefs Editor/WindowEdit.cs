using UnityEngine;
using UnityEditor;
namespace GDTB.EditorPrefsEditor
{
    public class WindowEdit : EditorWindow
    {
        public static WindowEdit Instance { get; private set; }
        public static bool IsOpen {
            get { return Instance != null; }
        }

        // ========================= Editor layouting =========================
        private const int IconSize = 16;
        private const int ButtonWidth = 60;
        private const int ButtonHeight = 18;

        // ======================= Class functionality ========================
        private Pref _originalPref;
        private string _key = "";
        private int _type = 0;
        private string[] arr_prefTypes = { "Bool", "Int", "Float", "String" };

        private bool val_bool = false;
        private int idx_bool = 0;
        private string[] arr_boolValues = { "False", "True" };
        private int val_int = 0;
        private float val_float = 0.0f;
        private string val_string = "";

        //============================ Editor GUI =============================
        private GUISkin skin_custom, skin_default;
        private GUIStyle style_bold, style_customGrid;
        private Texture2D t_edit;


        public static void Init(Pref aPref)
        {
            WindowEdit window = (WindowEdit)EditorWindow.GetWindow(typeof(WindowEdit));
            window.minSize = new Vector2(275, 209);
            window.titleContent = new GUIContent("Edit EditorPref");
            window.InitInputValues(aPref);
            window.InitButtonTextures();

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
            DrawKeyField();
            DrawType();
            DrawValueField();
            DrawEditButton();
        }


        /// Draw the background texture.
        private void DrawBG()
        {
            EditorGUI.DrawRect(new Rect(0,0, position.width, position.height), Preferences.Color_Primary);
        }


        /// Draw key input field.
        private void DrawKeyField()
        {
            var labelRect = new Rect(10, 10, Mathf.Clamp(position.width - 20, 80, position.width), 16);
            EditorGUI.LabelField(labelRect, "Key:", style_bold);

            var keyRect = new Rect(10, 29, Mathf.Clamp(position.width - 20, 80, position.width), 32);
            _key = EditorGUI.TextField(keyRect, _key);
        }


        /// Draw type popup.
        private void DrawType()
        {
            var labelRect = new Rect(10, 71, Mathf.Clamp(position.width - 20, 80, position.width), 16);
            EditorGUI.LabelField(labelRect, "Type:", style_bold);

            var typeRect = new Rect(10, 90, position.width - 20, 20);
            if(Preferences.ButtonsDisplay == ButtonsDisplayFormat.REGULAR_BUTTONS)
            {
                GUI.skin = skin_default;
                _type = GUI.SelectionGrid(typeRect, _type, arr_prefTypes, arr_prefTypes.Length);
            }
            else
            {
                GUI.skin = skin_custom;
                _type = GUI.SelectionGrid(typeRect, _type, arr_prefTypes, arr_prefTypes.Length, style_customGrid);
            }
        }


        /// Draw value input field.
        private void DrawValueField()
        {
            var labelRect = new Rect(10, 118, Mathf.Clamp(position.width - 20, 80, position.width), 16);
            EditorGUI.LabelField(labelRect, "Value:", style_bold);

            switch (_type)
            {
                case 0:
                    var boolRect = new Rect(10, 137, 130, 20);
                    if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.REGULAR_BUTTONS)
                    {
                        GUI.skin = skin_default;
                        idx_bool = GUI.SelectionGrid(boolRect, idx_bool, arr_boolValues, arr_boolValues.Length);
                    }
                    else
                    {
                        GUI.skin = skin_custom;
                        idx_bool = GUI.SelectionGrid(boolRect, idx_bool, arr_boolValues, arr_boolValues.Length, style_customGrid);
                    }
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


        /// Draw Edit button based on preferences.
        private void DrawEditButton()
        {
            Rect editRect;
            GUIContent editContent;

            switch (Preferences.ButtonsDisplay)
            {
                case ButtonsDisplayFormat.REGULAR_BUTTONS:
                    Button_Edit_default(out editRect, out editContent);
                    break;
                case ButtonsDisplayFormat.COOL_ICONS:
                default:
                    Button_Edit_icon(out editRect, out editContent);
                    break;
            }

            if (GUI.Button(editRect, editContent))
            {
                if (_key == "")
                {
                    EditorUtility.DisplayDialog("No key to use", "Please add a key.", "Ok");
                }
                else
                {
                    NewEditorPrefs.DeleteKey(_originalPref.Key);

                    switch (_type)
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
                }

                if (WindowMain.IsOpen)
                {
                    EditorWindow.GetWindow(typeof(WindowMain)).Repaint();
                }
                EditorWindow.GetWindow(typeof(WindowEdit)).Close();
            }
        }


        /// Create rect and content for default Add.
        private void Button_Edit_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(Mathf.Clamp((Screen.width / 2) - ButtonWidth/2, 0, position.width), 179, ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Save", "Save changes");
        }


        /// Create rect and content for icon Add.
        private void Button_Edit_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect(Mathf.Clamp((Screen.width / 2) - IconSize/2, 0, position.width), 179, IconSize, IconSize);
            aContent = new GUIContent(t_edit, "Save changes");
        }


        /// Set the values of the original pref for input fields.
        private void InitInputValues(Pref aPref)
        {
            _originalPref = aPref;
            _key = _originalPref.Key;
            _type = (int)_originalPref.Type;
            switch (_type)
            {
                case 0:
                    val_bool = aPref.Value == "True" ? true : false;
                    idx_bool = val_bool == true ? 1 : 0;
                    break;
                case 1:
                    val_int = int.Parse(aPref.Value);
                    break;
                case 2:
                    val_float = float.Parse(aPref.Value);
                    break;
                case 3:
                    val_string = aPref.Value;
                    break;
            }
        }


        /// Initialize textures (so that they're not instantiated every frame in OnGUI).
        private void InitButtonTextures()
        {
            t_edit = Resources.Load(Constants.FILE_GDTB_EDIT, typeof(Texture2D)) as Texture2D;
        }


        /// Remove textures from memory when not needed anymore.
        private void OnDestroy()
        {
            Resources.UnloadUnusedAssets();
        }
    }
}