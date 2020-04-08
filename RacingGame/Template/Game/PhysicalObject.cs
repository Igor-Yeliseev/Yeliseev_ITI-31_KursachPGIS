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

            boundingBox = new OrientedBoundingBox(new Vector3(minX, minY, minZ), new Vector3(maxX, maxY, maxZ));
        }

        public PhysicalObject(List<MeshObject> meshes, Vector4 position) : base(meshes, position)
        {
            
        }

        public PhysicalObject(List<MeshObject> meshes) : base(meshes)
        {

        }

        public override void MoveBy(float dX, float dY, float dZ)
        {
            base.MoveBy(dX, dY, dZ);

            boundingBox.Translate(new Vector3(dX, dY, dZ));
        }

        public override void MoveBy(Vector4 v)
        {
            MoveBy(v.X, v.Y, v.Z);
        }

        public abstract void CollisionTest(PhysicalObject obj);
    }
}
