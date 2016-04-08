using UnityEditor;
using UnityEngine;

namespace GDTB.EditorPrefsEditor
{
    public static class DrawingUtils
    {
        public static Texture2D Texture_Add = Resources.Load(Constants.FILE_ADD_DARK, typeof(Texture2D)) as Texture2D;
        public static Texture2D Texture_Get = Resources.Load(Constants.FILE_GET_DARK, typeof(Texture2D)) as Texture2D;
        public static Texture2D Texture_Refresh = Resources.Load(Constants.FILE_REFRESH_DARK, typeof(Texture2D)) as Texture2D;
        public static Texture2D Texture_Settings = Resources.Load(Constants.FILE_SETTINGS_DARK, typeof(Texture2D)) as Texture2D;
        public static Texture2D Texture_Nuke = Resources.Load(Constants.FILE_REMOVEALL_DARK, typeof(Texture2D)) as Texture2D;
        public static Texture2D Texture_Edit = Resources.Load(Constants.FILE_EDIT_DARK, typeof(Texture2D)) as Texture2D;
        public static Texture2D Texture_Remove = Resources.Load(Constants.FILE_REMOVE_DARK, typeof(Texture2D)) as Texture2D;

        public static IconStyle CurrentIconStyle = IconStyle.LIGHT;


        /// Draw "fake" texture button for a button based on preferences.
        public static void DrawTextureButton(Rect aRect, Texture2D aTexture)
        {
            DrawBoundingRect(aRect);
            var bgRect = new Rect(aRect.x + Constants.BUTTON_BORDER_THICKNESS, aRect.y + Constants.BUTTON_BORDER_THICKNESS, aRect.width - Constants.BUTTON_BORDER_THICKNESS * 2, aRect.height - Constants.BUTTON_BORDER_THICKNESS * 2);
            EditorGUI.DrawRect(bgRect, Preferences.Color_Primary);
            GUI.DrawTexture(new Rect(aRect.x + 2, aRect.y + 2, Constants.BUTTON_TEXTURE_SIZE, Constants.BUTTON_TEXTURE_SIZE), aTexture);
        }


        /// Draw "fake" text button based on preferences.
        public static void DrawTextButton(Rect aRect, string aText, GUIStyle aStyle)
        {
            DrawBoundingRect(aRect);
            var bgRect = new Rect(aRect.x + Constants.BUTTON_BORDER_THICKNESS, aRect.y + Constants.BUTTON_BORDER_THICKNESS, aRect.width - Constants.BUTTON_BORDER_THICKNESS * 2, aRect.height - Constants.BUTTON_BORDER_THICKNESS * 2);
            EditorGUI.DrawRect(bgRect, Preferences.Color_Primary);
            GUI.Label(new Rect(bgRect.x, bgRect.y - 1, bgRect.width, bgRect.height), aText, aStyle);
            aStyle.normal.textColor = Preferences.Color_Tertiary;
        }


        /// Draw "fake" text button in the pressed state.
        public static void DrawPressedTextButton(Rect aRect, string aText, GUIStyle aStyle)
        {
            DrawBoundingRect(aRect);
            aStyle.normal.textColor = Preferences.Color_Primary;
            GUI.Label(new Rect(aRect.x, aRect.y - 1, aRect.width, aRect.height), aText, aStyle);
        }


        /// Draw borders of a rect.
        public static void DrawBoundingRect(Rect aRect)
        {
            EditorGUI.DrawRect(aRect, Preferences.Color_Secondary);
        }


        /// Draw custom selectionGrid.
        public static void DrawSelectionGrid(Rect aRect, string[] anElementArray, int aSelectedIndex, float anHorizontalSize, float aSpace, GUIStyle aNormalStyle, GUIStyle aPressedStyle)
        {
            var x = aRect.x;
            for (var i = 0; i < anElementArray.Length; i++)
            {
                var gridRect = aRect;
                x = i == 0 ? x : x + anHorizontalSize + aSpace; // Add parameters to horizontal pos., but not for the first rect.
                gridRect.x = x;
                gridRect.width = anHorizontalSize;

                if (i != aSelectedIndex)
                {
                    DrawTextButton(gridRect, anElementArray[i], aNormalStyle);
                }
                else
                {
                    DrawPressedTextButton(gridRect, anElementArray[i], aPressedStyle);
                }
            }
        }

        public static void LoadTextures(IconStyle aStyle)
        {
            Texture_Add = null;
            Texture_Get = null;
            Texture_Refresh = null;
            Texture_Settings = null;
            Texture_Nuke = null;
            Texture_Edit = null;
            Texture_Remove = null;

            if (aStyle == IconStyle.DARK)
            {
                Texture_Add = Resources.Load(Constants.FILE_ADD_DARK, typeof(Texture2D)) as Texture2D;
                Texture_Get = Resources.Load(Constants.FILE_GET_DARK, typeof(Texture2D)) as Texture2D;
                Texture_Refresh = Resources.Load(Constants.FILE_REFRESH_DARK, typeof(Texture2D)) as Texture2D;
                Texture_Settings = Resources.Load(Constants.FILE_SETTINGS_DARK, typeof(Texture2D)) as Texture2D;
                Texture_Nuke = Resources.Load(Constants.FILE_REMOVEALL_DARK, typeof(Texture2D)) as Texture2D;
                Texture_Edit = Resources.Load(Constants.FILE_EDIT_DARK, typeof(Texture2D)) as Texture2D;
                Texture_Remove = Resources.Load(Constants.FILE_REMOVE_DARK, typeof(Texture2D)) as Texture2D;
                CurrentIconStyle = IconStyle.DARK;
            }
            else
            {
                Texture_Add = Resources.Load(Constants.FILE_ADD_LIGHT, typeof(Texture2D)) as Texture2D;
                Texture_Get = Resources.Load(Constants.FILE_GET_LIGHT, typeof(Texture2D)) as Texture2D;
                Texture_Refresh = Resources.Load(Constants.FILE_REFRESH_LIGHT, typeof(Texture2D)) as Texture2D;
                Texture_Settings = Resources.Load(Constants.FILE_SETTINGS_LIGHT, typeof(Texture2D)) as Texture2D;
                Texture_Nuke = Resources.Load(Constants.FILE_REMOVEALL_LIGHT, typeof(Texture2D)) as Texture2D;
                Texture_Edit = Resources.Load(Constants.FILE_EDIT_LIGHT, typeof(Texture2D)) as Texture2D;
                Texture_Remove = Resources.Load(Constants.FILE_REMOVE_LIGHT, typeof(Texture2D)) as Texture2D;
                CurrentIconStyle = IconStyle.LIGHT;
            }
        }
    }
}