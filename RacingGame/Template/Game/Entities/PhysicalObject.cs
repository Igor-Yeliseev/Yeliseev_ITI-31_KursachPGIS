using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    abstract class PhysicalObject : GameObject
    {
        /// <summary> The oriented bounding box </summary>
        public OrientedBoundingBox boundingBox;

        protected ContainmentType collied;
        /// <summary> Столкнулся ли объект с другим объектом </summary>
        public bool IsCollied
        {
            get
            {
                return (collied == ContainmentType.Intersects) ? true : false;
            }
        }

        public PhysicalObject(MeshObject mesh) : base(mesh)
        {
            boundingBox = SetOBB(mesh); 
        }

        /// <summary>
        /// Задание бокса коллизий
        /// </summary>
        /// <param name="mesh"> Меш</param>
        /// <returns></returns>
        protected virtual  OrientedBoundingBox SetOBB(MeshObject mesh)
        {
            float minX, minY, minZ, maxX, maxY, maxZ;
            var vertex = mesh.Vertices[0].position;
            minX = vertex.X;
            maxX = vertex.X;
            minY = vertex.Y;
            maxY = vertex.Y;
            minZ = vertex.Z;
            maxZ = vertex.Z;

            mesh.Vertices.ForEach(v =>
            {
                if (v.position.X < minX) minX = v.position.X;
                if (v.position.X > maxX) maxX = v.position.X;

                if (v.position.Y < minY) minY = v.position.Y;
                if (v.position.Y > maxY) maxY = v.position.Y;

                if (v.position.Z < minZ) minZ = v.position.Z;
                if (v.position.Z > maxZ) maxZ = v.position.Z;
            });

            var min = new Vector3(minX, minY, minZ);
            var max = new Vector3(maxX, maxY, maxZ);
            
            return new OrientedBoundingBox(min, max);
        }
        
        public PhysicalObject(List<MeshObject> meshes, Vector3 position) : base(meshes, position)
        {
            boundingBox = new OrientedBoundingBox();
        }

        public PhysicalObject(List<MeshObject> meshes) : base(meshes)
        {
            boundingBox = new OrientedBoundingBox();
        }

        /// <summary>
        /// Поворот бокса коллизий
        /// </summary>
        /// <param name="angle"> Угол поворота</param>
        protected virtual void RotateOBB(float angle)
        {
            if (angle == 0) return;
            
            Vector3 dv = new Vector3(-_position.X, -_position.Y, -_position.Z);
            boundingBox.Translate(dv);
            boundingBox.Transform(Matrix.RotationY(angle));
            boundingBox.Translate(-dv);
        }

        public override void MoveBy(float dX, float dY, float dZ)
        {
            base.MoveBy(dX, dY, dZ);

            boundingBox.Translate(new Vector3(dX, dY, dZ));
        }

        public override void MoveBy(Vector3 v)
        {
            MoveBy(v.X, v.Y, v.Z);
        }

        public override void MoveTo(float x, float y, float z)
        {
            float X = x - _position.X;
            float Y = y - _position.Y;
            float Z = z - _position.Z;

            base.MoveTo(x, y, z);

            boundingBox.Translate(new Vector3(X, Y, Z));
        }

        public override void MoveTo(Vector3 v)
        {
            MoveTo(v.X, v.Y, v.Z);
        }

        public abstract bool CollisionTest(PhysicalObject obj);
    }
}
