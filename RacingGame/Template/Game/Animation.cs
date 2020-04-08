using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    internal class Animation
    {
        public static bool IsWheelsAnimate { get; set; } = false;

        public static void AnimateWheels(Car car, float alpha)
        {
            if (IsWheelsAnimate == true)
            {
                IsWheelsAnimate = car.AnimationWheels(alpha);
            }
        }
    }
}
