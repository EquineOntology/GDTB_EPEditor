using System.Collections.Generic;
using System.IO;
using System;

namespace GDTB.EditorPrefsEditor
{
    public static class IO
    {
        /// Write Prefs in memory to the backup file.
        public static void WritePrefsToFile()
        {
            var tempFile = Path.GetTempFileName();
            var bakFile = GetFirstInstanceOfFolder("EditorPrefs Editor") + "/bak.gdtb";

            var writer = new StreamWriter(tempFile, false);
            try
            {
                foreach (var pref in WindowMain.Prefs)
                {
                    var type = pref.Type.ToString();
                    var key = pref.Key;
                    var line = type + "|" + key;
                    writer.WriteLine(line);
                }
                writer.Close();
            }
            catch (Exception)
            {
                writer.Dispose();
            }

            if (File.Exists(bakFile))
            {
                File.Delete(bakFile);
            }
            File.Move(tempFile, bakFile);
        }

        /// Return the first instance of the given folder.
        /// This is a non-recursive, breadth-first search algorithm.
        private static string GetFirstInstanceOfFolder(string aFolderName)
        {
            var projectDirectoryPath = Directory.GetCurrentDirectory();
            var projectDirectoryInfo = new DirectoryInfo(projectDirectoryPath);
            var listOfAssetsDirs = projectDirectoryInfo.GetDirectories("Assets");
            var assetsDir = "";
            foreach (var dir in listOfAssetsDirs)
            {
                if (dir.FullName.EndsWith("\\Assets"))
                {
                    assetsDir = dir.FullName;
                }
            }
            var path = assetsDir;

            var q = new Queue<string>();
            q.Enqueue(path);
            var absolutePath = "";
            while (q.Count > 0)
            {
                path = q.Dequeue();
                try
                {
                    foreach (string subDir in Directory.GetDirectories(path))
                    {
                        q.Enqueue(subDir);
                    }
                }
                catch (Exception) { }

                string[] folders = null;
                try
                {
                    folders = Directory.GetDirectories(path);
                }
                catch (Exception) { }

                if (folders != null)
                {
                    for (int i = 0; i < folders.Length; i++)
                    {
                        if (folders[i].EndsWith(aFolderName))
                        {
                            absolutePath = folders[i];
                        }
                    }
                }
            }
            var relativePath = absolutePath.Remove(0, projectDirectoryPath.Length + 1);
            return relativePath;
        }

        public static List<Pref> LoadStoredPrefs()
        {
            var backedPrefs = new List<Pref>();

            var bakFile = GetFirstInstanceOfFolder("EditorPrefs Editor") + "/bak.gdtb";

            if (File.Exists(bakFile))
            {
                // Parse the document for exclusions.
                string line;
                var reader = new StreamReader(bakFile);
                try
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.StartsWith("#") || String.IsNullOrEmpty(line) || line == " ") // If the line is a comment, is empty, or is a single space, ignore them.
                        {
                            continue;
                        }
                        else
                        {
                            var pref = ParsePref(line);
                            if (pref != null)
                            {
                                backedPrefs.Add(pref);
                            }
                        }
                    }
                    reader.Close();
                }
                catch (Exception)
                {
                    reader.Dispose();
                }
            }
            return backedPrefs;
        }


        /// Parse a line in the backup file.
        private static Pref ParsePref(string aString)
        {
            var parts = aString.Split('|');

            // Get the key.
            var key = parts[1].Replace("(U+007C)", "|");

            // If the key doesn't exist, no sense in adding it.
            if (!NewEditorPrefs.HasKey(key))
            {
                return null;
            }

            // Get the type.
            var typeString = parts[0];
            PrefType type;
            bool boolValue;
            int intValue;
            float floatValue;
            string stringValue;

            Pref pref;
            switch (typeString)
            {
                case "BOOL":
                    type = PrefType.BOOL;
                    boolValue = NewEditorPrefs.GetBool(key, false);
                    pref = new Pref(type, key, boolValue.ToString());
                    break;
                case "INT":
                    type = PrefType.INT;
                    intValue = NewEditorPrefs.GetInt(key, 0);
                    pref = new Pref(type, key, intValue.ToString());
                    break;
                case "FLOAT":
                    type = PrefType.FLOAT;
                    floatValue = NewEditorPrefs.GetFloat(key, 0.0f);
                    pref = new Pref(type, key, floatValue.ToString());
                    break;
                case "STRING":
                default:
                    type = PrefType.STRING;
                    stringValue = NewEditorPrefs.GetString(key, "");
                    pref = new Pref(type, key, stringValue);
                    break;
            }
            return pref;
        }


        // Empty the bak file.
        public static void ClearStoredPrefs()
        {
            var bakFile = GetFirstInstanceOfFolder("EditorPrefs Editor") + "/bak.gdtb";
            if (File.Exists(bakFile))
            {
                System.IO.File.WriteAllText(bakFile, string.Empty);
            }
        }
    }
}