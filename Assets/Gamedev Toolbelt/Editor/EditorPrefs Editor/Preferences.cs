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
        public static ButtonsDisplayFormat ButtonsDisplay
        {
            get { return _buttonsDisplay; }
        }
        private static string[] _buttonsFormatsString = { "Cool icons", "Regular buttons" };


        // Confirmation dialogs
        private const string PREFS_EPEDITOR_CONFIRMATION_DIALOGS = "GDTB_EPEditor_ConfirmationDialogs";
        private static bool _confirmationDialogs = true;
        private static bool _confirmationDialogs_default = true;
        public static bool ShowConfirmationDialogs
        {
            get { return _confirmationDialogs; }
        }


        // Color of URGENT tasks.
        private const string PREFS_EPEDITOR_COLOR_PRIMARY = "GDTB_EPEditor_Primary";
        private static Color _primary = new Color(56, 56, 56, 1);
        private static Color _primary_default = new Color(56, 56, 56, 1);
        private static Color _oldPrimary;
        public static Color Color_Primary
        {
            get { return _primary; }
        }

        // Color of NORMAL tasks
        private const string PREFS_EPEDITOR_COLOR_SECONDARY = "GDTB_EPEditor_Secondary";
        private static Color _secondary = new Color(55, 222, 179, 1);
        private static Color _secondary_default = new Color(55, 222, 179, 1);
        private static Color _oldSecondary;
        public static Color Color_Secondary
        {
            get { return _secondary; }
        }

        // Color of MINOR tasks
        private const string PREFS_EPEDITOR_COLOR_TERTIARY = "GDTB_EPEditor_Tertiary";
        //private static Color _tertiary = new Color(164, 230, 200, 1);
        private static Color _tertiary = new Color(255, 255, 255, 1);
        //private static Color _tertiary_default = new Color(164, 230, 200, 1);
        private static Color _tertiary_default = new Color(255, 255, 255, 1);
        private static Color _oldTertiary;
        public static Color Color_Tertiary
        {
            get { return _tertiary; }
        }


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
        // Want absolute control over values.
        private static string[] _shortcutKeys = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "LEFT", "RIGHT", "UP", "DOWN", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "HOME", "END", "PGUP", "PGDN" };
        #endregion fields


        [PreferenceItem("EP Editor")]
        public static void PreferencesGUI()
        {
            GetAllPrefValues();

            EditorGUILayout.BeginVertical();
            _buttonsDisplay = (ButtonsDisplayFormat)EditorGUILayout.Popup("Button style", System.Convert.ToInt16(_buttonsDisplay), _buttonsFormatsString);
            _confirmationDialogs = EditorGUILayout.Toggle("Show confirmation dialogs", _confirmationDialogs);
            EditorGUILayout.Separator();
            _primary = EditorGUILayout.ColorField("Primary color", _primary);
            _secondary = EditorGUILayout.ColorField("Secondary color", _secondary);
            _tertiary = EditorGUILayout.ColorField("Tertiary color", _tertiary);
            EditorGUILayout.Separator();
            _newShortcut = DrawShortcutSelector();
            GUILayout.Space(20);
            DrawResetButton();
            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
                // Reload skins if colors changed.
                if (_primary != _oldPrimary || _secondary != _oldSecondary || _tertiary != _oldTertiary)
                {
                    _oldPrimary = _primary;
                    _oldSecondary = _secondary;
                    _oldTertiary = _tertiary;
                    ReloadSkins();
                }
                SetPrefValues();
                GetAllPrefValues();
                RepaintOpenWindows();
            }
        }


        /// Set the value of all preferences.
        private static void SetPrefValues()
        {
            EditorPrefs.SetInt(PREFS_EPEDITOR_BUTTONS_DISPLAY, System.Convert.ToInt16(_buttonsDisplay));
            EditorPrefs.SetBool(PREFS_EPEDITOR_CONFIRMATION_DIALOGS, _confirmationDialogs);
            SetColorPrefs();
            SetShortcutPrefs();
        }


        /// Set the value of a Color preference.
        private static void SetColorPrefs()
        {
            EditorPrefs.SetString(PREFS_EPEDITOR_COLOR_PRIMARY, RGBA.ColorToString(_primary));
            EditorPrefs.SetString(PREFS_EPEDITOR_COLOR_SECONDARY, RGBA.ColorToString(_secondary));
            EditorPrefs.SetString(PREFS_EPEDITOR_COLOR_TERTIARY, RGBA.ColorToString(_tertiary));
        }


        /// Set the value of the shortcut preference.
        private static void SetShortcutPrefs()
        {
            if (_newShortcut != _shortcut)
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
            _confirmationDialogs = GetPrefValue(PREFS_EPEDITOR_CONFIRMATION_DIALOGS, _confirmationDialogs_default);
            _primary = GetPrefValue(PREFS_EPEDITOR_COLOR_PRIMARY, _primary_default); // PRIMARY color.
            _oldPrimary = _primary;
            _secondary = GetPrefValue(PREFS_EPEDITOR_COLOR_SECONDARY, _secondary_default); // SECONDARY color.
            _oldSecondary = _secondary;
            _tertiary = GetPrefValue(PREFS_EPEDITOR_COLOR_TERTIARY, _tertiary_default); // TERTIARY color.
            _oldTertiary = _tertiary;
            _shortcut = GetPrefValue(PREFS_EPEDITOR_SHORTCUT, _shortcut_default); // Shortcut.
            ParseShortcutValues();
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
            _modifierKeys[2] = GUILayout.Toggle(_modifierKeys[2], "SHIFT", EditorStyles.miniButton, GUILayout.Width(60));
            _mainShortcutKeyIndex = EditorGUILayout.Popup(_mainShortcutKeyIndex, _shortcutKeys, GUILayout.Width(60));
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


        /// Draw reset button.
        private static void DrawResetButton()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.Space();
            if (GUILayout.Button("Reset preferences", GUILayout.Width(120)))
            {
                ResetPrefsToDefault();
            }
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();
        }


        /// Reset all preferences to default.
        private static void ResetPrefsToDefault()
        {
            _buttonsDisplay = (ButtonsDisplayFormat)_buttonsDisplay_default;
            _primary = new Color(_primary_default.r / 255, _primary_default.g / 255, _primary_default.b / 255, _primary_default.a);
            _secondary = new Color(_secondary_default.r / 255, _secondary_default.g / 255, _secondary_default.b / 255, _secondary_default.a);
            _tertiary = new Color(_tertiary_default.r / 255, _tertiary_default.g / 255, _tertiary_default.b / 255, _tertiary_default.a);
            _shortcut = _shortcut_default;
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
}