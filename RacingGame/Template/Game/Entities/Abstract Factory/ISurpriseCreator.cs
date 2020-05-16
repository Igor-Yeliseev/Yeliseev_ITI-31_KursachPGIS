
namespace Template.Entities.Abstract_Factory
{
    interface ISurpriseCreator
    {
        /// <summary>
        /// Создание сюрприза
        /// </summary>
        /// <param name="mesh"> Меш объекта</param>
        /// <returns></returns>
        SurPrise Create(MeshObject mesh);
    }
}
