using UnityEditor;

namespace GDTB.EditorPrefsEditor
{
    public static class PrefManager
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
            // Iterate backwards by index, a foreach will throw an InvalidOperationException.
            for (var i = WindowMain.Prefs.Count - 1; i >= 0; i--)
            {
                if (WindowMain.Prefs[i].Key == aKey)
                {
                    WindowMain.Prefs.Remove(WindowMain.Prefs[i]);
                }
            }
            IO.WritePrefsToFile();
            EditorWindow.GetWindow(typeof(WindowMain)).Repaint();
        }

        public static void RemovePref(Pref aPref)
        {
            WindowMain.Prefs.Remove(aPref);
            IO.WritePrefsToFile();
            EditorWindow.GetWindow(typeof(WindowMain)).Repaint();
        }


        public static void RefreshPrefs()
        {
            var storedPrefs = IO.LoadStoredPrefs();
            if (storedPrefs.Count >= WindowMain.Prefs.Count)
            {
                WindowMain.Prefs.Clear();
                WindowMain.Prefs.AddRange(storedPrefs);
            }
        }
    }
}