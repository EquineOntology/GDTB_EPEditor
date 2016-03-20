using UnityEngine;
using UnityEditor;
namespace GDTB.EditorPrefsEditor
{
    public class WindowAdd : EditorWindow
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

        public static void Init()
        {
            WindowAdd window = (WindowAdd)EditorWindow.GetWindow(typeof(WindowAdd));
            window.minSize = new Vector2(275, 207);
            window.titleContent = new GUIContent("Add EditorPref");
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
            var labelRect = new Rect(10, 10, position.width - 20, 16);
            EditorGUI.LabelField(labelRect, "Write the key:", _boldStyle);

            var keyRect = new Rect(10, 29, position.width - 20, 32);
            _key = EditorGUI.TextField(keyRect, _key);
        }


        /// Draw type popup.
        private void DrawTypePopup()
        {
            var labelRect = new Rect(10, 71, Mathf.Clamp(position.width - 20, 80, 500), 16);
            EditorGUI.LabelField(labelRect, "Choose a type:", _boldStyle);

            var typeRect = new Rect(10, 90, position.width - 20, 20);
            _type = GUI.SelectionGrid(typeRect, _type, _prefTypes, _prefTypes.Length, _gridStyle);
        }


        /// Draw value input field.
        private void DrawValueField()
        {
            var labelRect = new Rect(10, 118, Mathf.Clamp(position.width - 20, 80, 500), 16);
            EditorGUI.LabelField(labelRect, "Insert the value:", _boldStyle);

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
            Pref currentPref = null;

            var buttonRect = new Rect(Mathf.Clamp((Screen.width / 2) - 60, 0, position.width), 177, 120, 20);

            if (GUI.Button(buttonRect, "Add EditorPref"))
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
                        catch (System.Exception) {}

                        try
                        {
                            iValue = NewEditorPrefs.GetInt(_key);
                            currentPref = new Pref(PrefType.INT, _key, iValue.ToString());
                        }
                        catch (System.Exception) {}

                        try
                        {
                            fValue = NewEditorPrefs.GetFloat(_key);
                            currentPref = new Pref(PrefType.FLOAT, _key, fValue.ToString());
                        }
                        catch (System.Exception) {}

                        try
                        {
                            sValue = NewEditorPrefs.GetString(_key);
                            currentPref = new Pref(PrefType.STRING, _key, sValue);
                        }
                        catch (System.Exception) {}
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
                            PrefManager.AddPref(currentPref);
                        }
                    }
                }
                else
                {
                    if (EditorUtility.DisplayDialog("Add editor preference?", "Are you sure you want to add this key to EditorPrefs?", "Add key", "Cancel"))
                    {
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
                        EditorWindow.GetWindow(typeof(WindowAdd)).Close();
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("No key added", "There was an unknown issue when adding your key.\nPlease try again", "Ok");
                    }
                }
            }
        }
    }
}