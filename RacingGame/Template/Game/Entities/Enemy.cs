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


        // Ray[] sensors = new Ray[5];

        /// <summary> Передний левый луч </summary>
        public Ray rayLeft;
        /// <summary> Передний правый луч </summary>
        public Ray rayRight;
        /// <summary> Передний сентральный луч </summary>
        public Ray rayCenter;
        /// <summary> Боковой левый луч </summary>
        public Ray raySideLeft;
        /// <summary> Боковой правый луч </summary>
        public Ray raySideRight;

        /// <summary> Расстояние от точки вращения до поцизии левого сенсора </summary>
        Vector3 rayLftToRearAxle;
        /// <summary> Расстояние от точки вращения до поцизии правого сенсора </summary>
        Vector3 rayRhtToRearAxle;
        /// <summary> Расстояние от точки вращения до поцизии центрального сенсора </summary>
        Vector3 rayCenToRearAxle;

        /// <summary> Расстояние от точки вращения до поцизии бокового левого сенсора </summary>
        Vector3 raySideLftToRearAxle;
        /// <summary> Расстояние от точки вращения до поцизии правого левого сенсора </summary>
        Vector3 raySideRhtToRearAxle;

        public EnemyCar(List<MeshObject> meshes) : base(meshes)
        {
            IsDead = false;
            wheelsDirection = _direction;
            
            var Z = boundingBox.Center.Z + boundingBox.Extents.Z; // Машины должны стоять параллельно осям

            var rayRPos = new Vector3(wheel1.Position.X, wheel1.Position.Y, wheel1.Position.Z + 0); // ИЗМЕНИТЬ ПОТОМ Z !!!
            var rayLPos = new Vector3(wheel2.Position.X, wheel2.Position.Y, wheel2.Position.Z + 0);
            var rayCenterPos = new Vector3(_frontAxle.X, _frontAxle.Y, _frontAxle.Z + 0);

            var raySideRPos = new Vector3(wheel1.Position.X + 0, wheel1.Position.Y, Z); // ИЗМЕНИТЬ ПОТОМ X !!!
            var raySideLPos = new Vector3(wheel2.Position.X - 0, wheel2.Position.Y, Z);
            
            // Передние лучи
            rayRight = new Ray(rayRPos, _direction);
            rayLeft = new Ray(rayLPos, _direction);
            rayCenter = new Ray(rayCenterPos, _direction);
            rayLftToRearAxle = rayLeft.Position - _rearAxle;
            rayRhtToRearAxle = rayRight.Position - _rearAxle;
            rayCenToRearAxle = rayCenter.Position - _rearAxle;

            // Боковые лучи
            raySideLftToRearAxle = raySideLPos - _rearAxle;
            raySideRhtToRearAxle = raySideRPos - _rearAxle;
            raySideLeft = new Ray(raySideLPos, Vector3.Transform(_direction, Matrix3x3.RotationY(-(float)Math.PI / 2)));
            raySideRight = new Ray(raySideRPos, Vector3.Transform(_direction, Matrix3x3.RotationY((float)Math.PI / 2)));

            //Target = new Vector3(0.1f, 0.0f, -0.9f);
            //float w = MyVector.GetAngle(_direction, _target) * 180 / (float)Math.PI;
        }
        
        public override void TurnWheelsLeft(float alpha)
        {
            if (turnCount >= -itrs)
            {
                _direction = Vector3.Transform(_direction, Matrix3x3.RotationY(-alpha));
                wheelsDirection = Vector3.Transform(wheelsDirection, Matrix3x3.RotationY(-alpha));
                wheel1.YawBy(-alpha);
                wheel2.YawBy(-alpha);
                turnCount -= 2;
            }
        }

        public override void TurnWheelsRight(float alpha)
        {
            if (turnCount <= itrs)
            {
                _direction = Vector3.Transform(_direction, Matrix3x3.RotationY(alpha));
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
        
        /// <summary>
        /// Пересчитать направление лучей сенсоров
        /// </summary>
        private void SetRaysDirection(float alpha)
        {
            // Передние лучи
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

            // Боковые лучи
            raySideLeft.Direction = Vector3.Transform(raySideLeft.Direction, Matrix3x3.RotationY(alpha));
            raySideRight.Direction = Vector3.Transform(raySideRight.Direction, Matrix3x3.RotationY(alpha));

            raySideLftToRearAxle = Vector3.Transform(raySideLftToRearAxle, Matrix3x3.RotationY(alpha));
            raySideRhtToRearAxle = Vector3.Transform(raySideRhtToRearAxle, Matrix3x3.RotationY(alpha));
            raySideLeft.Position = _rearAxle + raySideLftToRearAxle;
            raySideRight.Position = _rearAxle + raySideRhtToRearAxle;
        }

        /// <summary>
        /// Знак косого произведения для колес
        /// </summary>
        private int signCosW = 0;
        private bool wheelsOnTarget = false;
        /// <summary> Повернуты ли колеса в точку Target </summary>
        public bool IsWheelsOnTarget { get => wheelsOnTarget; set => wheelsOnTarget = value; }

        

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

            raySideLeft.Position += offset;
            raySideRight.Position += offset;
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

            raySideLeft.Position += direction;
            raySideRight.Position += direction;
        }

        private int signCos = 0;
        private bool onTarget = false;
        /// <summary> Направлен ли вектор Direction в точку Target </summary>
        public bool IsOnTarget { get => onTarget; set => onTarget = value; }

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
        /// Поворачивать колеса в сторону заданного направления на угол angle
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public void TurnWheelsToTarget(float angle)
        {
            if (onTarget)
                return;

            CheckWheelsDirOnTarget(angle);

            float cosProd = MyVector.CosProduct(_carDirection, _target);

            if (cosProd != 0 && signCos == Math.Sign(cosProd))
            {
                if (cosProd < 0)
                    TurnWheelsRight(angle);
                else
                    TurnWheelsLeft(angle);

                signCos = Math.Sign(cosProd);
                onTarget = false;
            }
            else if (signCos != 0)
            {
                float ang = MyVector.GetAngle(_carDirection, _target);

                if (MyVector.CosProduct(_carDirection, _target) < 0)
                    TurnWheelsRight(angle);
                else
                    TurnWheelsLeft(angle);

                signCos = 0;
                onTarget = true;
            }
            else
                signCos = Math.Sign(cosProd);

        }

        /// <summary> Вектор направления передних колес </summary>
        private Vector3 wheelsDirection;
        /// <summary> Вектор направления передних колес </summary>
        public Vector3 WheelsDirection { get => wheelsDirection; set => wheelsDirection = value; }

        /// <summary>
        /// Проверить направравлены ли колеса вдоль вектора от цели до заднего моста
        /// </summary>
        public void CheckWheelsDirOnTarget(float angle) 
        {
            if (wheelsOnTarget)
                return;

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
                //float ang = MyVector.GetAngle(wheelsDirection, _target);

                if (MyVector.CosProduct(wheelsDirection, _target) < 0)
                    TurnWheelsRight(angle);
                else
                    TurnWheelsLeft(angle);

                signCosW = 0;
                wheelsOnTarget = true;
            }
            else
                signCosW = Math.Sign(cosProd);
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
            // Передние лучи
            CheckObstacleFrontRays(orientedBoundingBox, alpha);
            // Боковые лучи
            CheckObstacleSideRays(orientedBoundingBox, alpha);
        }

        /// <summary> Минимальное расстояние до препятствия до принятия решения </summary>
        private float minFrontDistance = 6;
        private void CheckObstacleFrontRays(OrientedBoundingBox orientedBoundingBox, float alpha)
        {
            float distanceL, distanceR;

            bool interLeft = MyMath.RayIntersects(ref rayLeft, orientedBoundingBox, out distanceL);
            bool interRight = MyMath.RayIntersects(ref rayRight, orientedBoundingBox, out distanceR);
            //bool interCenter = MyMath.RayIntersects(ref rayCenter, orientedBoundingBox, out disntaceC);

            if (interLeft && interRight)
            {
                if (distanceL <= minFrontDistance || distanceR <= minFrontDistance)
                {

                    if (distanceL < distanceR)
                        TurnCar(alpha);
                    else if (distanceR < distanceL)
                        TurnCar(-alpha);
                }
            }
            else if (interLeft && distanceL <= minFrontDistance)
            {
                TurnCar(alpha); // Поворачиваю направо
            }
            else if (interRight && distanceR <= minFrontDistance)
            {
                TurnCar(-alpha); // Поворачиваю налево
            }
        }

        /// <summary> Минимальное расстояние до препятствия до принятия решения </summary>
        private float minSideDistance = 2;
        private void CheckObstacleSideRays(OrientedBoundingBox orientedBoundingBox, float alpha)
        {
            float distanceL, distanceR;

            bool interLeft = MyMath.RayIntersects(ref raySideLeft, orientedBoundingBox, out distanceL);
            bool interRight = MyMath.RayIntersects(ref raySideRight, orientedBoundingBox, out distanceR);

            if (!interLeft && !interRight)
            {
                return;
            }
            else if (interLeft && distanceL <= minSideDistance)
            {
                TurnCar(alpha); // Поворачиваю направо
            }
            else if (interRight && distanceR <= minSideDistance)
            {
                TurnCar(-alpha); // Поворачиваю налево
            }
        }
    }
}
