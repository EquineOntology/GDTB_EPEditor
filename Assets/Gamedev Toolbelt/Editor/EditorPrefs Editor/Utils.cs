using UnityEditor;

namespace GDTB.EditorPrefsEditor
{
    public static class Utils
    {
        public static void AddPref(Pref aPref)
        {
            WindowMain.Prefs.Add(aPref);
            IO.WritePrefsToFile();
            EditorWindow.GetWindow(typeof(WindowMain)).Repaint();
        }

        public static void AddPref(string aKey, bool aValue)
        {
            WindowMain.Prefs.Add(new Pref(PrefType.BOOL, aKey, aValue.ToString()));
            IO.WritePrefsToFile();
            EditorWindow.GetWindow(typeof(WindowMain)).Repaint();
        }

        public static void AddPref(string aKey, int aValue)
        {
            WindowMain.Prefs.Add(new Pref(PrefType.INT, aKey, aValue.ToString()));
            IO.WritePrefsToFile();
            EditorWindow.GetWindow(typeof(WindowMain)).Repaint();
        }

        public static void AddPref(string aKey, float aValue)
        {
            WindowMain.Prefs.Add(new Pref(PrefType.FLOAT, aKey, aValue.ToString()));
            IO.WritePrefsToFile();
            EditorWindow.GetWindow(typeof(WindowMain)).Repaint();
        }

        public static void AddPref(string aKey, string aValue)
        {
            WindowMain.Prefs.Add(new Pref(PrefType.STRING, aKey, aValue));
            IO.WritePrefsToFile();
            EditorWindow.GetWindow(typeof(WindowMain)).Repaint();
        }

        public static void RemovePref(string aKey)
        {
            GDTBEditorPrefs.DeleteKey(aKey);
            foreach (var pref in WindowMain.Prefs)
            {
                if (pref.Key == aKey)
                {
                    WindowMain.Prefs.Remove(pref);
                }
            }
            IO.WritePrefsToFile();
            EditorWindow.GetWindow(typeof(WindowMain)).Repaint();
        }

        public static void RemovePref(Pref aPref)
        {
            GDTBEditorPrefs.DeleteKey(aPref.Key);
            WindowMain.Prefs.Remove(aPref);
            IO.WritePrefsToFile();
            EditorWindow.GetWindow(typeof(WindowMain)).Repaint();
        }
    }
}