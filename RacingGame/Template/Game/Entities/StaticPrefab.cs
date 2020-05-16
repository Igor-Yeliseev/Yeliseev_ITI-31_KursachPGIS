using System.Collections.Generic;

namespace Template
{
    class StaticPrefab : PhysicalObject
    {
        public StaticPrefab(MeshObject mesh) : base(mesh)
        {
        }

        public StaticPrefab(List<MeshObject> meshes) : base(meshes)
        {

        }

        public override void CollisionResponce(PhysicalObject obj)
        {

        }

        public override bool CollisionTest(PhysicalObject obj)
        {
            if (AABBox.Intersects(obj.AABBox))
            {
                collied = OBBox.Contains(ref obj.OBBox);

                if (collied == SharpDX.ContainmentType.Intersects)
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
    }
}
