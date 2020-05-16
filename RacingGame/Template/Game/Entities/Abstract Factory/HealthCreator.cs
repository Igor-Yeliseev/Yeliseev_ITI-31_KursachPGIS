
namespace Template.Entities.Abstract_Factory
{
    class HealthCreator : ISurpriseCreator
    {
        public SurPrise Create(MeshObject mesh)
        {
            return new Health(mesh, 20);
        }
    }
}
