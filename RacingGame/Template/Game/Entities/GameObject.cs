using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    abstract class GameObject
    {
        protected List<MeshObject> _meshes;

        protected Vector3 _position;

        public Vector3 Position { get => _position;}

        public GameObject(MeshObject mesh)
        {
            _meshes = new List<MeshObject>();
            _meshes.Capacity = 1;
            _meshes.Add(mesh);
            _position = mesh._position;
        }

        public GameObject(List<MeshObject> meshes, Vector3 position)
        {
            _meshes = meshes;
            _position = position;
        }

        public GameObject(List<MeshObject> meshes)
        {
            _meshes = meshes;
            _position = new Vector3(0.0f, 0.0f, 0.0f);
        }

        public virtual void MoveBy(Vector3 vector)
        {
            _meshes.ForEach(m => m.MoveBy(vector));
            _position += vector;
        }

        public virtual void MoveTo(float x, float y, float z)
        {
            Vector3 vector = new Vector3(x - _position.X, y - _position.Y, z - _position.Z);

            _meshes.ForEach(m => m.MoveBy(vector));

            _position.X = x;
            _position.Y = y;
            _position.Z = z;   
        }

        public virtual void MoveTo(Vector3 v)
        {
            MoveTo(v.X, v.Y, v.Z);
        }

        public virtual void RotateY(float angle)
        {
            _meshes.ForEach(m => m.YawBy(angle));
        }

        public virtual void RotateX(float angle)
        {
            _meshes.ForEach(m => m.PitchBy(angle));
        }

        public virtual void RotateZ(float angle)
        {
            _meshes.ForEach(m => m.RollBy(angle));
        }
    }
}
