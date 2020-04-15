using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Resources;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.Windows;
using Template.Properties; // For work with resources of project Assembly.Properties. Then we can access to all inside Resorces.resx.
using Template.Graphics;
using System.IO;

namespace Template
{
    class Game : IDisposable
    {
        // TODO: Realize game state logic.
        ///// <summary>Game states.</summary>
        ///// <remarks>
        ///// <list type="bullet">
        ///// <listheader>Correct state transitions:</listheader>
        ///// <item>BeforeStart -> Play,</item>
        ///// <item>BeforeStart -> Exit,</item>
        ///// <item>Play <-> Pause,</item>
        ///// <item>Play -> Finish,</item>
        ///// <item>Play -> Die,</item>
        ///// <item>Finish -> BeforeStart,</item>
        ///// <item>Die -> BeforeStart,</item>
        ///// <item>BeforeStart -> Exit,</item>
        ///// <item>Pause -> Exit,</item>
        ///// <item>Finish -> Exit,</item>
        ///// <item>Die -> Exit</item>
        ///// </list>
        ///// </remarks>
        //public enum GameState
        //{
        //    BeforeStart,
        //    Play,
        //    Pause,
        //    Finish,
        //    Die,
        //    Exit
        //}

        ///// <summary>Game states.</summary>
        //private GameState _gameState;
        ///// <summary>Game states.</summary>
        //public GameState State { get => _gameState; }

        // TODO: HUD to separate class.
        public struct HUDResources
        {
            public int textFPSTextFormatIndex;
            public int textFPSBrushIndex;
            public int armorIconIndex;
        }

        /// <summary>Main form of application.</summary>
        private RenderForm _renderForm;

        /// <summary>Flag if render form resized by user.</summary>
        //private bool _renderFormUserResized;

        /// <summary>DirectX 3D graphics objects.</summary>
        private DirectX3DGraphics _directX3DGraphics;

        /// <summary>Renderer.</summary>
        private Renderer _renderer;

        /// <summary>DirectX 2D graphic object.</summary>
        private DirectX2DGraphics _directX2DGraphics;

        private SamplerStates _samplerStates;
        private Textures _textures;
        private Materials _materials;
        private Illumination _illumination;
        

        private MeshObject _cube;
        private MeshObject line1, line2;
        
        /// <summary>List of objects with meshes.</summary>
        private MeshObjects _meshObjects;

        /// <summary>HUD resources.</summary>
        private HUDResources _HUDResources;

        /// <summary>Flag for display help.</summary>
        private bool _displayHelp;
        private string _helpString;

        /// <summary>Character.</summary>
        private Character _character;
        private Car car;
        private EnemyCar enemy;
        private Box box1, box2;
        private GameField gameField;

        /// <summary> Ogbjects animations </summary>
        Animations anims;

        /// <summary>Camera object.</summary>
        private Camera _camera;

        /// <summary>Projection matrix.</summary>
        private Matrix _projectionMatrix;

        /// <summary>View matrix.</summary>
        private Matrix _viewMatrix;

        /// <summary>Camera angular ratation step for moving mouse by 1 pixel.</summary>
        private float _angularCameraRotationStep;

        /// <summary>Input controller.</summary>
        private InputController _inputController;

        /// <summary>Time helper object for current time and delta time measurements.</summary>
        private TimeHelper _timeHelper;

        private Random _random;

        /// <summary>First run flag for create DirectX buffers before render in first time.</summary>
        private bool _firstRun = true;

        /// <summary>Init HUD resources.</summary>
        /// <remarks>Create text format, text brush and armor icon.</remarks>
        private void InitHUDResources()
        {
            _HUDResources.textFPSTextFormatIndex = _directX2DGraphics.NewTextFormat("Input", SharpDX.DirectWrite.FontWeight.Normal,
                SharpDX.DirectWrite.FontStyle.Normal, SharpDX.DirectWrite.FontStretch.Normal, 12,
                SharpDX.DirectWrite.TextAlignment.Leading, SharpDX.DirectWrite.ParagraphAlignment.Near);
            _HUDResources.textFPSBrushIndex = _directX2DGraphics.NewSolidColorBrush(new SharpDX.Mathematics.Interop.RawColor4(1.0f, 1.0f, 0.0f, 1.0f)); // Желтый цвет
            _directX2DGraphics.NewSolidColorBrush(new SharpDX.Mathematics.Interop.RawColor4(1.0f, 0.0f, 0.0f, 1.0f)); // Красный цвет
            _directX2DGraphics.NewSolidColorBrush(new SharpDX.Mathematics.Interop.RawColor4(0.0f, 0.0f, 0.0f, 1.0f)); // Черный цвет
            _HUDResources.armorIconIndex = _directX2DGraphics.LoadBitmapFromFile("Resources\\armor.bmp");  // Don't use before Resizing. Bitmaps loaded, but not created.
        }

