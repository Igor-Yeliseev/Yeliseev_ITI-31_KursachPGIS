﻿using SharpDX;
using System;
using System.Collections.Generic;

namespace Template
{

    class Car : PhysicalObject
    {
        /// <summary> Переднее левое колесо </summary>
        protected MeshObject wheel1;
        /// <summary> Переднее правое колесо </summary>
        protected MeshObject wheel2;
        /// <summary> Car's body mesh </summary>
        protected MeshObject Body;

        /// <summary> Вектор направления движения </summary>
        protected Vector3 _direction;
        /// <summary> Вектор направления движения </summary>
        public Vector3 Direction { get => _direction; set => _direction = value; }

        /// <summary> Передний мост (точка центра) </summary>
        protected Vector3 _frontAxle;
        
        /// <summary> Задний мост (точка центра) </summary>
        protected Vector3 _rearAxle;

        /// <summary> Вектор направления машины (от центра заднего до центре переднего моста) </summary>
        protected Vector3 _carDirection;
        /// <summary> Вектор направления машины (от центра заднего до центре переднего моста) </summary>
        public Vector3 CarDirection { get => _carDirection; private set => _carDirection = value; }

        /// <summary> Мертв ли враг </summary>
        public bool IsDead { get; set; }
        
        /// <summary> Здоровье </summary>
        protected float _health = 100;
        /// <summary> Здоровье </summary>
        public float Health
        {
            get
            {
                return _health;
            }
            set
            {
                if (value <= 0)
                {
                    _health = 0;
                    IsDead = true;
                }
                else if (value > 100)
                    _health = 100;
                else
                    _health = value;
            }
        }

        /// <summary> Текущий круг </summary>
        public int Lap { get; set; } = 0;

        /// <summary> Величина скорости автомобиля (ед. в секунду) </summary>
        protected float _speed;
        /// <summary>
        /// Величина скорости автомобиля (ед. в секунду)
        /// </summary>
        public float Speed
        {
            get
            {
                return _speed; // scale * 60;
            }
            set
            {
                if (value == 0)
                {
                    scale = 1.0f;
                    angle = 0.0f;
                }
                else
                    scale = Math.Abs(scale * (value / (scale * 60)));

                Acceleration = (value - _speed);

                _speed = value;
            }
        }

        /// <summary> Угол поворота </summary>
        private float angle = 0;
        /// <summary> Угол поворота </summary>
        public float Angle { get => angle; private set => angle = value; }

        /// <summary>
        /// На сколько процентов изменилась скорость
        /// </summary>
        public float Acceleration { get; set; }

        public static float MAX_SPEED { get { return 30.0f; } } 

        /// <summary>
        /// Максимальная скорость автомобиля
        /// </summary>
        public virtual float MaxSpeed { get; set; } = 30.0f;
        /// <summary> Максимальная скорость задним ходом </summary>
        public float MaxBackSpeed { get; } = -6.0f;

        /// <summary> Масштаб изменения скорости </summary>
        protected float scale = 0.1f;

        protected ContainmentType colliedCheckPts;
        /// <summary> Столкнулся ли объект с чекпоинтом </summary>
        public bool IsColliedCheckPts
        {
            get
            {
                return (colliedCheckPts == ContainmentType.Intersects) ? true : false;
            }
            set
            {
                colliedCheckPts = (value == true) ? ContainmentType.Intersects : ContainmentType.Disjoint;
            }
        }

        /// <summary> Радиус колеса </summary>
        private float _wheelRadius;

        /// <summary> Повернуты ли передние колеса</summary>
        private bool _isWheelsTurned;
        /// <summary> Повернуты ли передние колеса</summary>
        public bool IsWheelsTurned
        {
            get
            {
                _isWheelsTurned = (turnCount != 0) ? true : false;
                return _isWheelsTurned;
            }
            set
            {
                _isWheelsTurned = value;
            }
        }

        protected int turnCount = 0;
        protected int itrs = 48;

        /// <summary>
        /// Конструктор машины
        /// </summary>
        /// <param name="meshes"></param>
        public Car(List<MeshObject> meshes) : base(meshes)
        {
            _direction = new Vector3(0.0f, 0.0f, 1.0f);
            _speed = 0;

            wheel1 = meshes.Find(m => m.Name.Contains("wheel1"));
            wheel2 = meshes.Find(m => m.Name.Contains("wheel2"));
            float x = (wheel1.Position.X + wheel2.Position.X) / 2;
            _frontAxle = new Vector3(x, wheel1.Position.Y, wheel1.Position.Z);
            _rearAxle = new Vector3(0.0f, wheel1.Position.Y, 0.0f);
            _carDirection = _frontAxle - _rearAxle;

            float minZ = wheel1.Vertices[0].position.Z;
            float maxZ = wheel1.Vertices[0].position.Z;
            wheel1.Vertices.ForEach(v =>
            {
                if (v.position.Z < minZ)
                    minZ = v.position.Z;
                if (v.position.Z > maxZ)
                    maxZ = v.position.Z;
            });
            _wheelRadius = (maxZ - minZ) / 2;

            Body = meshes.Find(m => m.Name.Contains("Body"));
            OBBox = SetOBB(Body);

            _position = Body.Position;
            OBBox.Translate(_position);
            TranslateAABB(_position);
        }
        
