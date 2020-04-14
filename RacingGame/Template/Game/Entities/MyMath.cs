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
        /// <param name="boundingBox"> Ориентированный бокс</param>
        /// <returns></returns>
        public static bool RayIntersects(ref Ray ray, OrientedBoundingBox boundingBox)
        {
            return boundingBox.Intersects(ref ray);
        }

        /// <summary>
        /// Пересечение луча с боксом коллизий
        /// </summary>
        /// <param name="ray"> Луч</param>
        /// <param name="boundingBox"> Ориентированный бокс</param>
        /// <param name="point"> Точка пересечения (SharpDX.Vector3.Zero если нет пересечения)</param>
        /// <returns></returns>
        public static bool RayIntersects(ref Ray ray, OrientedBoundingBox boundingBox, out Vector3 point)
        {
            return boundingBox.Intersects(ref ray, out point);
        }

        /// <summary>
        /// Пересечение луча с боксом коллизий
        /// </summary>
        /// <param name="ray"> Луч</param>
        /// <param name="boundingBox"> Ориентированный бокс</param>
        /// <param name="distance"> Расстояние между точкой пересечения и позицией луча</param>
        /// <returns></returns>
        public static bool RayIntersects(ref Ray ray, OrientedBoundingBox boundingBox, out float distance)
        {
            distance = -1;
            Vector3 point;
            if(boundingBox.Intersects(ref ray, out point))
            {
                distance = (point - ray.Position).Length();
                return true;
            }
            else
            {
                return false;
            } 
        }
    }
}
