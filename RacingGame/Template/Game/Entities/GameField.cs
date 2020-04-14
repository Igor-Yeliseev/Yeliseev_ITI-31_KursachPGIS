using SharpDX;
using System;
using System.Collections.Generic;
using Template.Graphics;

namespace Template
{
    class GameField
    {
        private Material material;

        public CheckPoint[] checkPoints;
        private Vector3[] targetPts;
        private int trgIndex = 8;

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

        private TimeHelper timeHelper;
        private Car car;
        private EnemyCar enemy;

        public GameField(TimeHelper timeHelper, Material material)
        {
            this.timeHelper = timeHelper;
            this.material = material;
        }

        public void SetCheckPoints(List<MeshObject> meshes)
        {
            checkPoints = new CheckPoint[meshes.Count];
            targetPts = new Vector3[meshes.Count];

            for (int i = 0; i < checkPoints.Length; i++)
            {
                checkPoints[i] = new CheckPoint(meshes[i]);
                var v = meshes[i].CenterPosition;
                targetPts[i] = new Vector3(v.X, 0, v.Z);
            }

            Array.Sort(checkPoints);
        }

        public void SetCar(Car car)
        {
            this.car = car;
        }

        public void SetEnemy(EnemyCar enemy)
        {
            this.enemy = enemy;
            this.enemy.Target = targetPts[trgIndex];
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
        private void RotateEnemyToTarget(float angle)
        {
            enemy.TurnToTarget(angle);

            if (enemy.IsColliedCheckPts && enemy.IsOnTarget)
            {
                enemy.IsColliedCheckPts = false;
                trgIndex++;

                if (trgIndex == targetPts.Length)
                    trgIndex = 0;

                enemy.Target = targetPts[trgIndex];
            }

            //if(trgIndex < targetPts.Length && enemy.IsOnTarget)
            //{
            //    trgIndex++;

            //    if (trgIndex < targetPts.Length)
            //    {
            //        enemy.Target = targetPts[trgIndex];
            //    }
            //}
        }

        /// <summary> Двигать врага к цели </summary>
        private void GoToTarget()
        {
            if (enemy.IsOnTarget) // Враг повернут в сторону цели
            {
                if (!enemy.CollisionCheckPoint(checkPoints[trgIndex])) // Двингать врага к цели пока не столкнется
                {
                    enemy.Speed = 10;
                    enemy.Move();
                }
            }
        }

        /// <summary> Поворачивать и перемещать врага к цели </summary>
        public void MoveEnemyToTargets()
        {
            float alpha = 2.0f * (float)Math.PI * 0.25f * timeHelper.DeltaT;

            RotateEnemyToTarget(alpha);

            //TurnEnemyWheelsToTarget(alpha);

            GoToTarget();
        }

        public void StopEnemy()
        {
            enemy.Speed = MyMath.Lerp(enemy.Speed, 0, 0.6f, 0.2f);
        }                                          
        
        /// <summary> Поворот колес врага в сторону цели на указаный угол </summary>
        /// <param name="angle"> Угол</param>
        private void TurnEnemyWheelsToTarget(float angle)
        {
            enemy.TurnWheelsToTarget(angle);

            //if (enemy.IsWheelsOnTarget) // Менять цель
            //{
            //    trgIndex++;

            //    if (trgIndex == targetPts.Length)
            //        trgIndex = 0;

            //    enemy.Target = targetPts[trgIndex];
            //}
        }

    }
}
