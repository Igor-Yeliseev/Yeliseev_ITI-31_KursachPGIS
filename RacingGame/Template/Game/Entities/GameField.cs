using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using Template.Entities.Abstract_Factory;

namespace Template
{
    class GameField
    {
        /// <summary> Массив чекпоинтов </summary>
        public CheckPoint[] checkPoints;
        /// <summary> Массив центральных точек боксов чекпоинтов </summary>
        private Vector3[] centerPts; // private

        private List<MeshObject> pillars;

        /// <summary> Количество кругов в гонке </summary>
        private int lapsCount = 1;
        /// <summary> Количество кругов в гонке </summary>
        public int LapsCount
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
        
        /// <summary> Индекс для коллеции чекпоинтов </summary>
        private int chptIndex = 0;
        /// <summary> Тайм хелпер </summary>
        private TimeHelper timeHelper;
        /// <summary> Машина игрока </summary>
        private Car car;
        /// <summary> Соперники по гонке </summary>
        private List<Enemy> enemies = new List<Enemy>();
        /// <summary> Физические объекты в игре</summary>
        private List<PhysicalObject> prefabs = new List<PhysicalObject>();

        /// <summary> Угол за один кадр (расчитывается в самом начале во избежание просадок FPS)</summary>
        private float angle;
        public float Angle { get => angle; set => angle = value; }
        
        /// <summary> Количество физических объектов на сцене</summary>
        public int PhysObjCount
        {
            get
            {
                return prefabs.Count +
                       enemies.Count +
                       surprises.Count +
                       checkPoints.Length +
                       1; // <- This is a player
            }
        }

        /// <summary> Мой гоночный 2D Худ </summary>
        HUDRacing hud;
        /// <summary> Игровые звуки</summary>
        Sounds sounds;


        public GameField(TimeHelper timeHelper, HUDRacing hud, Sounds sounds)
        {
            this.timeHelper = timeHelper;
            this.hud = hud;
            this.hud.lapCount = lapsCount;
            this.sounds = sounds;
        }

        public void SetCheckPoints(List<MeshObject> meshes)
        {
            meshes.Sort(new CheckPointsComparer());

            checkPoints = new CheckPoint[meshes.Count];
            centerPts = new Vector3[meshes.Count];

            for (int i = 0; i < checkPoints.Length; i++)
            {
                checkPoints[i] = new CheckPoint(meshes[i]);
                var v = meshes[i].CenterPosition;
                centerPts[i] = new Vector3(v.X, 0, v.Z);
            }

        }

        /// <summary> Значек из симс </summary>
        MeshObject _hedra1, _hedra2;
        /// <summary> Значок над цилиднром </summary>
        public MeshObject Hedra
        {
            get
            {
                return _hedra1;
            }
            set
            {
                _hedra1 = value;
                _hedra2 = (MeshObject)_hedra1.Clone();
            }
        }

        public void SetPillars(List<MeshObject> pillars)
        {
            pillars.Sort(new PillarsComparer());
            this.pillars = pillars;
        }

        /// <summary> Специфическая отрисовка </summary>
        /// <param name="viewMatrix"></param>
        /// <param name="projectionMatrix"></param>
        public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            int indx = 2 * ((chptIndex != checkPoints.Length - 1) ? chptIndex : chptIndex - 2);
            indx *= (chptIndex == checkPoints.Length) ? 0 : 1; 

            pillars[indx].Render(viewMatrix, projectionMatrix);
            pillars[indx + 1].Render(viewMatrix, projectionMatrix);

            _hedra1.Position = pillars[indx].Position;
            _hedra1.Render(viewMatrix, projectionMatrix);

            _hedra2.Position = pillars[indx + 1].Position;
            _hedra2.Render(viewMatrix, projectionMatrix);

            _hedra1.YawBy(angle);
            _hedra2.YawBy(angle);

            // Отрисовка бонусов и ловушек
            surprises.ForEach(b => b.Draw(viewMatrix, projectionMatrix, angle * 0.7f));
        }

        public void SetPlayer(Car car)
        {
            this.car = car;
            sounds.Car = car;
        }

