using SharpDX;
using System;
using System.Collections.Generic;

namespace Template
{
    class EnemyCar : Car
    {

        private Vector3 _target;
        public int targetIndex = 0;
        /// <summary> Вектор от центра заднего моста к точки цели </summary>
        public Vector3 Target
        {
            get => _target;
            set
            {
                _target = new Vector3(value.X - _rearAxle.X, 0.0f, value.Z - _rearAxle.Z);
                //wheelsOnTarget = false;
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

        
        /// <summary> Передний левый луч </summary>
        public Ray rayFrontLeft;
        /// <summary> Передний правый луч </summary>
        public Ray rayFrontRight;
        /// <summary> Передний сентральный луч </summary>
        public Ray rayFrontCenter;
        /// <summary> Боковой передний левый луч </summary>
        public Ray rayFrontSideLeft;
        /// <summary> Боковой передний правый луч </summary>
        public Ray rayFrontSideRight;
        /// <summary> Боковой центральный левый луч </summary>
        public Ray rayCenterSideLeft;
        /// <summary> Боковой центральный правый луч </summary>
        public Ray rayCenterSideRight;

        /// <summary> Расстояние от точки вращения до поцизии левого сенсора </summary>
        Vector3 rayLftToRearAxle;
        /// <summary> Расстояние от точки вращения до поцизии правого сенсора </summary>
        Vector3 rayRhtToRearAxle;
        /// <summary> Расстояние от точки вращения до поцизии центрального сенсора </summary>
        Vector3 rayCenToRearAxle;

        /// <summary> Расстояние от точки вращения до поцизии бокового левого сенсора </summary>
        Vector3 raySideLftToRearAxle;
        /// <summary> Расстояние от точки вращения до поцизии бокового левого сенсора </summary>
        Vector3 raySideRhtToRearAxle;

        /// <summary> Расстояние от точки вращения до поцизии центральнго бокового левого сенсора </summary>
        Vector3 rayCenSideLftToRearAxle;
        /// <summary> Расстояние от точки вращения до поцизии центральнго бокового левого сенсора </summary>
        Vector3 rayCenSideRhtToRearAxle;

        public EnemyCar(List<MeshObject> meshes) : base(meshes)
        {
            IsDead = false;
            wheelsDirection = _direction;
            
            var Z = (OBBox.Center.Z + OBBox.Extents.Z) / 2; // Машины должны стоять параллельно осям

            float z = 2.3f;

            // Передние прямые лучи
            var rayFrontRPos = new Vector3(wheel1.Position.X, wheel1.Position.Y, wheel1.Position.Z + z); // ИЗМЕНИТЬ ПОТОМ Z !!!
            var rayFrontLPos = new Vector3(wheel2.Position.X, wheel2.Position.Y, wheel2.Position.Z + z);
            var rayFrontCPos = new Vector3(_frontAxle.X, _frontAxle.Y, _frontAxle.Z + 0);
            rayFrontRight = new Ray(rayFrontRPos, _direction);
            rayFrontLeft = new Ray(rayFrontLPos, _direction);
            rayFrontCenter = new Ray(rayFrontCPos, _direction);
            rayLftToRearAxle = rayFrontLeft.Position - _rearAxle;
            rayRhtToRearAxle = rayFrontRight.Position - _rearAxle;
            rayCenToRearAxle = rayFrontCenter.Position - _rearAxle;

            // Боковые передние лучи
            var rayFrontSideRPos = new Vector3(wheel1.Position.X, wheel1.Position.Y, wheel1.Position.Z + z); // ИЗМЕНИТЬ ПОТОМ Z !!!
            var rayFrontSideLPos = new Vector3(wheel2.Position.X, wheel2.Position.Y, wheel2.Position.Z + z);
            raySideLftToRearAxle = rayFrontSideLPos - _rearAxle;
            raySideRhtToRearAxle = rayFrontSideRPos - _rearAxle;
            rayFrontSideLeft = new Ray(rayFrontSideLPos, Vector3.Transform(_direction, Matrix3x3.RotationY(-(float)Math.PI / 4)));
            rayFrontSideRight = new Ray(rayFrontSideRPos, Vector3.Transform(_direction, Matrix3x3.RotationY((float)Math.PI / 4)));
            minFrontSideDistance = (float)Math.Round(minFrontDistance / (float)Math.Cos(Math.PI / 4.0), 2);

            // Боковые центральные лучи
            var rayCenterSideRPos = new Vector3(wheel1.Position.X + 0, wheel1.Position.Y, wheel1.Position.Z + z); // ИЗМЕНИТЬ ПОТОМ X !!!
            var rayCenterSideLPos = new Vector3(wheel2.Position.X - 0, wheel2.Position.Y, wheel2.Position.Z + z);
            rayCenSideLftToRearAxle = rayCenterSideLPos - _rearAxle;
            rayCenSideRhtToRearAxle = rayCenterSideRPos - _rearAxle;
            rayCenterSideLeft = new Ray(rayCenterSideLPos, Vector3.Transform(_direction, Matrix3x3.RotationY(-(float)Math.PI / 2)));
            rayCenterSideRight = new Ray(rayCenterSideRPos, Vector3.Transform(_direction, Matrix3x3.RotationY((float)Math.PI / 2)));
            
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
            var matrix = Matrix3x3.RotationY(alpha);

            // Передние лучи
            Vector3 dir = Vector3.Transform(rayFrontCenter.Direction, matrix);
            rayFrontLeft.Direction = dir;
            rayFrontRight.Direction = dir;
            rayFrontCenter.Direction = dir;

            rayLftToRearAxle = Vector3.Transform(rayLftToRearAxle, matrix);
            rayRhtToRearAxle = Vector3.Transform(rayRhtToRearAxle, matrix);
            rayCenToRearAxle = Vector3.Transform(rayCenToRearAxle, matrix);

            rayFrontLeft.Position = _rearAxle + rayLftToRearAxle;
            rayFrontRight.Position = _rearAxle + rayRhtToRearAxle;
            rayFrontCenter.Position = _rearAxle + rayCenToRearAxle;

            // Боковые передние лучи
            rayFrontSideLeft.Direction = Vector3.Transform(rayFrontSideLeft.Direction, matrix);
            rayFrontSideRight.Direction = Vector3.Transform(rayFrontSideRight.Direction, matrix);

            raySideLftToRearAxle = Vector3.Transform(raySideLftToRearAxle, matrix);
            raySideRhtToRearAxle = Vector3.Transform(raySideRhtToRearAxle, matrix);
            rayFrontSideLeft.Position = _rearAxle + raySideLftToRearAxle;
            rayFrontSideRight.Position = _rearAxle + raySideRhtToRearAxle;

            // Боковые центральные лучи
            rayCenterSideLeft.Direction = Vector3.Transform(rayCenterSideLeft.Direction, matrix);
            rayCenterSideRight.Direction = Vector3.Transform(rayCenterSideRight.Direction, matrix);

            rayCenSideLftToRearAxle = Vector3.Transform(rayCenSideLftToRearAxle, matrix);
            rayCenSideRhtToRearAxle = Vector3.Transform(rayCenSideRhtToRearAxle, matrix);
            rayCenterSideLeft.Position = _rearAxle + rayCenSideLftToRearAxle;
            rayCenterSideRight.Position = _rearAxle + rayCenSideRhtToRearAxle;

        }

        /// <summary>
        /// Знак косого произведения для колес
        /// </summary>
        private int signCosW = 0;
        private bool wheelsOnTarget = false;
        /// <summary> Повернуты ли колеса в точку Target </summary>
        public bool IsWheelsOnTarget { get => wheelsOnTarget; set => wheelsOnTarget = value; }

        public override void Accelerate()
        {
            base.Accelerate();
            minFrontDistance = MyMath.Lerp(minFrontDistance, 20, 0.4f, 0.1f);
        }

        public override void Accelerate(float speed)
        {
            base.Accelerate(speed);
            minFrontDistance = MyMath.Lerp(minFrontDistance, speed * 0.66f, 0.4f, 0.1f);
        }

        public override void Brake()
        {
            if (_speed > MaxSpeed / 1.4f)
            {
                Speed = MyMath.Lerp(_speed, 0, 0.7f, 0.1f);
                minFrontDistance = MyMath.Lerp(minFrontDistance, 6, 0.7f, 0.1f);
            }
            else if (_speed > MaxSpeed / 2.5f)
            {
                Speed = MyMath.Lerp(_speed, 0, 1.0f, 0.1f);
                minFrontDistance = MyMath.Lerp(minFrontDistance, 6, 1.0f, 0.1f);
            }
            else
            {
                Speed = MyMath.Lerp(_speed, 0, 1.4f, 0.1f);
                minFrontDistance = MyMath.Lerp(minFrontDistance, 6, 1.5f, 0.1f);
            }
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

        public override void MoveBy(Vector3 vector)
        {
            base.MoveBy(vector);
            MoveByRays(vector);
        }

        private void MoveByRays(Vector3 offset)
        {
            rayFrontLeft.Position += offset;
            rayFrontRight.Position += offset;
            rayFrontCenter.Position += offset;

            rayFrontSideLeft.Position += offset;
            rayFrontSideRight.Position += offset;

            rayCenterSideLeft.Position += offset;
            rayCenterSideRight.Position += offset;
        }

        /// <summary> Смещение лучей сенсоров вдоль направления </summary>
        /// <param name="direction"> Вектор направления</param>
        /// <param name="sign"> Знак (вперед, назад)</param>
        private void MoveRays(Vector3 direction, int sign)
        {
            if (sign == 0) return;

            direction *= scale * sign;

            rayFrontLeft.Position += direction;
            rayFrontRight.Position += direction;
            rayFrontCenter.Position += direction;

            rayFrontSideLeft.Position += direction;
            rayFrontSideRight.Position += direction;

            rayCenterSideLeft.Position += direction;
            rayCenterSideRight.Position += direction;
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
            if (_isFrontObstacle || _isSideObstacle)
                return;

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
            if (AABBox.Intersects(obj.AABBox))
            {
                collied = OBBox.Contains(ref obj.OBBox);

                if (collied == ContainmentType.Intersects)
                {
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        
        private float distance;
        /// <summary> Дистанция до объекта </summary>
        public float Distance { get => distance; }

        /// <summary>
        /// Проверить лучи на предмет опасного приближения
        /// </summary>
        /// <param name="orientedBoundingBox"></param>
        /// <param name="alpha"></param>
        public void CheckObstacle(ref OrientedBoundingBox orientedBoundingBox, float alpha)
        {
            // Передние лучи
            CheckObstacleFrontRays(ref orientedBoundingBox, alpha / 3);
            // Боковые лучи
            CheckObstacleSideRays(ref rayFrontSideLeft, ref rayFrontSideRight, ref orientedBoundingBox, minFrontSideDistance, alpha);
            CheckObstacleSideRays(ref rayCenterSideLeft, ref rayCenterSideRight, ref orientedBoundingBox, minCenterSideDistance, alpha);
        }

        private bool _isFrontObstacle = false;
        private bool _isSideObstacle = false;
        
        /// <summary> Минимальное расстояние от препятствия до передних лучей </summary>
        public float minFrontDistance = 6;
        private void CheckObstacleFrontRays(ref OrientedBoundingBox orientedBoundingBox, float alpha)
        {
            float distanceL, distanceR;

            bool interLeft = MyMath.RayIntersects(ref rayFrontLeft, ref orientedBoundingBox, out distanceL);
            bool interRight = MyMath.RayIntersects(ref rayFrontRight, ref orientedBoundingBox, out distanceR);
            //bool interCenter = MyMath.RayIntersects(ref rayFrontCenter, orientedBoundingBox, out disntaceC);

            if (interLeft && interRight)
            {
                if (distanceL <= minFrontDistance || distanceR <= minFrontDistance)
                {

                    if (distanceL < distanceR) {
                        TurnWheelsRight(alpha);

                    }
                    else if (distanceR < distanceL) {
                        TurnWheelsLeft(alpha);
                    }
                    Brake();

                    _isFrontObstacle = true;
                }
                else
                    _isFrontObstacle = false;
            }
            else if (interLeft && distanceL <= minFrontDistance)
            {
                TurnWheelsRight(alpha); //TurnCar(alpha); // Поворачиваю направо
                //Brake();
                _isFrontObstacle = true;
            }
            else if (interRight && distanceR <= minFrontDistance)
            {
                TurnWheelsLeft(alpha);
                //Brake();
                _isFrontObstacle = true;
            }
            else
            {
                _isFrontObstacle = false;
                //AnimationWheels(alpha);
            }
        }

        /// <summary> Минимальное расстояние от препятствия до передних боковых </summary>
        private float minFrontSideDistance;
        /// <summary> Минимальное расстояние от препятствия до центральных боковых </summary>
        private float minCenterSideDistance = 2;
        
        private void CheckObstacleSideRays(ref Ray raySideLeft, ref Ray raySideRight, ref OrientedBoundingBox orientedBoundingBox, float minDistance, float alpha)
        {
            float distanceL, distanceR;

            bool interLeft = MyMath.RayIntersects(ref raySideLeft, ref orientedBoundingBox, out distanceL);
            bool interRight = MyMath.RayIntersects(ref raySideRight, ref orientedBoundingBox, out distanceR);

            if (!interLeft && !interRight)
            {
                _isSideObstacle = false;
                return;
            }
            else if (interLeft && distanceL <= minDistance)
            {
                TurnWheelsRight(alpha);
                //Brake();
                _isSideObstacle = true;
            }
            else if (interRight && distanceR <= minDistance)
            {
                TurnWheelsLeft(alpha);
                //Brake();
                _isSideObstacle = true;
            }
            else
            {
                _isSideObstacle = false;
            }
        }
    }
}
