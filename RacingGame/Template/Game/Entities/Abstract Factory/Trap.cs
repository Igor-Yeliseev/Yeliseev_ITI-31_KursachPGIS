
namespace Template.Entities.Abstract_Factory
{
    class Trap : SurPrise
    {
        public Trap(MeshObject mesh, float value) : base(mesh, value)
        {

        }

        public override SurpriseType Type => SurpriseType.Damage;
    }
}
