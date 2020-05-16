
namespace Template.Entities.Abstract_Factory
{
    class ExtraSpeed : SurPrise
    {
        public ExtraSpeed(MeshObject mesh, float value) :base (mesh, value)
        {

        }

        public override SurpriseType Type => SurpriseType.Speed;

    }
}
