using SharpDX;
using System.Collections.Generic;

namespace Template
{
    abstract class PhysicalObject : GameObject
    {
        /// <summary> The oriented bounding box </summary>
        public OrientedBoundingBox OBBox;
        /// <summary> The axis aligned bounding box </summary>
        public BoundingBox AABBox;
        protected void TranslateAABB(Vector3 vector)
        {
            AABBox.Minimum += vector;
            AABBox.Maximum += vector;
        }

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
            OBBox = SetOBB(mesh); 
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

            AABBox = new BoundingBox(min, max);

            return new OrientedBoundingBox(min, max);
        }
        
        public PhysicalObject(List<MeshObject> meshes, Vector3 position) : base(meshes, position)
        {

        }

        public PhysicalObject(List<MeshObject> meshes) : base(meshes)
        {

        }

        /// <summary>
        /// Поворот бокса коллизий
        /// </summary>
        /// <param name="angle"> Угол поворота</param>
        protected virtual void RotateOBB(float angle)
        {
            if (angle == 0) return;
            
            Vector3 dv = new Vector3(-_position.X, -_position.Y, -_position.Z);
            OBBox.Translate(dv);
            OBBox.Transform(Matrix.RotationY(angle));
            OBBox.Translate(-dv);
            BoundingBox.FromPoints(OBBox.GetCorners(), out AABBox);
        }

        public override void MoveBy(Vector3 vector)
        {
            base.MoveBy(vector);
            OBBox.Translate(vector);
            TranslateAABB(vector);
        }

        public override void MoveTo(float x, float y, float z)
        {
            float X = x - _position.X;
            float Y = y - _position.Y;
            float Z = z - _position.Z;

            base.MoveTo(x, y, z);

            OBBox.Translate(new Vector3(X, Y, Z));
            TranslateAABB(new Vector3(X, Y, Z));
        }

        public override void MoveTo(Vector3 v)
        {
            MoveTo(v.X, v.Y, v.Z);
        }

        public abstract bool CollisionTest(PhysicalObject obj);

        public abstract void CollisionResponce(PhysicalObject obj);
    }
}
