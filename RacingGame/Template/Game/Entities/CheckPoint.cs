using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Graphics;

namespace Template
{
    class CheckPoint : PhysicalObject, IComparable<CheckPoint>
    {
        public string Name { get; set; }
        private int index;

        public CheckPoint(MeshObject mesh) : base(mesh)
        {
            Name = mesh.Name;
            index = int.Parse(Name.Last().ToString());
        }

        public void SetMaterial(Material material)
        {
            _meshes.First().Material = material;
        }

        public override bool CollisionTest(PhysicalObject obj)
        {
            return false;
        }

        public int CompareTo(CheckPoint obj)
        {
            if (index < obj.index)
                return -1;
            else if (index > obj.index)
                return 1;
            else
                return 0;
        }
    }
}
