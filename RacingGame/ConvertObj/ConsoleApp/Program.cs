using ObjLoader.Loader.Loaders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            string file = "D:\\3ds max Export\\box\\2box.obj";
            LoadResult result = null;

            IObjLoaderFactory factory = new ObjLoaderFactory();
            IObjLoader objLoader = factory.Create();
            using (var stream = new FileStream(file, FileMode.Open))
            {
                result = objLoader.Load(stream);
            }

            Console.WriteLine(result);
            Console.ReadLine();
        }
    }
}
