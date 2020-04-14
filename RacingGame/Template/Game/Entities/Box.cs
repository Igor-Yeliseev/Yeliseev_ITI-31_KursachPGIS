using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using Template.Graphics;

namespace Template
{
    class Box : PhysicalObject
    {
        private MeshObject _mesh;

        public void SetMaterial(Material material)
        {
            _mesh.Material = material;
        } 

        private Vector3 _direction;
        /// <summary> Напрвление движения </summary>
        public Vector3 Direction { get => _direction; set => _direction = value; }

        public short moveSign;

        public new Vector3 Position
        {
            get
            {
                return _mesh.Position;
            }
            set
            {
                Vector3 dv = (value - _mesh.Position);
                _mesh.Position = value;
                boundingBox.Translate(dv);
            }
        }


        public Box(MeshObject mesh) : base(mesh)
        {
            _mesh = mesh;

            _direction = new Vector3(0.0f, 0.0f, 1.0f);
            moveSign = 1;
        }

        public void MoveTo(float x, float y, float z)
        {
            var pos = _mesh.Position;
            pos = new Vector3(x, y, z);
            Position += pos;
        }

        public new void MoveBy(float dX, float dY, float dZ)
        {
            _mesh.Position += new Vector3(dX, dY, dZ);

            boundingBox.Translate(new Vector3(dX, dY, dZ));
        }

        public void MoveForward()
        {
            Position += _direction / 20;
            moveSign = 1;
        }

        public void MoveBackward()
        {
            var pos = Position;

            Position -= _direction / 20;
            moveSign = -1;
        }

        public void Move(Vector3 direction)
        {
            Position += direction / 20;
        }

        public void RotateY(float angle)
        {
            _mesh.YawBy(angle);
            _direction = Vector3.Transform(_direction, Matrix3x3.RotationY(angle));
            Vector3 dv = new Vector3(-Position.X, -Position.Y, -Position.Z);
            boundingBox.Translate(dv);
            boundingBox.Transform(Matrix.RotationY(angle));
            boundingBox.Translate(-dv);
        }


        public override bool CollisionTest(PhysicalObject obj)
        {
            return (boundingBox.Contains(ref obj.boundingBox) == ContainmentType.Intersects) ? true : false;
        }
    }
}
