using SharpDX;
using System.Collections.Generic;
using Template.Graphics;

namespace Template
{
    class GameField
    {
        private Material material;

        public CheckPoint[] checkPoints;
        private Vector4[] targetPts;
        private int tarIndex = 0;

        private int lapsCount = 3;
        public int LapCount
        {
            get => lapsCount;
            set
            {
                if (value >= 0)
                    lapsCount = value;
            }
        }

        private int lapIndex = 0;

        private Car car;
        private EnemyCar enemy;

        public GameField(Material material)
        {
            this.material = material;
        }

        public void SetCheckPoints(List<MeshObject> meshes)
        {
            checkPoints = new CheckPoint[meshes.Count];
            targetPts = new Vector4[meshes.Count];

            for (int i = 0; i < checkPoints.Length; i++)
            {
                checkPoints[i] = new CheckPoint(meshes[i]);
                var v = meshes[i].CenterPosition;
                targetPts[i] = new Vector4(v.X, 0, v.Z, 0);
            }

            System.Array.Sort(checkPoints);
        }

        public void SetCar(Car car)
        {
            this.car = car;
        }

        public void SetEnemy(EnemyCar enemy)
        {
            this.enemy = enemy;
            this.enemy.Target = targetPts[0];
        }

        /// <summary>
        /// Проверка окончания гонки
        /// </summary>
        /// <returns></returns>
        public bool CheckRaceFinish()
        {
            if (lapIndex < checkPoints.Length && (car.CollisionCheckPoint(checkPoints[lapIndex])))
            {
                checkPoints[lapIndex].SetMaterial(material);

                lapIndex++;
            }

            return (lapIndex == lapsCount - 1) ? true : false;
        }
        

        /// <summary>
        /// Поворот врага к цели
        /// </summary>
        /// <param name="angle"></param>
        public void RotateEnemyToTarget(float angle)
        {
            enemy.TurnToTarget(angle);

            if(tarIndex < targetPts.Length && enemy.IsOnTarget)
            {
                tarIndex++;
                
                if (tarIndex < targetPts.Length)
                {
                    enemy.Target = targetPts[tarIndex];
                    tarIndex += 0;
                }
            }
        }

    }
}
