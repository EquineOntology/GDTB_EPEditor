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

            _newShortcut = DrawShortcutSelector();

            GUILayout.Space(20);

            DrawResetButton();
            EditorGUILayout.EndVertical();

            if (GUI.changed)
            {
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
            SetShortcutPrefs();
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
            _shortcut = _shortcut_default;

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
    }


    public enum ButtonsDisplayFormat
    {
        COOL_ICONS,
        REGULAR_BUTTONS
    }
}