﻿using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    class EnemyCar : Car
    {

        private Vector4 _target;
        /// <summary> Вектор от центра заднего моста к точки цели </summary>
        public Vector4 Target
        {
            get => _target;
            set
            {
                _target = new Vector4(value.X - _rearAxle.X, 0.0f, value.Z - _rearAxle.Z, 0.0f);
                onTarget = false;
                wheelsOnTarget = false;
            }
        }
        
        /// <summary> Мертв ли враг </summary>
        public bool IsDead { get; set; }

        /// <summary> Здоровье </summary>
        int _health = 100;
        /// <summary> Здоровье </summary>
        public int Health
        {
            get
            {
                return _health;
            }
            set
            {
                _health += value;
                if (_health < 0)
                {
                    _health = 0;
                    IsDead = true;
                }
                else if (_health > 100)
                    _health = 100;
            }
        }
        
        /// <summary> Лучи сенсоров для ИИ </summary>
        Ray[] sensors = new Ray[5];
        
        public EnemyCar(List<MeshObject> meshes) : base(meshes)
        {
            IsDead = false;

            wheelsDirection = _direction;

            //Target = new Vector4(0.1f, 0.0f, -0.9f, 0.0f);
            //float w = MyVector.GetAngle(_direction, _target) * 180 / (float)Math.PI;
        }

        
        public override void TurnWheelsLeft(float alpha)
        {
            //wheelsDirection = Vector4.Transform(wheelsDirection, Matrix.RotationY(-alpha));
            //wheel1.YawBy(-alpha);
            //wheel2.YawBy(-alpha);

            if (turnCount >= -itrs)
            {
                wheelsDirection = Vector4.Transform(wheelsDirection, Matrix.RotationY(-alpha));
                wheel1.YawBy(-alpha);
                wheel2.YawBy(-alpha);
                turnCount -= 2;
            }

        }

        public override void TurnWheelsRight(float alpha)
        {
            //wheelsDirection = Vector4.Transform(wheelsDirection, Matrix.RotationY(alpha));
            //wheel1.YawBy(alpha);
            //wheel2.YawBy(alpha);

            if (turnCount <= itrs)
            {
                wheelsDirection = Vector4.Transform(wheelsDirection, Matrix.RotationY(alpha));
                wheel1.YawBy(alpha);
                wheel2.YawBy(alpha);
                turnCount += 2;
            }
        }

        public override void TurnCar(float alpha)
        {
            base.TurnCar(alpha);
            wheelsDirection = Vector4.Transform(wheelsDirection, Matrix.RotationY(alpha));
        }

        /// <summary>
        /// Знак косого произведения для колес
        /// </summary>
        private int signCosW = 0;
        private bool wheelsOnTarget = false;
        /// <summary> Повернуты ли колеса в точку Target </summary>
        public bool IsWheelsOnTarget { get => wheelsOnTarget; }

        public Vector4 wheelsDirection;
        /// <summary>
        /// Поворачивать колеса к цели на угол angle
        /// </summary>
        /// <param name="angle"> Угол поворота</param>
        public void TurnWheelsToTarget(float angle)
        {
            //if (wheelsOnTarget)
            //    return;

            float cosProd = MyVector.CosProduct(wheelsDirection, _target);

            if (cosProd != 0 && signCosW == Math.Sign(cosProd))
            {
                if (cosProd < 0)
                    TurnWheelsRight(angle);
                else
                    TurnWheelsLeft(angle);

                signCosW = Math.Sign(cosProd);
                wheelsOnTarget = false;
            }
            else if (signCosW != 0)
            {
                float ang = MyVector.GetAngle(wheelsDirection, _target);

                if (MyVector.CosProduct(wheelsDirection, _target) < 0)
                    TurnWheelsRight(angle);
                else
                    TurnWheelsLeft(angle);

                signCosW = 0;
                wheelsOnTarget = true;

                //_direction = Target;
                //Target = Vector4.Transform(Target, Matrix.RotationY(((float)Math.PI / 2)));
                //if (Math.Abs(Target.X) < 0.000001) Target.X = 0;
                //if (Math.Abs(Target.Z) < 0.000001) Target.Z = 0;
            }
            else
                signCosW = Math.Sign(cosProd);
        }

        /// <summary> Перемещать машину вдоль направления в соответствии с ее скоростью </summary>
        public void Move()
        {
            Move(_direction, Math.Sign(_speed));
        }

        private int signCos = 0;
        private bool onTarget = false;
        /// <summary> Направлен ли вектор Direction в точку Target </summary>
        public bool IsOnTarget { get => onTarget; }

        /// <summary>
        /// Поворачивать машину в сторону заданного направления на угол angle
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public void TurnToTarget(float angle)
        {
            if (onTarget)
                return;
            
            float cosProd = MyVector.CosProduct(_direction, _target);

            if (cosProd != 0 && signCos == Math.Sign(cosProd))
            {
                if (cosProd < 0)
                    TurnCar(angle);
                else
                    TurnCar(-angle);

                signCos = Math.Sign(cosProd);
                onTarget = false;
            }
            else if(signCos != 0)
            {
                angle = MyVector.GetAngle(_direction, _target);

                if (MyVector.CosProduct(_direction, _target) < 0)
                    TurnCar(angle);
                else
                    TurnCar(-angle);

                signCos = 0;
                onTarget = true;
            }
            else
                signCos = Math.Sign(cosProd);
        }

        /// <summary>
        /// Столкновение врага с объектом
        /// </summary>
        /// <param name="obj"> Физический объект</param>
        /// <returns></returns>
        public override bool CollisionTest(PhysicalObject obj)
        {
            return false;
        }
    }
}
