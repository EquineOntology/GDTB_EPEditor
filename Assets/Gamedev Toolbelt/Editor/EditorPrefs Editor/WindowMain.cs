using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace com.immortalyhydra.gdtb.epeditor
{
    public class WindowMain : EditorWindow
    {
        public static WindowMain Instance { get; private set; }
        public static bool IsOpen
        {
            get { return Instance != null; }
        }

        // ======================= Class functionality ========================
        public static List<Pref> Prefs = new List<Pref>();

        //============================ Editor GUI =============================
        private GUISkin skin_custom;
        private GUIStyle style_type, style_key, style_value, style_textButton;

        private bool clicked_add, clicked_get, clicked_refresh, clicked_settings, clicked_nuke, clicked_edit, clicked_delete, clicked_remove;

        // ========================= Editor layouting =========================
        private const int IconSize = Constants.ICON_SIZE;
        private const int ButtonWidth = 60;
        private const int ButtonHeight = 18;

        private int _offset = 5;

        private int width_type, width_prefs, width_buttons;
        private int width_typeLabel;
        private float height_totalPrefHeight = 0;
        private Vector2 _scrollPosition = new Vector2(0.0f, 0.0f);
        private Rect rect_scrollView, rect_scrollArea, rect_type, rect_remove, rect_pref, rect_buttons;
        private bool _showingScrollbar = false;


        [MenuItem("Window/Gamedev Toolbelt/EditorPrefs Editor %q")]
        public static void Init()
        {
            // Get existing open window or if none, make a new one.
            var window = (WindowMain)EditorWindow.GetWindow(typeof(WindowMain));
            window.SetMinSize(); // Window with icons and window with normal buttons need different minSizes.
            window.LoadStyles();
            window.UpdateLayoutingSizes(); // Calculate a rough size for each major element group (type, pref, buttons).
            PrefOps.RefreshPrefs(); // Load stored prefs (if any).

            //window.DebugPrefs();
            window.Show();
        }

        public void OnEnable()
        {
            #if UNITY_5_3_OR_NEWER || UNITY_5_1 || UNITY_5_2
                titleContent = new GUIContent("EditorPrefs Editor");
            #else
                title = "EditorPrefs Editor";
            #endif

            Instance = this;
            /* Load current preferences (like colours, etc.).
             * We do this here so that most preferences are updated as soon as they're changed.
             */
            Preferences.GetAllPrefValues();

            skin_custom = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
            LoadStyles();
        }

        private void OnDestroy()
        {
            Resources.UnloadUnusedAssets();
        }


        private void OnGUI()
        {
            UpdateLayoutingSizes();
            GUI.skin = skin_custom; // Without this, almost everything will work aside from the scrollbar.

            // If the list is clean (for instance because we just recompiled) load Prefs again.
            if (Prefs.Count == 0)
            {
                Prefs.Clear();
                Prefs.AddRange(IO.LoadStoredPrefs());
            }

            DrawWindowBackground();

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
        private void DrawWindowBackground()
        {
            EditorGUI.DrawRect(new Rect(0, 0, position.width, position.height), Preferences.Color_Primary);
        }


        /// Draw a message in center screen warning the user they have no prefs.
        private void DrawNoPrefsMessage()
        {
            var label = "There are currently no EditorPrefs loaded.\nYou can add a new EditorPref or\nget an existing one with the buttons below.\n\nIf you see this after the project recompiled,\ntry refreshing the window!\nYour prefs should come back just fine.";
            var labelContent = new GUIContent(label);

            Vector2 labelSize;
            #if UNITY_UNITY_5_3_OR_NEWER
                labelSize = EditorStyles.centeredGreyMiniLabel.CalcSize(labelContent);
            #else
                labelSize = EditorStyles.wordWrappedMiniLabel.CalcSize(labelContent);
            #endif

            var labelRect = new Rect(position.width / 2 - labelSize.x / 2, position.height / 2 - labelSize.y / 2 - _offset * 2.5f, labelSize.x, labelSize.y);

            #if UNITY_5_3_OR_NEWER
                EditorGUI.LabelField(labelRect, labelContent, EditorStyles.centeredGreyMiniLabel);
            #else
                EditorGUI.LabelField(labelRect, labelContent, EditorStyles.wordWrappedMiniLabel);
            #endif
        }


        /// Draw the list of EditorPrefs (with buttons etc.).
        private void DrawPrefs()
        {
            rect_scrollView.height = height_totalPrefHeight - _offset;

            // Diminish the width of scrollview and scroll area so that the scollbar is offset from the right edge of the window.
            rect_scrollArea.width += IconSize - _offset;
            rect_scrollView.width -= _offset;

            // Change size of the scroll area so that it fills the window when there's no scrollbar.
            if (_showingScrollbar == false)
            {
                rect_scrollView.width += IconSize;
            }

            _scrollPosition = GUI.BeginScrollView(rect_scrollArea, _scrollPosition, rect_scrollView);

            height_totalPrefHeight = _offset; // This includes all prefs, not just a single one.

            for (var i = 0; i < Prefs.Count; i++)
            {
                var key = new GUIContent(Prefs[i].Key);
                var val = new GUIContent(Prefs[i].Value);
                var height_key = style_key.CalcHeight(key, width_prefs);
                var height_value = style_value.CalcHeight(val, width_prefs);

                float height_prefBackground = 0;
                if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
                {
                    height_prefBackground = height_key + height_value + IconSize * 2;
                }
                else
                {
                    height_prefBackground = height_key + height_value + _offset * 7;
                }
                height_prefBackground = height_prefBackground < (IconSize * 3 - 3) ? (IconSize * 3 - 3) : height_prefBackground;

                rect_pref = new Rect(_offset, height_totalPrefHeight, width_prefs, height_prefBackground);
                rect_type = new Rect(-2, rect_pref.y, width_type, height_prefBackground);
                rect_remove = new Rect(rect_type.x, 0, rect_type.width, rect_type.height);
                rect_remove.y = rect_pref.y + rect_pref.height - _offset * 7;
                rect_buttons = new Rect(width_type + width_prefs + IconSize, rect_pref.y, width_buttons, height_prefBackground);

                var rect_prefBackground = rect_pref;
                rect_prefBackground.height = height_prefBackground - _offset;

                if (_showingScrollbar == false) // If we're not showing the scrollbar, prefs need to be larger too.
                {
                    rect_prefBackground.width = position.width - _offset * 2;
                }
                else
                {
                    rect_prefBackground.width = width_type + width_prefs + width_buttons;
                }

                height_totalPrefHeight += rect_prefBackground.height + _offset;

                // If the user removes a pref from the list in the middle of a draw call, the index in the for loop stays the same but Prefs.Count diminishes.
                // I couldn't find a way around it, so what we do is swallow the exception and wait for the next draw call.
                try
                {
                    DrawPrefBackground(rect_prefBackground);
                    DrawType(rect_type, Prefs[i]);
                    DrawRemove(rect_remove, Prefs[i]);
                    DrawKeyAndValue(rect_pref, Prefs[i], height_key);
                    DrawEditAndDelete(rect_buttons, Prefs[i]);
                }
                catch (System.Exception) { }
            }

            // Are we showing the scrollbar?
            if (rect_scrollArea.height < rect_scrollView.height)
            {
                _showingScrollbar = true;
            }
            else
            {
                _showingScrollbar = false;
            }
            GUI.EndScrollView();
        }


        /// Draw the rectangle that separates the prefs visually.
        private void DrawPrefBackground(Rect aRect)
        {
            EditorGUI.DrawRect(aRect, Preferences.Color_Secondary);
            EditorGUI.DrawRect(new Rect(
                    aRect.x + Constants.BUTTON_BORDER_THICKNESS,
                    aRect.y + Constants.BUTTON_BORDER_THICKNESS,
                    aRect.width - Constants.BUTTON_BORDER_THICKNESS * 2,
                    aRect.height - Constants.BUTTON_BORDER_THICKNESS * 2),
                Preferences.Color_Quaternary);
        }


        /// Draw the pref's type (string, int, etc).
        private void DrawType(Rect aRect, Pref aPref)
        {
            var typeRect = aRect;
            typeRect.width -= _offset;
            typeRect.height -= (IconSize / 2 + _offset);

            var newX = (int)typeRect.x + IconSize - _offset + 1;
            var newY = (int)typeRect.y + _offset;
            typeRect.position = new Vector2(newX, newY);

            var type = aPref.Type.ToString().ToLower();
            type = type.Substring(0, 1).ToUpper() + type.Substring(1);
            EditorGUI.LabelField(typeRect, type, style_type);
        }


        #region remove button
        /// Draw the hide/eye button.
        private void DrawRemove(Rect aRect, Pref aPref)
        {
            Rect removeRect;
            GUIContent removeContent;
            switch (Preferences.ButtonsDisplay)
            {
                case ButtonsDisplayFormat.REGULAR_BUTTONS:
                    Button_Remove_default(aRect, out removeRect, out removeContent);
                    break;
                default:
                    Button_Remove_icon(aRect, out removeRect, out removeContent);
                    break;
            }

            // On click.
            if (GUI.Button(removeRect, removeContent))
            {
                clicked_remove = true;
                PrefOps.RemovePref(aPref);
            }
            clicked_remove = DrawingUtils.DrawButton(removeRect, Preferences.ButtonsDisplay, clicked_remove, DrawingUtils.Texture_Remove, removeContent.text, style_textButton);
        }


        /// Create rect and content for default Remove button.
        private void Button_Remove_default(Rect aRect, out Rect aRemoveRect, out GUIContent aRemoveContent)
        {
            aRemoveRect = aRect;
            aRemoveRect.x += _offset * 2 + 1;
            aRemoveRect.y += _offset + 3;
            aRemoveRect.width = ButtonWidth / 2 + 3;
            aRemoveRect.height = ButtonHeight;
            aRemoveContent = new GUIContent("Hide", "Remove this EditorPref from\nthis list (without deleting it\nfrom EditorPrefs)");
        }


        /// Create rect and content for icon Remove button.
        private void Button_Remove_icon(Rect aRect, out Rect aRemoveRect, out GUIContent aRemoveContent)
        {
            aRemoveRect = aRect;
            aRemoveRect.x += IconSize / 2 + 1;
            aRemoveRect.y += _offset + 1;
            aRemoveRect.width = IconSize;
            aRemoveRect.height = IconSize;
            aRemoveContent = new GUIContent("", "Remove this EditorPref from\nthis list (without deleting it\nfrom EditorPrefs)");
        }
        #endregion


        /// Draw the key and value of the EditorPref.
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


        #region edit and delete
        /// Select which format to use based on the user preference.
        private void DrawEditAndDelete(Rect aRect, Pref aPref)
        {
            Rect editRect, deleteRect;
            GUIContent editContent, deleteContent;

            if (_showingScrollbar == true)
            {
                aRect.x -= _offset;
            }
            else
            {
                aRect.x = position.width - _offset * 2;
                if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.REGULAR_BUTTONS)
                {
                    aRect.x -= ButtonWidth;
                }
                else
                {
                    aRect.x -= IconSize;
                }
            }

            switch (Preferences.ButtonsDisplay)
            {
                case ButtonsDisplayFormat.REGULAR_BUTTONS:
                    Button_Edit_default(aRect, out editRect, out editContent);
                    Button_Delete_default(aRect, out deleteRect, out deleteContent);
                    break;
                default:
                    Button_Edit_icon(aRect, out editRect, out editContent);
                    Button_Delete_icon(aRect, out deleteRect, out deleteContent);
                    break;
            }

            if (GUI.Button(editRect, editContent))
            {
                clicked_edit = true;
                WindowEdit.Init(aPref);
            }
            clicked_edit = DrawingUtils.DrawButton(editRect, Preferences.ButtonsDisplay, clicked_edit, DrawingUtils.Texture_Edit, editContent.text, style_textButton);

            if (GUI.Button(deleteRect, deleteContent))
            {
                clicked_delete = true;
                // Get confirmation through dialog (or not if the user doesn't want to).
                var canExecute = false;
                if (Preferences.ShowConfirmationDialogs == true)
                {
                    if (EditorUtility.DisplayDialog("Delete EditorPref", "Are you sure you want to delete this EditorPref?", "Delete pref", "Cancel"))
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
            clicked_delete = DrawingUtils.DrawButton(deleteRect, Preferences.ButtonsDisplay, clicked_delete, DrawingUtils.Texture_Delete, deleteContent.text, style_textButton);
        }


        private void Button_Edit_default(Rect aRect, out Rect anEditRect, out GUIContent anEditContent)
        {
            anEditRect = aRect;
            anEditRect.y += _offset + 2;
            anEditRect.width = ButtonWidth;
            anEditRect.height = ButtonHeight;

            anEditContent = new GUIContent("Edit", "Edit this pref");
        }
        private void Button_Delete_default(Rect aRect, out Rect aDeleteRect, out GUIContent aDeleteContent)
        {
            aDeleteRect = aRect;
            aDeleteRect.y += ButtonHeight + _offset + 8;
            aDeleteRect.width = ButtonWidth;
            aDeleteRect.height = ButtonHeight;

            aDeleteContent = new GUIContent("Delete", "Delete this EditorPref");
        }

        private void Button_Edit_icon(Rect aRect, out Rect anEditRect, out GUIContent anEditContent)
        {
            anEditRect = aRect;
            anEditRect.y += _offset + 2;
            anEditRect.width = IconSize;
            anEditRect.height = IconSize;
            anEditContent = new GUIContent("", "Edit this EditorPref");
        }
        private void Button_Delete_icon(Rect aRect, out Rect aDeleteRect, out GUIContent aDeleteContent)
        {
            aDeleteRect = aRect;
            aDeleteRect.y += IconSize + _offset + 8;
            aDeleteRect.width = IconSize;
            aDeleteRect.height = IconSize;

            aDeleteContent = new GUIContent("", "Delete this EditorPref");
        }
        #endregion


        /// Draw a line separating scrollview and lower buttons.
        private void DrawSeparator()
        {
            var separator = new Rect(0, position.height - (_offset * 7), position.width, 1);
            EditorGUI.DrawRect(separator, Preferences.Color_Secondary);
        }


        #region bottom buttons
        /// Draw Add, Get, Refresh, Settings and Nuke, based on preferences.
        private void DrawBottomButtons()
        {
            Rect addRect, getRect, refreshRect, settingsRect, nukeRect;
            GUIContent addContent, getContent, refreshContent, settingsContent, nukeContent;

            switch (Preferences.ButtonsDisplay)
            {
                case ButtonsDisplayFormat.REGULAR_BUTTONS:
                    Button_Add_default(out addRect, out addContent);
                    Button_Get_default(out getRect, out getContent);
                    Button_Refresh_default(out refreshRect, out refreshContent);
                    Button_Settings_default(out settingsRect, out settingsContent);
                    Button_Nuke_default(out nukeRect, out nukeContent);
                    break;
                case ButtonsDisplayFormat.COOL_ICONS:
                default:
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
                clicked_add = true;
                WindowAdd.Init();
            }
            clicked_add = DrawingUtils.DrawButton(addRect, Preferences.ButtonsDisplay, clicked_add, DrawingUtils.Texture_Add, addContent.text, style_textButton);


            // Get already existing pref.
            if (GUI.Button(getRect, getContent))
            {
                clicked_get = true;
                WindowGet.Init();
            }
            clicked_get = DrawingUtils.DrawButton(getRect, Preferences.ButtonsDisplay, clicked_get, DrawingUtils.Texture_Get, getContent.text, style_textButton);


            // Refresh list of prefs.
            if (GUI.Button(refreshRect, refreshContent))
            {
                clicked_refresh = true;
                PrefOps.RefreshPrefs();
            }
            clicked_refresh = DrawingUtils.DrawButton(refreshRect, Preferences.ButtonsDisplay, clicked_refresh, DrawingUtils.Texture_Refresh, refreshContent.text, style_textButton);


            // Open settings.
            if (GUI.Button(settingsRect, settingsContent))
            {
                clicked_settings = true;

                CloseOtherWindows();
                // Unfortunately EditorApplication.ExecuteMenuItem(...) doesn't work, so we have to rely on a bit of reflection.
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
                var type = assembly.GetType("UnityEditor.PreferencesWindow");
                var method = type.GetMethod("ShowPreferencesWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                method.Invoke(null, null);
            }
            clicked_settings = DrawingUtils.DrawButton(settingsRect, Preferences.ButtonsDisplay, clicked_settings, DrawingUtils.Texture_Settings, settingsContent.text, style_textButton);


            // Nuke prefs.
            if (GUI.Button(nukeRect, nukeContent))
            {
                clicked_nuke = true;

                var canExecute = false;
                if (Preferences.ShowConfirmationDialogs == true)
                {
                    if (EditorUtility.DisplayDialog("Remove ALL EditorPrefs", "Are you sure ABSOLUTELY sure you want to remove ALL EditorPrefs currently set?\nThis is IRREVERSIBLE, only do this if you know what you're doing.\nYOU WILL ALSO NEED TO RESTART UNITY, as you'll be deleting some EditorPrefs that are required for the engine to work.", "Nuke EditorPrefs", "Cancel"))
                    {
                        canExecute = true;
                    }
                }
                else
                {
                    canExecute = true;
                }

                if (canExecute == true)
                {
                    NewEditorPrefs.DeleteAll();
                }
            }
            clicked_nuke = DrawingUtils.DrawButton(nukeRect, Preferences.ButtonsDisplay, clicked_nuke, DrawingUtils.Texture_Nuke, nukeContent.text, style_textButton);
        }


        private void Button_Add_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - ButtonWidth * 2.5f - 8), position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Add", "Add a new key");
        }
        private void Button_Get_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - ButtonWidth * 1.5f - 4), position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Get", "Add existing key");
        }
        private void Button_Refresh_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - ButtonWidth / 2), position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Refresh", "Refresh list");
        }
        private void Button_Settings_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 + ButtonWidth * 0.5f + 4), position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Settings", "Open Settings");
        }
        private void Button_Nuke_default(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 + ButtonWidth * 1.5f + 8), position.height - (ButtonHeight * 1.4f), ButtonWidth, ButtonHeight);
            aContent = new GUIContent("Nuke all", "Delete ALL prefs from EditorPrefs");
        }


        private void Button_Add_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - IconSize * 2.5f - 10), position.height - (IconSize * 1.4f), IconSize, IconSize);
            aContent = new GUIContent("", "Add a new key");
        }
        private void Button_Get_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - IconSize * 1.5f - 5), position.height - (IconSize * 1.4f), IconSize, IconSize);
            aContent = new GUIContent("", "Add existing key");
        }
        private void Button_Refresh_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - IconSize / 2), position.height - (IconSize * 1.4f), IconSize, IconSize);
            aContent = new GUIContent("", "Refresh list");
        }
        private void Button_Settings_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 + IconSize * 0.5f + 5), position.height - (IconSize * 1.4f), IconSize, IconSize);
            aContent = new GUIContent("", "Open Settings");
        }
        private void Button_Nuke_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 + IconSize * 1.5f + 10), position.height - (IconSize * 1.4f), IconSize, IconSize);
            aContent = new GUIContent("", "Delete ALL prefs from EditorPrefs");
        }
        #endregion


        /// Calculate the correct size of GUI elements based on preferences.
        private void UpdateLayoutingSizes()
        {
            width_typeLabel = (int)style_type.CalcSize(new GUIContent("String")).x;
            var width = position.width - _offset * 2;
            rect_scrollArea = new Rect(_offset, _offset, width - (_offset * 2), position.height - IconSize - _offset * 4);
            rect_scrollView = rect_scrollArea;
            width_type = width_typeLabel + (_offset * 2);

            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                if (_showingScrollbar)
                    width_buttons = IconSize + _offset * 3;
                else
                    width_buttons = IconSize + _offset;
            }
            else
            {
                if (_showingScrollbar)
                    width_buttons = ButtonWidth + _offset * 3;
                else
                    width_buttons = ButtonWidth + _offset * 1;
            }
            width_prefs = (int)width - width_type - width_buttons - _offset * 3;
        }


        /// Assign the proper values to styles based on colors in Preferences.
        public void LoadStyles()
        {
            style_type = skin_custom.GetStyle("GDTB_EPEditor_type");
            style_type.normal.textColor = Preferences.Color_Tertiary;
            style_type.active.textColor = Preferences.Color_Tertiary;
            style_key = skin_custom.GetStyle("GDTB_EPEditor_key");
            style_key.normal.textColor = Preferences.Color_Secondary;
            style_key.active.textColor = Preferences.Color_Secondary;
            style_value = skin_custom.GetStyle("GDTB_EPEditor_value");
            style_value.normal.textColor = Preferences.Color_Tertiary;
            style_value.active.textColor = Preferences.Color_Tertiary;
            style_textButton = skin_custom.GetStyle("GDTB_EPEditor_buttonText");
            style_textButton.active.textColor = Preferences.Color_Primary;
            style_textButton.normal.textColor = Preferences.Color_Tertiary;

            skin_custom.settings.selectionColor = Preferences.Color_Secondary;

            // Change scrollbar color.
            var scrollbar = Resources.Load(Constants.TEX_SCROLLBAR, typeof(Texture2D)) as Texture2D;

            #if UNITY_5 || UNITY_5_3_OR_NEWER
                scrollbar.SetPixel(0,0, Preferences.Color_Secondary);
            #else
				var pixels = scrollbar.GetPixels();
				// We do it like this because minimum texture size in older versions of Unity is 2x2.
				for(var i = 0; i < pixels.GetLength(0); i++)
				{
					scrollbar.SetPixel(i, 0, Preferences.Color_Secondary);
					scrollbar.SetPixel(i, 1, Preferences.Color_Secondary);
				}
            #endif

            scrollbar.Apply();
            skin_custom.verticalScrollbarThumb.normal.background = scrollbar;
        }


        /// Set the minSize of the window based on preferences.
        public void SetMinSize()
        {
            var window = GetWindow(typeof(WindowMain)) as WindowMain;
            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                window.minSize = new Vector2(222f, 150f);
            }
            else
            {
                window.minSize = new Vector2(322f, 150f);
            }
        }


        /// Close open sub-windows (add, get, edit) when opening prefs.
        private void CloseOtherWindows()
        {
            if (WindowAdd.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowAdd)).Close();
            }
            if (WindowGet.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowGet)).Close();
            }
            if (WindowEdit.IsOpen)
            {
                EditorWindow.GetWindow(typeof(WindowEdit)).Close();
            }
        }


        /// List all prefs and their values. For debugging purposes.
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
