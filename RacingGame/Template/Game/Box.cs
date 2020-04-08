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

        private Vector4 _direction;
        /// <summary> Напрвление движения </summary>
        public Vector4 Direction { get => _direction; set => _direction = value; }

        public short moveSign;

        public new Vector4 Position
        {
            get
            {
                return _mesh.Position;
            }
            set
            {
                Vector3 dv = (Vector3)(value - _mesh.Position);
                _mesh.Position = value;
                boundingBox.Translate(dv);
            }
        }


        public Box(MeshObject mesh) : base(mesh)
        {
            _mesh = mesh;

            _direction = new Vector4(0.0f, 0.0f, 1.0f, 0.0f);
            moveSign = 1;
        }
        
        public void MoveTo(float x, float y, float z)
        {
            var pos = _mesh.Position;
            pos = new Vector4(x, y, z, pos.W);
            Position += pos;
        }

        public new void MoveBy(float dX, float dY, float dZ)
        {
            var pos = _mesh.Position;
            pos += new Vector4(dX, dY, dZ, pos.W);
            _mesh.Position = pos;
            
            boundingBox.Translate(new Vector3(dX, dY, dZ));
        }

        public void MoveForward()
        {
            Position += _direction / 20;
            moveSign = 1;
        }

        public void MoveBackward()
        {
            Position -= _direction / 20;
            moveSign = -1;
        }

        public void Move(Vector4 direction)
        {
            Position += direction / 20;
        }

        public void RotateY(float angle)
        {
            Matrix matrix = Matrix.RotationY(angle);
            _mesh.YawBy(angle);
            _direction = Vector4.Transform(_direction, matrix);
            Vector3 dv = new Vector3(-Position.X, -Position.Y, -Position.Z);
            boundingBox.Translate(dv);
            boundingBox.Transform(matrix);
            boundingBox.Translate(-dv);
        }

        public override void CollisionTest(PhysicalObject obj)
        {
            throw new NotImplementedException();
        }
    }
}
