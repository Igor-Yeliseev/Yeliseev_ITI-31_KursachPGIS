using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Entities.Abstract_Factory
{
    class TrapCreator : ISurpriseCreator
    {
        public SurPrise Create(MeshObject mesh)
        {
            return new Trap(mesh, 20);
        }
    }
}