        /// <summary>
        /// Constructor. Initialize all objects.
        /// </summary>
        public Game()
        {

            //_gameState = GameState.BeforeStart;
            _helpString = Resources.HelpString;

            // Initialization order:
            // 1. Render form.
            _renderForm = new RenderForm("SharpDX");
            _renderForm.UserResized += RenderFormResizedCallback;
            _renderForm.Activated += RenderFormActivatedCallback;
            _renderForm.Deactivate += RenderFormDeactivateCallback;
            // Input controller and time helper.
            _inputController = new InputController(_renderForm);
            _timeHelper = new TimeHelper();
            _random = new Random();
            // 2. DirectX 3D.
            _directX3DGraphics = new DirectX3DGraphics(_renderForm) { RenderMode = DirectX3DGraphics.RenderModes.Wireframe };
            // 3. Renderer.
            _renderer = new Renderer(_directX3DGraphics);
            _renderer.CreateConstantBuffers();
            // 4. DirectX 2D.
            _directX2DGraphics = new DirectX2DGraphics(_directX3DGraphics);
            // 5. Load materials
            Loader loader = new Loader(_directX3DGraphics, _directX2DGraphics, _renderer, _directX2DGraphics.ImagingFactory);
            _samplerStates = new SamplerStates(_directX3DGraphics);
            _textures = new Textures();
            _textures.Add(loader.LoadTextureFromFile("Resources\\white.bmp", false, _samplerStates.Colored));
            _renderer.SetWhiteTexture(_textures["white.bmp"]);
            _textures.Add(loader.LoadTextureFromFile("Resources\\cube.png", true, _samplerStates.Textured));
            _textures.Add(loader.LoadTextureFromFile("Resources\\red.bmp", true, _samplerStates.Textured));
            _textures.Add(loader.LoadTextureFromFile("Resources\\green.bmp", true, _samplerStates.Textured));
            _textures.Add(loader.LoadTextureFromFile("Resources\\blue.bmp", true, _samplerStates.Textured));

            _textures.Add(loader.LoadTextureFromFile("Resources\\road.png", true, _samplerStates.Textured));
            SamplerStateDescription samplerStateDescription = new SamplerStateDescription
            {
                Filter = Filter.MinMagMipPoint,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                MipLodBias = 0.0f,
                MaximumAnisotropy = 1,
                ComparisonFunction = Comparison.Never,
                BorderColor = new SharpDX.Mathematics.Interop.RawColor4(1.0f, 1.0f, 1.0f, 1.0f),
                MinimumLod = 0,
                MaximumLod = float.MaxValue
            };
            _textures[_textures.Count - 1].SamplerState = new SamplerState(_directX3DGraphics.Device, samplerStateDescription);

            _textures.Add(loader.LoadTextureFromFile("Resources\\delorean.png", true, _samplerStates.Textured));
            _materials = loader.LoadMaterials("Resources\\materials.txt", _textures);

            gameField = new GameField(_timeHelper, _materials[2]);

            // 6. Load meshes.
            _meshObjects = new MeshObjects();
            _cube = loader.LoadMeshObject("Resources\\cube.txt", _materials);
            line1 = loader.LoadMeshFromObject("Resources\\line.obj", _materials[1]);

            var mbox1 = loader.LoadMeshObject("Resources\\box.txt", _materials);
            var mbox2 = loader.LoadMeshObject("Resources\\box.txt", _materials);
            line2 = loader.LoadMeshFromObject("Resources\\line.obj", _materials[3]);
            var plane = loader.LoadMeshFromObject("Resources\\plane.obj", _materials[0]);
            var road = loader.LoadMeshFromObject("Resources\\road.obj", _materials[7]);
            var cube2 = loader.LoadMeshFromObject("Resources\\box.obj", _materials[0]);
            // Чекпоинты
            var checkPointMeshes = loader.LoadMeshesFromObject("Resources\\checkPoints.obj", _materials[1]);
            gameField.SetCheckPoints(checkPointMeshes);
            // Машина игрока
            var carMeshes = loader.LoadMeshesFromObject("Resources\\enemyCar.obj", _materials[2]);
            car = new Car(carMeshes);
            car.MoveBy(-9.0f, 0.0f, 5.0f);
            gameField.SetCar(car);
            // Машина соперника
            var enemyMeshes = loader.LoadMeshesFromObject("Resources\\delorean.obj", _materials[5]);
            enemy = new EnemyCar(enemyMeshes);
            enemy.MoveBy(-15.0f, 0.0f, 5.0f);
            gameField.SetEnemy(enemy);

            // Кубы для тестов
            box1 = new Box(mbox1);
            box1.MoveBy(0.0f, 1.0f, 0.0f); mbox1.Material = _materials[1];
            box2 = new Box(mbox2);
            box2.MoveBy(0.0f, 1.0f, 5.0f);

            // Перемещения
            _cube.MoveBy(0.0f, 9.0f, 0.0f);
            plane.MoveBy(-45.0f, 0.0f, 5.0f);
            cube2.MoveBy(4.0f, 0.0f, 12.0f);
            line2.MoveTo(0, 0, 0);
            line1.MoveTo(carMeshes[5].CenterPosition);

            

            anims = new Animations();

            // Добавление мешей
            _meshObjects.Add(mbox1);
            _meshObjects.Add(mbox2);
            _meshObjects.Add(_cube);
            _meshObjects.Add(cube2);
            _meshObjects.Add(line1);
            _meshObjects.Add(line2);
            _meshObjects.Add(plane);
            _meshObjects.Add(road);
            enemyMeshes.ForEach(m => _meshObjects.Add(m));
            checkPointMeshes.ForEach(m => _meshObjects.Add(m));
            car.AddToMeshes(_meshObjects);
            // 6. Load HUD resources into DirectX 2D object.
            InitHUDResources();

            loader = null;

            _illumination = new Illumination(Vector4.Zero, new Vector4(1.0f, 1.0f, 0.9f, 1.0f), new LightSource[]
            {
                new LightSource(LightSource.LightType.DirectionalLight,
                    new Vector4(0.0f, 20.0f, 0.0f, 1.0f),   // Position
                    new Vector4(0.0f, -1.0f, 0.0f, 1.0f),   // Direction
                    new Vector4(1.0f, 0.9f, 0.8f, 1.0f),    // Color
                    0.0f,                                   // Spot angle
                    1.0f,                                   // Const atten
                    0.0f,                                   // Linear atten
                    0.0f,                                   // Quadratic atten
                    1),
                new LightSource(LightSource.LightType.SpotLight,
                    new Vector4(0.0f, 8.0f, 0.0f, 1.0f),
                    new Vector4(0.0f, -1.0f, 0.0f, 1.0f),
                    new Vector4(0.7f, 0.7f, 1.0f, 1.0f),
                    Game3DObject._PI2 / 4.0f,
                    1.0f,
                    0.02f,
                    0.005f,
                    1),
                new LightSource(LightSource.LightType.PointLight,
                    new Vector4(-4.0f, 2.0f, 0.0f, 1.0f),
                    Vector4.Zero,
                    new Vector4(1.0f, 1.0f, 1.0f, 1.0f),
                    0.0f,
                    0.0f, //0.8f,
                    0.02f, //0.0002f,
                    0.005f,
                    1),
                new LightSource(),
                new LightSource(),
                new LightSource(),
                new LightSource(),
                new LightSource()
            });

            // Character and camera. X0Z - ground, 0Y - to up.
            _character = new Character(new Vector4(0.0f, 6.0f, -12.0f, 1.0f), Game3DObject._PI, 0.0f, 0.0f); //********
            _camera = new Camera(new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            _camera.AttachToObject(_character);
            
        }

        /// <summary>Render form activated callback. Hide cursor.</summary>
        /// <param name="sender">Sender of event.</param>
        /// <param name="args">Arguments.</param>
        public void RenderFormActivatedCallback(object sender, EventArgs args)
        {
            Cursor.Hide();
        }

        /// <summary>Render form deactivate event callback. Show cursor.</summary>
        /// <param name="sender">Sender of event.</param>
        /// <param name="args">Arguments.</param>
        public void RenderFormDeactivateCallback(object sender, EventArgs args)
        {
            Cursor.Show();
        }

        /// <summary>Render form user resized callback. Perform resizing of DirectX 3D object and renew camera rotation step and projection matrix.</summary>
        /// <param name="sender">Sender of event.</param>
        /// <param name="args">Arguments.</param>
        public void RenderFormResizedCallback(object sender, EventArgs args)
        {
            _directX3DGraphics.Resize();
            _camera.Aspect = _renderForm.ClientSize.Width / (float)_renderForm.ClientSize.Height;
            _angularCameraRotationStep = _camera.FOVY / _renderForm.ClientSize.Height;
            _projectionMatrix = _camera.GetProjectionMatrix();
        }

        float ANGLE;
        float alpha;
        string screen = "";
        int ct = 0;
        int idx = 0;
        float increm = 1;
        float min, max;
        //FileStream fs;

        Vector3 before;
        //ContainmentType collied;

        /// <summary>Callback for RenderLoop.Run. Handle input and render scene.</summary>
        public void RenderLoopCallback()
        {

            _timeHelper.Update();
            _inputController.UpdateKeyboardState();
            _inputController.UpdateMouseState();
            if (_inputController.MouseUpdated) // TODO: Move handle input to separate thread.
            {
                _character.PitchBy(-_inputController.MouseRelativePositionY * _angularCameraRotationStep); // вверх вниз
                _character.YawBy(_inputController.MouseRelativePositionX * _angularCameraRotationStep); // вправо влево
            }

            alpha = 2.0f * (float)Math.PI * 0.25f * _timeHelper.DeltaT;

            if (_firstRun)
            {
                _firstRun = false; alpha = 0;
                RenderFormResizedCallback(this, new EventArgs());
                //fs = new FileStream("D:/difs.txt", FileMode.OpenOrCreate);

                _character.Position = new Vector3(-3.0f, 6.0f, -3.0f);
                //_character.Yaw = -2.3600119f;
                //_character.Pitch = -0.7657637f;
            }

            if (_inputController.KeyboardUpdated)
            {
                ANGLE = 0;
                
                if (_inputController.WPressed)
                {
                    _character.MoveForwardBy(_timeHelper.DeltaT * _character.Speed);
                }
                if (_inputController.SPressed)
                {
                    _character.MoveForwardBy(-_timeHelper.DeltaT * _character.Speed);
                }
                if (_inputController.DPressed) _character.MoveRightBy(_timeHelper.DeltaT * _character.Speed);
                if (_inputController.APressed) _character.MoveRightBy(-_timeHelper.DeltaT * _character.Speed);

                before = car.Position;

                if (_inputController.UpPressed)
                {
                    if (!car.IsCollied)
                        car.Accelerate(); //car.Speed = 6;
                }
                else if (_inputController.DownPressed)
                {
                    if (!car.IsCollied)
                        car.Brake(); //car.Speed = -6;
                }
                else
                    car.MoveInertia(); //car.Speed = 0;
                car.MoveProperly();


                _character.Position += car.Position - before;

                if (_inputController.LeftPressed)
                {
                    anims.IsWheelsAnimate = false;
                    car.TurnWheelsLeft(alpha);
                }
                if (_inputController.RightPressed)
                {
                    anims.IsWheelsAnimate = false;
                    car.TurnWheelsRight(alpha);
                }
                
                if (_inputController.Space)
                {
                    //anims.IsEnemyTurned = false;

                    min = (gameField.checkPoints[idx].Direction * gameField.checkPoints[idx].boundingBox.Extents.X).X;
                    max = (-gameField.checkPoints[idx].Direction * gameField.checkPoints[idx].boundingBox.Extents.X).X;
                    increm = MyMath.Random(min, max);

                    idx++;
                    if (idx == 9) idx = 0;

                    //var verts = gameField.checkPoints[5].boundingBox.GetCorners();
                    //switch (ct)
                    //{
                    //    case 0:
                    //        line2.MoveTo(verts[0]);
                    //        break;
                    //    case 1:
                    //        line2.MoveTo(verts[1]);
                    //        break;
                    //    case 2:
                    //        line2.MoveTo(verts[2]);
                    //        break;
                    //    case 3:
                    //        line2.MoveTo(verts[3]);
                    //        break;
                    //    case 4:
                    //        line2.MoveTo(verts[4]);
                    //        break;
                    //    case 5:
                    //        line2.MoveTo(verts[5]);
                    //        break;
                    //    case 6:
                    //        line2.MoveTo(verts[6]);
                    //        break;
                    //    case 7:
                    //        line2.MoveTo(verts[7]);
                    //        break;
                    //}
                    //ct++;
                }


                // Вражеская машина
                //enemy.CheckObstacle(gameField.checkPoints[1].boundingBox, alpha);
                //enemy.Move();
                line1.MoveTo(enemy.RearAxle);
                line2.MoveTo(enemy.RearAxle + enemy.CarDirection);

                // АНИМАЦИЯ ----- АНИМАЦИЯ ----- АНИМАЦИЯ ----- АНИМАЦИЯ ----- АНИМАЦИЯ ----- АНИМАЦИЯ ----- АНИМАЦИЯ ----- АНИМАЦИЯ ----- АНИМАЦИЯ
                // Поворот врага вдоль указанного направления
                //if (!_inputController.Space && !anims.IsEnemyTurned)
                //{
                //    anims.AnimateEnemyToTarget(enemy, alpha);
                //}

                // Анимация возврата колес
                //if((!_inputController.RightPressed && !_inputController.LeftPressed) && car.IsWheelsTirned)
                //{
                //    anims.IsWheelsAnimate = true;
                //} anims.AnimateWheels(car, alpha);
                // --------------------------------------------------------------------------------------------------------------------------------


                // Игровое поле
                gameField.MoveEnemyToTargets();



                if (_inputController.Num1Pressed)
                {
                    
                }
                if (_inputController.Num2Pressed)
                {
                    
                }


                if (_inputController.Num4Pressed)
                {
                    //box1.RotateY(-alpha);
                    enemy.TurnCar(-alpha);
                }
                if (_inputController.Num6Pressed)
                {
                    //box1.RotateY(alpha);
                    enemy.TurnCar(alpha);
                }
                if (_inputController.Num8Pressed)
                {
                    //box1.MoveForward();
                    enemy.Speed = 9;
                }
                else if (_inputController.Num5Pressed)
                {
                    //box1.MoveBackward();
                    enemy.Speed = -9;
                }
                else
                    //box1.moveSign = 0;


                if (_inputController.Num7Pressed)
                {
                    //var p = box1.Position;
                    //p.X -= alpha;
                    //box1.Position = p;
                }
                if (_inputController.Num9Pressed)
                {
                    //var p = box1.Position;
                    //p.X += alpha;
                    //box1.Position = p;                    
                }
                

                if (_inputController.Esc) { _renderForm.Close(); }                               // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                // Toggle help by F1.
                if (_inputController.Func[0]) _displayHelp = !_displayHelp;
                // Switch solid and wireframe modes by F2, F3.
                if (_inputController.Func[1]) _directX3DGraphics.RenderMode = DirectX3DGraphics.RenderModes.Solid;
                if (_inputController.Func[2]) _directX3DGraphics.RenderMode = DirectX3DGraphics.RenderModes.Wireframe;
                // Toggle fullscreen mode by F4, F5.
                if (_inputController.Func[3]) _directX3DGraphics.IsFullScreen = false;
                if (_inputController.Func[4]) _directX3DGraphics.IsFullScreen = true;
            }


            // Collision Detection
            {
                //car.CollisionTest(box2);
                //gameField.CheckRaceFinish();

                screen = (car.IsCollied) ? "True" : "False";
                
            }


            _viewMatrix = _camera.GetViewMatrix();

            _renderer.BeginRender();

            _illumination.EyePosition = (Vector4)_camera.Position;
            LightSource light2 = _illumination[2];
            if (RandomUtil.NextFloat(_random, 0.0f, 1.0f) < 0.2f) light2.Enabled = (1 ==light2.Enabled ? 0 : 1);
            _illumination[2] = light2;
            _renderer.UpdateIlluminationProperties(_illumination);

            _renderer.SetPerObjectConstants(_timeHelper.Time, 0);
            //float angle = _timeHelper.Time * 2.0f * (float)Math.PI * 0.25f; // Frequency = 0.25 Hz
            //_cube.Pitch = angle;

            //Matrix worldMatrix;

            // Render 3D objects.
            for (int i = 0; i <= _meshObjects.Count - 1; i++)
            {
                if (i > 0)
                {
                    _renderer.SetPerObjectConstants(_timeHelper.Time, 0);
                }
                MeshObject meshObject = _meshObjects[i];
                meshObject.Render(_viewMatrix, _projectionMatrix);
            }
            //cube2
            float time = _timeHelper.Time;
            _cube.Render(_viewMatrix, _projectionMatrix);

            RenderHUD();

            _renderer.EndRender();
            
        }

        /// <summary>Render HUD.</summary>
        private void RenderHUD()
        {
            string text = $"FPS: {_timeHelper.FPS,3:d2}\ntime: {_timeHelper.Time:f1}\n" +
                                $"MX: {_inputController.MouseRelativePositionX,3:d2} MY: {_inputController.MouseRelativePositionY,3:d2} MZ: {_inputController.MouseRelativePositionZ,4:d3}\n" +
                                $"LB: {(_inputController.MouseButtons[0] ? 1 : 0)} MB: {(_inputController.MouseButtons[2] ? 1 : 0)} RB: {(_inputController.MouseButtons[1] ? 1 : 0)}\n" +
                                $"Pos: {_character.Position.X,6:f1}, {_character.Position.Y,6:f1}, {_character.Position.Z,6:f1}\n" +
                                $"Yaw: {_character.Yaw}, Pitch: {_character.Pitch}, Roll: {_character.Roll}\n\n" +

                                $"car itrs: {car.itrs} \n" +
                                $"car Position X: {car.Position.X} Y:{car.Position.Y} Z:{car.Position.Z}\n" +
                                $"car Speed: {car.Speed} ups\n\n" +
                                
                                $"Distance to obj: {enemy.Distance}\n" +
                                $"enemy Position   X: {enemy.Position.X} Y:{enemy.Position.Y} Z:{enemy.Position.Z}\n" +
                                $"enemy Direction   X: {enemy.Direction.X} Y:{enemy.Direction.Y} Z:{enemy.Direction.Z}\n" +
                                $"enemy Speed: {enemy.Speed} ups\n" +
                                $"enemy Acceleration: {enemy.Acceleration} ups\n" +
                                $"Data: {screen}\n";

            if (_displayHelp) text += "\n\n" + _helpString;
            float armorWidthInDIP = _directX2DGraphics.Bitmaps[_HUDResources.armorIconIndex].Size.Width;
            float armorHeightInDIP = _directX2DGraphics.Bitmaps[_HUDResources.armorIconIndex].Size.Height;
            Matrix3x2 armorTransformMatrix = Matrix3x2.Translation(new Vector2(_directX2DGraphics.RenderTargetClientRectangle.Right - armorWidthInDIP - 1, 1));
            _directX2DGraphics.BeginDraw();
            _directX2DGraphics.DrawText(text, _HUDResources.textFPSTextFormatIndex,
                _directX2DGraphics.RenderTargetClientRectangle, _HUDResources.textFPSBrushIndex + 1); // 0 - Желтый, 1 - Красный, 2 - Черный
            _directX2DGraphics.DrawBitmap(_HUDResources.armorIconIndex, armorTransformMatrix, 1.0f, SharpDX.Direct2D1.BitmapInterpolationMode.Linear);
            _directX2DGraphics.EndDraw();
        }

        /// <summary>Rum main render loop.</summary>
        public void Run()
        {
            RenderLoop.Run(_renderForm, RenderLoopCallback);
        }

        /// <summary>Realise all resources</summary>
        public void Dispose()
        {
            // MeshObjects disposing
            _textures.Dispose();
            _samplerStates.Dispose();
            _inputController.Dispose();
            _directX2DGraphics.Dispose();
            _renderer.Dispose();
            _directX3DGraphics.Dispose();
            _renderForm.Dispose();
        }
    }
}
