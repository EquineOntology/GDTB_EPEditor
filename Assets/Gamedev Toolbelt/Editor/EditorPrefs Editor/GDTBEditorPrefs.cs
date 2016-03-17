using UnityEditor;
using GDTB.EditorPrefsEditor;

public static class GDTBEditorPrefs
{
    #region bools
    public static void SetBool(string aKey, bool aValue)
    {
        EditorPrefs.SetBool(aKey, aValue);
        Utils.AddPref(aKey, aValue);
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
        Utils.AddPref(aKey, aValue);
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
        Utils.AddPref(aKey, aValue);
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
        Utils.AddPref(aKey, aValue);
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

    public static void DeleteAll ()
    {
        EditorPrefs.DeleteAll();
    }

    public static void DeleteKey(string aKey)
    {
        EditorPrefs.DeleteKey(aKey);
    }

    public static bool HasKey(string aKey)
    {
        return EditorPrefs.HasKey(aKey);
    }
}