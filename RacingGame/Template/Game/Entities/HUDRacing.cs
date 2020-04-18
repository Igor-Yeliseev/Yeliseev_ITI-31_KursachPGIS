using SharpDX;
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
        struct Icon
        {
            public Bitmap bitmap;
            public int index;
            public Matrix3x2 matrix;

        }

        float Left, Right, Bottom, Top;

        DirectX2DGraphics directX2DGraphics;
        InputController inputController;

        Icon speedometer;
        
        Icon arrowSpeed;
        float angle = 3.1415926535897931f / 180;
        float oldAngle = 0;
        float maxAngle = (float)Math.PI;

        Icon lapsIcon;
        Icon[] numbers;
        int numbIndex = 0;

        private Car car;
        public Car Car
        {
            get { return car; }
            set
            {
                car = value;
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
                numbIndex++;
                if (numbIndex > 9) numbIndex = 0;
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
                else if(angle > maxAngle / 4.0)
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

            float Height = Bottom - arrowSpeed.bitmap.Size.Height;
            arrowSpeed.matrix *= Matrix3x2.Translation(new Vector2(-arrowSpeed.bitmap.Size.Width / 2, -Height - arrowSpeed.bitmap.Size.Height / 2));
            arrowSpeed.matrix *= Matrix3x2.Rotation(angle - oldAngle);
            arrowSpeed.matrix *= Matrix3x2.Translation(new Vector2(arrowSpeed.bitmap.Size.Width / 2, Height + arrowSpeed.bitmap.Size.Height / 2));

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
            lapsIcon.index = directX2DGraphics.LoadBitmapFromFile("Resources\\HUD\\Laps.png");

            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i].index = directX2DGraphics.LoadBitmapFromFile("Resources\\HUD\\number" + i + ".png");
                numbers[i].matrix = Matrix3x2.Identity;
            }

            //number.index = directX2DGraphics.LoadBitmapFromFile("Resources\\HUD\\number.png");

            speedometer.matrix = arrowSpeed.matrix = lapsIcon.matrix = Matrix3x2.Identity;
        }

        public void GetBitmaps()
        {
            speedometer.bitmap = directX2DGraphics.Bitmaps[speedometer.index];
            arrowSpeed.bitmap = directX2DGraphics.Bitmaps[arrowSpeed.index];
            lapsIcon.bitmap = directX2DGraphics.Bitmaps[lapsIcon.index];

            for (int i = 0; i < numbers.Length; i++)
            {
                numbers[i].bitmap = directX2DGraphics.Bitmaps[numbers[i].index];
            }

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

        public void DrawBitmaps()
        {
            directX2DGraphics.DrawBitmap(speedometer.index, GetTransformMatrix(ref speedometer), 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
            directX2DGraphics.DrawBitmap(arrowSpeed.index, GetTransformMatrix(ref arrowSpeed), 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
            directX2DGraphics.DrawBitmap(lapsIcon.index, GetTransformMatrix(ref lapsIcon), 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
            directX2DGraphics.DrawBitmap(numbers[numbIndex].index, GetTransformMatrix(ref numbers[numbIndex], 500, Bottom - numbers[numbIndex].bitmap.Size.Height), 1.0f, 
                                         SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
        }

    }
}
