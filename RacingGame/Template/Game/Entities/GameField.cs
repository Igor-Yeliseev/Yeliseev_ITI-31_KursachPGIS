using SharpDX;
using System;
using System.Collections.Generic;
using Template.Graphics;

namespace Template
{
    class GameField
    {
        private Material material;
        /// <summary> Массив чекпоинтов </summary>
        public CheckPoint[] checkPoints;
        /// <summary> Массив центральных точек боксов чекпоинтов </summary>
        public Vector3[] centerPts; // private
        public int trgIndex = 0;

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
        Animations animations;

        public GameField(TimeHelper timeHelper, Material material)
        {
            this.timeHelper = timeHelper;
            this.material = material;
            animations = new Animations();
        }

        public void SetCheckPoints(List<MeshObject> meshes)
        {
            checkPoints = new CheckPoint[meshes.Count];
            centerPts = new Vector3[meshes.Count];

            for (int i = 0; i < checkPoints.Length; i++)
            {
                checkPoints[i] = new CheckPoint(meshes[i]);
                var v = meshes[i].CenterPosition;
                centerPts[i] = new Vector3(v.X, 0, v.Z);
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
            this.enemy.Target = centerPts[trgIndex];
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
        
        /// <summary> Рандом приращения координаты от центра чекпоинта </summary>
        /// <param name="checkPoint"> Экземпляр чекпоинта</param>
        /// <returns></returns>
        private float randomIncremCoord(CheckPoint checkPoint)
        {
            float min = (checkPoint.Direction * checkPoint.OBBox.Extents.X).X;
            float max = (- checkPoint.Direction * checkPoint.OBBox.Extents.X).X;
            return MyMath.Random(min, max);
        }

        /// <summary>
        /// Поворот врага к цели
        /// </summary>
        /// <param name="angle"></param>
        private void RotateEnemyToTarget(float angle)
        {
            enemy.Target = centerPts[trgIndex];
            enemy.CheckWheelsDirOnTarget(angle);
            
            if (enemy.IsColliedCheckPts) // && enemy.IsOnTarget
            {
                enemy.IsColliedCheckPts = false;
                enemy.IsWheelsOnTarget = false;

                trgIndex++;

                if (trgIndex == centerPts.Length)
                    trgIndex = 0;

                enemy.Target = centerPts[trgIndex];
                //enemy.Target = centerPts[trgIndex] + checkPoints[trgIndex].Direction * randomIncremCoord(checkPoints[trgIndex]);
            }
            
        }

        /// <summary> Двигать врага к цели </summary>
        private void GoToTarget()
        {
            if (!enemy.CollisionCheckPoint(checkPoints[trgIndex])) // Двигать врага к цели пока не столкнется
            {
                enemy.Accelerate(12);
            }
            //else
            //{
            //    enemy.Brake();
            //}

            enemy.MoveProperly();
        }

        /// <summary> Поворачивать и перемещать врага к цели </summary>
        public void MoveEnemyToTargets()
        {
            float alpha = 2.0f * (float)Math.PI * 0.25f * timeHelper.DeltaT;

            RotateEnemyToTarget(alpha);

            GoToTarget();
        }

        public void StopEnemy()
        {
            enemy.Speed = MyMath.Lerp(enemy.Speed, 0, 0.6f, 0.2f);
        }                                          
        
        /// <summary> Поворот колес врага в сторону цели на указаный угол </summary>
        /// <param name="angle"> Угол</param>
        //private void TurnEnemyWheelsToTarget(float angle)
        //{
        //    enemy.TurnWheelsToTarget(angle);

        //    if (enemy.IsColliedCheckPts && enemy.IsWheelsOnTarget) // Менять цель
        //    {
        //        enemy.IsColliedCheckPts = false;
        //        trgIndex++;

        //        if (trgIndex == centerPts.Length)
        //            trgIndex = 0;
        //        enemy.Target = centerPts[trgIndex];
        //    }
        //}

    }
}
