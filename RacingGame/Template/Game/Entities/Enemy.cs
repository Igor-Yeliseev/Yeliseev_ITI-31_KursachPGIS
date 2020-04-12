using SharpDX;
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
            
            //Target = new Vector4(0.1f, 0.0f, -0.9f, 0.0f);
            //float w = MyVector.GetAngle(_direction, _target) * 180 / (float)Math.PI;
        }
        
        public void Move(short sign)
        {
            Move(_direction, sign);
        }


        private int signCos = 0;
        private bool onTarget = false;
        /// <summary> Направлен ли вектор Direction в точку Target </summary>
        public bool IsOnTarget { get => onTarget; }

        /// <summary>
        /// Поворачивать машину на угол в сторону заданного направления
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public bool TurnToTarget(float angle)
        {
            float cosProd = MyVector.CosProduct(_direction, _target);

            float w = MyVector.GetAngle(_direction, _target) * 180 / (float)Math.PI;

            if (cosProd != 0 && signCos == Math.Sign(cosProd))
            {
                if (cosProd < 0)
                    TurnCar(angle);
                else
                    TurnCar(-angle);

                signCos = Math.Sign(cosProd);
                onTarget = false;
                return false;
            }
            else if(signCos != 0)
            {
                float ang = /*(float)Math.PI -*/ MyVector.GetAngle(_direction, _target);

                if (MyVector.CosProduct(_direction, _target) < 0)
                    TurnCar(ang);
                else
                    TurnCar(-ang);

                //_direction = Target;

                //Target = Vector4.Transform(Target, Matrix.RotationY(((float)Math.PI / 2)));
                //if (Math.Abs(Target.X) < 0.000001) Target.X = 0;
                //if (Math.Abs(Target.Z) < 0.000001) Target.Z = 0;
                signCos = 0;
                onTarget = true;
                return true;
            }
            else if(!onTarget)
                signCos = Math.Sign(cosProd);

            return false;
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
