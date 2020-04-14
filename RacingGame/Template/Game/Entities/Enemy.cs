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

        private Vector3 _target;
        /// <summary> Вектор от центра заднего моста к точки цели </summary>
        public Vector3 Target
        {
            get => _target;
            set
            {
                _target = new Vector3(value.X - _rearAxle.X, 0.0f, value.Z - _rearAxle.Z);
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
        public Ray rayLeft, rayRight, rayCenter;

        public EnemyCar(List<MeshObject> meshes) : base(meshes)
        {
            IsDead = false;
            wheelsDirection = _direction;
            
            var rayRPos = new Vector3(wheel1.Position.X, wheel1.Position.Y, wheel1.Position.Z + 0); // ИЗМЕНИТЬ ПОТОМ !!!
            var rayLPos = new Vector3(wheel2.Position.X, wheel2.Position.Y, wheel2.Position.Z + 0);
            var rayCenterPos = new Vector3(_frontAxle.X, _frontAxle.Y, _frontAxle.Z + 0);
            rayRight = new Ray(rayRPos, _direction);
            rayLeft = new Ray(rayLPos, _direction);
            rayCenter = new Ray(rayCenterPos, _direction);

            rayLftToRearAxle = rayLeft.Position - _rearAxle;
            rayRhtToRearAxle = rayRight.Position - _rearAxle;
            rayCenToRearAxle = rayCenter.Position - _rearAxle;

            //Target = new Vector3(0.1f, 0.0f, -0.9f);
            //float w = MyVector.GetAngle(_direction, _target) * 180 / (float)Math.PI;
        }
        
        public override void TurnWheelsLeft(float alpha)
        {
            //wheelsDirection = Vector3.Transform(wheelsDirection, Matrix3x3.RotationY(-alpha));
            //wheel1.YawBy(-alpha);
            //wheel2.YawBy(-alpha);

            if (turnCount >= -itrs)
            {
                wheelsDirection = Vector3.Transform(wheelsDirection, Matrix3x3.RotationY(-alpha));
                wheel1.YawBy(-alpha);
                wheel2.YawBy(-alpha);
                turnCount -= 2;
            }

        }

        public override void TurnWheelsRight(float alpha)
        {
            //wheelsDirection = Vector3.Transform(wheelsDirection, Matrix3x3.RotationY(alpha));
            //wheel1.YawBy(alpha);
            //wheel2.YawBy(alpha);

            if (turnCount <= itrs)
            {
                wheelsDirection = Vector3.Transform(wheelsDirection, Matrix3x3.RotationY(alpha));
                wheel1.YawBy(alpha);
                wheel2.YawBy(alpha);
                turnCount += 2;
            }
        }

        public override void TurnCar(float alpha)
        {
            if (alpha == 0) return;

            base.TurnCar(alpha);
            wheelsDirection = Vector3.Transform(wheelsDirection, Matrix3x3.RotationY(alpha));
            SetRaysDirection(alpha);
        }

        /// <summary> Расстояние от точки вращения до поцизии левого сенсора </summary>
        Vector3 rayLftToRearAxle;
        /// <summary> Расстояние от точки вращения до поцизии правого сенсора </summary>
        Vector3 rayRhtToRearAxle;
        /// <summary> Расстояние от точки вращения до поцизии центрального сенсора </summary>
        Vector3 rayCenToRearAxle;

        /// <summary>
        /// Пересчитать направление лучей сенсоров
        /// </summary>
        private void SetRaysDirection(float alpha)
        {
            Vector3 dir = Vector3.Transform(rayCenter.Direction, Matrix3x3.RotationY(alpha));
            rayLeft.Direction = dir;
            rayRight.Direction = dir;
            rayCenter.Direction = dir;

            rayLftToRearAxle = Vector3.Transform(rayLftToRearAxle, Matrix3x3.RotationY(alpha));
            rayRhtToRearAxle = Vector3.Transform(rayRhtToRearAxle, Matrix3x3.RotationY(alpha));
            rayCenToRearAxle = Vector3.Transform(rayCenToRearAxle, Matrix3x3.RotationY(alpha));

            rayLeft.Position = _rearAxle + rayLftToRearAxle;
            rayRight.Position = _rearAxle + rayRhtToRearAxle;
            rayCenter.Position = _rearAxle + rayCenToRearAxle;
        }

        /// <summary>
        /// Знак косого произведения для колес
        /// </summary>
        private int signCosW = 0;
        private bool wheelsOnTarget = false;
        /// <summary> Повернуты ли колеса в точку Target </summary>
        public bool IsWheelsOnTarget { get => wheelsOnTarget; }

        public Vector3 wheelsDirection;
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
                //Target = Vector3.Transform(Target, Matrix3x3.RotationY(((float)Math.PI / 2)));
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

        /// <summary> Смещение вдоль направления </summary>
        /// <param name="direction"> Вектор направления</param>
        /// <param name="sign"> Знак (вперед, назад)</param>
        public override void Move(Vector3 direction, int sign)
        {
            base.Move(direction, sign);
            MoveRays(direction, sign);
        }

        public override void MoveBy(float dX, float dY, float dZ)
        {
            base.MoveBy(dX, dY, dZ);
            MoveByRays(new Vector3(dX, dY, dZ));
        }
        private void MoveByRays(Vector3 offset)
        {
            rayLeft.Position += offset;
            rayRight.Position += offset;
            rayCenter.Position += offset;
        }

        /// <summary> Смещение лучей сенсоров вдоль направления </summary>
        /// <param name="direction"> Вектор направления</param>
        /// <param name="sign"> Знак (вперед, назад)</param>
        private void MoveRays(Vector3 direction, int sign)
        {
            if (sign == 0) return;

            direction *= scale * sign;
        
            rayLeft.Position += direction;
            rayRight.Position += direction;
            rayCenter.Position += direction;
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
        
        private float distance;
        /// <summary> Дистанция до объекта </summary>
        public float Distance { get => distance; }
        
        public void CheckObstacle(OrientedBoundingBox orientedBoundingBox, float alpha)
        {
            float distanceL, distanceR;

            bool interLeft = MyMath.RayIntersects(ref rayLeft, orientedBoundingBox, out distanceL);
            bool interRight = MyMath.RayIntersects(ref rayRight, orientedBoundingBox, out distanceR);
            //bool interCenter = MyMath.RayIntersects(ref rayCenter, orientedBoundingBox, out disntaceC);

            if (interLeft && interRight)
            {
                //Brake();
                //Speed = 0;
            }

            if (interLeft && distanceL <= 5)
            {
                TurnCar(alpha); // Поворачиваю направо
            }

            if(interRight && distanceR <= 5)
            {
                TurnCar(-alpha); // Поворачиваю налево
            }
            
        }
    }
}
