﻿using SharpDX;
using SharpDX.Direct2D1;
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

        DirectX2DGraphics directX2DGraphics;
        InputController inputController;

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
        public int lapCount = 0;
        public int placeIndex = 1;
        Icon slash;

        Icon healthBar;
        Icon barFrame;
        /// <summary> Величина отнятия здоровья (10% от макс здоровья)</summary>
        float hitValue = 0.2f;
        float widthHeath;

        public Icon placeIcon;

        private Car car;
        public Car Car
        {
            get { return car; }
            set
            {
                car = value;
                car.Collied += (c) =>
                {
                    if (car.IsDead)
                        return;

                    float scale = hitValue / widthHeath;
                    widthHeath -= scale * widthHeath;
                    healthBar.matrix *= Matrix3x2.Scaling(1 - scale, 1.0f, new Vector2(50, lapIcon.bitmap.Size.Height));
                };
            }
        }

        public HUDRacing(DirectX2DGraphics directX2DGraphics, InputController inputController)
        {
            this.directX2DGraphics = directX2DGraphics;
            this.inputController = inputController;

            numbers = new Icon[10];
        }

        public void Update()
        {
            if (inputController.Space)
            {
                //numbers[placeIndex].matrix = Matrix3x2.Identity;
                //placeNumber = numbers[placeIndex];
                //placeNumber.matrix = Matrix3x2.Identity;
                //placeNumber.matrix *= Matrix3x2.Scaling(2.0f);

                //lapIndex++;
                //if (lapIndex > 9) lapIndex = 0;

                //if (widthHeath > 0)
                //{
                //    float scale = hitValue / widthHeath;
                //    widthHeath -= scale * widthHeath;
                //    healthBar.matrix *= Matrix3x2.Scaling(1 - scale, 1.0f, new Vector2(50, lapIcon.bitmap.Size.Height));
                //}

                //float scale = hitValue / widthHeath;
                //widthHeath += scale * widthHeath;
                //healthBar.matrix *= Matrix3x2.Scaling(1 + scale, 1.0f, new Vector2(50, lapIcon.bitmap.Size.Height));

            }

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
            //placeIcon.matrix *= 0;
        }

        public void GetBitmaps()
        {
            speedometer.bitmap = directX2DGraphics.Bitmaps[speedometer.index];
            lapIcon.bitmap = directX2DGraphics.Bitmaps[lapIcon.index];
            healthBar.bitmap = directX2DGraphics.Bitmaps[healthBar.index];
            slash.bitmap = directX2DGraphics.Bitmaps[slash.index];
            ammoIcon.bitmap = directX2DGraphics.Bitmaps[ammoIcon.index];
            barFrame.bitmap = directX2DGraphics.Bitmaps[barFrame.index];
            placeIcon.bitmap = directX2DGraphics.Bitmaps[placeIcon.index];
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


        private float X, Y;
        public void DrawBitmaps()
        {
            directX2DGraphics.DrawBitmap(speedometer.index, GetTransformMatrix(ref speedometer), 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
            directX2DGraphics.DrawBitmap(arrowSpeed.index, GetTransformMatrix(ref arrowSpeed), 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
            directX2DGraphics.DrawBitmap(lapIcon.index, GetTransformMatrix(ref lapIcon, 50, 0), 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
            directX2DGraphics.DrawBitmap(numbers[lapIndex].index, GetTransformMatrix(ref numbers[lapIndex], 50 + lapIcon.bitmap.Size.Width, 0),
                                          1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);

            X = lapIcon.bitmap.Size.Width + numbers[lapIndex].bitmap.Size.Width;
            directX2DGraphics.DrawBitmap(slash.index, GetTransformMatrix(ref slash, X, 0), 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
            
            directX2DGraphics.DrawBitmap(numbers[lapCount].index, GetTransformMatrix(ref numbers[lapCount], X + 60, 0),
                                          1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);

            Y = lapIcon.bitmap.Size.Height;
            directX2DGraphics.DrawBitmap(healthBar.index, GetTransformMatrix(ref healthBar, 50, Y),
                                          1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
            directX2DGraphics.DrawBitmap(barFrame.index, GetTransformMatrix(ref barFrame, 50-6, Y - 6),
                                          1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);

            directX2DGraphics.DrawBitmap(ammoIcon.index, GetTransformMatrix(ref ammoIcon, 50, Y + 20),
                                          1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);


            directX2DGraphics.DrawBitmap(placeIcon.index, GetTransformMatrix(ref placeIcon, (Right - placeIcon.bitmap.Size.Width) / 2,
                                                                                            (Bottom - placeIcon.bitmap.Size.Height) / 2), 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);

            directX2DGraphics.DrawBitmap(placeNumber.index, GetTransformMatrix(ref placeNumber, 400, 50),
                                          1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
        }
    }
}
