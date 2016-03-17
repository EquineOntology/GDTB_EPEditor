using UnityEditor;

namespace GDTB.EditorPrefsEditor
{
    public static class EPEManager
    {
        public static void AddPref(EditorPref aPref)
        {
            EPEditor.Prefs.Add(aPref);
            EPEditorIO.WritePrefsToFile();
        }

        public static void AddPref(string aKey, bool aValue)
        {
           EPEditor.Prefs.Add(new EditorPref(EditorPrefType.BOOL, aKey, aValue.ToString()));
            EPEditorIO.WritePrefsToFile();
        }

        public static void AddPref(string aKey, int aValue)
        {
            EPEditor.Prefs.Add(new EditorPref(EditorPrefType.INT, aKey, aValue.ToString()));
            EPEditorIO.WritePrefsToFile();
        }

        public static void AddPref(string aKey, float aValue)
        {
            EPEditor.Prefs.Add(new EditorPref(EditorPrefType.FLOAT, aKey, aValue.ToString()));
            EPEditorIO.WritePrefsToFile();
        }

        public static void AddPref(string aKey, string aValue)
        {
            EPEditor.Prefs.Add(new EditorPref(EditorPrefType.STRING, aKey, aValue));
            EPEditorIO.WritePrefsToFile();
        }

        public static void RemovePref(string aKey)
        {
            EditorPrefs.DeleteKey(aKey);
            foreach (var pref in EPEditor.Prefs)
            {
                if (pref.Key == aKey)
                {
                    EPEditor.Prefs.Remove(pref);
                }
            }
            EPEditorIO.WritePrefsToFile();
        }
    }
}