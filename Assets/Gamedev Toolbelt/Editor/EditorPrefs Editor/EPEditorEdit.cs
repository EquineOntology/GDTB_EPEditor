using UnityEngine;
using UnityEditor;
namespace GDTB.EditorPrefsEditor
{
    public class EPEditorEdit : EditorWindow
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

        private GUISkin _GDTBSkin;
        private GUISkin _defaultSkin;
        private GUIStyle _wrappingStyle;

        private EditorPref _originalPref;

        public static void Init(EditorPref aPref)
        {
            EPEditorEdit window = (EPEditorEdit)EditorWindow.GetWindow(typeof(EPEditorEdit));
            window.minSize = new Vector2(200, 207);
            window.titleContent = new GUIContent("Edit EditorPref");
            window.InitInputValues(aPref);

            window.ShowUtility();
        }

        public void OnEnable()
        {
            _GDTBSkin = Resources.Load(EPConstants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
            _wrappingStyle = _GDTBSkin.GetStyle("textArea");
        }

        public void OnGUI()
        {
            if (_defaultSkin == null)
            {
                _defaultSkin = GUI.skin;
            }
            DrawKeyField();
            DrawTypePopup();
            DrawValueField();
            DrawButton();
        }


        /// Draw key input field.
        private void DrawKeyField()
        {
            var labelRect = new Rect(10, 10, Mathf.Clamp(position.width - 20, 80, position.width), 16);
            EditorGUI.LabelField(labelRect, "Key:", EditorStyles.boldLabel);

            var keyRect = new Rect(10, 29, Mathf.Clamp(position.width - 20, 80, position.width), 32);
            _key = EditorGUI.TextField(keyRect, _key, _wrappingStyle);
        }


        /// Draw type popup.
        private void DrawTypePopup()
        {
            var labelRect = new Rect(10, 71, Mathf.Clamp(position.width - 20, 80, position.width), 16);
            EditorGUI.LabelField(labelRect, "Type:", EditorStyles.boldLabel);

            var priorityRect = new Rect(10, 90, 70, 16);
            _type = EditorGUI.Popup(priorityRect, _type, _prefTypes);
        }


        /// Draw value input field.
        private void DrawValueField()
        {
            var labelRect = new Rect(10, 116, Mathf.Clamp(position.width - 20, 80, position.width), 16);
            EditorGUI.LabelField(labelRect, "Value:", EditorStyles.boldLabel);

            switch (_type)
            {
                case 0:
                    var boolRect = new Rect(10, 135, 150, 16);
                    _boolIndex = EditorGUI.Popup(boolRect, _boolIndex, _boolValues);
                    _boolValue = _boolIndex == 0 ? false : true;
                    break;
                case 1:
                    var intRect = new Rect(10, 135, 70, 16);
                    _intValue = EditorGUI.IntField(intRect, _intValue);
                    break;
                case 2:
                    var floatRect = new Rect(10, 135, 70, 16);
                    _floatValue = EditorGUI.FloatField(floatRect, _floatValue);
                    break;
                case 3:
                    var stringRect = new Rect(10, 135, Mathf.Clamp(position.width - 20, 80, position.width), 32);
                    _stringValue = EditorGUI.TextField(stringRect, _stringValue, _wrappingStyle);
                    break;
            }
        }


        /// Draw "Add pref" button.
        private void DrawButton()
        {
            GUI.skin = _GDTBSkin;

            var buttonRect = new Rect(Mathf.Clamp((Screen.width / 2) - 60, 0, 190), 177, 120, 20);

            if (GUI.Button(buttonRect, "Edit EditorPref"))
            {
                if (_key == "")
                {
                    EditorUtility.DisplayDialog("No key to use", "Please add a key.", "Ok");
                }
                else
                {
                    switch (_type)
                    {
                        case 0:
                            GDTBEditorPrefs.SetBool(_key, _boolValue);
                            break;
                        case 1:
                            GDTBEditorPrefs.SetInt(_key, _intValue);
                            break;
                        case 2:
                            GDTBEditorPrefs.SetFloat(_key, _floatValue);
                            break;
                        case 3:
                            GDTBEditorPrefs.SetString(_key, _stringValue);
                            break;
                    }
                    if (_key != _originalPref.Key)
                    {
                        GDTBEditorPrefs.DeleteKey(_originalPref.Key);
                    }
                }
                EditorWindow.GetWindow(typeof(EPEditorEdit)).Close();
                EditorWindow.GetWindow(typeof(EPEditor)).Repaint();
            }
        }


        /// Set the values of the original pref for input fields.
        private void InitInputValues(EditorPref aPref)
        {
            _originalPref = aPref;
            _key = _originalPref.Key;
            _type = (int)_originalPref.Type;
            switch (_type)
            {
                case 0:
                    _boolValue = aPref.Value == "true" ? true : false;
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