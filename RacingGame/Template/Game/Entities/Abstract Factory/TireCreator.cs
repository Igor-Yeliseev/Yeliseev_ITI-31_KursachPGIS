using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Entities.Abstract_Factory
{
    class TireCreator : ISurpriseCreator
    {
        public SurPrise Create(MeshObject mesh)
        {
            return new Tire(mesh, 20);
        }
    }
}
