using UnityEngine;
using UnityEditor;

namespace GDTB.EditorPrefsEditor
{
    public class Preferences
    {
        #region fields
        // Buttons displayed as normal buttons or smaller icons.
        private const string PREFS_EPEDITOR_BUTTONS_DISPLAY = "GDTB_EPEditor_ButtonDisplay";
        private static ButtonsDisplayFormat _buttonsDisplay = ButtonsDisplayFormat.COOL_ICONS;
        private static int _buttonsDisplay_default = 1;
        private static ButtonsDisplayFormat _oldDisplayFormat;
        public static ButtonsDisplayFormat ButtonsDisplay
        {
            get { return _buttonsDisplay; }
        }

        private static string[] arr_buttonStyle = { "Cool icons", "Regular buttons" };

        // Confirmation dialogs
        private const string PREFS_EPEDITOR_CONFIRMATION_DIALOGS = "GDTB_EPEditor_ConfirmationDialogs";
        private static bool _confirmationDialogs = true;
        private static bool _confirmationDialogs_default = true;
        public static bool ShowConfirmationDialogs
        {
            get { return _confirmationDialogs; }
        }

        #region Colors
        // Style of icons (light or dark).
        private const string PREFS_EPEDITOR_ICON_STYLE = "GDTB_EPEditor_IconStyle";
        private static IconStyle _iconStyle = IconStyle.DARK;
        private static int _iconStyle_default = 0;
        private static IconStyle _oldIconStyle;
        public static IconStyle IconStyle
        {
            get { return _iconStyle; }
        }
        private static string[] arr_iconStyle = { "Dark", "Light" };

        // Primary color.
        private const string PREFS_EPEDITOR_COLOR_PRIMARY = "GDTB_EPEditor_Primary";
        private static Color _primary = new Color(56, 56, 56, 1);
        private static Color _primary_dark = new Color(56, 56, 56, 1);
        private static Color _primary_light = new Color(222, 222, 222, 1);//231, 231, 222, 1);
        private static Color _primary_default = new Color(56, 56, 56, 1);
        public static Color Color_Primary
        {
            get { return _primary; }
        }

        // Secondary color.
        private const string PREFS_EPEDITOR_COLOR_SECONDARY = "GDTB_EPEditor_Secondary";
        private static Color _secondary = new Color(55, 222, 179, 1);
        private static Color _secondary_dark = new Color(55, 222, 179, 1);
        private static Color _secondary_light = new Color(0, 148, 130, 1);
        private static Color _secondary_default = new Color(55, 222, 179, 1);
        public static Color Color_Secondary
        {
            get { return _secondary; }
        }

        // Tertiary color.
        private const string PREFS_EPEDITOR_COLOR_TERTIARY = "GDTB_EPEditor_Tertiary";
        private static Color _tertiary = new Color(255, 255, 255, 1);
        private static Color _tertiary_dark = new Color(164, 230, 200, 1);
        private static Color _tertiary_light = new Color(56, 56, 56, 1);
        private static Color _tertiary_default = new Color(164, 230, 200, 1);
        public static Color Color_Tertiary
        {
            get { return _tertiary; }
        }
        #endregion

        // Custom shortcut
        private const string PREFS_EPEDITOR_SHORTCUT = "GDTB_EPEditor_Shortcut";
        private static string _shortcut = "%|q";
        private static string _newShortcut;
        private static string _shortcut_default = "%|q";
        public static string Shortcut
        {
            get { return _shortcut; }
        }
        private static bool[] _modifierKeys = new bool[] { false, false, false }; // Ctrl/Cmd, Alt, Shift.
        private static int _mainShortcutKeyIndex = 0;
        // Restrict options to what we're sure works.
        private static string[] _shortcutKeys = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "LEFT", "RIGHT", "UP", "DOWN", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "HOME", "END", "PGUP", "PGDN" };
        #endregion fields


