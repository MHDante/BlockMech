
using System.Globalization;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OrbItUtils
{
    public static partial class Utils
    {

        public static Color ContrastColor(this Color col)
        {
            byte r = (byte)((((int)col.r) * 255 + 128) % 255);
            byte g = (byte)((((int)col.g) * 255 + 128) % 255);
            byte b = (byte)((((int)col.b) * 255 + 128) % 255);
            return new Color32(r, g, b, ((byte)(col.a * 255)));
        }

        public static Color HexToColor(string hex)
        {
            byte r = byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber);
            byte g = byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber);
            byte b = byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber);
            return new Color32(r, g, b, 255);
        }

        public static Color IntToColor(int i, byte alpha = 255)
        {
            var r = (byte)((i / (255 * 255)) % 255);
            var g = (byte)((i / 255) % 255);
            var b = (byte)(i % 255);

            return new Color32(r, g, b, alpha);
        }

        public static Color Invert(this Color color)
        {
            return new Color(.3f * color.r, .3f * color.g, .3f * color.b);
        }

        public static Color RandomColor()
        {
            return new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        }
        public static Color ToColor(this Vector4 v)
        {
            return new Color(v.x, v.y, v.z, v.w);
        }

        public static Color ToColor(this Vector3 v, byte alpha)
        {
            return new Color(v.x, v.y, v.z, alpha / 255f);
        }

    }
}