        public virtual void TurnWheelsLeft(float alpha)
        {
            if (_speed > MaxSpeed * 0.5) // Доработать
            {
                alpha /= 2;
            }
            else if (_speed > MaxSpeed * 0.7)
            {
                alpha *= 0.3f;
            }

            if (turnCount >= -itrs)
            {
                _direction = Vector3.Transform(_direction, Matrix3x3.RotationY(-alpha));
                wheel1.YawBy(-alpha);
                wheel2.YawBy(-alpha);
                turnCount -= 2;
            }
        }

        public virtual void TurnWheelsRight(float alpha)
        {
            if (_speed > MaxSpeed * 0.5) // Доработать
            {
                alpha /= 2;
            }
            else if (_speed > MaxSpeed * 0.7)
            {
                alpha *= 0.3f;
            }

            if (turnCount <= itrs)
            {
                _direction = Vector3.Transform(_direction, Matrix3x3.RotationY(alpha));
                wheel1.YawBy(alpha);
                wheel2.YawBy(alpha);
                turnCount += 2;
            }
        }

        public override void MoveTo(float x, float y, float z)
        {
            float X = x - _position.X;
            float Y = y - _position.Y;
            float Z = z - _position.Z;

            base.MoveTo(x, y, z);

            var dir = new Vector3(X, Y, Z);
            _frontAxle += dir;
            _rearAxle += dir;
        }

        public override void MoveBy(Vector3 vector)
        {
            base.MoveBy(vector);
            _frontAxle += vector;
            _rearAxle += vector;
        }

        private float GetRotateAngle(float offset)
        {
            return offset / _wheelRadius;
        }

        private void rotFrontAxel(float alpha)
        {
            _carDirection = _frontAxle - _rearAxle;
            _carDirection = Vector3.Transform(_carDirection, Matrix3x3.RotationY(alpha));
            _frontAxle = _rearAxle + _carDirection;

            //_frontAxle -= _rearAxle;
            //_frontAxle = Vector3.Transform(_frontAxle, Matrix3x3.RotationY(alpha));
            //_frontAxle += _rearAxle;            
        }

        /// <summary>
        /// Вернуть колеса в начальное положение
        /// </summary>
        public void BackWheels()
        {
            var v1 = _frontAxle - _rearAxle; //_carDirection;
            v1.Normalize();
            float angle = MyVector.GetAngle(v1, _direction);

            if (MyVector.CosProduct(v1, _direction) < 0)
            {
                wheel1.YawBy(-angle); wheel2.YawBy(-angle);
                _direction = Vector3.Transform(_direction, Matrix3x3.RotationY(-angle));
            }
            else
            {
                wheel1.YawBy(angle); wheel2.YawBy(angle);
                _direction = Vector3.Transform(_direction, Matrix3x3.RotationY(angle));
            }
            turnCount = 0;
        }

        /// <summary>
        /// Двигать автомобиль в соответствии с физикой
        /// </summary>
        public void MoveProperly()
        {
            int sign = Math.Sign(_speed);
            if (_speed == 0) return;

            MoveProperly(sign);
        }

        protected int Sign(float x)
        {
            if (x > 0) return 1;
            else if (x < 0) return -1;
            else return 0;
        }

        /// <summary>
        /// Двигать автомобиль, sign - направление движения (1 - вперед, -1 - назад)
        /// </summary>
        /// <param name="sign"> Направление движения (1 - вперед, -1 - назад)</param>
        private void MoveProperly(int sign)
        {
            var point = _frontAxle + _direction;
            var v1 = _frontAxle - _rearAxle; v1.Normalize();
            var v2 = point - _rearAxle;
            angle = MyVector.GetAngle(v1, v2) * scale; // Угол поворота машины

            float projDir = MyVector.DotProduct(v1, _direction) / v1.Length(); // Смещение вдоль направления
            v1 = v1 * projDir; // Получаем проекцию вектора направления

            if (sign > 0) // Двигаюсь вперед
            {
                if (MyVector.CosProduct(v1, v2) < 0)
                    angle *= 1;
                else
                    angle *= -1;

                TurnCar(angle);
                Move(v1, sign);
            }
            else if (sign < 0) // Двигаюсь назад
            {
                if (MyVector.CosProduct(v1, v2) < 0)
                    angle *= -1;
                else
                    angle *= 1;

                TurnCar(angle);

                v1 = _frontAxle - _rearAxle; v1.Normalize();
                projDir = MyVector.DotProduct(v1, _direction) / v1.Length();
                v1 = v1 * projDir;

                Move(v1, sign);
            }
        }

