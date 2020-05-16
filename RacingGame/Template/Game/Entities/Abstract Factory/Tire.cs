using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Entities.Abstract_Factory
{
    class Tire : SurPrise
    {
        public Tire(MeshObject mesh, float value) : base(mesh, value)
        {

        }

        public override SurpriseType Type => SurpriseType.Tire;
    }
}
