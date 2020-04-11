using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Graphics;
using static Template.MeshObject;

namespace Template
{
    class CheckPoint : PhysicalObject, IComparable<CheckPoint>
    {
        public string Name { get; }
        private int index;

        public CheckPoint(MeshObject mesh) : base(mesh)
        {
            Name = mesh.Name;
            index = int.Parse(Name.Last().ToString());
            
            //RotateOBB(1);

            //if (MyVector.CosProduct(dir, (point1 - point0)) < 0)
            //    RotateOBB(-angle);
            //else
            //    RotateOBB(angle);

        }

        /// <summary>
        /// Задать баундин бокс
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        protected override OrientedBoundingBox SetOBB(MeshObject mesh)
        {
            var set = new HashSet<Vector3>();

            var normals = new HashSet<Vector4>();

            foreach (var v in mesh.Vertices)
            {
                set.Add((Vector3)v.position);
                normals.Add(v.normal);
            }

            var normal = normals.ElementAt(2);
            normal.W = 0;
            float angle = MyVector.GetAngle(normal, new Vector4(0.0f, 0.0f, 1.0f, 0.0f));

            var verts = set.ToArray();

            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] -= (Vector3)mesh.CenterPosition;
                if (angle != 0)
                    verts[i] = Vector3.Transform(verts[i], Matrix3x3.RotationY(angle));
            }

            OrientedBoundingBox boundingBox = new OrientedBoundingBox(verts);
            Matrix matrix = Matrix.RotationY(-angle);
            boundingBox.Transform(matrix);
            boundingBox.Translate((Vector3)mesh.CenterPosition);
            return boundingBox;
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
