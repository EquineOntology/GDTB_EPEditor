using UnityEngine;
using UnityEditor;
namespace GDTB.EditorPrefsEditor
{
    public class WindowEdit : EditorWindow
    {
        private string _key = "";
        private int _type = 0;
        private string[] _prefTypes = { "Bool", "Int", "Float", "String" };

        private bool _boolValue = false;
        private int _boolIndex = 0;
        private string[] _boolValues = { "False", "True" };
        private int _intValue = 0;
        private float _floatValue = 0.0f;
        private string _stringValue = "";

        private GUISkin _skin;
        private GUIStyle _boldStyle, _gridStyle;

        private Pref _originalPref;

        public static void Init(Pref aPref)
        {
            WindowEdit window = (WindowEdit)EditorWindow.GetWindow(typeof(WindowEdit));
            window.minSize = new Vector2(275, 209);
            window.titleContent = new GUIContent("Edit EditorPref");
            window.InitInputValues(aPref);

            window.ShowUtility();
        }

        public void OnEnable()
        {
            _skin = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
            _boldStyle = _skin.GetStyle("GDTB_EPEditor_key");
            _gridStyle = _skin.GetStyle("GDTB_EPEditor_selectionGrid");
        }

        public void OnGUI()
        {
            DrawBG();
            DrawKeyField();
            DrawTypePopup();
            DrawValueField();
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
            var labelRect = new Rect(10, 10, Mathf.Clamp(position.width - 20, 80, position.width), 16);
            EditorGUI.LabelField(labelRect, "Key:", _boldStyle);

            var keyRect = new Rect(10, 29, Mathf.Clamp(position.width - 20, 80, position.width), 32);
            _key = EditorGUI.TextField(keyRect, _key);
        }


        /// Draw type popup.
        private void DrawTypePopup()
        {
            var labelRect = new Rect(10, 71, Mathf.Clamp(position.width - 20, 80, position.width), 16);
            EditorGUI.LabelField(labelRect, "Type:", _boldStyle);

            var typeRect = new Rect(10, 90, position.width - 20, 20);
            _type = GUI.SelectionGrid(typeRect, _type, _prefTypes, _prefTypes.Length, _gridStyle);
        }


        /// Draw value input field.
        private void DrawValueField()
        {
            var labelRect = new Rect(10, 118, Mathf.Clamp(position.width - 20, 80, position.width), 16);
            EditorGUI.LabelField(labelRect, "Value:", _boldStyle);

            switch (_type)
            {
                case 0:
                    var boolRect = new Rect(10, 137, 130, 20);
                    _boolIndex = GUI.SelectionGrid(boolRect, _boolIndex, _boolValues, _boolValues.Length, _gridStyle);
                    _boolValue = _boolIndex == 0 ? false : true;
                    break;
                case 1:
                    var intRect = new Rect(10, 137, position.width - 20, 16);
                    _intValue = EditorGUI.IntField(intRect, _intValue);
                    break;
                case 2:
                    var floatRect = new Rect(10, 137, position.width - 20, 16);
                    _floatValue = EditorGUI.FloatField(floatRect, _floatValue);
                    break;
                case 3:
                    var stringRect = new Rect(10, 137, position.width - 20, 32);
                    _stringValue = EditorGUI.TextField(stringRect, _stringValue);
                    break;
            }
        }


        /// Draw "Add pref" button.
        private void DrawButton()
        {
            GUI.skin = _skin;

            var buttonRect = new Rect(Mathf.Clamp((Screen.width / 2) - 60, 0, position.width), 179, 120, 20);

            if (GUI.Button(buttonRect, "Edit EditorPref"))
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
                            NewEditorPrefs.SetBool(_key, _boolValue);
                            break;
                        case 1:
                            NewEditorPrefs.SetInt(_key, _intValue);
                            break;
                        case 2:
                            NewEditorPrefs.SetFloat(_key, _floatValue);
                            break;
                        case 3:
                            NewEditorPrefs.SetString(_key, _stringValue);
                            break;
                    }
                }
                EditorWindow.GetWindow(typeof(WindowEdit)).Close();
                EditorWindow.GetWindow(typeof(WindowMain)).Repaint();
            }
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
                    _boolValue = aPref.Value == "True" ? true : false;
                    _boolIndex = _boolValue == true ? 1 : 0;
                    break;
                case 1:
                    _intValue = int.Parse(aPref.Value);
                    break;
                case 2:
                    _floatValue = float.Parse(aPref.Value);
                    break;
                case 3:
                    _stringValue = aPref.Value;
                    break;
            }
        }
    }
}