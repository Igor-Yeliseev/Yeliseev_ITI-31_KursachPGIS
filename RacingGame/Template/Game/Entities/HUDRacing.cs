using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.DirectWrite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    class HUDRacing
    {
        public struct Icon
        {
            public Bitmap bitmap;
            public int index;
            public Matrix3x2 matrix;

        }

        float Left, Right, Bottom, Top;

        private DirectX2DGraphics directX2DGraphics;
        private InputController inputController;
        private TimeHelper timeHelper;

        /// <summary Спидометр </summary>
        Icon speedometer;
        /// <summary> Стрелка </summary>
        Icon arrowSpeed;
        float arrowHeight;

        float angle = 3.1415926535897931f / 180;
        float oldAngle = 0;
        float maxAngle = (float)Math.PI;

        /// <summary> Круги </summary>
        Icon lapIcon;
        Icon ammoIcon;

        /// <summary> Цифры </summary>
        public Icon[] numbers;
        public Icon placeNumber;
        public int lapIndex = 1;
        /// <summary> Количество кругов в гонке </summary>
        public int lapCount;
        public int placeIndex = 1;
        Icon slash;

        Icon healthBar;
        Icon barFrame;
        /// <summary> Величина отнятия здоровья (% от макс здоровья)</summary>
        float hitValue = 0.2f;
        float widthHeath;

        public Icon placeIcon;

        /// <summary> Машина игрока </summary>
        private Car car;
        /// <summary> Машина игркоа </summary>
        public Car Car
        {
            get { return car; }
            set
            {
                car = value;
                car.Collied += (persent) =>
                {
                    if (car.IsDead)
                        return;

                    hitValue = persent * widthHeath;

                    float scale = hitValue / widthHeath;
                    widthHeath -= scale * widthHeath;
                    healthBar.matrix *= Matrix3x2.Scaling(1 - scale, 1.0f, new Vector2(50, lapIcon.bitmap.Size.Height));
                };
            }
        }

        public void AddBonus(Bonus bonus)
        {
            if(bonus.Type == BonusType.Health)
            {
                bonus.OnCatched += (persent) =>
                {
                    if (persent == 0)
                        return;

                    float scale = persent;
                    widthHeath += scale * widthHeath;
                    healthBar.matrix *= Matrix3x2.Scaling(1 + scale, 1.0f, new Vector2(50, lapIcon.bitmap.Size.Height));
                };
            }
        }

        /// <summary> Конструктор худа </summary>
        /// <param name="directX2DGraphics"></param>
        /// <param name="inputController"></param>
        public HUDRacing(DirectX2DGraphics directX2DGraphics, InputController inputController, TimeHelper timeHelper)
        {
            this.directX2DGraphics = directX2DGraphics;
            this.inputController = inputController;
            this.timeHelper = timeHelper;
            
            numbers = new Icon[10];

            textFormatIndex = directX2DGraphics.NewTextFormat("BankGothic Md BT", FontWeight.UltraBold, FontStyle.Normal,
                                FontStretch.Normal, 55, TextAlignment.Leading, ParagraphAlignment.Near);
            textBrushIndex = directX2DGraphics.NewSolidColorBrush(new SharpDX.Mathematics.Interop.RawColor4(0.78f, 0.337f, 0.153f, 1.0f));
        }

        private int textFormatIndex;
        private int textBrushIndex;
        
        /// <summary> Обновить состояние всех изображений hud-а </summary>
        public void Update()
        {
            if (inputController.UpPressed)
            {
                oldAngle = angle;
                angle = MyMath.Lerp(angle, maxAngle, 0.4f, 0.01f);

            }
            else if (inputController.DownPressed)
            {
                oldAngle = angle;

                if (angle > maxAngle / 1.4f)
                {
                    angle = MyMath.Lerp(angle, 0, 0.7f, 0.01f);
                }
                else if (angle > maxAngle / 2.5)
                {
                    angle = MyMath.Lerp(angle, 0, 1.0f, 0.01f);
                }
                else if (angle > maxAngle / 4.0)
                {
                    angle = MyMath.Lerp(angle, 0, 1.3f, 0.01f);
                }
                else
                    angle = MyMath.Lerp(angle, 0, 1.7f, 0.01f);
            }
            else
            {
                oldAngle = angle;
                angle = MyMath.Lerp(angle, 0, 0.3f, 0.01f);
            }

            arrowSpeed.matrix *= Matrix3x2.Translation(new Vector2(-arrowSpeed.bitmap.Size.Width / 2, -(Bottom - arrowHeight) - arrowHeight / 2));
            arrowSpeed.matrix *= Matrix3x2.Rotation(angle - oldAngle);
            arrowSpeed.matrix *= Matrix3x2.Translation(new Vector2(arrowSpeed.bitmap.Size.Width / 2, (Bottom - arrowHeight) + arrowHeight / 2));
            
            if(inputController.Func[3] || inputController.Func[4]){
                arrowSpeed.matrix = Matrix3x2.Identity;
            }
            
        }

        public void GetRectSides()
        {
            Left = directX2DGraphics.RenderTargetClientRectangle.Left;
            Right = directX2DGraphics.RenderTargetClientRectangle.Right;
            Bottom = directX2DGraphics.RenderTargetClientRectangle.Bottom;
            Top = directX2DGraphics.RenderTargetClientRectangle.Top;
        }
        
        /// <summary> Инициализация всех индексов картинок </summary>
        public void InitPicsIndicies()
        {
            speedometer.index = directX2DGraphics.LoadBitmapFromFile("Resources\\HUD\\speedometer.png");
            arrowSpeed.index = directX2DGraphics.LoadBitmapFromFile("Resources\\HUD\\arrow.png");
            lapIcon.index = directX2DGraphics.LoadBitmapFromFile("Resources\\HUD\\lap.png");
            healthBar.index = directX2DGraphics.LoadBitmapFromFile("Resources\\HUD\\HealthBar.png");
            barFrame.index = directX2DGraphics.LoadBitmapFromFile("Resources\\HUD\\barFrame.png");
            slash.index = directX2DGraphics.LoadBitmapFromFile("Resources\\HUD\\slash.png");
            ammoIcon.index = directX2DGraphics.LoadBitmapFromFile("Resources\\HUD\\ammo.png");
            placeIcon.index = directX2DGraphics.LoadBitmapFromFile("Resources\\HUD\\place.png");

            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i].index = directX2DGraphics.LoadBitmapFromFile("Resources\\HUD\\number" + i + ".png");
                numbers[i].matrix = Matrix3x2.Identity;
            }
            
            speedometer.matrix = arrowSpeed.matrix = lapIcon.matrix = healthBar.matrix = 
                barFrame.matrix = slash.matrix = ammoIcon.matrix = Matrix3x2.Identity;
            placeIcon.matrix = Matrix3x2.Identity;
        }

        int dd = 0;

        /// <summary> Получить все битмапы картинок </summary>
        public void GetBitmaps()
        {
            speedometer.bitmap = directX2DGraphics.Bitmaps[speedometer.index];
            lapIcon.bitmap = directX2DGraphics.Bitmaps[lapIcon.index];
            healthBar.bitmap = directX2DGraphics.Bitmaps[healthBar.index];
            slash.bitmap = directX2DGraphics.Bitmaps[slash.index];
            ammoIcon.bitmap = directX2DGraphics.Bitmaps[ammoIcon.index];
            barFrame.bitmap = directX2DGraphics.Bitmaps[barFrame.index];
            placeIcon.bitmap = directX2DGraphics.Bitmaps[placeIcon.index];
            
            hitValue = 0.2f;
            widthHeath = healthBar.bitmap.Size.Width;
            hitValue = widthHeath * hitValue;

            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i].bitmap = directX2DGraphics.Bitmaps[numbers[i].index];
            }
           

            arrowSpeed.bitmap = directX2DGraphics.Bitmaps[arrowSpeed.index];
            arrowHeight = arrowSpeed.bitmap.Size.Height;
        }

        private Matrix3x2 GetTransformMatrix(ref Icon picture)
        {
            float WidthInDIP = picture.bitmap.Size.Width;
            float HeightInDIP = picture.bitmap.Size.Height;
            Matrix3x2 transformMatrix = Matrix3x2.Translation(new Vector2(0, Bottom - HeightInDIP));
            transformMatrix *= picture.matrix;
            return transformMatrix;
        }

        private Matrix3x2 GetTransformMatrix(ref Icon picture, float offsetX, float offsetY)
        {
            Matrix3x2 transformMatrix = Matrix3x2.Translation(new Vector2(offsetX, offsetY));
            transformMatrix *= picture.matrix;
            return transformMatrix;
        }

        /// <summary> Отступы по соответствующим осям </summary>
        private float X, Y;
        /// <summary> Отрисовка всех картинок hud-а </summary>
        public void DrawBitmaps()
        {
            directX2DGraphics.DrawBitmap(speedometer.index, GetTransformMatrix(ref speedometer), 1.0f, BitmapInterpolationMode.Linear);
            directX2DGraphics.DrawBitmap(arrowSpeed.index, GetTransformMatrix(ref arrowSpeed), 1.0f, BitmapInterpolationMode.Linear);
            directX2DGraphics.DrawBitmap(lapIcon.index, GetTransformMatrix(ref lapIcon, 50, 0), 1.0f, BitmapInterpolationMode.Linear);
            directX2DGraphics.DrawBitmap(numbers[lapIndex].index, GetTransformMatrix(ref numbers[lapIndex], 50 + lapIcon.bitmap.Size.Width, 0), 1.0f, BitmapInterpolationMode.Linear);

            X = lapIcon.bitmap.Size.Width + numbers[lapIndex].bitmap.Size.Width;
            directX2DGraphics.DrawBitmap(slash.index, GetTransformMatrix(ref slash, X, 0), 1.0f, BitmapInterpolationMode.Linear);
            
            directX2DGraphics.DrawBitmap(numbers[lapCount].index, GetTransformMatrix(ref numbers[lapCount], X + 60, 0), 1.0f, BitmapInterpolationMode.Linear);

            Y = lapIcon.bitmap.Size.Height;
            directX2DGraphics.DrawBitmap(healthBar.index, GetTransformMatrix(ref healthBar, 50, Y), 1.0f, BitmapInterpolationMode.Linear);
            directX2DGraphics.DrawBitmap(barFrame.index, GetTransformMatrix(ref barFrame, 50-6, Y - 6), 1.0f, BitmapInterpolationMode.Linear);

            directX2DGraphics.DrawBitmap(ammoIcon.index, GetTransformMatrix(ref ammoIcon, 50, Y + 20), 1.0f, BitmapInterpolationMode.Linear);

            if(car.Lap == lapCount)
            {
                directX2DGraphics.DrawBitmap(placeIcon.index, GetTransformMatrix(ref placeIcon, (Right - placeIcon.bitmap.Size.Width) / 2,
                                        (Bottom - placeIcon.bitmap.Size.Height) / 2), 1.0f, BitmapInterpolationMode.Linear);
                directX2DGraphics.DrawBitmap(placeNumber.index, GetTransformMatrix(ref placeNumber, 400, 50), 1.0f, BitmapInterpolationMode.Linear);
            }
        }

        /// <summary> Минуты </summary>
        private int minutes;
        private int minPassed = 60;
        /// <summary> Время гонки </summary>
        private string raceTime;

        /// <summary> Текущее время гонки </summary>
        /// <returns></returns>
        private string getTime()
        {
            if (car.Lap == lapCount)
                return raceTime;

            if (timeHelper.Time >= minPassed)
            {
                minPassed += 60;
                minutes++;
            }

            raceTime = $"0:{ minutes }:{ timeHelper.Time:f2}";
            return raceTime;
        }

        /// <summary> Отрисовка текста </summary>
        public void DrawText()
        {
            Matrix3x2 transform = Matrix3x2.Translation(new Vector2(Right * 0.75f, 70));
            directX2DGraphics.DrawText(getTime(), textFormatIndex, transform, directX2DGraphics.RenderTargetClientRectangle, textBrushIndex);
        }
    }
}
