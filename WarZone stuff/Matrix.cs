using System;
using System.Diagnostics;

namespace WarZoneLib
{
    public struct Matrix
    {
        public float M00;
        public float M01;
        public float M02;
        public float M03;
        public float M10;
        public float M11;
        public float M12;
        public float M13;
        public float M20;
        public float M21;
        public float M22;
        public float M23;
        public float M30;
        public float M31;
        public float M32;
        public float M33;

        public Matrix(
          float m00,
          float m01,
          float m02,
          float m03,
          float m10,
          float m11,
          float m12,
          float m13,
          float m20,
          float m21,
          float m22,
          float m23,
          float m30,
          float m31,
          float m32,
          float m33)
        {
            M00 = m00;
            M01 = m01;
            M02 = m02;
            M03 = m03;
            M10 = m10;
            M11 = m11;
            M12 = m12;
            M13 = m13;
            M20 = m20;
            M21 = m21;
            M22 = m22;
            M23 = m23;
            M30 = m30;
            M31 = m31;
            M32 = m32;
            M33 = m33;
        }

        public static Matrix operator *(Matrix p, Matrix q)
        {
            return new Matrix()
            {
                M00 = (float)((double)p.M00 * (double)q.M00 + (double)p.M01 * (double)q.M10 + (double)p.M02 * (double)q.M20 + (double)p.M03 * (double)q.M30),
                M01 = (float)((double)p.M00 * (double)q.M01 + (double)p.M01 * (double)q.M11 + (double)p.M02 * (double)q.M21 + (double)p.M03 * (double)q.M31),
                M02 = (float)((double)p.M00 * (double)q.M02 + (double)p.M01 * (double)q.M12 + (double)p.M02 * (double)q.M22 + (double)p.M03 * (double)q.M32),
                M03 = (float)((double)p.M00 * (double)q.M03 + (double)p.M01 * (double)q.M13 + (double)p.M02 * (double)q.M23 + (double)p.M03 * (double)q.M33),
                M10 = (float)((double)p.M10 * (double)q.M00 + (double)p.M11 * (double)q.M10 + (double)p.M12 * (double)q.M20 + (double)p.M13 * (double)q.M30),
                M11 = (float)((double)p.M10 * (double)q.M01 + (double)p.M11 * (double)q.M11 + (double)p.M12 * (double)q.M21 + (double)p.M13 * (double)q.M31),
                M12 = (float)((double)p.M10 * (double)q.M02 + (double)p.M11 * (double)q.M12 + (double)p.M12 * (double)q.M22 + (double)p.M13 * (double)q.M32),
                M13 = (float)((double)p.M10 * (double)q.M03 + (double)p.M11 * (double)q.M13 + (double)p.M12 * (double)q.M23 + (double)p.M13 * (double)q.M33),
                M20 = (float)((double)p.M20 * (double)q.M00 + (double)p.M21 * (double)q.M10 + (double)p.M22 * (double)q.M20 + (double)p.M23 * (double)q.M30),
                M21 = (float)((double)p.M20 * (double)q.M01 + (double)p.M21 * (double)q.M11 + (double)p.M22 * (double)q.M21 + (double)p.M23 * (double)q.M31),
                M22 = (float)((double)p.M20 * (double)q.M02 + (double)p.M21 * (double)q.M12 + (double)p.M22 * (double)q.M22 + (double)p.M23 * (double)q.M32),
                M23 = (float)((double)p.M20 * (double)q.M03 + (double)p.M21 * (double)q.M13 + (double)p.M22 * (double)q.M23 + (double)p.M23 * (double)q.M33),
                M30 = (float)((double)p.M30 * (double)q.M00 + (double)p.M31 * (double)q.M10 + (double)p.M32 * (double)q.M20 + (double)p.M33 * (double)q.M30),
                M31 = (float)((double)p.M30 * (double)q.M01 + (double)p.M31 * (double)q.M11 + (double)p.M32 * (double)q.M21 + (double)p.M33 * (double)q.M31),
                M32 = (float)((double)p.M30 * (double)q.M02 + (double)p.M31 * (double)q.M12 + (double)p.M32 * (double)q.M22 + (double)p.M33 * (double)q.M32),
                M33 = (float)((double)p.M30 * (double)q.M03 + (double)p.M31 * (double)q.M13 + (double)p.M32 * (double)q.M23 + (double)p.M33 * (double)q.M33)
            };
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public static Matrix Identity
        {
            get
            {
                return new Matrix(1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f);
            }
        }

        public static Matrix Scaling(float x, float y, float z)
        {
            return new Matrix(x, 0.0f, 0.0f, 0.0f, 0.0f, y, 0.0f, 0.0f, 0.0f, 0.0f, z, 0.0f, 0.0f, 0.0f, 0.0f, 1f);
        }

        public static Matrix Translation(float x, float y, float z)
        {
            return new Matrix(1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, x, y, z, 1f);
        }

        public static Matrix RotationZ(float radians)
        {
            return Matrix.RotationAxis(new Vector3(0.0f, 0.0f, 1f), radians);
        }

        public static Matrix RotationAxis(Vector3 axis, float radians)
        {
            float num1 = (float)Math.Cos((double)radians);
            float num2 = (float)Math.Sin((double)radians);
            float num3 = 1f - num1;
            float x = axis.X;
            float y = axis.Y;
            float z = axis.Z;
            return new Matrix()
            {
                M00 = num3 * x * x + num1,
                M01 = (float)((double)num3 * (double)x * (double)y + (double)z * (double)num2),
                M02 = (float)((double)num3 * (double)x * (double)z - (double)y * (double)num2),
                M03 = 0.0f,
                M10 = (float)((double)num3 * (double)x * (double)y - (double)z * (double)num2),
                M11 = num3 * y * y + num1,
                M12 = (float)((double)num3 * (double)y * (double)z + (double)x * (double)num2),
                M13 = 0.0f,
                M20 = (float)((double)num3 * (double)x * (double)z + (double)y * (double)num2),
                M21 = (float)((double)num3 * (double)y * (double)z - (double)x * (double)num2),
                M22 = num3 * z * z + num1,
                M23 = 0.0f,
                M30 = 0.0f,
                M31 = 0.0f,
                M32 = 0.0f,
                M33 = 1f
            };
        }

        public Matrix Transpose()
        {
            return new Matrix(M00, M10, M20, M30, M01, M11, M21, M31, M02, M12, M22, M32, M03, M13, M23, M33);
        }

        public Vector3 TransformPoint(Vector3 p)
        {
            return new Vector3((float)((double)p.X * (double)M00 + (double)p.Y * (double)M10 + (double)p.Z * (double)M20) + M30, (float)((double)p.X * (double)M01 + (double)p.Y * (double)M11 + (double)p.Z * (double)M21) + M31, (float)((double)p.X * (double)M02 + (double)p.Y * (double)M12 + (double)p.Z * (double)M22) + M32);
        }
    }
}
