
namespace Template.Entities.Abstract_Factory
{
    class Health : SurPrise
    {
        public Health(MeshObject mesh, float value) : base(mesh, value)
        {

        }

        public override SurpriseType Type => SurpriseType.Health;
    }
}
