using System;

namespace WarZoneLib
{
    public struct Vector3
    {
        public float X;
        public float Y;
        public float Z;

        public Vector3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public static Vector3 operator +(Vector3 u, Vector3 v)
        {
            return new Vector3(u.X + v.X, u.Y + v.Y, u.Z + v.Z);
        }

        public static Vector3 operator -(Vector3 u, Vector3 v)
        {
            return new Vector3(u.X - v.X, u.Y - v.Y, u.Z - v.Z);
        }

        public static Vector3 operator *(float t, Vector3 v)
        {
            return new Vector3(t * v.X, t * v.Y, t * v.Z);
        }

        public static Vector3 operator *(Vector3 v, float t)
        {
            return t * v;
        }

        public static Vector3 operator /(Vector3 v, float d)
        {
            return 1f / d * v;
        }

        public float Length
        {
            get
            {
                return (float)Math.Sqrt((double)LengthSquared);
            }
        }

        public float LengthSquared
        {
            get
            {
                return Vector3.Dot(this, this);
            }
        }

        public static float Dot(Vector3 u, Vector3 v)
        {
            return (float)((double)u.X * (double)v.X + (double)u.Y * (double)v.Y + (double)u.Z * (double)v.Z);
        }

        public static Vector3 Cross(Vector3 u, Vector3 v)
        {
            Vector3 vector3;
            vector3.X = (float)((double)u.Y * (double)v.Z - (double)u.Z * (double)v.Y);
            vector3.Y = (float)-((double)u.X * (double)v.Z - (double)u.Z * (double)v.X);
            vector3.Z = (float)((double)u.X * (double)v.Y - (double)u.Y * (double)v.X);
            return vector3;
        }

        public Vector3 Normalize()
        {
            return this / Length;
        }
    }
}
