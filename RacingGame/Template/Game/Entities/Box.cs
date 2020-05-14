using SharpDX;
using Template.Graphics;

namespace Template
{
    class Box 
    {
        //public void SetMaterial(Material material)
        //{
        //    _meshes.ForEach(m => m.Material = material);
        //} 

        //private Vector3 _direction;
        ///// <summary> Напрвление движения </summary>
        //public Vector3 Direction { get => _direction; set => _direction = value; }
        
        //public Box(MeshObject mesh) : base(mesh)
        //{
        //    _direction = new Vector3(0.0f, 0.0f, 1.0f);
        //}

        //public void MoveForward()
        //{
        //    MoveBy(_direction / 20);
        //    moveSign = 1;
        //}

        //public void MoveBackward()
        //{
        //    MoveBy(-_direction / 20);
        //    moveSign = -1;
        //}

        //public void Move(Vector3 direction)
        //{
        //    MoveBy(direction / 20);
        //}

        //public void RotateY(float angle)
        //{
        //    _meshes.ForEach(m => m.YawBy(angle));
        //    _direction = Vector3.Transform(_direction, Matrix3x3.RotationY(angle));
        //    //Vector3 dv = new Vector3(-Position.X, -Position.Y, -Position.Z);
        //    //OBBox.Translate(dv);
        //    //OBBox.Transform(Matrix.RotationY(angle));
        //    //OBBox.Translate(-dv);
        //    RotateOBB(angle);
        //}

        //public int moveSign = 0;

        //public override bool CollisionTest(PhysicalObject obj)
        //{
        //    if (AABBox.Intersects(obj.AABBox))
        //    {
        //        collied = OBBox.Contains(ref obj.OBBox);

        //        return (collied == ContainmentType.Intersects) ? true : false;
        //    }
        //    else
        //        return false;
        //}

        //public override void CollisionResponce(PhysicalObject obj)
        //{
        //    if (moveSign > 0)
        //    {
        //        while (OBBox.Contains(ref obj.OBBox) == ContainmentType.Intersects)
        //            Move(-_direction);
        //    }
        //    else
        //    {
        //        while (OBBox.Contains(ref obj.OBBox) == ContainmentType.Intersects)
        //            Move(_direction);
        //    }
        //}

    }
}
