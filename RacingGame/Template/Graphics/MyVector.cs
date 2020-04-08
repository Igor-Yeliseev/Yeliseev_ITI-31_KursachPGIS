using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    internal class MyVector
    {
        public static float GetLength(Vector4 v)
        {
            return (float)Math.Sqrt(v.X * v.X + v.Y * v.Y + v.Z * v.Z);
        }

        public static float DotProduct(Vector4 v1, Vector4 v2)
        {
            return (v1.X * v2.X + v1.Y * v2.Y + v1.Z * v2.Z);
        }

        /// <summary>
        /// Угол между двумя векторами
        /// </summary>
        /// <param name="v1"> Первый вектор</param>
        /// <param name="v2"> Второй вектор</param>
        /// <returns></returns>
        public static float GetAngle(Vector4 v1, Vector4 v2)
        {
            float x = DotProduct(v1, v2) / (v1.Length() * v2.Length());

            if (x > 1)
                return 0;
            else if (x < -1)
                return (float)Math.Acos(-1.0);
            else
                return (float)Math.Acos(x);
        }

        /// <summary>
        /// Получить проекцию вектора v1 на вектор v2
        /// </summary>
        /// <param name="v1"> Первый вектор</param>
        /// <param name="v2"> Второй вектор</param>
        /// <returns></returns>
        public static float GetProjection(Vector4 v1, Vector4 v2)
        {
            return DotProduct(v2, v1) / v2.Length();
        }

        public static float CosProduct(Vector4 v1, Vector4 v2)
        {
            return (v1.X * v2.Z - v2.X * v1.Z);
        }

        //public static Vector4 GetNormal(Vector4 v)
        //{

        //}
    }
}
