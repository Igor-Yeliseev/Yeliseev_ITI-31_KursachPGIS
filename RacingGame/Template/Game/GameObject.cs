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

        protected Vector4 _position;

        public Vector4 Position { get => _position; set => _position = value; }

        public GameObject(MeshObject mesh)
        {
            _meshes.Add(mesh);
            _position = mesh._position;
        }

        public GameObject(List<MeshObject> meshes, Vector4 position)
        {
            _meshes = meshes;
            _position = position;
        }

        public GameObject(List<MeshObject> meshes)
        {
            _meshes = meshes;
            _position = new Vector4(0.0f, 0.0f, 0.0f, 0.0f);
        }

        public virtual void MoveBy(float dX, float dY, float dZ)
        {
            _meshes.ForEach(m => m.MoveBy(dX, dY, dZ));

            _position.X += dX;
            _position.Y += dY;
            _position.Z += dZ;
        }

        public virtual void MoveBy(Vector4 v)
        {
            MoveBy(v.X, v.Y, v.Z);
        }
    }
}
