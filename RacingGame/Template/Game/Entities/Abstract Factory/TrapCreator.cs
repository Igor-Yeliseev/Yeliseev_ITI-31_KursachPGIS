
namespace Template.Entities.Abstract_Factory
{
    class TrapCreator : ISurpriseCreator
    {
        public SurPrise Create(MeshObject mesh)
        {
            // 20% от максимальной скорости
            return new Trap(mesh, 20);
        }
    }
}
