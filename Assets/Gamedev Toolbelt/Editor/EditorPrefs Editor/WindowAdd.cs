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

        private GUISkin _GDTBSkin;

        public static void Init()
        {
            WindowAdd window = (WindowAdd)EditorWindow.GetWindow(typeof(WindowAdd));
            window.minSize = new Vector2(200, 207);
            window.titleContent = new GUIContent("Add EditorPref");
            window.ShowUtility();
        }

        public void OnEnable()
        {
            _GDTBSkin = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        }

        public void OnGUI()
        {
            DrawKeyField();
            DrawTypePopup();
            DrawValueField();
            DrawButton();
        }


        /// Draw key input field.
        private void DrawKeyField()
        {
            var labelRect = new Rect(10, 10, Mathf.Clamp(position.width - 20, 80, 500), 16);
            EditorGUI.LabelField(labelRect, "Write the key:", EditorStyles.boldLabel);

            var keyRect = new Rect(10, 29, Mathf.Clamp(position.width - 20, 80, 500), 32);
            _key = EditorGUI.TextField(keyRect, _key);
        }


        /// Draw type popup.
        private void DrawTypePopup()
        {
            var labelRect = new Rect(10, 71, Mathf.Clamp(position.width - 20, 80, 500), 16);
            EditorGUI.LabelField(labelRect, "Choose a type:", EditorStyles.boldLabel);

            var priorityRect = new Rect(10, 90, 70, 16);
            _type = EditorGUI.Popup(priorityRect, _type, _prefTypes);
        }


        /// Draw value input field.
        private void DrawValueField()
        {
            var labelRect = new Rect(10, 116, Mathf.Clamp(position.width - 20, 80, 500), 16);
            EditorGUI.LabelField(labelRect, "Insert the value:", EditorStyles.boldLabel);

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
                    var stringRect = new Rect(10, 135, Mathf.Clamp(position.width - 20, 80, 500), 32);
                    _stringValue = EditorGUI.TextField(stringRect, _stringValue);
                    break;
            }
        }


        /// Draw "Add pref" button.
        private void DrawButton()
        {
            GUI.skin = _GDTBSkin;
            Pref currentPref = null;

            var buttonRect = new Rect(Mathf.Clamp((Screen.width / 2) - 60, 0, 190), 177, 120, 20);

            if (GUI.Button(buttonRect, "Add EditorPref"))
            {

                if (_key == "")
                {
                    EditorUtility.DisplayDialog("No key to add", "Please add a key.", "Ok");
                }

                // If the key exists.
                else if (GDTBEditorPrefs.HasKey(_key))
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
                            bValue = GDTBEditorPrefs.GetBool(_key);
                            currentPref = new Pref(PrefType.BOOL, _key, bValue.ToString());
                            Utils.AddPref(currentPref);
                        }
                        catch (System.Exception) { }

                        try
                        {
                            iValue = GDTBEditorPrefs.GetInt(_key);
                            currentPref = new Pref(PrefType.BOOL, _key, iValue.ToString());
                            Utils.AddPref(currentPref);
                        }
                        catch (System.Exception) { }

                        try
                        {
                            fValue = GDTBEditorPrefs.GetFloat(_key);
                            currentPref = new Pref(PrefType.BOOL, _key, fValue.ToString());
                            Utils.AddPref(currentPref);
                        }
                        catch (System.Exception) { }

                        try
                        {
                            sValue = GDTBEditorPrefs.GetString(_key);
                            currentPref = new Pref(PrefType.BOOL, _key, sValue);
                            Utils.AddPref(currentPref);
                        }
                        catch (System.Exception) { }
                    }

                    // Does the user want to edit the already existing key?
                    if (currentPref != null)
                    {
                        if (EditorUtility.DisplayDialog("Pref already exists.", "The key you're trying to use already exists.\nDo you want to edit it?", "Edit", "Cancel"))
                        {
                            EditorWindow.GetWindow(typeof(WindowAdd)).Close();
                            WindowEdit.Init(currentPref);
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
                                Utils.AddPref(_key, _boolValue);
                                break;
                            case 1:
                                Utils.AddPref(_key, _intValue);
                                break;
                            case 2:
                                Utils.AddPref(_key, _floatValue);
                                break;
                            case 3:
                                Utils.AddPref(_key, _stringValue);
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