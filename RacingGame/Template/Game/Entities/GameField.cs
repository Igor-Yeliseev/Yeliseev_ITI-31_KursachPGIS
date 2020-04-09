using SharpDX;
using System.Collections.Generic;
using Template.Graphics;

namespace Template
{
    class GameField
    {
        private Material material;

        public CheckPoint[] checkPoints;
        private int lapsCount = 3;
        public int LapCount
        {
            get => lapsCount;
            set
            {
                if(value >= 0)
                    lapsCount = value;
            }
        }

        private int lapIndex = 0;

        private Car car;

        public GameField(Material material)
        {
            this.material = material;
        }

        public void SetCheckPoints(List<MeshObject> meshes)
        {
            checkPoints = new CheckPoint[meshes.Count];
            
            for (int i = 0; i < checkPoints.Length; i++)
            {
                checkPoints[i] = new CheckPoint(meshes[i]);
            }

            System.Array.Sort(checkPoints);
        }

        public void SetCar(Car car)
        {
            this.car = car;
        }

        /// <summary>
        /// Проверка окончания гонки
        /// </summary>
        /// <returns></returns>
        public bool CheckRaceFinish()
        {
            if (lapIndex < checkPoints.Length && (car.CollisionCheckPt(checkPoints[lapIndex])))
            {
                checkPoints[lapIndex].SetMaterial(material);

                lapIndex++;
            }

            return (lapIndex == lapsCount - 1) ? true : false;
        }




    }
}
