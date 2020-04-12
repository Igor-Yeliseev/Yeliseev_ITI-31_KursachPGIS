using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    internal class Animations
    {
        public bool IsWheelsAnimate { get; set; } = false;

        public bool IsEnemyTurned { get; set; } = true;

        /// <summary>
        /// Анимация поворота колес в начальное положение
        /// </summary>
        /// <param name="car"></param>
        /// <param name="alpha"></param>
        public void AnimateWheels(Car car, float alpha)
        {
            if (IsWheelsAnimate == true)
            {
                IsWheelsAnimate = car.AnimationWheels(alpha);
            }
        }

        public void AnimateEnemyToTarget(EnemyCar enemy, float alpha)
        {
            enemy.TurnToTarget(alpha);
            IsEnemyTurned = enemy.IsOnTarget;
        }
    }
}
