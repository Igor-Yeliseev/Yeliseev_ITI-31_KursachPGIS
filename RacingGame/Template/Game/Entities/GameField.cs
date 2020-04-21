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

        
        /// <summary> Количество кругов в гонке </summary>
        private int lapsCount = 1;
        /// <summary> Количество кругов в гонке </summary>
        public int LapCount
        {
            get => lapsCount;
            set
            {
                if (value >= 0)
                {
                    lapsCount = value;
                }
            }
        }

        private int place = 1;

        private int chptIndex = 0;

        private TimeHelper timeHelper;
        private Car car;
        private EnemyCar enemy;
        private List<EnemyCar> enemies = new List<EnemyCar>();

        private float angle;
        public float Angle { get => angle; set => angle = value; }

        Animations animations;

        /// <summary> 2D Худ </summary>
        HUDRacing hud;

        public GameField(TimeHelper timeHelper, HUDRacing hud, Material material)
        {
            this.timeHelper = timeHelper;
            this.material = material;
            this.hud = hud;
            this.hud.lapCount = lapsCount;
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
            
        }

        public void SetCar(Car car)
        {
            this.car = car;
        }

        public void AddEnemy(EnemyCar enemy)
        {
            enemies.Add(enemy);
            enemy.Target = centerPts[0];
        }


        /// <summary>
        /// Проверка окончания гонки
        /// </summary>
        /// <returns></returns>
        public void CheckRaceFinish()
        {
            if (car.Lap == lapsCount)
                return;

            if (chptIndex < checkPoints.Length && car.CollisionCheckPoint(checkPoints[chptIndex]))
            {
                checkPoints[chptIndex].SetMaterial(material);
                chptIndex++;
                if (chptIndex == checkPoints.Length)
                {
                    car.Lap++;
                    if(car.Lap == lapsCount)
                    {
                        hud.placeIcon.matrix = Matrix3x2.Identity;
                        hud.placeNumber = hud.numbers[hud.placeIndex];
                        hud.placeNumber.matrix = Matrix3x2.Identity;
                        hud.placeNumber.matrix *= Matrix3x2.Scaling(2.0f);
                        return;
                    }

                    hud.lapIndex++;
                    chptIndex = 0;
                }
            }
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
        private void RotateEnemyToTarget(float angle, EnemyCar enemy)
        {
            enemy.Target = centerPts[enemy.targetIndex];
            enemy.CheckWheelsDirOnTarget(angle);
            
            if (enemy.IsColliedCheckPts) // && enemy.IsOnTarget
            {
                enemy.IsColliedCheckPts = false;
                enemy.IsWheelsOnTarget = false;

                enemy.targetIndex++;

                if (enemy.targetIndex == centerPts.Length)
                {
                    enemy.Lap++;
                    if(enemy.Lap == lapsCount)
                    {
                        hud.placeIndex++;
                    }
                    enemy.targetIndex = 0;
                }

                enemy.Target = centerPts[enemy.targetIndex];
                //enemy.Target = centerPts[trgIndex] + checkPoints[trgIndex].Direction * randomIncremCoord(checkPoints[trgIndex]);
            }
            
        }

        /// <summary> Двигать врага к цели </summary>
        private void GoToTarget(EnemyCar enemy)
        {
            if (!enemy.CollisionCheckPoint(checkPoints[enemy.targetIndex])) // Двигать врага к цели пока не столкнется
            {
                enemy.Accelerate(15);
            }
            //else
            //{
            //    enemy.Brake();
            //}

            enemy.MoveProperly();
        }

        /// <summary> Поворачивать и перемещать врага к цели </summary>
        private void MoveEnemyToTargets(EnemyCar enemy)
        {
            RotateEnemyToTarget(angle, enemy);
            GoToTarget(enemy);
        }
        
        public void MoveEnemies()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                MoveEnemyToTargets(enemies[i]);
            }
        }
    }
}
