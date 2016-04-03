using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GDTB.EditorPrefsEditor
{
    public class WindowMain : EditorWindow
    {
        public static List<Pref> Prefs = new List<Pref>();

        public static WindowMain Instance { get; private set; }
        public static bool IsOpen {
            get { return Instance != null; }
        }

        private GUISkin _customSkin, _defaultSkin;
        private GUIStyle _typeStyle, _keyStyle, _valueStyle;

        // ========================= Editor layouting =========================
        private const int IconSize = 16;
        private const int ButtonWidth = 60;
        private const int ButtonHeight = 18;

        private int _offset = 5;

        private int _typeWidth, _prefsWidth, _buttonsWidth;
        private int _typeLabelWidth;
        private float _heightIndex = 0;
        private Vector2 _scrollPosition = new Vector2(Screen.width - 5, Screen.height);
        private Rect _scrollViewRect, _scrollRect, _typeRect, _keyValueRect, _buttonsRect;

        // ====================================================================
        [MenuItem("Window/Gamedev Toolbelt/EditorPrefs Editor %q")]
        public static void Init()
        {
            // Get existing open window or if none, make a new one.
            var window = (WindowMain)EditorWindow.GetWindow(typeof(WindowMain));
            window.titleContent = new GUIContent("EditorPrefs");
            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                window.minSize = new Vector2(221f, 150f);
            }
            else
            {
                window.minSize = new Vector2(321f, 150f);
            }
            window._typeLabelWidth = (int)window._typeStyle.CalcSize(new GUIContent("String")).x; // Not with the other layouting sizes because it only needs to be done once.
            window.UpdateLayoutingSizes();

            PrefOps.RefreshPrefs();

            //window.DebugPrefs();
            window.Show();
        }


        public void OnEnable()
        {
            Instance = this;
            Preferences.GetAllPrefValues();
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
            GUI.skin = _customSkin;

            // If the list is clean (for instance because we just recompiled) load Prefs again.
            if (Prefs.Count == 0)
            {
                Prefs.Clear();
                Prefs.AddRange(IO.LoadStoredPrefs());
            }

            DrawBG();

            // If the list is still clean after the above, then we really have no Prefs to show.
            if (Prefs.Count == 0)
            {
                DrawNoPrefsMessage();
            }

            DrawPrefs();
            DrawSeparator();
            DrawBottomButtons();
        }


        /// Draw the background texture.
        private void DrawBG()
        {
            EditorGUI.DrawRect(new Rect(0,0, position.width, position.height), Constants.COLOR_UI_BG);
        }


        /// Draw a message in center screen warning the user they have no prefs.
        private void DrawNoPrefsMessage()
        {
            var label = "There are currently no EditorPrefs loaded.\nYou can add a new EditorPref or\nget an existing one with the buttons below.\n\nIf you see this after the project recompiled,\ntry refreshing the window!\nYour prefs should come back just fine.";
            var labelContent = new GUIContent(label);
            var labelSize = EditorStyles.centeredGreyMiniLabel.CalcSize(labelContent);
            var labelRect = new Rect(position.width / 2 - labelSize.x / 2, position.height / 2 - labelSize.y / 2 - _offset * 2.5f, labelSize.x, labelSize.y);
            EditorGUI.LabelField(labelRect, labelContent, EditorStyles.centeredGreyMiniLabel);
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
                if (Preferences.ButtonsDisplay.ToString() == "REGULAR_BUTTONS")
                {
                    prefBGHeight += 4;
                }

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
                DrawEditAndRemove(_buttonsRect, Prefs[i]);
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


        #region EditAndRemove
        /// Select which format to use based on the user preference.
        private void DrawEditAndRemove(Rect aRect, Pref aPref)
        {
            Rect editRect, removeRect;
            GUIContent editContent, removeContent;
            switch (Preferences.ButtonsDisplay)
            {
                case ButtonsDisplayFormat.REGULAR_BUTTONS:
                    GUI.skin = _defaultSkin;
                    CreateEditButton_Default(aRect, out editRect, out editContent);
                    CreateRemoveButton_Default(aRect, out removeRect, out removeContent);
                    break;
                default:
                    GUI.skin = _customSkin;
                    CreateEditButton_Icon(aRect, out editRect, out editContent);
                    CreateRemoveButton_Icon(aRect, out removeRect, out removeContent);
                    break;
            }

            if (GUI.Button(editRect, editContent))
            {
                WindowEdit.Init(aPref);
            }

            if (GUI.Button(removeRect, removeContent))
            {
                // Get confirmation through dialog (or not if the user doesn't want to).
                var canExecute = false;
                if (Preferences.ShowConfirmationDialogs == true)
                {
                    if (EditorUtility.DisplayDialog("Remove EditorPref", "Are you sure you want to remove this EditorPref?", "Remove pref", "Cancel"))
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
                    NewEditorPrefs.DeleteKey(aPref.Key);
                }
            }
        }


        /// Create rect and content for normal Edit button.
        private void CreateEditButton_Default(Rect aRect, out Rect anEditRect, out GUIContent anEditContent)
        {
            anEditRect = aRect;
            anEditRect.x = position.width - ButtonWidth - IconSize * 2 + 2;
            anEditRect.y += _offset;
            anEditRect.width = ButtonWidth;
            anEditRect.height = ButtonHeight;

            anEditContent = new GUIContent("Edit", "Edit this pref");
        }


        /// Create rect and content for normal Remove button.
        private void CreateRemoveButton_Default(Rect aRect, out Rect aRemoveRect, out GUIContent aRemoveContent)
        {
            aRemoveRect = aRect;
            aRemoveRect.x = position.width - ButtonWidth - IconSize * 2 + 2;
            aRemoveRect.y += ButtonHeight + _offset + 2;
            aRemoveRect.width = ButtonWidth;
            aRemoveRect.height = ButtonHeight;

            aRemoveContent = new GUIContent("Remove", "Remove this EditorPref");
        }


        /// Create rect and content for icon Edit button.
        private void CreateEditButton_Icon(Rect aRect, out Rect anEditRect, out GUIContent anEditContent)
        {
            anEditRect = aRect;
            anEditRect.x = position.width - (IconSize * 3) + 2;
            anEditRect.y += _offset;
            anEditRect.width = IconSize;
            anEditRect.height = IconSize;

            anEditContent = new GUIContent(Resources.Load(Constants.FILE_GDTB_EDIT, typeof(Texture2D)) as Texture2D, "Edit this EditorPref");
        }


        /// Create rect and content for icon Remove button.
        private void CreateRemoveButton_Icon(Rect aRect, out Rect aRemoveRect, out GUIContent aRemoveContent)
        {
            aRemoveRect = aRect;
            aRemoveRect.x = position.width - (IconSize * 3) + 2;
            aRemoveRect.y += IconSize +  _offset + 2;
            aRemoveRect.width = IconSize;
            aRemoveRect.height = IconSize;

            aRemoveContent = new GUIContent(Resources.Load(Constants.FILE_GDTB_REMOVE, typeof(Texture2D)) as Texture2D, "Remove this EditorPref");
        }
        #endregion


        /// Draw a white line separating scrollview and lower buttons.
        private void DrawSeparator()
        {
            var separator = new Rect(0, position.height - (IconSize * 2), position.width, 1);
            EditorGUI.DrawRect(separator, Color.white);
        }


        #region A-G-R buttons
        /// Draw Add, Get and Refresh based on preferences.
        private void DrawBottomButtons()
        {
            Rect addRect, getRect, refreshRect, settingsRect, nukeRect;
            GUIContent addContent, getContent, refreshContent, settingsContent, nukeContent;

            switch (Preferences.ButtonsDisplay)
            {
                case ButtonsDisplayFormat.REGULAR_BUTTONS:
                    GUI.skin = _defaultSkin;
                    CreateAddButton_Default(out addRect, out addContent);
                    CreateGetButton_Default(out getRect, out getContent);
                    CreateRefreshButton_Default(out refreshRect, out refreshContent);
                    CreateSettingsButton_Default(out settingsRect, out settingsContent);
                    CreateNukeButton_Default(out nukeRect, out nukeContent);
                    break;
                case ButtonsDisplayFormat.COOL_ICONS:
                default:
                    GUI.skin = _customSkin;
                    CreateAddButton_Icon(out addRect, out addContent);
                    CreateGetButton_Icon(out getRect, out getContent);
                    CreateRefreshButton_Icon(out refreshRect, out refreshContent);
                    CreateSettingsButton_Icon(out settingsRect, out settingsContent);
                    CreateNukeButton_Icon(out nukeRect, out nukeContent);
                    break;
            }

            // Add new pref.
            if (GUI.Button(addRect, addContent))
            {
                WindowAdd.Init();
            }

            // Get already existing pref.
            if (GUI.Button(getRect, getContent))
            {
                EditorPrefsEditor.WindowGet.Init();
            }

            // Refresh list of prefs.
            if (GUI.Button(refreshRect, refreshContent))
            {
                PrefOps.RefreshPrefs();
            }

            // Open settings.
            if (GUI.Button(settingsRect, settingsContent))
            {
                // Unfortunately EditorApplication.ExecuteMenuItem(...) doesn't work, so we have to rely on a bit of reflection.
                var asm = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
                var T = asm.GetType("UnityEditor.PreferencesWindow");
                var M = T.GetMethod("ShowPreferencesWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                M.Invoke(null, null);
            }

            // Nuke prefs.
            if (GUI.Button(nukeRect, nukeContent))
            {
                // Get confirmation through dialog (or not if the user doesn't want to).
                var canExecute = false;
                if (Preferences.ShowConfirmationDialogs == true)
                {
                    if (EditorUtility.DisplayDialog("Remove ALL EditorPrefs", "Are you sure ABSOLUTELY sure you want to remove ALL EditorPrefs currently set?\nThis is IRREVERSIBLE, only do this if you know what you're doing.", "Nuke EditorPrefs", "Cancel"))
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
                    NewEditorPrefs.DeleteAll();
                }
            }
        }


        /// Draw default Add.
        private void CreateAddButton_Default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - ButtonWidth * 2.5f - 8), position.height - (ButtonHeight * 1.5f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Add", "Add a new key");
        }

        /// Draw icon Add.
        private void CreateAddButton_Icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - IconSize * 2.5f - 10), position.height - (IconSize * 1.5f), IconSize, IconSize);
            aContent = new GUIContent(Resources.Load(Constants.FILE_GDTB_ADD, typeof(Texture2D)) as Texture2D, "Add a new key");
        }

        /// Draw default Get.
        private void CreateGetButton_Default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - ButtonWidth * 1.5f - 4), position.height - (ButtonHeight * 1.5f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Get", "Add existing key");
        }

        /// Draw icon Get.
        private void CreateGetButton_Icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - IconSize * 1.5f - 5), position.height - (IconSize * 1.5f), IconSize, IconSize);
            aContent = new GUIContent(Resources.Load(Constants.FILE_GDTB_GET, typeof(Texture2D)) as Texture2D, "Add existing key");
        }

        /// Draw default Refresh.
        private void CreateRefreshButton_Default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - ButtonWidth/2), position.height - (ButtonHeight * 1.5f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Refresh", "Refresh list");
        }

        /// Draw icon Refresh.
        private void CreateRefreshButton_Icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - IconSize/2), position.height - (IconSize * 1.5f), IconSize, IconSize);
            aContent = new GUIContent(Resources.Load(Constants.FILE_GDTB_REFRESH, typeof(Texture2D)) as Texture2D, "Refresh list");
        }

        /// Draw default Refresh.
        private void CreateSettingsButton_Default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 + ButtonWidth *0.5f  + 4), position.height - (ButtonHeight * 1.5f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Settings", "Open Settings");
        }

        /// Draw icon Refresh.
        private void CreateSettingsButton_Icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 + IconSize * 0.5f + 5), position.height - (IconSize * 1.5f), IconSize, IconSize);
            aContent = new GUIContent(Resources.Load(Constants.FILE_GDTB_SETTINGS, typeof(Texture2D)) as Texture2D, "Open Settings");
        }

        /// Draw default Nuke.
        private void CreateNukeButton_Default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 + ButtonWidth * 1.5f + 8), position.height - (ButtonHeight * 1.5f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Nuke all", "Delete ALL prefs from EditorPrefs");
        }

        /// Draw icon Nuke.
        private void CreateNukeButton_Icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 + IconSize * 1.5f + 10), position.height - (IconSize * 1.5f), IconSize, IconSize);
            aContent = new GUIContent(Resources.Load(Constants.FILE_GDTB_REMOVEALL, typeof(Texture2D)) as Texture2D, "Delete ALL prefs from EditorPrefs");
        }
        #endregion


        private void UpdateLayoutingSizes()
        {
            var width = position.width - IconSize;

            _scrollRect = new Rect(_offset, _offset, width - (_offset * 2), position.height - IconSize - _offset * 4);

            _scrollViewRect = _scrollRect;

            _typeWidth = _typeLabelWidth + (_offset * 2);
            // Same for buttons size
            if(Preferences.ButtonsDisplay.ToString() == "COOL_ICONS")
            {
                _buttonsWidth = (IconSize * 2) + 5;
            }
            else
            {
                _buttonsWidth = ButtonWidth + _offset * 3 - 2;
            }
            _prefsWidth = (int)width - _typeWidth - _buttonsWidth - (_offset * 2);
        }


        /// Load the EPEditor skin.
        private void LoadSkin()
        {
            _customSkin = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        }


        /// Assign the GUI Styles
        private void LoadStyles()
        {
            _typeStyle = _customSkin.GetStyle("GDTB_EPEditor_type");
            _keyStyle = _customSkin.GetStyle("GDTB_EPEditor_key");
            _valueStyle = _customSkin.GetStyle("GDTB_EPEditor_value");
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