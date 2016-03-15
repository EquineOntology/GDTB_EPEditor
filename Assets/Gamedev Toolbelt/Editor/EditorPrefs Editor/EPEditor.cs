using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GDTB.EditorPrefsEditor
{
    public class EPEditor : EditorWindow
    {
        public static List<EditorPref> Prefs = new List<EditorPref>();
        private static List<EditorPref> _oldPrefs;

        private GUISkin _epEditorSkin;
        private GUIStyle _typeStyle, _keyStyle, _valueStyle;

        // ========================= Editor layouting =========================
        private const int IconSize = 16;

        private int _typeWidth, _prefsWidth, _buttonsWidth;
        private int _offset = 5;

        private int _typeLabelWidth;
        private float _heightIndex = 0;
        private Vector2 _scrollPosition = new Vector2(Screen.width - 5, Screen.height);
        private Rect prefContent, _scrollViewRect, _scrollRect, _typeRect, _keyValueRect, _buttonsRect;

        // ====================================================================
        [MenuItem("Window/EditorPrefs Editor %]")]
        public static void Init()
        {
            // Get existing open window or if none, make a new one.
            var window = (EPEditor)EditorWindow.GetWindow(typeof(EPEditor));
            window.titleContent = new GUIContent("EditorPrefs Editor");
            window.minSize = new Vector2(250f, 100f);
            window._typeLabelWidth = (int)window._typeStyle.CalcSize(new GUIContent("String")).x; // Not with the other layouting sizes because it only needs to be done once.
            window.UpdateLayoutingSizes();

            // Restore stored preferences.
            var storedPrefs = EPEditorIO.LoadStoredPrefs();
            if (storedPrefs.Count >= Prefs.Count)
            {
                EPEditor.Prefs.Clear();
                EPEditor.Prefs.AddRange(storedPrefs);
            }

            window.Show();
        }


        private void OnGUI()
        {
            UpdateLayoutingSizes();
            CheckChangesToList();
            GUI.skin = _epEditorSkin;

            DrawPrefs();
            DrawAddButton();
            DrawGetButton();
        }


        public void OnEnable()
        {
            LoadSkin();
            LoadStyles();
        }


        /// Draw preferences.
        private void DrawPrefs()
        {
            _scrollViewRect.height = _heightIndex;
            _scrollRect.width += IconSize + 2;
            _scrollPosition = GUI.BeginScrollView(_scrollRect, _scrollPosition, _scrollViewRect);
            _heightIndex = _offset;
            for (var i = 0; i < Prefs.Count; i++)
            {
                var key = new GUIContent(Prefs[i].Key);
                var val = new GUIContent(Prefs[i].Value);
                var keyHeight = _keyStyle.CalcHeight(key, _prefsWidth);
                var valHeight = +_valueStyle.CalcHeight(val, _prefsWidth);

                var helpBoxHeight = keyHeight + valHeight + EPConstants.LINE_HEIGHT + 5;
                helpBoxHeight = helpBoxHeight < IconSize * 2.5f ? IconSize * 2.5f : helpBoxHeight;

                _keyValueRect = new Rect(_offset, _heightIndex, _prefsWidth, helpBoxHeight);
                _typeRect = new Rect(0, _keyValueRect.y, _typeWidth, helpBoxHeight);
                _buttonsRect = new Rect(_prefsWidth + (_offset * 2), _keyValueRect.y, _buttonsWidth, helpBoxHeight);

                var helpBoxRect = _keyValueRect;
                helpBoxRect.height = helpBoxHeight;
                helpBoxRect.width = position.width - (IconSize * 2) + 1;
                helpBoxRect.x += _offset;

                _heightIndex += (int)helpBoxHeight + _offset;
                _scrollViewRect.height = _heightIndex;

                DrawHelpBox(helpBoxRect);
                DrawType(_typeRect, Prefs[i]);
                DrawKeyAndValue(_keyValueRect, Prefs[i], keyHeight);
                DrawEditAndRemoveButtons(_buttonsRect, Prefs[i]);
            }
            GUI.EndScrollView();
        }


        /// Draw the "Help box" style rectangle that separates the prefs visually.
        private void DrawHelpBox(Rect aRect)
        {
            EditorGUI.LabelField(aRect, "", EditorStyles.helpBox);
        }


        /// Draw the pref's type.
        private void DrawType(Rect aRect, EditorPref aPref)
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


        private void DrawKeyAndValue(Rect aRect, EditorPref aPref, float aHeight)
        {
            // Key.
            var keyRect = aRect;
            keyRect.x = _typeWidth + IconSize;
            keyRect.y += _offset;
            keyRect.height = aHeight;
            EditorGUI.LabelField(keyRect, aPref.Key, _keyStyle);

            // Value.
            var valueRect = aRect;
            valueRect.x = _typeWidth + IconSize;
            valueRect.y = aRect.y + aHeight + _offset;

            EditorGUI.LabelField(valueRect, aPref.Value, _valueStyle);
        }


        /// Draw the "Edit" and "Remove" buttons.
        private void DrawEditAndRemoveButtons(Rect aRect, EditorPref aPref)
        {
            // "Edit" button.
            var editRect = aRect;
            editRect.x = position.width - (IconSize * 2) - (_offset * 2);
            editRect.y += _offset;
            editRect.width = IconSize;
            editRect.height = IconSize;

            var editButton = new GUIContent(Resources.Load(EPConstants.FILE_GDTB_EDIT, typeof(Texture2D)) as Texture2D, "Edit this EditorPref");

            // Open edit window on click.
            if (GUI.Button(editRect, editButton))
            {
                //GDTB_EPEditorEdit.Init(aPref);
            }

            // "Complete" button.
            var removeRect = editRect;
            removeRect.y = editRect.y + editRect.height + 2;

            var removeButton = new GUIContent(Resources.Load(EPConstants.FILE_GDTB_REMOVE, typeof(Texture2D)) as Texture2D, "Remove this EditorPref");

            // Complete QQQ on click.
            if (GUI.Button(removeRect, removeButton))
            {
                // Confirmation dialog.
                if (EditorUtility.DisplayDialog("Remove EditorPref", "Are you sure you want to delete this EditorPref?", "Delete pref", "Cancel"))
                {
                    EditorPrefs.DeleteKey(aPref.Key);
                    Prefs.Remove(aPref);
                    EPEditorIO.WritePrefsToFile();
                    EditorWindow.GetWindow(typeof(EPEditor)).Repaint();
                }
            }
        }


        /// Draw the "Add" button.
        private void DrawAddButton()
        {
            var addRect = new Rect((position.width / 2) - (IconSize * 1.5f) - _offset, position.height - (IconSize * 1.5f), IconSize, IconSize);
            var addButton = new GUIContent(Resources.Load(EPConstants.FILE_GDTB_ADD, typeof(Texture2D)) as Texture2D, "Add a new key");

            // Add QQQ on click.
            if (GUI.Button(addRect, addButton))
            {
                EPEditorAdd.Init();
            }
        }


        /// Draw the "Get key" button
        private void DrawGetButton()
        {
            var getRect = new Rect((position.width / 2) + (IconSize * 1.5f) - _offset, position.height - (IconSize * 1.5f), IconSize, IconSize);
            var getButton = new GUIContent(Resources.Load(EPConstants.FILE_GDTB_ADD, typeof(Texture2D)) as Texture2D, "Add existing key");

            // Get value and type on click.
            if (GUI.Button(getRect, getButton))
            {
                EPEditorGet.Init();
            }
        }


        private void UpdateLayoutingSizes()
        {
            var width = position.width - IconSize;

            _scrollRect = new Rect(_offset, _offset, width - (_offset * 2), position.height - IconSize * 2.5f);

            _scrollViewRect = _scrollRect;

            _typeWidth = _typeLabelWidth + (_offset * 2);
            _buttonsWidth = (IconSize * 2) + 5;
            _prefsWidth = (int)width - _typeWidth - _buttonsWidth - (_offset * 2);
        }

        /// Load the GDTB_GDTB_EPEditor skin.
        private void LoadSkin()
        {
            _epEditorSkin = Resources.Load(EPConstants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        }


        /// Assign the GUI Styles
        private void LoadStyles()
        {
            _typeStyle = _epEditorSkin.GetStyle("GDTB_EPEditor_type");
            _keyStyle = _epEditorSkin.GetStyle("GDTB_EPEditor_key");
            _valueStyle = _epEditorSkin.GetStyle("GDTB_EPEditor_value");
        }

        /// Called when the window is closed.
        private void OnDestroy()
        {
            EPEditorIO.WritePrefsToFile();
        }
    }
}