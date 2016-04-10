using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace com.immortalyhydra.gdtb.epeditor
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
        private GUIStyle style_type, style_key, style_value, style_buttonText;

        // ========================= Editor layouting =========================
        private const int IconSize = Constants.ICON_SIZE;
        private const int ButtonWidth = 60;
        private const int ButtonHeight = 18;

        private int _offset = 5;

        private int width_type, width_prefs, width_buttons;
        private int width_typeLabel;
        private float idx_height = 0;
        private Vector2 _scrollPosition = new Vector2(0.0f, 0.0f);
        private Rect rect_scrollView, rect_scrollArea, rect_type, rect_pref, rect_buttons;
        private bool showingScrollbar = false;


        [MenuItem("Window/Gamedev Toolbelt/EditorPrefs Editor %q")]
        public static void Init()
        {
            // Get existing open window or if none, make a new one.
            var window = (WindowMain)EditorWindow.GetWindow(typeof(WindowMain));
            window.SetMinSize();
            window.width_typeLabel = (int)window.style_type.CalcSize(new GUIContent("String")).x; // Not with the other layouting sizes because it only needs to be done once.
            window.UpdateLayoutingSizes();
            PrefOps.RefreshPrefs();

            //window.DebugPrefs();
            window.Show();
        }


        public void OnEnable()
        {
            titleContent = new GUIContent("EditorPrefs Editor");
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
            rect_scrollView.height = idx_height - _offset;

            //Diminish the width of scrollview and scroll area so that the scollbar is offset from the right edge of the window.
            rect_scrollArea.width += IconSize - _offset;
            rect_scrollView.width -= _offset;

            // Change size of the scroll area so that it fills the window when there's no scrollbar.
            if(showingScrollbar == false)
            {
                rect_scrollView.width += IconSize;
            }

            _scrollPosition = GUI.BeginScrollView(rect_scrollArea, _scrollPosition, rect_scrollView);
            idx_height = _offset;
            for (var i = 0; i < Prefs.Count; i++)
            {
                var key = new GUIContent(Prefs[i].Key);
                var val = new GUIContent(Prefs[i].Value);
                var height_key = style_key.CalcHeight(key, width_prefs);
                var height_value = style_value.CalcHeight(val, width_prefs);

                var prefBGHeight = height_key + height_value + Constants.LINE_HEIGHT + _offset * 2 + 4;
                prefBGHeight = prefBGHeight < IconSize * 2.5f ? IconSize * 2.5f : prefBGHeight;
                if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.REGULAR_BUTTONS)
                {
                    prefBGHeight += 4;
                }

                rect_pref = new Rect(_offset, idx_height, width_prefs, prefBGHeight);
                rect_type = new Rect(-2, rect_pref.y, width_type, prefBGHeight);
                rect_buttons = new Rect(width_type + width_prefs + IconSize, rect_pref.y, width_buttons, prefBGHeight);


                var prefBGRect = rect_pref;
                prefBGRect.height = prefBGHeight - _offset;

                if (showingScrollbar == false)
                {
                    prefBGRect.width = position.width - _offset * 2;
                }
                else
                {
                    prefBGRect.width = width_type + width_prefs + width_buttons;
                }

                idx_height += prefBGRect.height + _offset;

                DrawPrefBG(prefBGRect);
                DrawType(rect_type, Prefs[i]);
                DrawKeyAndValue(rect_pref, Prefs[i], height_key);
                DrawEditAndRemove(rect_buttons, Prefs[i]);
            }

            if(rect_scrollArea.height < rect_scrollView.height)
            {
                showingScrollbar = true;
            }
            else
            {
                showingScrollbar = false;
            }
            GUI.EndScrollView();
        }


        /// Draw the rectangle that separates the prefs visually.
        private void DrawPrefBG(Rect aRect)
        {
            EditorGUI.DrawRect(aRect, Preferences.Color_Secondary);
            EditorGUI.DrawRect(new Rect(aRect.x + Constants.BUTTON_BORDER_THICKNESS, aRect.y + Constants.BUTTON_BORDER_THICKNESS, aRect.width - Constants.BUTTON_BORDER_THICKNESS * 2, aRect.height - Constants.BUTTON_BORDER_THICKNESS * 2), Preferences.Color_Primary);
        }


        /// Draw the pref's type.
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

            if (showingScrollbar == true)
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
                    //GUI.skin = _defaultSkin;
                    Button_Edit_default(aRect, out editRect, out editContent);
                    Button_Remove_default(aRect, out removeRect, out removeContent);
                    break;
                default:
                    //GUI.skin = skin_custom;
                    Button_Edit_icon(aRect, out editRect, out editContent);
                    Button_Remove_icon(aRect, out removeRect, out removeContent);
                    break;
            }

            if (GUI.Button(editRect, editContent))
            {
                WindowEdit.Init(aPref);
            }
            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                DrawingUtils.DrawTextureButton(editRect, DrawingUtils.Texture_Edit);
            }
            else
            {
                DrawingUtils.DrawTextButton(editRect, editContent.text, style_buttonText);
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
            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                DrawingUtils.DrawTextureButton(removeRect, DrawingUtils.Texture_Remove);
            }
            else
            {
                DrawingUtils.DrawTextButton(removeRect, removeContent.text, style_buttonText);
            }
        }


        /// Create rect and content for normal Edit button.
        private void Button_Edit_default(Rect aRect, out Rect anEditRect, out GUIContent anEditContent)
        {
            anEditRect = aRect;
            anEditRect.y += _offset;
            anEditRect.width = ButtonWidth;
            anEditRect.height = ButtonHeight;

            anEditContent = new GUIContent("Edit", "Edit this pref");
        }

        /// Create rect and content for normal Remove button.
        private void Button_Remove_default(Rect aRect, out Rect aRemoveRect, out GUIContent aRemoveContent)
        {
            aRemoveRect = aRect;
            aRemoveRect.y += ButtonHeight + _offset + 2;
            aRemoveRect.width = ButtonWidth;
            aRemoveRect.height = ButtonHeight;

            aRemoveContent = new GUIContent("Remove", "Remove this EditorPref");
        }


        /// Create rect and content for icon Edit button.
        private void Button_Edit_icon(Rect aRect, out Rect anEditRect, out GUIContent anEditContent)
        {
            anEditRect = aRect;
            anEditRect.y += _offset;
            anEditRect.width = IconSize;
            anEditRect.height = IconSize;
            anEditContent = new GUIContent("", "Edit this EditorPref");
        }

        /// Create rect and content for icon Remove button.
        private void Button_Remove_icon(Rect aRect, out Rect aRemoveRect, out GUIContent aRemoveContent)
        {
            aRemoveRect = aRect;
            aRemoveRect.y += IconSize +  _offset + 2;
            aRemoveRect.width = IconSize;
            aRemoveRect.height = IconSize;

            aRemoveContent = new GUIContent("", "Remove this EditorPref");
        }
        #endregion


        /// Draw a white line separating scrollview and lower buttons.
        private void DrawSeparator()
        {
            var separator = new Rect(0, position.height - (_offset * 7), position.width, 1);
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
            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                DrawingUtils.DrawTextureButton(addRect, DrawingUtils.Texture_Add);
            }
            else
            {
                DrawingUtils.DrawTextButton(addRect, addContent.text, style_buttonText);
            }

            // Get already existing pref.
            if (GUI.Button(getRect, getContent))
            {
                WindowGet.Init();
            }
            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                DrawingUtils.DrawTextureButton(getRect, DrawingUtils.Texture_Get);
            }
            else
            {
                DrawingUtils.DrawTextButton(getRect, getContent.text, style_buttonText);
            }

            // Refresh list of prefs.
            if (GUI.Button(refreshRect, refreshContent))
            {
                PrefOps.RefreshPrefs();
            }
            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                DrawingUtils.DrawTextureButton(refreshRect, DrawingUtils.Texture_Refresh);
            }
            else
            {
                DrawingUtils.DrawTextButton(refreshRect, refreshContent.text, style_buttonText);
            }

            // Open settings.
            if (GUI.Button(settingsRect, settingsContent))
            {
                CloseOtherWindows();
                // Unfortunately EditorApplication.ExecuteMenuItem(...) doesn't work, so we have to rely on a bit of reflection.
                var assembly = System.Reflection.Assembly.GetAssembly(typeof(EditorWindow));
                var type = assembly.GetType("UnityEditor.PreferencesWindow");
                var method = type.GetMethod("ShowPreferencesWindow", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                method.Invoke(null, null);
            }

            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                DrawingUtils.DrawTextureButton(settingsRect, DrawingUtils.Texture_Settings);
            }
            else
            {
                DrawingUtils.DrawTextButton(settingsRect, settingsContent.text, style_buttonText);
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
            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                DrawingUtils.DrawTextureButton(nukeRect, DrawingUtils.Texture_Nuke);
            }
            else
            {
                DrawingUtils.DrawTextButton(nukeRect, nukeContent.text, style_buttonText);
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
            aRect = new Rect((position.width / 2 - IconSize * 2.5f - 10), position.height - (IconSize * 1.4f), IconSize, IconSize);
            aContent = new GUIContent("", "Add a new key");
        }

        /// Draw icon Get.
        private void Button_Get_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - IconSize * 1.5f - 5), position.height - (IconSize * 1.4f), IconSize, IconSize);
            aContent = new GUIContent("", "Add existing key");
        }
        /// Draw icon Refresh.
        private void Button_Refresh_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 - IconSize/2), position.height - (IconSize * 1.4f), IconSize, IconSize);
            aContent = new GUIContent("", "Refresh list");
        }

        /// Draw icon Settings.
        private void Button_Settings_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 + IconSize * 0.5f + 5), position.height - (IconSize * 1.4f), IconSize, IconSize);
            aContent = new GUIContent("", "Open Settings");
        }

        /// Draw icon Nuke.
        private void Button_Nuke_icon(out Rect aRect, out GUIContent aContent)
        {
            aRect = new Rect((position.width / 2 + IconSize * 1.5f + 10), position.height - (IconSize * 1.4f), IconSize, IconSize);
            aContent = new GUIContent("", "Delete ALL prefs from EditorPrefs");
        }
        #endregion


        /// Calculate the correct size of GUI elements based on preferences.
        private void UpdateLayoutingSizes()
        {
            var width = position.width - _offset * 2;
            rect_scrollArea = new Rect(_offset, _offset, width - (_offset * 2), position.height - IconSize - _offset * 4);
            rect_scrollView = rect_scrollArea;
            width_type = width_typeLabel + (_offset * 2);

            if (Preferences.ButtonsDisplay == ButtonsDisplayFormat.COOL_ICONS)
            {
                if (showingScrollbar)
                    width_buttons = IconSize + _offset * 3;
                else
                    width_buttons = IconSize + _offset;
            }
            else
            {
                if (showingScrollbar)
                    width_buttons = ButtonWidth + _offset * 3;
                else
                    width_buttons = ButtonWidth + _offset * 1;
            }
            width_prefs = (int)width - width_type - width_buttons - _offset * 3;
        }


        /// Load the EPEditor skin.
        private void LoadSkin()
        {
            skin_custom = Resources.Load(Constants.FILE_GUISKIN, typeof(GUISkin)) as GUISkin;
        }


        /// Assign the GUI Styles
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
            style_buttonText = skin_custom.GetStyle("GDTB_EPEditor_buttonText");
            style_buttonText.active.textColor = Preferences.Color_Tertiary;
            style_buttonText.normal.textColor = Preferences.Color_Tertiary;

            // Change scrollbar color.
            var scrollbar = Resources.Load("GUI/epeditor_scrollbar", typeof(Texture2D)) as Texture2D;
            scrollbar.SetPixel(0, 0, Preferences.Color_Secondary);
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

            width_typeLabel = (int)style_type.CalcSize(new GUIContent("String")).x;
        }


        /// Remove textures from memory when not needed anymore.
        private void OnDestroy()
        {
            Resources.UnloadUnusedAssets();
        }


        /// Close sub-windows when opening prefs.
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
