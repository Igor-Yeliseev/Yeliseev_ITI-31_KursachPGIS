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
    }
}
