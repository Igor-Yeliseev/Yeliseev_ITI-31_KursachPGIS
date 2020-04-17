using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    class MyMath
    {
        static Random random = new Random();

        /// <summary>
        /// Линейная интерполяция
        /// </summary>
        /// <param name="x"> Начальное значение</param>
        /// <param name="target"> Конечное значение</param>
        /// <param name="amount"> Величина 0 < amount < 1</param>
        /// <returns></returns>
        public static float Lerp(float x, float target, float amount)
        {
            if (Math.Abs(target - x) < 0.01)
                return target;

            float dif = target - x;

            return (x + dif * amount / 60);
        }

        /// <summary>
        /// Линейная интерполяция
        /// </summary>
        /// <param name="x"> Начальное значение</param>
        /// <param name="target"> Конечное значение</param>
        /// <param name="amount"> Величина , amount U [0,1]</param>
        /// <param name="eps"> Точность </param>
        /// <returns></returns>
        public static float Lerp(float x, float target, float amount, float eps)
        {
            if (Math.Abs(target - x) < eps)
                return target;

            float dif = target - x;

            return (x + dif * amount / 60);
        }

        /// <summary>
        /// Пересечение луча с боксом коллизий
        /// </summary>
        /// <param name="ray"> Луч</param>
        /// <param name="OBBox"> Ориентированный бокс</param>
        /// <returns></returns>
        public static bool RayIntersects(ref Ray ray, OrientedBoundingBox OBBox)
        {
            return OBBox.Intersects(ref ray);
        }

        /// <summary>
        /// Пересечение луча с боксом коллизий
        /// </summary>
        /// <param name="ray"> Луч</param>
        /// <param name="OBBox"> Ориентированный бокс</param>
        /// <param name="point"> Точка пересечения (SharpDX.Vector3.Zero если нет пересечения)</param>
        /// <returns></returns>
        public static bool RayIntersects(ref Ray ray, ref OrientedBoundingBox OBBox, out Vector3 point)
        {
            return OBBox.Intersects(ref ray, out point);
        }

        /// <summary>
        /// Пересечение луча с боксом коллизий
        /// </summary>
        /// <param name="ray"> Луч</param>
        /// <param name="OBBox"> Ориентированный бокс</param>
        /// <param name="distance"> Расстояние между точкой пересечения и позицией луча</param>
        /// <returns></returns>
        public static bool RayIntersects(ref Ray ray, ref OrientedBoundingBox OBBox, out float distance)
        {
            distance = -1;
            Vector3 point;
            if(OBBox.Intersects(ref ray, out point))
            {
                distance = (point - ray.Position).Length();
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// Gets random float number within range.
        /// </summary>
        /// <param name="min"> Minimum.</param>
        /// <param name="max"> Maximum.</param>
        /// <returns> Random float number.</returns>
        public static float Random(float min, float max)
        {
            return SharpDX.RandomUtil.NextFloat(random, min, max);
        }
    }
}
