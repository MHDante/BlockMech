using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Random = UnityEngine.Random;

namespace OrbItUtils
{
    public  static partial class Utils
    {
        private static HashSet<String> UniqueStringSet;

        public static bool AsBool(this int i)
        {
            return i != 0;
        }

        public static T GetCustomAttribute<T>(this MemberInfo mInfo) where T : Attribute
        {
            var infos = mInfo.GetCustomAttributes(typeof (T), false);
            if (infos.Length > 0)
            {
                return (T) infos.ElementAt(0);
            }
            return null;
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
            Func<TValue> valueCreator)
        {
            TValue value;
            if (!dictionary.TryGetValue(key, out value))
            {
                value = valueCreator();
                dictionary.Add(key, value);
            }
            return value;
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
            where TValue : new()
        {
            return dictionary.GetOrAdd(key, () => new TValue());
        }


        public static bool In<T>(this T x, params T[] args) where T : struct, IConvertible
        {
            return args.Contains(x);
        }



        public static bool IsGenericType(Type genericType, Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == genericType;
        }


        public static string LastWord(this string s, char delim)
        {
            return s.Substring(s.LastIndexOf(delim) + 1);
        }

        public static string GetSimpleName(this Type t)
        {
            return t.ToString().LastWord('.');
        }

        public static object ParsePrimitive(Type primitiveType, String value)
        {
            string s = value.Trim();

            if (primitiveType == typeof (int))
            {
                int v;
                if (Int32.TryParse(s, out v))
                {
                    //fpinfo.SetValue(v, parentItem.obj);
                    return v;
                }
                return null;
            }
            if (primitiveType == typeof (float))
            {
                float v;
                if (float.TryParse(s, out v))
                {
                    //fpinfo.SetValue(v, parentItem.obj);
                    return v;
                }
                return null;
            }
            if (primitiveType == typeof (double))
            {
                double v;
                if (double.TryParse(s, out v))
                {
                    //fpinfo.SetValue(v, parentItem.obj);
                    return v;
                }
                return null;
            }
            if (primitiveType == typeof (byte))
            {
                byte v;
                if (byte.TryParse(s, out v))
                {
                    //fpinfo.SetValue(v, parentItem.obj);
                    return v;
                }
                return null;
            }
            if (primitiveType == typeof (bool))
            {
                bool v;
                if (bool.TryParse(s, out v))
                {
                    //fpinfo.SetValue(v, parentItem.obj);
                    return v;
                }
                return null;
            }
            if (primitiveType.IsEnum)
            {
                return Enum.GetValues(primitiveType).Cast<object>().FirstOrDefault(
                    val => val.ToString().ToLower().Equals(s.ToLower()));
            }
            if (primitiveType == typeof (string))
            {
                return s;
            }
            return null;
        }

        public static string RandomName(List<Type> seedTypes, int tries = 0)
        {
            int depth = Random.Range(0, seedTypes.Count);
            Type t = seedTypes.ElementAt(depth);
            var props = t.GetProperties();
            int i = Random.Range(0, props.Length);
            var pinfo = props.ElementAt(i);
            if (tries < 10 && t.GetProperty(pinfo.Name) != null)
            {
                return RandomName(seedTypes, ++tries);
            }
            return pinfo.Name;
        }

        public static string RandomString()
        {
            Guid g = Guid.NewGuid();
            string guidString = Convert.ToBase64String(g.ToByteArray());
            guidString = guidString.Replace("=", "");
            guidString = guidString.Replace("+", "");
            return guidString;
        }

        public static float SmootherStep(float start, float end, float t)
        {
            float affection = end - start;
            t = t*t*t*(t*(6f*t - 15f) + 10f);
            affection *= t;
            return start + affection;
        }

        //thanks, skeet!
        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> source)
        {
            return new HashSet<T>(source);
        }

        public static int ToInt(this bool b)
        {
            return b ? 1 : 0;
        }

        public static string UniqueString()
        {
            string guidString = RandomString();

            UniqueStringSet = UniqueStringSet ?? new HashSet<string>();

            while (UniqueStringSet.Contains(guidString))
            {
                guidString = RandomString();
            }

            return guidString;
        }

        public static string UppercaseFirst(this string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        public static string WordWrap(this string message, int maxCharsPerLine)
        {
            int chars = maxCharsPerLine;
            for (int i = 1; i <= 4; i++)
                if (message.Length > chars*i)
                    for (int j = chars*i; j > (chars*i) - chars; j--)
                        if (message.ElementAt(j).Equals(' ') || message.ElementAt(j).Equals('/'))
                        {
                            message = message.Insert(j + 1, "\n");
                            break;
                        }
            return message;
        }
    } // end of class.
}