        public void AddEnemy(Enemy enemy)
        {
            enemies.Add(enemy);
            enemy.MaxSpeed = 25;
            enemy.CheckPoint = centerPts[0] + checkPoints[0].Direction * randomIncremCoord(checkPoints[0]);
            enemy.Target = enemy.CheckPoint;
        }

        /// <summary> Сюрпризы </summary>
        private List<SurPrise> surprises = new List<SurPrise>();

        /// <summary> Добавить бонус </summary>
        /// <param name="surprise"> Добавляемый сюрприз </param>
        private void AddBonus(SurPrise surprise)
        {
            surprises.Add(surprise);
            sounds.AddSoundBonus(surprise);
            hud.AddBonus(surprise);
        }

        /// <summary>
        /// Словарь мешей бонусов по типу бонуса
        /// </summary>
        private Dictionary<SurpriseType, MeshObject> bonusMeshes = new Dictionary<SurpriseType, MeshObject>();
        /// <summary> Добавить меши бонусов </summary>
        /// <param name="mesh"></param>
        public void AddPriseMesh(MeshObject mesh, SurpriseType type)
        {
            bonusMeshes.Add(type, mesh);
        }

        /// <summary> Добавить физический объект </summary>
        /// <param name="prefab"></param>
        public void AddPrefab(PhysicalObject prefab)
        {
            prefabs.Add(prefab);
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
                chptIndex++;
                if (chptIndex == checkPoints.Length)
                {
                    car.Lap++;
                    if (car.Lap == lapsCount)
                    {
                        hud.placeNumber = hud.numbers[hud.placeIndex];
                        //hud.placeNumber.matrix = Matrix3x2.Identity;
                        hud.placeNumber.matrix *= Matrix3x2.Scaling(2.0f);
                        chptIndex = 0;
                        return;
                    }

                    hud.lapIndex++;
                    chptIndex = 0;
                }
            }
        }


        int sign = 0;
        SurPrise spike;
        int countHP, countSpeed, countDamage, countTire;
        /// <summary> Фабрика сюрпризов </summary>
        ISurpriseCreator surpriseFactory = null;
        SurPrise prise;

        /// <summary> Метод создания сюрпризов на карте </summary>
        public void CreateSurprises()
        {

            countHP = surprises.Where(b => b.Type == SurpriseType.Health).Count();
            countSpeed = surprises.Where(b => b.Type == SurpriseType.Speed).Count();
            countDamage = surprises.Where(b => b.Type == SurpriseType.Damage).Count();
            countTire = surprises.Where(b => b.Type == SurpriseType.Tire).Count();

            if (spike != null)
            {
                var dir = spike.Position - (car.Position - (car.Direction * 10));
                if (Math.Sign(MyVector.CosProduct(spike.Direction, dir)) != sign)
                {
                    surprises.Remove(spike);
                    spike.Dispose();
                    sign = 0;
                }
            }

            if (countHP == 0)
            {
                surpriseFactory = new HealthCreator();
                SurPrise health = surpriseFactory.Create((MeshObject)bonusMeshes[SurpriseType.Health].Clone());
                health.MoveTo(getRandPos(chptIndex, health.Position.Y));

                //Surprise heatlh = new Surprise((MeshObject)bonusMeshes[SurpriseType.Health].Clone(), SurpriseType.Health, 20);
                //heatlh.MoveTo(getRandPos(chptIndex, heatlh.Position.Y));
                AddBonus(health);
            }
            if (countSpeed == 0)
            {
                surpriseFactory = new ExtraSpeedCreator();
                SurPrise speed = surpriseFactory.Create((MeshObject)bonusMeshes[SurpriseType.Speed].Clone());
                speed.MoveTo(getRandPos(chptIndex + 1, speed.Position.Y));

                //Surprise speed = new Surprise((MeshObject)bonusMeshes[SurpriseType.Speed].Clone(), SurpriseType.Speed, 20);
                //speed.MoveTo(getRandPos(chptIndex + 1, speed.Position.Y));
                AddBonus(speed);
            }
            if (countTire == 0)
            {
                surpriseFactory = new TireCreator();
                SurPrise tire = surpriseFactory.Create((MeshObject)bonusMeshes[SurpriseType.Tire].Clone());
                tire.MoveTo(getRandPos(chptIndex + 1, tire.Position.Y));

                AddBonus(tire);
            }
            if (countDamage == 0)
            {
                surpriseFactory = new TrapCreator();
                SurPrise trap = surpriseFactory.Create((MeshObject)bonusMeshes[SurpriseType.Damage].Clone());
                trap.MoveTo(getRandPos(chptIndex, trap.Position.Y));


                //Surprise damage = new Surprise((MeshObject)bonusMeshes[SurpriseType.Damage].Clone(), SurpriseType.Damage, 20);
                //damage.MoveTo(getRandPos(chptIndex, damage.Position.Y));
                // Знак косого произведения
                sign = Math.Sign(MyVector.CosProduct(trap.Direction, trap.Position - car.Position));
                spike = trap;
                AddBonus(trap);
            }

        }

        private Vector3 getRandPos(int idx, float Y = 0)
        {
            if (idx == checkPoints.Length - 1)
                idx = 0;

            var pt = centerPts[idx] + checkPoints[idx].Direction * randomIncremCoord(checkPoints[idx]);
            pt += (checkPoints[idx + 1].Position - checkPoints[idx].Position) * 0.8f;
            return new Vector3(pt.X, Y, pt.Z);
        }

        /// <summary> Рандом приращения координаты от центра чекпоинта </summary>
        /// <param name="checkPoint"> Экземпляр чекпоинта</param>
        /// <returns></returns>
        private float randomIncremCoord(CheckPoint checkPoint)
        {
            float min = (checkPoint.Direction * checkPoint.OBBox.Extents.X).X;
            float max = (-checkPoint.Direction * checkPoint.OBBox.Extents.X).X;
            return MyMath.Random(min, max);
        }

        /// <summary>
        /// Поворот врага к цели
        /// </summary>
        /// <param name="angle"></param>
        private void RotateEnemyToTarget(float angle, Enemy enemy)
        {
            enemy.Target = enemy.CheckPoint;
            enemy.CheckWheelsDirOnTarget(angle);

            if (enemy.IsColliedCheckPts) // && enemy.IsOnTarget
            {
                enemy.IsColliedCheckPts = false;
                enemy.IsWheelsOnTarget = false;

                enemy.targetIndex++;

                if (enemy.targetIndex == centerPts.Length)
                {
                    enemy.targetIndex = 0;
                    enemy.Lap++;
                    if (enemy.Lap == lapsCount)
                    {
                        hud.placeIndex++;
                        enemy.IsFinishRace = true;
                        enemy.Speed = 0;
                        return;
                    }
                }
                
                int index = enemy.targetIndex;
                enemy.CheckPoint = centerPts[index] + checkPoints[index].Direction * randomIncremCoord(checkPoints[index]);
                enemy.Target = enemy.CheckPoint;

                enemy.MaxSpeed = MyMath.Random(18, 28);
            }

        }
        
        /// <summary> Двигать врага к цели </summary>
        private void GoToTarget(Enemy enemy)
        {
            if (!enemy.CollisionCheckPoint(checkPoints[enemy.targetIndex])) // Двигать врага к цели пока не столкнется
            {
                if(enemy.Speed > enemy.MaxSpeed)
                {
                    enemy.SlowDown();
                }
                else
                    enemy.Accelerate();
            }

            enemy.MoveProperly();
        }

        /// <summary> Поворачивать и перемещать врага к цели </summary>
        private void MoveEnemyToTargets(Enemy enemy)
        {
            if (enemy.IsFinishRace)
                return;

            RotateEnemyToTarget(angle, enemy);
            GoToTarget(enemy);
        }

        /// <summary> Симуляция движения соперников </summary>
        public void MoveEnemies()
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    if (i == j)
                        enemies[i].CheckObstacle(car, angle);
                    else
                        enemies[i].CheckObstacle(enemies[j], angle);
                }
                MoveEnemyToTargets(enemies[i]);
            }
        }

        /// <summary> Проверка и обработка столкновений всех физических объектов </summary>
        public void CheckCollisions()
        {
            enemies.ForEach(enemy => car.CollisionTest(enemy));

            prefabs.ForEach(p => car.CollisionTest(p));
            
            int countB = surprises.Count;
            for (int i = 0; i < countB; i++)
            {
                if (surprises[i].CollisionTest(car))
                {
                    surprises.RemoveAt(i);
                    countB--;
                }
            }
        }

    }
}
