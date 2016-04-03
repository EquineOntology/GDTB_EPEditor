using UnityEditor;
using UnityEngine;

namespace GDTB.EditorPrefsEditor
{
    public static class DrawingUtils
    {
        public static Texture2D Texture_Add = Resources.Load(Constants.FILE_GDTB_ADD, typeof(Texture2D)) as Texture2D;
        public static Texture2D Texture_Get = Resources.Load(Constants.FILE_GDTB_GET, typeof(Texture2D)) as Texture2D;
        public static Texture2D Texture_Refresh = Resources.Load(Constants.FILE_GDTB_REFRESH, typeof(Texture2D)) as Texture2D;
        public static Texture2D Texture_Settings = Resources.Load(Constants.FILE_GDTB_SETTINGS, typeof(Texture2D)) as Texture2D;
        public static Texture2D Texture_Nuke = Resources.Load(Constants.FILE_GDTB_REMOVEALL, typeof(Texture2D)) as Texture2D;
        public static Texture2D Texture_Edit = Resources.Load(Constants.FILE_GDTB_EDIT, typeof(Texture2D)) as Texture2D;
        public static Texture2D Texture_Remove = Resources.Load(Constants.FILE_GDTB_REMOVE, typeof(Texture2D)) as Texture2D;

        // Draw texture for a button based on preferences.
        public static void DrawButtonTexture(Rect aRect, Texture2D aTexture)
        {
            EditorGUI.DrawRect(aRect, Preferences.Color_Primary);
            GUI.DrawTexture(new Rect(aRect.x + 2, aRect.y + 2, Constants.BUTTON_TEXTURE_SIZE, Constants.BUTTON_TEXTURE_SIZE), aTexture);
            DrawBoundingRect(aRect);
        }

        public static void DrawBoundingRect(Rect aRect)
        {
            EditorGUI.DrawRect(new Rect(aRect.x, aRect.y, Constants.BUTTON_BORDER_THICKNESS, aRect.height), Preferences.Color_Secondary);
            EditorGUI.DrawRect(new Rect(aRect.x + aRect.width - Constants.BUTTON_BORDER_THICKNESS, aRect.y, Constants.BUTTON_BORDER_THICKNESS, aRect.height), Preferences.Color_Secondary);
            EditorGUI.DrawRect(new Rect(aRect.x, aRect.y, aRect.width, Constants.BUTTON_BORDER_THICKNESS), Preferences.Color_Secondary);
            EditorGUI.DrawRect(new Rect(aRect.x, aRect.y + aRect.height - Constants.BUTTON_BORDER_THICKNESS, aRect.width, Constants.BUTTON_BORDER_THICKNESS), Preferences.Color_Secondary);
        }
    }
}
