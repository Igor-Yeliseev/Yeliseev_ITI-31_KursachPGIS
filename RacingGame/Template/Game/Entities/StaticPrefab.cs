using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            return false;
        }
    }
}