        /// <summary>
        /// Поворот машины
        /// </summary>
        /// <param name="alpha"> Угол поворота</param>
        public virtual void TurnCar(float alpha)
        {
            if (alpha == 0)
                return;

            //Vector3 oldCenterPos, c2_pos;
            Matrix3x3 matrix = Matrix3x3.RotationY(alpha);

            _direction = Vector3.Transform(_direction, Matrix3x3.RotationY(alpha));
            rotFrontAxel(alpha);
            RotateOBB(alpha); // Поворот баудин боксов
            
            _meshes.ForEach(m =>
            {
                if (m.Name.Contains("wheel"))
                {
                    m.YawBy(alpha);
                    m.Position = _rearAxle + Vector3.Transform(m.Position - _rearAxle, matrix);

                }
                else m.YawBy(alpha);
            });
        }
        
        /// <summary>
        /// Смещение вдоль направления
        /// </summary>
        /// <param name="direction"> Вектор направления</param>
        /// <param name="sign"> Знак (вперед, назад)</param>
        public virtual void Move(Vector3 direction, int sign)
        {
            if (sign == 0)
                return;

            direction *= scale * sign;

            _meshes.ForEach(m =>
            {
                if (m.Name.Contains("wheel"))
                {
                    m.PitchBy(GetRotateAngle(sign * direction.Length())); // Вращение колес
                }
                m.MoveBy(direction); // Перемещение вперед
            });

            _position += direction;
            _frontAxle += direction;
            _rearAxle += direction;
            OBBox.Translate(direction);
            TranslateAABB(direction);
        }

        /// <summary> Ускорение автомобиля </summary>
        public virtual void Accelerate()
        {
            Speed = MyMath.Lerp(_speed, MaxSpeed, 0.4f, 0.1f);
        }

        public virtual void Accelerate(float speed)
        {
            Speed = MyMath.Lerp(_speed, speed, 0.4f, 0.1f);
        }

        /// <summary> Торможение автомобиля </summary>
        public virtual void Brake()
        {
            if (_speed > MaxSpeed / 1.4f)
            {
                Speed = MyMath.Lerp(_speed, 0, 0.7f, 0.1f);
            }
            else if (_speed > MaxSpeed / 2.5)
            {
                Speed = MyMath.Lerp(_speed, 0, 1.0f, 0.1f);
            }
            else if (_speed > MaxSpeed / 4)
            {
                Speed = MyMath.Lerp(_speed, 0, 1.3f, 0.1f);
            }
            else
            {
                Speed = MyMath.Lerp(_speed, -3.0f, 1.7f, 0.1f);
                //Speed = MyMath.Lerp(_speed, MaxBackSpeed, 1.0f, 0.1f);
            }
        }

        /// <summary> Движение по инерции с затуханием скорости </summary>
        public virtual void MoveInertia()
        {
            Speed = MyMath.Lerp(_speed, 0, 0.4f, 0.2f);
        }

        /// <summary>
        /// Анимация возврата коле в начальное положение
        /// </summary>
        /// <param name="alpha"> Угол</param>
        /// <returns></returns>
        public bool AnimationWheels(float alpha)
        {
            if (turnCount != 0)
            {
                var v1 = _carDirection; v1.Normalize();

                if (MyVector.CosProduct(v1, _direction) < 0)
                {
                    wheel1.YawBy(-alpha); wheel2.YawBy(-alpha);
                    _direction = Vector3.Transform(_direction, Matrix3x3.RotationY(-alpha));
                    turnCount -= 2;
                }
                else
                {
                    wheel1.YawBy(alpha); wheel2.YawBy(alpha);
                    _direction = Vector3.Transform(_direction, Matrix3x3.RotationY(alpha));
                    turnCount += 2;
                }

                return (turnCount == 0) ? false : true;
            }
            else
                return false;
        }

        public bool CollisionCheckPoint(CheckPoint chpt)
        {
            colliedCheckPts = OBBox.Contains(ref chpt.OBBox);
            if (colliedCheckPts == ContainmentType.Intersects)
            {
                ColliedCheckPoint?.Invoke();
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary> Raise event when car collied </summary>
        public event Action<float> Collied;
        public event Action ColliedCheckPoint;

        /// <summary>
        /// Определение и разрешение коллизий
        /// </summary>
        /// <param name="obj"></param>
        public override bool CollisionTest(PhysicalObject obj)
        {
            if (AABBox.Intersects(obj.AABBox))
            {
                collied = OBBox.Contains(ref obj.OBBox);

                if (collied == ContainmentType.Intersects)
                {
                    CollisionResponce(obj);
                    return true;
                }
                else
                    return false;
            }
            else
                return false;
        }
        
        /// <summary>
        /// Разрешение столкновения машины и объекта
        /// </summary>
        /// <param name="obj"></param>
        public override void CollisionResponce(PhysicalObject obj) // Добавить проверки на тип объекта (Стена, Враг, Приз и т.д.)
        {
            if(_speed > 8)
            {
                Collied?.Invoke(20 / Health);
                Health -= 20;
            }

            if (_speed > 0)
            {
                while (OBBox.Contains(ref obj.OBBox) == ContainmentType.Intersects)
                    MoveProperly(-1);
            }
            else
            {
                while (OBBox.Contains(ref obj.OBBox) == ContainmentType.Intersects)
                    MoveProperly(1);
            }
            Speed = -_speed * 0.2f;
        }
    }
    
}
