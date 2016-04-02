using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GDTB.EditorPrefsEditor
{
    public class WindowMain : EditorWindow
    {
        public static List<Pref> Prefs = new List<Pref>();
        private static List<Pref> _oldPrefs;

        private GUISkin _skin, _defaultSkin;
        private GUIStyle _typeStyle, _keyStyle, _valueStyle;

        // ========================= Editor layouting =========================
        private const int IconSize = 16;

        private int _offset = 5;

        private int _typeWidth, _prefsWidth, _buttonsWidth;
        private int _typeLabelWidth;
        private float _heightIndex = 0;
        private Vector2 _scrollPosition = new Vector2(Screen.width - 5, Screen.height);
        private Rect _scrollViewRect, _scrollRect, _typeRect, _keyValueRect, _buttonsRect;

        // ====================================================================
        [MenuItem("Window/Gamedev Toolbelt/EditorPrefs Editor")]
        public static void Init()
        {
            // Get existing open window or if none, make a new one.
            var window = (WindowMain)EditorWindow.GetWindow(typeof(WindowMain));
            window.titleContent = new GUIContent("EditorPrefs");
            window.minSize = new Vector2(250f, 100f);
            window._typeLabelWidth = (int)window._typeStyle.CalcSize(new GUIContent("String")).x; // Not with the other layouting sizes because it only needs to be done once.
            window.UpdateLayoutingSizes();

            PrefManager.RefreshPrefs();

            //window.DebugPrefs();
            window.Show();
        }


        public void OnEnable()
        {
            LoadSkin();
            LoadStyles();
        }


        private void OnGUI()
        {
            UpdateLayoutingSizes();
            if (_defaultSkin == null)
            {
                _defaultSkin = GUI.skin;
            }
            GUI.skin = _skin;

            // If the list is clean (for instance because we just recompiled) load QQQs based on preferences.
            if (Prefs.Count == 0)
            {
                Prefs.Clear();
                Prefs.AddRange(IO.LoadStoredPrefs());
            }

            DrawBG();
            DrawPrefs();
            DrawSeparator();
            DrawAddButton();
            DrawGetButton();
            DrawRefreshButton();
        }


        /// Draw the background texture.
        private void DrawBG()
        {
            EditorGUI.DrawRect(new Rect(0,0, position.width, position.height), Constants.COLOR_UI_ACCENT);
        }


        /// Draw preferences.
        private void DrawPrefs()
        {
            _scrollViewRect.height = _heightIndex - _offset;
            _scrollRect.width += IconSize;
            _scrollPosition = GUI.BeginScrollView(_scrollRect, _scrollPosition, _scrollViewRect);
            _heightIndex = _offset;
            for (var i = 0; i < Prefs.Count; i++)
            {
                var key = new GUIContent(Prefs[i].Key);
                var val = new GUIContent(Prefs[i].Value);
                var keyHeight = _keyStyle.CalcHeight(key, _prefsWidth);
                var valHeight = _valueStyle.CalcHeight(val, _prefsWidth);

                var prefBGHeight = keyHeight + valHeight + Constants.LINE_HEIGHT + _offset;
                prefBGHeight = prefBGHeight < IconSize * 2.5f ? IconSize * 2.5f : prefBGHeight;

                _keyValueRect = new Rect(_offset, _heightIndex, _prefsWidth, prefBGHeight);
                _typeRect = new Rect(-2, _keyValueRect.y, _typeWidth, prefBGHeight);
                _buttonsRect = new Rect(_prefsWidth + (_offset * 2), _keyValueRect.y, _buttonsWidth, prefBGHeight);

                var prefBGRect = _keyValueRect;
                prefBGRect.height = prefBGHeight - _offset;
                prefBGRect.width = position.width - (IconSize * 2) + 2;

                _heightIndex += (int)prefBGHeight + _offset;

                DrawPrefBG(prefBGRect);
                DrawType(_typeRect, Prefs[i]);
                DrawKeyAndValue(_keyValueRect, Prefs[i], keyHeight);
                DrawEditAndRemoveButtons(_buttonsRect, Prefs[i]);
            }
            GUI.EndScrollView();
        }


        /// Draw the rectangle that separates the prefs visually.
        private void DrawPrefBG(Rect aRect)
        {
            EditorGUI.DrawRect(new Rect(aRect.x,    aRect.y,    2,    aRect.height), Color.white);
            EditorGUI.DrawRect(new Rect(aRect.x + aRect.width - 2,      aRect.y,    2,    aRect.height), Color.white);
            EditorGUI.DrawRect(new Rect(aRect.x,    aRect.y,    aRect.width,    2),Color.white);
            EditorGUI.DrawRect(new Rect(aRect.x,    aRect.y + aRect.height - 2,    aRect.width,    2), Color.white);
        }


        /// Draw the pref's type.
        private void DrawType(Rect aRect, Pref aPref)
        {
            var typeRect = aRect;
            typeRect.width -= _offset;
            typeRect.height -= (IconSize / 2 + _offset);

            var newX = (int)typeRect.x + IconSize;
            var newY = (int)typeRect.y + _offset;
            typeRect.position = new Vector2(newX, newY);

            var type = aPref.Type.ToString().ToLower();
            type = type.Substring(0, 1).ToUpper() + type.Substring(1);
            EditorGUI.LabelField(typeRect, type, _typeStyle);
        }


        private void DrawKeyAndValue(Rect aRect, Pref aPref, float aHeight)
        {
            // Key.
            var keyRect = aRect;
            keyRect.x = _typeWidth + IconSize / 2;
            keyRect.y += _offset;
            keyRect.height = aHeight;
            EditorGUI.LabelField(keyRect, aPref.Key, _keyStyle);

            // Value.
            var valueRect = aRect;
            valueRect.x = _typeWidth + IconSize / 2;
            valueRect.y = aRect.y + aHeight + (_offset * 1.5f);

            EditorGUI.LabelField(valueRect, aPref.Value, _valueStyle);
        }


        /// Draw the "Edit" and "Remove" buttons.
        private void DrawEditAndRemoveButtons(Rect aRect, Pref aPref)
        {
            // "Edit" button.
            var editRect = aRect;
            editRect.x = position.width - (IconSize * 3) + 2;
            editRect.y += _offset;
            editRect.width = IconSize;
            editRect.height = IconSize;

            var editButton = new GUIContent(Resources.Load(Constants.FILE_GDTB_EDIT, typeof(Texture2D)) as Texture2D, "Edit this EditorPref");
            if (GUI.Button(editRect, editButton))
            {
                WindowEdit.Init(aPref);
            }

            // "Remove" button.
            var removeRect = editRect;
            removeRect.y = editRect.y + editRect.height + 2;

            var removeButton = new GUIContent(Resources.Load(Constants.FILE_GDTB_REMOVE, typeof(Texture2D)) as Texture2D, "Remove this EditorPref");
            if (GUI.Button(removeRect, removeButton))
            {
                if (EditorUtility.DisplayDialog("Remove EditorPref", "Are you sure you want to remove this EditorPref?", "Remove pref", "Cancel"))
                {
                    NewEditorPrefs.DeleteKey(aPref.Key);
                }
            }
        }


        /// Draw a white line separating scrollview and lower buttons.
        private void DrawSeparator()
        {
            var separator = new Rect(0, position.height - (IconSize * 2), position.width, 1);
            EditorGUI.DrawRect(separator, Color.white);
        }


        /// Draw the "Add" button.
        private void DrawAddButton()
        {
            var addRect = new Rect((position.width / 2 - IconSize * 2), position.height - (IconSize * 1.5f), IconSize, IconSize);
            var addButton = new GUIContent(Resources.Load(Constants.FILE_GDTB_ADD, typeof(Texture2D)) as Texture2D, "Add a new key");

            if (GUI.Button(addRect, addButton))
            {
                WindowAdd.Init();
            }
        }


        /// Draw the "Get" button.
        private void DrawGetButton()
        {
            var getRect = new Rect((position.width / 2 - IconSize * 0.5f), position.height - (IconSize * 1.5f), IconSize, IconSize);
            var getButton = new GUIContent(Resources.Load(Constants.FILE_GDTB_GET, typeof(Texture2D)) as Texture2D, "Add existing key");

            if (GUI.Button(getRect, getButton))
            {
                EditorPrefsEditor.WindowGet.Init();
            }
        }


        /// Draw the "Refresh" button.
        private void DrawRefreshButton()
        {
            var refreshRect = new Rect((position.width / 2 + IconSize * 1), position.height - (IconSize * 1.5f), IconSize, IconSize);
            var refreshButton = new GUIContent(Resources.Load(Constants.FILE_GDTB_REFRESH, typeof(Texture2D)) as Texture2D, "Refresh list");

            if (GUI.Button(refreshRect, refreshButton))
            {
                PrefManager.RefreshPrefs();
            }
        }


        private void UpdateLayoutingSizes()
        {
            var width = position.width - IconSize;

            _scrollRect = new Rect(_offset, _offset, width - (_offset * 2), position.height - IconSize - _offset * 4);

            _scrollViewRect = _scrollRect;

            _typeWidth = _typeLabelWidth + (_offset * 2);
            _buttonsWidth = (IconSize * 2) + 5;
            _prefsWidth = (int)width - _typeWidth - _buttonsWidth - (_offset * 2);
        }


        /// Load the EPEditor skin.
        private void LoadSkin()
        {
            _skin = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        }


        /// Assign the GUI Styles
        private void LoadStyles()
        {
            _typeStyle = _skin.GetStyle("GDTB_EPEditor_type");
            _keyStyle = _skin.GetStyle("GDTB_EPEditor_key");
            _valueStyle = _skin.GetStyle("GDTB_EPEditor_value");
        }


        private void DebugPrefs()
        {
            for (var i = 0; i < Prefs.Count; i++)
            {
                Debug.Log("[" + i + "] Type: " + Prefs[i].Type + ", Key: " + Prefs[i].Key + ", Value: " + Prefs[i].Value);
                Debug.Log("Key exists: " + EditorPrefs.HasKey(Prefs[i].Key));

                if (Prefs[i].Type == PrefType.BOOL)
                {
                    Debug.Log("EditorPref value: " + EditorPrefs.GetBool(Prefs[i].Key));
                }
                else if (Prefs[i].Type == PrefType.INT)
                {
                    Debug.Log("EditorPref value: " + EditorPrefs.GetInt(Prefs[i].Key));
                }
                else if (Prefs[i].Type == PrefType.FLOAT)
                {
                    Debug.Log("EditorPref value: " + EditorPrefs.GetFloat(Prefs[i].Key));
                }
                else if (Prefs[i].Type == PrefType.STRING)
                {
                    Debug.Log("EditorPref value: " + EditorPrefs.GetString(Prefs[i].Key));
                }
            }
        }
    }
}