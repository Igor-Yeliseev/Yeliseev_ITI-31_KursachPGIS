using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    class PillarsComparer : IComparer<MeshObject>
    {
        public int Compare(MeshObject m1, MeshObject m2)
        {
            int x = int.Parse(m1.Name.Substring(m1.Name.IndexOf('r') + 1));
            int y = int.Parse(m2.Name.Substring(m2.Name.IndexOf('r') + 1));

            if (x < y)
                return -1;
            else if (x > y)
                return 1;
            else
                return 0;
        }
    }

}
