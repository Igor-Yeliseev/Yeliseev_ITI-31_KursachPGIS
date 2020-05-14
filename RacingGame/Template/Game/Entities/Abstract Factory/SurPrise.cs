using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Entities.Abstract_Factory
{
    abstract class SurPrise : StaticPrefab, IDisposable
    {

        /// <summary> Направление вдоль прямоугольника коллизий </summary>
        public Vector3 Direction;

        public float Value { get; set; }

        public abstract SurpriseType Type { get; }

        /// <summary>
        /// Конструктор класса сюрприза (Урон - единиц здоровья, Скорость - процент увеличения масимальной)
        /// </summary>
        /// <param name="mesh"></param>
        /// <param name="type"> Тип бонуса</param>
        /// <param name="value"> Величина очков бонуса (в зависимости от типа)</param>
        public SurPrise(MeshObject mesh, float value) : base(mesh)
        {
            Value = value;
            Direction = new Vector3(OBBox.Extents.X, 0, 0);
        }

        /// <summary>
        /// Отрисовка сюрприза
        /// </summary>
        /// <param name="viewMatrix"></param>
        /// <param name="projectionMatrix"></param>
        /// <param name="angle"></param>
        public void Draw(Matrix viewMatrix, Matrix projectionMatrix, float angle = 0)
        {
            _meshes.ForEach(m =>
            {
                if (angle != 0 && Type != SurpriseType.Damage)
                {
                    m.YawBy(angle);
                    RotateOBB(angle);
                }
                m.Render(viewMatrix, projectionMatrix);
            });
        }

        /// <summary> Событие срабатывающее при поднятии бонуса </summary>
        public event Action<float> OnCatched;

        public override void CollisionResponce(PhysicalObject obj)
        {
            Car car = obj as Car;

            if (car != null)
            {
                switch (Type)
                {
                    case SurpriseType.Health:
                        if (car.Health + Value > 100)
                        {
                            OnCatched?.Invoke((100 - car.Health) / car.Health);
                        }
                        else
                            OnCatched?.Invoke(Value / car.Health);
                        car.Health += Value;
                        break;
                    case SurpriseType.Speed:
                        car.MaxSpeed -= car.MaxSpeed * (Value / 100);
                        OnCatched?.Invoke(0);
                        break;
                    case SurpriseType.Damage:
                        car.MaxSpeed -= car.MaxSpeed * (Value / 100);
                        OnCatched?.Invoke(0);
                        break;
                }
                Dispose();
            }

        }

        public void Dispose()
        {
            _meshes.ForEach(m => m.Dispose());
        }
    }
}
