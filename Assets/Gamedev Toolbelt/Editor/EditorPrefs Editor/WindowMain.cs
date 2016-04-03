using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace GDTB.EditorPrefsEditor
{
    public class WindowMain : EditorWindow
    {
        public static WindowMain Instance { get; private set; }
        public static bool IsOpen {
            get { return Instance != null; }
        }

        // ======================= Class functionality ========================
        public static List<Pref> Prefs = new List<Pref>();

        //============================ Editor GUI =============================
        private GUISkin skin_custom, _defaultSkin;
        private GUIStyle style_type, style_key, style_value;
        private Texture2D t_add, t_get, t_refresh, t_settings, t_nuke, t_edit, t_remove;

        // ========================= Editor layouting =========================
        private const int IconSize = 16;
        private const int ButtonWidth = 60;
        private const int ButtonHeight = 18;

        private int _offset = 5;

        private int width_type, w_prefs, w_buttons;
        private int width_typeLabel;
        private float _heightIndex = 0;
        private Vector2 _scrollPosition = new Vector2(Screen.width - 5, Screen.height);
        private Rect rect_scrollView, rect_scroll, _typeRect, _keyValueRect, _buttonsRect;


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
            window.width_typeLabel = (int)window.style_type.CalcSize(new GUIContent("String")).x; // Not with the other layouting sizes because it only needs to be done once.
            window.UpdateLayoutingSizes();
            window.InitButtonTextures();

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
            GUI.skin = skin_custom;

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
            EditorGUI.DrawRect(new Rect(0,0, position.width, position.height), Preferences.Color_Primary);
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
            rect_scrollView.height = _heightIndex - _offset;
            rect_scroll.width += IconSize;
            _scrollPosition = GUI.BeginScrollView(rect_scroll, _scrollPosition, rect_scrollView);
            _heightIndex = _offset;
            for (var i = 0; i < Prefs.Count; i++)
            {
                var key = new GUIContent(Prefs[i].Key);
                var val = new GUIContent(Prefs[i].Value);
                var keyHeight = style_key.CalcHeight(key, w_prefs);
                var valHeight = style_value.CalcHeight(val, w_prefs);

                var prefBGHeight = keyHeight + valHeight + Constants.LINE_HEIGHT + _offset;
                prefBGHeight = prefBGHeight < IconSize * 2.5f ? IconSize * 2.5f : prefBGHeight;
                if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.REGULAR_BUTTONS)
                {
                    prefBGHeight += 4;
                }

                _keyValueRect = new Rect(_offset, _heightIndex, w_prefs, prefBGHeight);
                _typeRect = new Rect(-2, _keyValueRect.y, width_type, prefBGHeight);
                _buttonsRect = new Rect(w_prefs + (_offset * 2), _keyValueRect.y, w_buttons, prefBGHeight);

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
            EditorGUI.DrawRect(new Rect(aRect.x,    aRect.y,    2,    aRect.height), Preferences.Color_Secondary);
            EditorGUI.DrawRect(new Rect(aRect.x + aRect.width - 2,      aRect.y,    2,    aRect.height), Preferences.Color_Secondary);
            EditorGUI.DrawRect(new Rect(aRect.x,    aRect.y,    aRect.width,    2), Preferences.Color_Secondary);
            EditorGUI.DrawRect(new Rect(aRect.x,    aRect.y + aRect.height - 2,    aRect.width,    2), Preferences.Color_Secondary);
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
            EditorGUI.LabelField(typeRect, type, style_type);
        }


        private void DrawKeyAndValue(Rect aRect, Pref aPref, float aHeight)
        {
            // Key.
            var keyRect = aRect;
            keyRect.x = width_type + IconSize / 2;
            keyRect.y += _offset;
            keyRect.height = aHeight;
            EditorGUI.LabelField(keyRect, aPref.Key, style_key);

            // Value.
            var valueRect = aRect;
            valueRect.x = width_type + IconSize / 2;
            valueRect.y = aRect.y + aHeight + (_offset * 1.5f);

            EditorGUI.LabelField(valueRect, aPref.Value, style_value);
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
                    Button_Edit_default(aRect, out editRect, out editContent);
                    Button_Remove_default(aRect, out removeRect, out removeContent);
                    break;
                default:
                    GUI.skin = skin_custom;
                    Button_Edit_icon(aRect, out editRect, out editContent);
                    Button_Remove_icon(aRect, out removeRect, out removeContent);
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
        private void Button_Edit_default(Rect aRect, out Rect anEditRect, out GUIContent anEditContent)
        {
            anEditRect = aRect;
            anEditRect.x = position.width - ButtonWidth - IconSize * 2 + 2;
            anEditRect.y += _offset;
            anEditRect.width = ButtonWidth;
            anEditRect.height = ButtonHeight;

            anEditContent = new GUIContent("Edit", "Edit this pref");
        }

        /// Create rect and content for normal Remove button.
        private void Button_Remove_default(Rect aRect, out Rect aRemoveRect, out GUIContent aRemoveContent)
        {
            aRemoveRect = aRect;
            aRemoveRect.x = position.width - ButtonWidth - IconSize * 2 + 2;
            aRemoveRect.y += ButtonHeight + _offset + 2;
            aRemoveRect.width = ButtonWidth;
            aRemoveRect.height = ButtonHeight;

            aRemoveContent = new GUIContent("Remove", "Remove this EditorPref");
        }


        /// Create rect and content for icon Edit button.
        private void Button_Edit_icon(Rect aRect, out Rect anEditRect, out GUIContent anEditContent)
        {
            anEditRect = aRect;
            anEditRect.x = position.width - (IconSize * 3) + 2;
            anEditRect.y += _offset;
            anEditRect.width = IconSize;
            anEditRect.height = IconSize;

            anEditContent = new GUIContent(t_edit, "Edit this EditorPref");
        }

        /// Create rect and content for icon Remove button.
        private void Button_Remove_icon(Rect aRect, out Rect aRemoveRect, out GUIContent aRemoveContent)
        {
            aRemoveRect = aRect;
            aRemoveRect.x = position.width - (IconSize * 3) + 2;
            aRemoveRect.y += IconSize +  _offset + 2;
            aRemoveRect.width = IconSize;
            aRemoveRect.height = IconSize;

            aRemoveContent = new GUIContent(t_remove, "Remove this EditorPref");
        }
        #endregion


        /// Draw a white line separating scrollview and lower buttons.
        private void DrawSeparator()
        {
            var separator = new Rect(0, position.height - (IconSize * 2), position.width, 1);
            EditorGUI.DrawRect(separator, Preferences.Color_Secondary);
        }


        #region bottom buttons
        /// Draw Add, Get, Refresh, Settings, Nuke, based on preferences.
        private void DrawBottomButtons()
        {
            Rect addRect, getRect, refreshRect, settingsRect, nukeRect;
            GUIContent addContent, getContent, refreshContent, settingsContent, nukeContent;

            switch (Preferences.ButtonsDisplay)
            {
                case ButtonsDisplayFormat.REGULAR_BUTTONS:
                    GUI.skin = _defaultSkin;
                    Button_Add_default(out addRect, out addContent);
                    Button_Get_default(out getRect, out getContent);
                    Button_Refresh_default(out refreshRect, out refreshContent);
                    Button_Settings_default(out settingsRect, out settingsContent);
                    Button_Nuke_default(out nukeRect, out nukeContent);
                    break;
                case ButtonsDisplayFormat.COOL_ICONS:
                default:
                    GUI.skin = skin_custom;
                    Button_Add_icon(out addRect, out addContent);
                    Button_Get_icon(out getRect, out getContent);
                    Button_Refresh_icon(out refreshRect, out refreshContent);
                    Button_Settings_icon(out settingsRect, out settingsContent);
                    Button_Nuke_icon(out nukeRect, out nukeContent);
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
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
                var type = assembly.GetType("UnityEditor.PreferencesWindow");
                var method = type.GetMethod("ShowPreferencesWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                method.Invoke(null, null);
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
        private void Button_Add_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - ButtonWidth * 2.5f - 8), position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Add", "Add a new key");
        }

        /// Draw default Get.
        private void Button_Get_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - ButtonWidth * 1.5f - 4), position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Get", "Add existing key");
        }

        /// Draw default Refresh.
        private void Button_Refresh_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - ButtonWidth/2), position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Refresh", "Refresh list");
        }

        /// Draw default Settings.
        private void Button_Settings_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 + ButtonWidth *0.5f  + 4), position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Settings", "Open Settings");
        }

        /// Draw default Nuke.
        private void Button_Nuke_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 + ButtonWidth * 1.5f + 8), position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Nuke all", "Delete ALL prefs from EditorPrefs");
        }


        /// Draw icon Add.
        private void Button_Add_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - IconSize * 2.5f - 10), position.height - (IconSize * 1.5f), IconSize, IconSize);
            aContent = new GUIContent(t_add, "Add a new key");
        }

        /// Draw icon Get.
        private void Button_Get_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - IconSize * 1.5f - 5), position.height - (IconSize * 1.5f), IconSize, IconSize);
            aContent = new GUIContent(t_get, "Add existing key");
        }
        /// Draw icon Refresh.
        private void Button_Refresh_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - IconSize/2), position.height - (IconSize * 1.5f), IconSize, IconSize);
            aContent = new GUIContent(t_refresh, "Refresh list");
        }

        /// Draw icon Settings.
        private void Button_Settings_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 + IconSize * 0.5f + 5), position.height - (IconSize * 1.5f), IconSize, IconSize);
            aContent = new GUIContent(t_settings, "Open Settings");
        }

        /// Draw icon Nuke.
        private void Button_Nuke_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 + IconSize * 1.5f + 10), position.height - (IconSize * 1.5f), IconSize, IconSize);
            aContent = new GUIContent(t_nuke, "Delete ALL prefs from EditorPrefs");
        }
        #endregion


        private void UpdateLayoutingSizes()
        {
            var width = position.width - IconSize;

            rect_scroll = new Rect(_offset, _offset, width - (_offset * 2), position.height - IconSize - _offset * 4);

            rect_scrollView = rect_scroll;

            width_type = width_typeLabel + (_offset * 2);
            // Same for buttons size
            if(Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                w_buttons = (IconSize * 2) + 5;
            }
            else
            {
                w_buttons = ButtonWidth + _offset * 3 - 2;
            }
            w_prefs = (int)width - width_type - w_buttons - (_offset * 2);
        }


        /// Load the EPEditor skin.
        private void LoadSkin()
        {
            skin_custom = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        }


        /// Assign the GUI Styles
        private void LoadStyles()
        {
            style_type = skin_custom.GetStyle("GDTB_EPEditor_type");
            style_key = skin_custom.GetStyle("GDTB_EPEditor_key");
            style_value = skin_custom.GetStyle("GDTB_EPEditor_value");
        }


        /// Initialize textures (so that they're not instantiated every frame in OnGUI).
        private void InitButtonTextures()
        {
            t_add = Resources.Load(Constants.FILE_GDTB_ADD, typeof(Texture2D)) as Texture2D;
            t_get = Resources.Load(Constants.FILE_GDTB_GET, typeof(Texture2D)) as Texture2D;
            t_refresh = Resources.Load(Constants.FILE_GDTB_REFRESH, typeof(Texture2D)) as Texture2D;
            t_settings = Resources.Load(Constants.FILE_GDTB_SETTINGS, typeof(Texture2D)) as Texture2D;
            t_nuke = Resources.Load(Constants.FILE_GDTB_REMOVEALL, typeof(Texture2D)) as Texture2D;
            t_edit = Resources.Load(Constants.FILE_GDTB_EDIT, typeof(Texture2D)) as Texture2D;
            t_remove = Resources.Load(Constants.FILE_GDTB_REMOVE, typeof(Texture2D)) as Texture2D;
        }


        /// Remove textures from memory when not needed anymore.
        private void OnDestroy()
        {
            Destroy(t_add);
            Destroy(t_get);
            Destroy(t_refresh);
            Destroy(t_settings);
            Destroy(t_nuke);
            Destroy(t_edit);
            Destroy(t_remove);
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