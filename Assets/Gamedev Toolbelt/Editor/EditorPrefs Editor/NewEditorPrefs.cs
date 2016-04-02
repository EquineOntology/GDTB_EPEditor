using UnityEditor;
using GDTB.EditorPrefsEditor;

public static class NewEditorPrefs
{
    #region bools
    public static void SetBool(string aKey, bool aValue)
    {
        EditorPrefs.SetBool(aKey, aValue);
        var duplicate = checkDuplicate(aKey, PrefType.BOOL);
        if (duplicate != null)
        {
            duplicate.Value = aValue.ToString();
        }
        else
        {
            PrefOps.AddPref(aKey, aValue);
        }
    }

    public static bool GetBool(string aKey, bool aDefaultValue)
    {
        return EditorPrefs.GetBool(aKey, aDefaultValue);
    }

    public static bool GetBool(string aKey)
    {
        return EditorPrefs.GetBool(aKey);
    }
    #endregion


    #region ints
    public static void SetInt(string aKey, int aValue)
    {
        EditorPrefs.SetInt(aKey, aValue);
        var duplicate = checkDuplicate(aKey, PrefType.INT);
        if (duplicate != null)
        {
            duplicate.Value = aValue.ToString();
        }
        else
        {
            PrefOps.AddPref(aKey, aValue);
        }
    }

    public static int GetInt(string aKey, int aDefaultValue)
    {
        return EditorPrefs.GetInt(aKey, aDefaultValue);
    }

    public static int GetInt(string aKey)
    {
        return EditorPrefs.GetInt(aKey);
    }
    #endregion


    #region floats
    public static void SetFloat(string aKey, float aValue)
    {
        EditorPrefs.SetFloat(aKey, aValue);
        var duplicate = checkDuplicate(aKey, PrefType.FLOAT);
        if (duplicate != null)
        {
            duplicate.Value = aValue.ToString();
        }
        else
        {
            PrefOps.AddPref(aKey, aValue);
        }
    }

    public static float GetFloat(string aKey, float aDefaultValue)
    {
        return EditorPrefs.GetFloat(aKey, aDefaultValue);
    }

    public static float GetFloat(string aKey)
    {
        return EditorPrefs.GetFloat(aKey);
    }
    #endregion


    #region strings
    public static void SetString(string aKey, string aValue)
    {
        EditorPrefs.SetString(aKey, aValue);
        var duplicate = checkDuplicate(aKey, PrefType.STRING);
        if (duplicate != null)
        {
            duplicate.Value = aValue;
        }
        else
        {
            PrefOps.AddPref(aKey, aValue);
        }
    }

    public static string GetString(string aKey, string aDefaultValue)
    {
        return EditorPrefs.GetString(aKey, aDefaultValue);
    }
    public static string GetString(string aKey)
    {
        return EditorPrefs.GetString(aKey);
    }
    #endregion

    // Delete all keys from EditorPrefs, WindowMain.Prefs, and empty the bak file.
    public static void DeleteAll ()
    {
        EditorPrefs.DeleteAll();
        WindowMain.Prefs.Clear();
        IO.ClearStoredPrefs();
    }


    // Remove a single Key
    public static void DeleteKey(string aKey)
    {
        EditorPrefs.DeleteKey(aKey);
        PrefOps.RemovePref(aKey);
    }


    public static bool HasKey(string aKey)
    {
        return EditorPrefs.HasKey(aKey);
    }


    // Return true if a pref is found in WindowMain.Prefs with the same key and type.
    private static Pref checkDuplicate (string aKey, PrefType aType)
    {
        foreach(var pref in WindowMain.Prefs)
        {
            if (pref.Key == aKey)
            {
                if (pref.Type == PrefType.BOOL)
                {
                    return pref;
                }
            }
        }
        return null;
    }
}