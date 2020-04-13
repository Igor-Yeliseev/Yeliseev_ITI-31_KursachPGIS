using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Graphics;

namespace Template
{
    class Car : PhysicalObject
    {
        /// <summary> Переднее левое колесо </summary>
        protected MeshObject wheel1;
        /// <summary> Переднее правое колесо </summary>
        protected MeshObject wheel2;

        /// <summary> Вектор направления движения </summary>
        protected Vector4 _direction;
        /// <summary> Вектор направления движения </summary>
        public Vector4 Direction { get => _direction; set => _direction = value; }

        /// <summary> Передний мост (точка центра) </summary>
        protected Vector4 _frontAxle;
        /// <summary> Передний мост (точка центра) </summary>
        public Vector4 FrontAxle { get => _frontAxle; set => _frontAxle = value; }

        /// <summary> Задний мост (точка центра) </summary>
        protected Vector4 _rearAxle;
        /// <summary> Задний мост (точка центра) </summary>
        public Vector4 RearAxle { get => _rearAxle; set => _rearAxle = value; }

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
                    scale = 1.0f;
                else
                    scale = Math.Abs(scale * (value / (scale * 60)));

                Acceleration = value - _speed;
                _speed = value;
            }
        }

        /// <summary>
        /// Величина ускорения
        /// </summary>
        public float Acceleration { get; set; }

        /// <summary>
        /// Максимальная скорость автомобиля
        /// </summary>
        public virtual float MaxSpeed { get; set; } = 20.0f;
        /// <summary> Максимальная скорость задним ходом </summary>
        public float MaxBackSpeed { get; } = -6.0f;

        /// <summary> Масштаб изменения скорости </summary>
        private float scale = 0.1f;

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
        private bool _isWheelsTirned;
        /// <summary> Повернуты ли передние колеса</summary>
        public bool IsWheelsTurned
        {
            get
            {
                _isWheelsTirned = (turnCount != 0) ? true : false;
                return _isWheelsTirned;
            }
            set
            {
                _isWheelsTirned = value;
            }
        }

        protected int turnCount = 0;
        protected int itrs = 54;
        

        /// <summary>
        /// Конструктор машины
        /// </summary>
        /// <param name="meshes"></param>
        public Car(List<MeshObject> meshes) : base(meshes) 
        {
            _direction = new Vector4(0.0f, 0.0f, 1.0f, 0.0f);
            _speed = 0;

            wheel1 = meshes.Find(m => m.Name.Contains("wheel1"));
            wheel2 = meshes.Find(m => m.Name.Contains("wheel2"));
            float x = (wheel1.Position.X + wheel2.Position.X) / 2;
            _frontAxle = new Vector4(x, wheel1.Position.Y, wheel1.Position.Z, 0.0f);
            //var vr = meshes[5].Position;
            //_rearAxle = new Vector4(vr.X, wheel1.Position.Y, vr.Z, 0.0f);
            _rearAxle = new Vector4(0.0f, wheel1.Position.Y, 0.0f, 0.0f);

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

            var body = meshes.Find(m => m.Name.Contains("Body"));
            boundingBox = SetOBB(body);
            _position = body.Position;
            boundingBox.Translate((Vector3)_position);
        }
        
        public MeshObject this[string name]
        {
            get
            {
                var mesh = _meshes.Where(m => m.Name == name);
                return mesh.FirstOrDefault();
            }
        }

        public void AddToMeshes(MeshObjects meshObjects)
        {
            _meshes.ForEach(m => meshObjects.Add(m));
        }

        public void TurnLeft(float alpha)
        {
            //_direction = Vector4.Transform(_direction, Matrix.RotationY(-alpha)); // Поворот вектора направления
            //rotFrontAxel(-alpha); // Поворот центра переднего моста

            //_meshes.ForEach(m =>
            //{
            //    if (m.Name.Contains("wheel"))
            //    {
            //        //var oldPos = m.Position;
            //        var oldCenterPos = m.Center2Position;
            //        m.MoveTo(0, 0, 0);
            //        m.YawBy(-alpha); // Поворот вокруг центра

            //        var c2_pos = m.Position;
            //        m.Position = -m.Center2Position;
            //        m.Center2Position = c2_pos;

            //        m.Position = Vector4.Transform(m.Position, Matrix.RotationY(-alpha)); // Поворот вокруг новой оси

            //        var vect = m.Position - m.Center2Position;
            //        m.MoveTo(oldCenterPos);
            //        m.Position += vect;
            //        m.Center2Position = oldCenterPos;

            //    }
            //    else m.YawBy(-alpha);
            //});
        }

        public void TurnRight(float alpha)
        {
            //_direction = Vector4.Transform(_direction, Matrix.RotationY(alpha));
            //rotFrontAxel(alpha);

            //_meshes.ForEach(m =>
            //{
            //    if (m.Name.Contains("wheel"))
            //    {
            //        var oldCenterPos = m.Center2Position;
            //        m.MoveTo(0, 0, 0);
            //        m.YawBy(alpha);

            //        var c2_pos = m.Position;
            //        m.Position = -m.Center2Position;
            //        m.Center2Position = c2_pos;

            //        m.Position = Vector4.Transform(m.Position, Matrix.RotationY(alpha));

            //        var vect = m.Position - m.Center2Position;
            //        m.MoveTo(oldCenterPos);
            //        m.Position += vect;
            //        m.Center2Position = oldCenterPos;
            //    }
            //    else m.YawBy(alpha);
            //});
        }
        
        public virtual void TurnWheelsLeft(float alpha)
        {
            if (turnCount >= -itrs)
            {
                _direction = Vector4.Transform(_direction, Matrix.RotationY(-alpha));
                wheel1.YawBy(-alpha);
                wheel2.YawBy(-alpha);
                turnCount -= 2;
            }
        }

        public virtual void TurnWheelsRight(float alpha)
        {
            if (turnCount <= itrs)
            {
                _direction = Vector4.Transform(_direction, Matrix.RotationY(alpha));
                wheel1.YawBy(alpha);
                wheel2.YawBy(alpha);
                turnCount += 2;
            }
        }

        public void MoveForward()
        {
            //_meshes.ForEach(m =>
            //{
            //    if (m.Name.Contains("wheel"))
            //    {
            //        m.PitchBy(GetRotateAngle((_direction / 100).Length())); // Вращение колес
            //    }
            //    m.MoveBy(_direction / 100); // Перемещение вперед
            //});

            //_frontAxle += _direction / 100;
            //_rearAxle += _direction / 100;
        }

        public void MoveBackward()
        {
            //_meshes.ForEach(m =>
            //{
            //    if (m.Name.Contains("wheel"))
            //    {
            //        m.PitchBy(-GetRotateAngle((_direction / 100).Length())); // Вращение колес
            //    }
            //    m.MoveBy(-_direction / 100); // Перемещение назад
            //});

            //_frontAxle -= _direction / 100;
            //_rearAxle -= _direction / 100;
        }

        public override void MoveTo(float x, float y, float z)
        {
            float X = x - _position.X;
            float Y = y - _position.Y;
            float Z = z - _position.Z;

            base.MoveTo(x, y, z);

            var dir = new Vector4(X, Y, Z, 0.0f);

            _frontAxle += dir;
            _rearAxle += dir;
        }

        public override void MoveBy(float dX, float dY, float dZ)
        {
            base.MoveBy(dX, dY, dZ);

            _frontAxle += new Vector4(dX, dY, dZ, 0.0f);
            _rearAxle += new Vector4(dX, dY, dZ, 0.0f);
        }

        private float GetRotateAngle(float offset)
        {
            return offset / _wheelRadius;
        }

        private void rotFrontAxel(float alpha)
        {
            _frontAxle -= _rearAxle;
            _frontAxle = Vector4.Transform(_frontAxle, Matrix.RotationY(alpha));
            _frontAxle += _rearAxle;
        }

        /// <summary>
        /// Вернуть колеса в начальное положение
        /// </summary>
        public void BackWheels()
        {
            var v1 = _frontAxle - _rearAxle; v1.Normalize();
            float angle = MyVector.GetAngle(v1, _direction);

            if(MyVector.CosProduct(v1, _direction) < 0)
            {
                wheel1.YawBy(-angle); wheel2.YawBy(-angle);
                _direction = Vector4.Transform(_direction, Matrix.RotationY(-angle));
            }
            else
            {
                wheel1.YawBy(angle); wheel2.YawBy(angle);
                _direction = Vector4.Transform(_direction, Matrix.RotationY(angle));
            }
            turnCount = 0;
        }

        /// <summary>
        /// Двигать автомобиль в соответствии с физикой
        /// </summary>
        public void MoveProperly()
        {
            //int sign = Sign(_speed) ;
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
            float angle = MyVector.GetAngle(v1, v2) * scale; // Угол поворота машины

            float projDir = MyVector.DotProduct(v1, _direction) / v1.Length(); // Смещение вдоль направления
            v1.X *= projDir; v1.Y *= projDir; v1.Z *= projDir; // Получаем проекцию вектора направления

            if (sign > 0) // Двигаюсь вперед
            {
                if (MyVector.CosProduct(v1, v2) < 0)
                    TurnCar(angle);
                else
                    TurnCar(-angle);

                Move(v1, sign);
            }
            else if (sign < 0) // Двигаюсь назад
            {
                if (MyVector.CosProduct(v1, v2) < 0)
                    TurnCar(-angle);
                else
                    TurnCar(angle);

                v1 = _frontAxle - _rearAxle; v1.Normalize();
                projDir = MyVector.DotProduct(v1, _direction) / v1.Length();
                v1.X *= projDir; v1.Y *= projDir; v1.Z *= projDir;

                Move(v1, sign);
            }
        }

        /// <summary>
        /// Поворот машины
        /// </summary>
        /// <param name="alpha"> Угол поворота</param>
        public void TurnCar(float alpha)
        {
            if (alpha == 0)
                return;

            Vector4 oldCenterPos, c2_pos;

            _direction = Vector4.Transform(_direction, Matrix.RotationY(alpha));
            rotFrontAxel(alpha);
            RotateOBB(alpha); // Поворот баудин бокса

            _meshes.ForEach(m =>
            {
                if (m.Name.Contains("wheel"))
                {
                    oldCenterPos = m.Center2Position;
                    m.MoveTo(0, 0, 0);
                    m.YawBy(alpha);

                    c2_pos = m.Position;
                    m.Position = -m.Center2Position;
                    m.Center2Position = c2_pos;

                    m.Position = Vector4.Transform(m.Position, Matrix.RotationY(alpha));

                    //var vect = m.Position - m.Center2Position;
                    m.MoveTo(oldCenterPos);
                    m.Position += (m.Position - m.Center2Position);
                    m.Center2Position = oldCenterPos;
                }
                else m.YawBy(alpha);
            });
        }
        
        /// <summary>
        /// Смещение вдоль направления
        /// </summary>
        /// <param name="direction"> Вектор направления</param>
        /// <param name="sign"> Знак (вперед, назад)</param>
        public virtual void Move(Vector4 direction, int sign)
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

            _frontAxle += direction;
            _rearAxle += direction;
            _position += direction;
            boundingBox.Translate((Vector3)direction);
        }

        /// <summary> Ускорение автомобиля </summary>
        public void Accelerate()
        {
            Speed = MyMath.Lerp(_speed, MaxSpeed, 0.4f, 0.1f);
        }

        /// <summary> Торможение автомобиля </summary>
        public void Brake()
        {
            if (_speed > MaxSpeed / 2)
            {
                Speed = MyMath.Lerp(_speed, 0, 0.7f, 0.1f);
            }
            else if (_speed > MaxSpeed / 3)
            {
                Speed = MyMath.Lerp(_speed, 0, 1.0f, 0.1f);
            }
            else if (_speed > MaxSpeed / 4)
            {
                Speed = MyMath.Lerp(_speed, 0, 1.7f, 0.1f);
            }
            else
                Speed = MyMath.Lerp(_speed, MaxBackSpeed, 2.0f, 0.1f);
        }

        /// <summary> Движение по инерции с затуханием скорости </summary>
        public void MoveInertia()
        {
            Speed = MyMath.Lerp(_speed, 0, 0.3f, 0.2f);
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
                var v1 = _frontAxle - _rearAxle; v1.Normalize();

                if (MyVector.CosProduct(v1, _direction) < 0)
                {
                    wheel1.YawBy(-alpha); wheel2.YawBy(-alpha);
                    _direction = Vector4.Transform(_direction, Matrix.RotationY(-alpha));
                    turnCount -= 2;
                }
                else
                {
                    wheel1.YawBy(alpha); wheel2.YawBy(alpha);
                    _direction = Vector4.Transform(_direction, Matrix.RotationY(alpha));
                    turnCount += 2;
                }

                return (turnCount == 0) ? false : true;
            }
            else
                return false;
        }

        /// <summary>
        /// Определение и разрешение коллизий
        /// </summary>
        /// <param name="obj"></param>
        public override bool CollisionTest(PhysicalObject obj)
        {
            collied = boundingBox.Contains(ref obj.boundingBox);

            if (collied == ContainmentType.Intersects)
            {
                if (_speed > 0)
                {
                    while (boundingBox.Contains(ref obj.boundingBox) == ContainmentType.Intersects)
                        MoveProperly(-1);
                }
                else
                {
                    while (boundingBox.Contains(ref obj.boundingBox) == ContainmentType.Intersects)
                        MoveProperly(1);
                }
                return true;
            }
            else return false;
        }

        public bool CollisionCheckPoint(CheckPoint chpt)
        {
            colliedCheckPts = boundingBox.Contains(ref chpt.boundingBox);
            return (colliedCheckPts == ContainmentType.Intersects)? true : false;
        }

        
    }
}
