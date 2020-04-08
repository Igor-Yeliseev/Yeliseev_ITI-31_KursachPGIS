using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Graphics;

namespace Template
{
    class Car
    {
        /// <summary> Меши объекта </summary>
        private List<MeshObject> _meshes;

        /// <summary> Переднее левое колесо </summary>
        private MeshObject wheel1;
        /// <summary> Переднее правое колесо </summary>
        private MeshObject wheel2;

        /// <summary> Вектор направления движения </summary>
        private Vector4 _direction;
        /// <summary> Вектор направления движения </summary>
        public Vector4 Direction { get => _direction; set => _direction = value; }

        /// <summary> Position of the car in virtual world. </summary>
        private Vector4 _position;
        /// <summary> Position of the car in virtual world. </summary>
        public Vector4 Position { get => _position; set => _position = value; }

        /// <summary> Передний мост (точка центра) </summary>
        private Vector4 _frontAxle;
        /// <summary> Передний мост (точка центра) </summary>
        public Vector4 FrontAxle { get => _frontAxle; set => _frontAxle = value; }

        /// <summary> Задний мост (точка центра) </summary>
        private Vector4 _rearAxle;
        /// <summary> Задний мост (точка центра) </summary>
        public Vector4 RearAxle { get => _rearAxle; set => _rearAxle = value; }
        
        private float scale = 10.0f;

        /// <summary> Радиус колеса </summary>
        private float _wheelRadius;

        /// <summary> Повернуты ли передние колеса</summary>
        private bool _isWheelsTirned;
        /// <summary> Повернуты ли передние колеса</summary>
        public bool IsWheelsTirned
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

        public int turnCount = 0;
        int itrs = 54;

        /// <summary> The oriented bounding box </summary>
        public OrientedBoundingBox boundingBox;

        /// <summary>
        /// Конструктор машины
        /// </summary>
        /// <param name="meshes"></param>
        public Car(List<MeshObject> meshes)
        {
            _meshes = meshes;

            wheel1 = meshes.Find(m => m.Name.Contains("wheel"));
            wheel2 = meshes.Find(m => m.Name.Contains("wheel2"));
            float x = (wheel1.Position.X + wheel2.Position.X) / 2;
            _frontAxle = new Vector4(x, wheel1.Position.Y, wheel1.Position.Z, 0.0f);
            var vr = meshes[5].Position;
            _rearAxle = new Vector4(vr.X, wheel1.Position.Y, vr.Z, 0.0f); // Потом изменить на центр между задними колесами (переместить в 3ds Max)

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

            var body = meshes.Find(m => m.Name.Contains("DELOREAN"));
            boundingBox = SetOBB(body); // Меши остаются в центре
            _position = body.Position;
            boundingBox.Translate((Vector3)_position);
        }

        /// <summary>
        /// Задание бокса коллизий
        /// </summary>
        /// <param name="mesh"> Меш</param>
        /// <returns></returns>
        private OrientedBoundingBox SetOBB(MeshObject mesh)
        {
            float minX, maxX, minY, maxY, minZ, maxZ;
            minX = maxX = minY = maxY = minZ = maxZ = 0;

            mesh.Vertices.ForEach(v =>
            {
                if (v.position.X < minX) minX = v.position.X;
                if (v.position.X > maxX) maxX = v.position.X;

                if (v.position.Y < minY) minY = v.position.Y;
                if (v.position.Y > maxY) maxY = v.position.Y;

                if (v.position.Z < minZ) minZ = v.position.Z;
                if (v.position.Z > maxZ) maxZ = v.position.Z;
            });

            return new OrientedBoundingBox(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
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
        
        public void TurnWheelsLeft(float alpha)
        {
            if (turnCount >= -itrs)
            {
                _direction = Vector4.Transform(_direction, Matrix.RotationY(-alpha));
                wheel1.YawBy(-alpha);
                wheel2.YawBy(-alpha);
                turnCount -= 2;
            }

        }

        public void TurnWheelsRight(float alpha)
        {
            if (turnCount <= itrs)
            {
                _direction = Vector4.Transform(_direction, Matrix.RotationY(alpha));
                wheel1.YawBy(alpha);
                wheel2.YawBy(alpha);
                turnCount += 2;
            }
        }

        public void MoveBy(float dX, float dY, float dZ)
        {
            _meshes.ForEach(m => m.MoveBy(dX, dY, dZ));

            boundingBox.Translate(new Vector3(dX, dY, dZ));
        }

        public void MoveBy(Vector4 v)
        {
            MoveBy(v.X, v.Y, v.Z);
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
        /// Двигать автомобиль, sign - направление движения (1 - вперед, -1 - назад)
        /// </summary>
        /// <param name="sign"> Направление движения (1 - вперед, -1 - назад)</param>
        public void MoveProperly(short sign)
        {
            var point = _frontAxle + _direction;
            var v1 = _frontAxle - _rearAxle; v1.Normalize();
            var v2 = point - _rearAxle;
            float angle = MyVector.GetAngle(v1, v2) / scale; // Угол поворота машины

            float projDir = MyVector.DotProduct(v1, _direction) / v1.Length(); // Смещение вдоль направления

            v1.X *= projDir; v1.Y *= projDir; v1.Z *= projDir;
            
            moveCar(v1, sign);

            if (sign > 0) // Двигаюсь вперед
            {
                if (MyVector.CosProduct(v1, v2) < 0)
                    turnCar(angle);
                else
                    turnCar(-angle);
            }
            else // Двигаюсь назад
            {
                if (MyVector.CosProduct(v1, v2) < 0)
                    turnCar(-angle);
                else
                    turnCar(angle);
            }
            
        }

        /// <summary>
        /// Поворот машины
        /// </summary>
        /// <param name="alpha"> Угол поворота</param>
        private void turnCar(float alpha)
        {
            if (alpha == 0)
                return;

            _direction = Vector4.Transform(_direction, Matrix.RotationY(alpha));
            rotFrontAxel(alpha);
            RotateOBB(alpha); // Поворот баудин бокса

            _meshes.ForEach(m =>
            {
                if (m.Name.Contains("wheel"))
                {
                    var oldCenterPos = m.Center2Position;
                    m.MoveTo(0, 0, 0);
                    m.YawBy(alpha);

                    var c2_pos = m.Position;
                    m.Position = -m.Center2Position;
                    m.Center2Position = c2_pos;

                    m.Position = Vector4.Transform(m.Position, Matrix.RotationY(alpha));

                    var vect = m.Position - m.Center2Position;
                    m.MoveTo(oldCenterPos);
                    m.Position += vect;
                    m.Center2Position = oldCenterPos;
                }
                else m.YawBy(alpha);
            });
        }

        /// <summary>
        /// Поворот бокса коллизий
        /// </summary>
        /// <param name="angle"> Угол поворота</param>
        private void RotateOBB(float angle)
        {
            Matrix matrix = Matrix.RotationY(angle);
            Vector3 dv = new Vector3(-_position.X, -_position.Y, -_position.Z);
            boundingBox.Translate(dv);
            boundingBox.Transform(matrix);
            boundingBox.Translate(-dv);
        }

        /// <summary>
        /// Смещение вдоль направления
        /// </summary>
        /// <param name="direction"> Вектор направления</param>
        /// <param name="sign"> Знак (вперед, назад)</param>
        private void moveCar(Vector4 direction, short sign)
        {
            direction /= scale * sign;

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
    }
}
