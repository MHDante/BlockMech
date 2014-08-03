using System;
using UnityEngine;

namespace OrbItUtils
{
    public static class GMath
    {
        public static float Epsilon = 0.00001f;
        public const float Pi = Mathf.PI;
        public const float TwoPi = (float)(Math.PI * 2);
        public const float PibyTwo = (float)(Math.PI / 2);
        public const float RootOfTwo = 1.41421356237f;
        public const float InvRootOfTwo = 0.70710678118f;
        public static float Between0And2Pi(this float value)
        {
            //sawtooth?
            if (value > 2 * Pi) value = value % (2 * Pi);
            if (value < 0) value = 2 * Pi + value;
            return value;
        }
        public static bool ApproxZero(this float f)
        {
            return Mathf.Abs(f) < Epsilon;
        }

        public static bool ApproxEquals(this float f, float other)
        {
            return Mathf.Abs(f - other) < Epsilon;
        }

        public static float Sawtooth(float x, float mod)
        {
            float ret = x % mod;
            if (x >= 0 || ret.ApproxZero())
            {
                return ret;
            }
            return mod - Mathf.Abs(ret);
        }
        public static float AngleLerp(float source, float dest, float amount)
        {
            float result;


            if (source < Pi && dest > source + Pi)
            {
                result = Mathf.Lerp(source, dest - (2 * Pi), amount);
            }
            else if (source > Pi && dest < source - Pi)
            {
                result = Mathf.Lerp(source, dest + (2 * Pi), amount);
            }
            else
            {
                result = Mathf.Lerp(source, dest, amount);
            }
            return result;
        }

        public static float CircularDistance(float x, float v, int t)
        {
            int half = t / 2;
            if (x.ApproxEquals(v)) return 0;
            if (x > v)
            {
                if (v > x - half)
                {
                    return v - x; //negative
                }
                return t - x + v;
            }
            if (v < x + half)
            {
                return v - x;
            }
            return v - t - x; //negative
        }
    }
    public static class VMath
    {
        #region /// Existing Methods ///
        public static void Test()
        {
            //Vector2.Add;
            //Vector2.Barycentric;
            //Vector2.CatmullRom;
            //Vector2.Clamp;
            //Vector2.Distance;
            //Vector2.DistanceSquared;
            //Vector2.Divide;
            //Vector2.Dot;
            //Vector2.Equals;
            //Vector2.Hermite;
            //Vector2.Lerp;
            //Vector2.Max;
            //Vector2.Min;
            //Vector2.Multiply;
            //Vector2.Negate;
            //Vector2.Normalize;
            //Vector2.One;
            //Vector2.Reflect;
            //Vector2.SmoothStep;
            //Vector2.Subtract;
            //Vector2.Transform;
            //Vector2.TransformNormal;
            //Vector2.UnitX;
            //Vector2.UnitY;
            //Vector2.Zero;
        }
        #endregion
        public static bool IsFucked(this Vector2 v)
        {
            return Single.IsInfinity(v.x) || Single.IsNaN(v.x) || Single.IsInfinity(v.y) || Single.IsNaN(v.y);
        }
        public static void Set(ref Vector2 v, float x, float y)
        {
            v.x = x; v.y = y;
        }
        public static Vector2 AngleToVector(float angle)
        {
            return new Vector2((float)Math.Sin(angle), -(float)Math.Cos(angle));
        }

        public static float[] ToFloatArray(this Vector2 v2)
        {
            var result = new float[2];
            result[0] = v2.x;
            result[1] = v2.y;
            return result;
        }

        public static float VectorToAngle(Vector2 vector)
        {
            float value = (float)Math.Atan2(vector.x, -vector.y);//should the components be swapped here?
            if (value > GMath.TwoPi)//should this be a sawtooth?
                value = value % GMath.TwoPi;
            if (value < 0)
                value = GMath.TwoPi + value;
            return value;
        }
        public static Vector2 VectorRotateLerp(Vector2 source, Vector2 direction, float amount)
        {
            float oldAngle = VectorToAngle(source);
            float newAngle = VectorToAngle(direction);
            float lerpedAngle = GMath.AngleLerp(oldAngle, newAngle, amount);
            //Vector2 finalDir = VMath.AngleToVector(lerpedAngle);
            return Redirect(source, AngleToVector(lerpedAngle));
        }


        public static bool IsWithin(this Vector2 v, Vector2 topLeft, Vector2 bottomRight)
        {
            return (v.x >= topLeft.x && v.y >= topLeft.y && v.x <= bottomRight.x && v.y <= bottomRight.y);
        }
        public static Vector2 Rotate(this Vector2 v, float radians)
        {
            double c = Math.Cos(radians);
            double s = Math.Sin(radians);
            double xp = v.x * c - v.y * s;
            double yp = v.x * s + v.y * c;
            v.x = (float)xp;
            v.y = (float)yp;
            return v;
        }
        public static Vector2 ProjectOnto(this Vector2 source, Vector2 target)
        {
            return (Vector2.Dot(source, target) / target.SqrMagnitude()) * target;
        }

        public static Vector2 Cross(Vector2 v, double a)
        {
            return new Vector2((float)a * v.y, -(float)a * v.x);
        }
        public static Vector2 Cross(double a, Vector2 v)
        {
            return new Vector2(-(float)a * v.y, (float)a * v.x);
        }
        public static double Cross(Vector2 a, Vector2 b)
        {
            return a.x * b.y - a.y * b.x;
        }
        public static Vector2 MultVectDouble(Vector2 v, double d)
        {
            return new Vector2(v.x * (float)d, v.y * (float)d);
        }
        //todo: test resize and redirect
        public static Vector2 Resize(Vector2 v, float length)
        {
            return v * length / v.magnitude;
        }
        public static Vector2 Redirect(Vector2 source, Vector2 direction)
        {
            return direction * source.magnitude / direction.magnitude;
        }

        public static Vector2 NormalizeSafe(this Vector2 v)
        {
            if (v.x.ApproxZero() && v.y.ApproxZero()) return v;
            float len = v.magnitude;
            if (len.ApproxZero()) return v;
            float invLen = 1.0f / len;
            v.x *= invLen;
            v.y *= invLen;
            return v;
        }

        public static void NormalizeSafe(ref Vector2 v)
        {
            if (v.x.ApproxZero() && v.y.ApproxZero()) return;
            float len = v.magnitude;
            if (len.ApproxZero()) return;
            float invLen = 1.0f / len;
            v.x *= invLen;
            v.y *= invLen;
        }
    }
}
