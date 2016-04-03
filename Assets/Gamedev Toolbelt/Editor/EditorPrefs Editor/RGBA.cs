using UnityEngine;
using System;

namespace GDTB.EditorPrefsEditor
{
    public class RGBA
    {
        public static string ColorToString(Color aColor)
        {
            string colorString = "";
            colorString = aColor.r.ToString() + '/' + aColor.g.ToString() + '/' + aColor.b.ToString() + '/' + aColor.a.ToString();
            return colorString;
        }

        public static Color StringToColor(string anRGBAString)
        {
            var color = new Color();
            var values = anRGBAString.Split('/');
            color.r = Single.Parse(values[0]);
            color.g = Single.Parse(values[1]);
            color.b = Single.Parse(values[2]);
            color.a = Single.Parse(values[3]);

            return color;
        }
    }
}