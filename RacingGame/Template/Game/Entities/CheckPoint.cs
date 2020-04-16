﻿using SharpDX;
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

        /// <summary> Направление прямоугольника </summary>
        public Vector3 Direction;

        public CheckPoint(MeshObject mesh) : base(mesh)
        {
            Name = mesh.Name;
            index = int.Parse(Name.Last().ToString());

        }

        /// <summary>
        /// Задать баундин бокс
        /// </summary>
        /// <param name="mesh"></param>
        /// <returns></returns>
        protected override OrientedBoundingBox SetOBB(MeshObject mesh)
        {
            var set = new HashSet<Vector3>();

            var normals = new HashSet<Vector3>();

            foreach (var v in mesh.Vertices)
            {
                set.Add((Vector3)v.position);
                normals.Add((Vector3)v.normal);
            }

            var normal = normals.ElementAt(2);
            float angle = MyVector.GetAngle(normal, new Vector3(0.0f, 0.0f, 1.0f));

            Direction = Vector3.Transform(new Vector3(1.0f, 0.0f, 0.0f), Matrix3x3.RotationY(-angle));

            var verts = set.ToArray();

            for (int i = 0; i < verts.Length; i++)
            {
                verts[i] -= mesh.CenterPosition;
                if (angle != 0)
                    verts[i] = Vector3.Transform(verts[i], Matrix3x3.RotationY(angle));
            }
            
            OrientedBoundingBox OBBox = new OrientedBoundingBox(verts);
            Matrix matrix = Matrix.RotationY(-angle);
            OBBox.Transform(matrix);
            OBBox.Translate(mesh.CenterPosition);
            AABBox = BoundingBox.FromPoints(OBBox.GetCorners());

            return OBBox;
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

        public override void CollisionResponce(PhysicalObject obj)
        {
            throw new NotImplementedException();
        }
    }
}