        [PreferenceItem("EP Editor")]
        public static void PreferencesGUI()
        {
            GetAllPrefValues();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField("General Settings", EditorStyles.boldLabel);
            _buttonsDisplay = (ButtonsDisplayFormat)EditorGUILayout.Popup("Button style", (int)_buttonsDisplay, arr_buttonStyle);
            _confirmationDialogs = EditorGUILayout.Toggle("Show confirmation dialogs", _confirmationDialogs);
            _newShortcut = DrawShortcutSelector();
            GUILayout.Space(20);
            EditorGUILayout.LabelField("UI", EditorStyles.boldLabel);
            _iconStyle = (IconStyle)EditorGUILayout.Popup("Icon style", (int)_iconStyle, arr_iconStyle);
            EditorGUILayout.Separator();
            _primary = EditorGUILayout.ColorField("Primary color", _primary);
            _secondary = EditorGUILayout.ColorField("Secondary color", _secondary);
            _tertiary = EditorGUILayout.ColorField("Tertiary color", _tertiary);
            EditorGUILayout.Separator();
            DrawThemeButtons();
            GUILayout.Space(20);
            EditorGUILayout.LabelField("Reset preferences", EditorStyles.boldLabel);
            DrawResetButton();
            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                // If buttons display changed we want to open and close the window, so that the new minsize is applied.
                var shouldReopenWindowMain = false;
                if (_buttonsDisplay != _oldDisplayFormat || _iconStyle != _oldIconStyle)
                {
                    _oldDisplayFormat = _buttonsDisplay;
                    _oldIconStyle = _iconStyle;
                    shouldReopenWindowMain = true;
                }

                SetPrefValues();
                GetAllPrefValues();

                // We need to set and get prefs before the change will be noticed by the window.
                if (shouldReopenWindowMain)
                {
                    if (WindowMain.IsOpen)
                    {
                        EditorWindow.GetWindow(typeof(WindowMain)).Close();
                        var window = EditorWindow.GetWindow(typeof(WindowMain)) as WindowMain;
                        window.SetMinSize();
                        window.Show();

                    }
                }
            }
        }


        /// Set the value of all preferences.
        private static void SetPrefValues()
        {
            EditorPrefs.SetInt(PREFS_EPEDITOR_BUTTONS_DISPLAY, (int)_buttonsDisplay);
            SetIconStyle();
            EditorPrefs.SetBool(PREFS_EPEDITOR_CONFIRMATION_DIALOGS, _confirmationDialogs);
            SetColorPrefs();
            SetShortcutPref();
        }


        /// Set the value of IconStyle.
        private static void SetIconStyle()
        {
            EditorPrefs.SetInt(PREFS_EPEDITOR_ICON_STYLE, (int)_iconStyle);
            DrawingUtils.LoadTextures(_iconStyle);
        }


        /// Set the value of a Color preference.
        private static void SetColorPrefs()
        {
            EditorPrefs.SetString(PREFS_EPEDITOR_COLOR_PRIMARY, RGBA.ColorToString(_primary));
            EditorPrefs.SetString(PREFS_EPEDITOR_COLOR_SECONDARY, RGBA.ColorToString(_secondary));
            EditorPrefs.SetString(PREFS_EPEDITOR_COLOR_TERTIARY, RGBA.ColorToString(_tertiary));
        }


        /// Set the value of the shortcut preference.
        private static void SetShortcutPref(bool forceUpdate = false)
        {
            if(forceUpdate == true)
            {
                EditorPrefs.SetString(PREFS_EPEDITOR_SHORTCUT, _shortcut);
                var formattedShortcut = _shortcut.Replace("|", "");
                IO.OverwriteShortcut(formattedShortcut);
                _newShortcut = _shortcut;
            }
            else if (_newShortcut != _shortcut)
            {
                _shortcut = _newShortcut;
                EditorPrefs.SetString(PREFS_EPEDITOR_SHORTCUT, _shortcut);
                var formattedShortcut = _shortcut.Replace("|", "");
                IO.OverwriteShortcut(formattedShortcut);
            }
        }


        /// If preferences have keys already saved in EditorPrefs, get them. Otherwise, set them.
        public static void GetAllPrefValues()
        {
            _buttonsDisplay = (ButtonsDisplayFormat)EditorPrefs.GetInt(PREFS_EPEDITOR_BUTTONS_DISPLAY, _buttonsDisplay_default); // Buttons display.
            _oldDisplayFormat = _buttonsDisplay;
            GetIconStyle();
            _confirmationDialogs = GetPrefValue(PREFS_EPEDITOR_CONFIRMATION_DIALOGS, _confirmationDialogs_default);
            GetColorPrefs();
            _shortcut = GetPrefValue(PREFS_EPEDITOR_SHORTCUT, _shortcut_default); // Shortcut.
            _newShortcut = _shortcut;
            ParseShortcutValues();
        }


        /// Get IconStyle.
        private static void GetIconStyle()
        {
            _iconStyle = (IconStyle)EditorPrefs.GetInt(PREFS_EPEDITOR_ICON_STYLE, _iconStyle_default); // Icon style.
            _oldIconStyle = _iconStyle;
        }

        /// Load color preferences.
        private static void GetColorPrefs()
        {
            _primary = GetPrefValue(PREFS_EPEDITOR_COLOR_PRIMARY, _primary_default); // PRIMARY color.
            _secondary = GetPrefValue(PREFS_EPEDITOR_COLOR_SECONDARY, _secondary_default); // SECONDARY color.
            _tertiary = GetPrefValue(PREFS_EPEDITOR_COLOR_TERTIARY, _tertiary_default); // TERTIARY color.
        }


        /// Get the value of a bool preference.
        private static bool GetPrefValue(string aKey, bool aDefault)
        {
            bool val;
            if (!EditorPrefs.HasKey(aKey))
            {
                EditorPrefs.SetBool(aKey, aDefault);
                val = aDefault;
            }
            else
            {
                val = EditorPrefs.GetBool(aKey, aDefault);
            }

            return val;
        }


        /// Get the value of a string preference.
        private static string GetPrefValue(string aKey, string aDefault)
        {
            string val;
            if (!EditorPrefs.HasKey(aKey))
            {
                EditorPrefs.SetString(aKey, aDefault);
                val = aDefault;
            }
            else
            {
                val = EditorPrefs.GetString(aKey, aDefault);
            }

            return val;
        }


        /// Get the value of a Color preference.
        private static Color GetPrefValue(string aKey, Color aDefault)
        {
            Color val;
            if (!EditorPrefs.HasKey(aKey))
            {
                EditorPrefs.SetString(aKey, RGBA.ColorToString(aDefault));
                val = aDefault;
            }
            else
            {
                val = RGBA.StringToColor(EditorPrefs.GetString(aKey, RGBA.ColorToString(aDefault)));
            }

            return val;
        }


        /// Draw the shortcut selector.
        private static string DrawShortcutSelector()
        {
            // Differentiate between Mac Editor (CMD) and Win editor (CTRL).
            var platformKey = Application.platform == RuntimePlatform.OSXEditor ? "CMD" : "CTRL";
            var shortcut = "";
            ParseShortcutValues();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Shortcut ");
            GUILayout.Space(20);
            _modifierKeys[0] = GUILayout.Toggle(_modifierKeys[0], platformKey, EditorStyles.miniButton, GUILayout.Width(50));
            _modifierKeys[1] = GUILayout.Toggle(_modifierKeys[1], "ALT", EditorStyles.miniButton, GUILayout.Width(40));
            _modifierKeys[2] = GUILayout.Toggle(_modifierKeys[2], "SHIFT", EditorStyles.miniButton, GUILayout.Width(50));
            _mainShortcutKeyIndex = EditorGUILayout.Popup(_mainShortcutKeyIndex, _shortcutKeys, GUILayout.Width(55));
            GUILayout.EndHorizontal();

            // Generate shortcut string.
            if (_modifierKeys[0] == true)
            {
                shortcut += "%|";
            }
            if (_modifierKeys[1] == true)
            {
                shortcut += "&|";
            }
            if (_modifierKeys[2] == true)
            {
                shortcut += "#|";
            }
            shortcut += _shortcutKeys[_mainShortcutKeyIndex];

            return shortcut;
        }


        /// Get usable values from the shortcut string pref.
        private static void ParseShortcutValues()
        {
            var foundCmd = false;
            var foundAlt = false;
            var foundShift = false;

            var keys = _shortcut.Split('|');
            for (var i = 0; i < keys.Length; i++)
            {
                switch (keys[i])
                {
                    case "%":
                        foundCmd = true;
                        break;
                    case "&":
                        foundAlt = true;
                        break;
                    case "#":
                        foundShift = true;
                        break;
                    default:
                        _mainShortcutKeyIndex = System.Array.IndexOf(_shortcutKeys, keys[i]);
                        break;
                }
            }
            _modifierKeys[0] = foundCmd; // Ctrl/Cmd.
            _modifierKeys[1] = foundAlt; // Alt.
            _modifierKeys[2] = foundShift; // Shift.
        }


        /// Draw update button.
        private static void DrawThemeButtons()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Apply new colors"))
            {
                ReloadSkins();
                RepaintOpenWindows();
            }
            if (GUILayout.Button("Load dark theme"))
            {
                // Get confirmation through dialog (or not if the user doesn't want to).
                var canExecute = false;
                if (ShowConfirmationDialogs == true)
                {
                    if (EditorUtility.DisplayDialog("Change to dark theme?", "Are you sure you want to change the color scheme to the dark (default) theme?", "Change color scheme", "Cancel"))
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
                    _primary = new Color(_primary_dark.r / 255.0f, _primary_dark.g / 255.0f, _primary_dark.b / 255.0f, 1.0f);
                    _secondary = new Color(_secondary_dark.r / 255.0f, _secondary_dark.g / 255.0f, _secondary_dark.b / 255.0f, 1.0f);
                    _tertiary = new Color(_tertiary_dark.r / 255.0f, _tertiary_dark.g / 255.0f, _tertiary_dark.b / 255.0f, 1.0f);
                    SetColorPrefs();
                    GetColorPrefs();
                    _iconStyle = IconStyle.LIGHT;
                    SetIconStyle();
                    GetIconStyle();
                    ReloadSkins();
                    RepaintOpenWindows();
                }
            }
            if (GUILayout.Button("Load light theme"))
            {
                // Get confirmation through dialog (or not if the user doesn't want to).
                var canExecute = false;
                if (ShowConfirmationDialogs == true)
                {
                    if (EditorUtility.DisplayDialog("Change to light theme?", "Are you sure you want to change the color scheme to the light theme?", "Change color scheme", "Cancel"))
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
                    _primary = new Color(_primary_light.r / 255.0f, _primary_light.g / 255.0f, _primary_light.b / 255.0f, 1.0f);
                    _secondary = new Color(_secondary_light.r / 255.0f, _secondary_light.g / 255.0f, _secondary_light.b / 255.0f, 1.0f);
                    _tertiary = new Color(_tertiary_light.r / 255.0f, _tertiary_light.g / 255.0f, _tertiary_light.b / 255.0f, 1.0f);
                    SetColorPrefs();
                    GetColorPrefs();
                    _iconStyle = IconStyle.DARK;
                    SetIconStyle();
                    GetIconStyle();
                    ReloadSkins();
                    RepaintOpenWindows();
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }


        /// Draw reset button.
        private static void DrawResetButton()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Reset preferences", GUILayout.Width(120)))
            {
                // Get confirmation through dialog (or not if the user doesn't want to).
                var canExecute = false;
                if (ShowConfirmationDialogs == true)
                {
                    if (EditorUtility.DisplayDialog("Reset preferences?", "Are you sure you want to reset preferences to their default values?", "Reset", "Cancel"))
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
                    ResetPrefsToDefault();
                }
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }


        /// Reset all preferences to default.
        private static void ResetPrefsToDefault()
        {
            _buttonsDisplay = (ButtonsDisplayFormat)_buttonsDisplay_default;
            _iconStyle = (IconStyle)_iconStyle_default;
            _primary = new Color(_primary_default.r / 255, _primary_default.g / 255, _primary_default.b / 255, _primary_default.a);
            _secondary = new Color(_secondary_default.r / 255, _secondary_default.g / 255, _secondary_default.b / 255, _secondary_default.a);
            _tertiary = new Color(_tertiary_default.r / 255, _tertiary_default.g / 255, _tertiary_default.b / 255, _tertiary_default.a);
            _shortcut = _shortcut_default;
            SetShortcutPref(true);
            ReloadSkins();
            SetPrefValues();
            GetAllPrefValues();
        }


        /// Repaint all open EPEditor windows.
        private static void RepaintOpenWindows()
        {
            if (WindowMain.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowMain)).Repaint();
            }
            if (WindowAdd.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowAdd)).Repaint();
            }
            if (WindowEdit.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowEdit)).Repaint();
            }
            if (WindowGet.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowGet)).Repaint();
            }
        }


        /// Reload skins of open windows.
        private static void ReloadSkins()
        {
            if (WindowMain.IsOpen)
            {
                var window = EditorWindow.GetWindow(typeof(WindowMain)) as WindowMain;
                window.LoadStyles();
            }
            if (WindowAdd.IsOpen)
            {
                var window = EditorWindow.GetWindow(typeof(WindowAdd)) as WindowMain;
                window.LoadStyles();
            }
            if (WindowEdit.IsOpen)
            {
                var window = EditorWindow.GetWindow(typeof(WindowEdit)) as WindowMain;
                window.LoadStyles();
            }
            if (WindowGet.IsOpen)
            {
                var window = EditorWindow.GetWindow(typeof(WindowGet)) as WindowMain;
                window.LoadStyles();
            }
        }
    }


    public enum ButtonsDisplayFormat
    {
        COOL_ICONS,
        REGULAR_BUTTONS
    }


    public enum IconStyle
    {
        DARK,
        LIGHT
    }
}