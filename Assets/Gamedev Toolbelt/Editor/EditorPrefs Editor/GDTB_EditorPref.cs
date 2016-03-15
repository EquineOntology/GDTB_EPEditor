[System.Serializable]
public class GDTB_EditorPref: System.Object
{
    public GDTB_EditorPrefType Type;
    public string Key;
    public string Value;

    public GDTB_EditorPref (GDTB_EditorPrefType aType, string aKey, string aValue)
    {
        this.Type = aType;
        this.Key = aKey;
        this.Value = aValue;
    }
}