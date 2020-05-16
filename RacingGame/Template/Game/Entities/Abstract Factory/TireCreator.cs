
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
