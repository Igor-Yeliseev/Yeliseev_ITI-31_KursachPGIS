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

        public Vector4 Target;
        

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

        //public EnemyCar(MeshObject mesh) : base(mesh)
        //{
        //    IsDead = false;
        //}

        public EnemyCar(List<MeshObject> meshes) :base(meshes)
        {
            IsDead = false;

            Target = new Vector4(1.0f, 0.0f, 0.0f, 0.0f);
        }
        
        public void Move(short sign)
        {
            Move(_direction, sign);
        }


        private int signCos = 0;

        /// <summary>
        /// Поворачивать машину на угол в сторону заданного направления
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public bool TurnToTarget(float angle)
        {
            float cosProd = MyVector.CosProduct(_direction, Target);

            if (cosProd != 0 && signCos == Math.Sign(cosProd))
            {
                if (cosProd < 0)
                    TurnCar(-angle);
                else
                    TurnCar(angle);

                signCos = Math.Sign(cosProd);

                return false;
            }
            else if(signCos != 0)
            {
                if (MyVector.CosProduct(_direction, Target) < 0)
                    TurnCar(-angle);
                else
                    TurnCar(angle);
                
                _direction = Target;
                
                Target = Vector4.Transform(Target, Matrix.RotationY(((float)Math.PI / 2)));
                //if (Math.Abs(Target.X) < 0.000001) Target.X = 0;
                //if (Math.Abs(Target.Z) < 0.000001) Target.Z = 0;
                return true;
            }
            else
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
