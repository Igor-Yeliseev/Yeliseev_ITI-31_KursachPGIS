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
        //private Car car;

        ///// <summary> Вектор направления </summary>
        //private Vector4 _direction;
        ///// <summary> Вектор направления </summary>
        //public Vector4 Direction
        //{
        //    get
        //    {
        //        return _direction;
        //    }
        //    set
        //    {
        //        _direction = value;
        //    }
        //}

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
