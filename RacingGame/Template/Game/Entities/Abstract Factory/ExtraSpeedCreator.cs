﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template.Entities.Abstract_Factory
{
    class ExtraSpeedCreator : ISurpriseCreator
    {
        public SurPrise Create(MeshObject mesh)
        {
            // 15% от максимальной скорости
            return new ExtraSpeed(mesh, 15);
        }
    }
}
