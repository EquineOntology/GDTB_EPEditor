namespace GDTB.EditorPrefsEditor
{
    [System.Serializable]
    public class EditorPref: System.Object
    {
        public EditorPrefType Type;
        public string Key;
        public string Value;

        public EditorPref (EditorPrefType aType, string aKey, string aValue)
        {
            this.Type = aType;
            this.Key = aKey;
            this.Value = aValue;
        }
    }
}