﻿using System;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.Windows;
using Template.Properties; // For work with resources of project Assembly.Properties. Then we can access to all inside Resorces.resx.
using Template.Graphics;
using Template.Entities.Abstract_Factory;

namespace Template
{
    class Game : IDisposable
    {
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
        private HUDRacing hudRacing;

        private SamplerStates _samplerStates;
        private Textures _textures;
        private Materials _materials;
        private Illumination _illumination;
        
        
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
        private Enemy enemy1, enemy2, enemy3;
        private GameField gameField;

        private Sounds sounds;
        

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
            _helpString = Resources.HelpString;

            // Initialization order:
            // 1. Render form.
            _renderForm = new RenderForm("SharpDX");
            _renderForm.ClientSize = new System.Drawing.Size(1600, 900);
            _renderForm.Text = "Автомобильные гонки";
            _renderForm.UserResized += RenderFormResizedCallback;
            _renderForm.Activated += RenderFormActivatedCallback;
            _renderForm.Deactivate += RenderFormDeactivateCallback;
            _renderForm.Closing += RenderForm_Closing;
            // Input controller and time helper.
            _inputController = new InputController(_renderForm);
            _timeHelper = new TimeHelper();
            _random = new Random();
            // 2. DirectX 3D.
            _directX3DGraphics = new DirectX3DGraphics(_renderForm) { RenderMode = DirectX3DGraphics.RenderModes.Solid};
            // 3. Renderer.
            _renderer = new Renderer(_directX3DGraphics);
            _renderer.CreateConstantBuffers();
            // 4. DirectX 2D.
            _directX2DGraphics = new DirectX2DGraphics(_directX3DGraphics);
            // Мой худ
            hudRacing = new HUDRacing(_directX2DGraphics, _inputController, _timeHelper);
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
            _textures.Add(loader.LoadTextureFromFile("Resources\\orange.bmp", true, _samplerStates.Textured));
            _textures.Add(loader.LoadTextureFromFile("Resources\\black.bmp", true, _samplerStates.Textured));
            _textures.Add(loader.LoadTextureFromFile("Resources\\trees.jpg", true, _samplerStates.Textured));
            _textures.Add(loader.LoadTextureFromFile("Resources\\startpoles.png", true, _samplerStates.Textured));
            _textures.Add(loader.LoadTextureFromFile("Resources\\road.png", true, _samplerStates.Textured));
            _textures.Add(loader.LoadTextureFromFile("Resources\\stadium.png", true, _samplerStates.Textured));
            _textures.Add(loader.LoadTextureFromFile("Resources\\bonus-nitro.png", true, _samplerStates.Textured));
            _textures.Add(loader.LoadTextureFromFile("Resources\\bonus-health.png", true, _samplerStates.Textured));
            _textures.Add(loader.LoadTextureFromFile("Resources\\bonus-tire.png", true, _samplerStates.Textured));

            _textures.Add(loader.LoadTextureFromFile("Resources\\grass.jpg", true, _samplerStates.Textured));
            SamplerStateDescription samplerStateDescription = new SamplerStateDescription
            {
                Filter = Filter.MinMagMipPoint,
                AddressU = TextureAddressMode.Mirror,
                AddressV = TextureAddressMode.Mirror,
                AddressW = TextureAddressMode.Mirror,
                MipLodBias = 0.0f,
                MaximumAnisotropy = 1,
                ComparisonFunction = Comparison.Never,
                BorderColor = new SharpDX.Mathematics.Interop.RawColor4(1.0f, 1.0f, 1.0f, 1.0f),
                MinimumLod = 0,
                MaximumLod = float.MaxValue
            };
            _textures[_textures.Count - 1].SamplerState = new SamplerState(_directX3DGraphics.Device, samplerStateDescription);

            _textures.Add(loader.LoadTextureFromFile("Resources\\delorean.png", true, _samplerStates.Textured));
            _textures.Add(loader.LoadTextureFromFile("Resources\\mustang.png", true, _samplerStates.Textured));
            _textures.Add(loader.LoadTextureFromFile("Resources\\corvette.png", true, _samplerStates.Textured));
            _textures.Add(loader.LoadTextureFromFile("Resources\\camaro.png", true, _samplerStates.Textured));
            _textures.Add(loader.LoadTextureFromFile("Resources\\startline.jpg", true, _samplerStates.Textured));
            _materials = loader.LoadMaterials("Resources\\materials.txt", _textures);

            // Load game sounds
            InitSounds();
            
            gameField = new GameField(_timeHelper, hudRacing, sounds);

            // 6. Load meshes.
            _meshObjects = new MeshObjects();

            var mustang = loader.LoadMeshesFromObject("Resources\\mustang.obj", _materials[6]);
            var corvette = loader.LoadMeshesFromObject("Resources\\corvette.obj", _materials[7]);
            var camaro = loader.LoadMeshesFromObject("Resources\\camaro.obj", _materials[9]);
            var road = loader.LoadMeshFromObject("Resources\\road.obj", _materials[8]);
            var startline = loader.LoadMeshFromObject("Resources\\startline.obj", _materials[10]);
            // Чекпоинты
            var checkPointMeshes = loader.LoadMeshesFromObject("Resources\\checkPoints.obj", _materials[1]);
            gameField.SetCheckPoints(checkPointMeshes);
            // Столбы
            var pillars = loader.LoadMeshesFromObject("Resources\\pillars.obj", _materials[12]);
            gameField.SetPillars(pillars);
            gameField.Hedra = loader.LoadMeshesFromObject("Resources\\prism.obj", _materials[2]).First();
            // Столбы с фишином
            var startPoles = loader.LoadMeshesFromObject("Resources\\startpoles.obj", _materials[13]);
            // Деревья
            var trees = loader.LoadMeshesFromObject("Resources\\trees.obj", _materials[11]);
            // Трава
            var grass = loader.LoadMeshesFromObject("Resources\\grass.obj", _materials[14]);
            // Стадион
            var staduim = loader.LoadMeshesFromObject("Resources\\stadium.obj", _materials[15]);
            var stadiumPrefab = new StaticPrefab(staduim);
            gameField.AddPrefab(stadiumPrefab);
            // Бонусы
            var bonusNitro = loader.LoadMeshFromObject("Resources\\bonus.obj",  _materials[16]);
            var bonusHealth = loader.LoadMeshFromObject("Resources\\bonus.obj", _materials[17]);
            var bonusSpike = loader.LoadMeshFromObject("Resources\\spikes.obj", _materials[18]);
            var bonusTire = loader.LoadMeshFromObject("Resources\\bonus.obj",   _materials[19]);
            //bonusNitro.MoveBy(new Vector3(0, 0, 65));
            //bonusSpike.MoveBy(new Vector3(-25, 0, 45));
            gameField.AddPriseMesh(bonusSpike, SurpriseType.Damage);
            gameField.AddPriseMesh(bonusNitro, SurpriseType.Speed);
            gameField.AddPriseMesh(bonusHealth, SurpriseType.Health);
            gameField.AddPriseMesh(bonusTire, SurpriseType.Tire);

            // Машина игрока
            car = new Car(mustang);
            gameField.SetPlayer(car);
            // Машина соперника
            var delorean = loader.LoadMeshesFromObject("Resources\\delorean.obj", _materials[5]);
            enemy1 = new Enemy(delorean);
            enemy2 = new Enemy(camaro);
            enemy3 = new Enemy(corvette);
            gameField.AddEnemy(enemy1);
            gameField.AddEnemy(enemy2);
            gameField.AddEnemy(enemy3);

            // Перемещения
            car.MoveBy(new Vector3(0.0f, 0.0f, -50.0f));
            enemy1.MoveBy(new Vector3(9.0f, 0.0f, -29.0f));
            enemy2.MoveBy(new Vector3(-7.0f, 0.0f, -30.0f));
            enemy3.MoveBy(new Vector3(1.0f, 0.0f, -30.0f));
            //line2.MoveTo(new Vector3(0, 0, 0));
            //line1.MoveTo(camaro.First().CenterPosition);


            // Добавление мешей
            _meshObjects.Add(startline);
            _meshObjects.Add(road);

            mustang.ForEach(m => _meshObjects.Add(m));
            corvette.ForEach(m => _meshObjects.Add(m));
            camaro.ForEach(m => _meshObjects.Add(m));
            delorean.ForEach(m => _meshObjects.Add(m));
            trees.ForEach(m => _meshObjects.Add(m));
            grass.ForEach(m => _meshObjects.Add(m));
            startPoles.ForEach(m => _meshObjects.Add(m));
            staduim.ForEach(m => _meshObjects.Add(m));

            // 6. Load HUD resources into DirectX 2D object.
            InitHUDResources();
            hudRacing.InitPicsIndicies();

            loader = null;

            _illumination = new Illumination(Vector4.Zero, new Vector4(1.0f, 1.0f, 1.0f, 1.0f), new LightSource[]
            {
                new LightSource(LightSource.LightType.DirectionalLight,
                    new Vector4(0.0f, 20.0f, 0.0f, 1.0f),   // Position
                    new Vector4(0.0f, -1.0f, 0.0f, 1.0f),   // Direction
                    new Vector4(1.0f, 1.0f, 1.0f, 1.0f),    // Color
                    0.0f,                                   // Spot angle
                    1.0f,                                   // Const atten
                    0.0f,                                   // Linear atten
                    0.0f,                                   // Quadratic atten
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

        private void RenderForm_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            sounds.Dispose();
            threadBonuses.Abort();
        }
        
        /// <summary> Инициализация и загрузка всех звуков в игру </summary>
        private void InitSounds()
        {
            sounds = new Sounds(_inputController);
            //sounds.Load("Resources\\Sounds\\idle.wav");
            sounds.Load("Resources\\Sounds\\melodyloop.wav");
            sounds.Load("Resources\\Sounds\\pickup.wav");
            sounds.Load("Resources\\Sounds\\crash.wav");
            sounds.Load("Resources\\Sounds\\bonus-nitro.wav");
            sounds.Load("Resources\\Sounds\\bonus-health.wav");
            sounds.Load("Resources\\Sounds\\bonus-tire-puncture.wav");
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
            hudRacing.GetRectSides();
            hudRacing.GetBitmaps();
        }
        
        float deltaTime = 0;
        float alpha;
        string screen = "";
        int cv = 1;

        /// <summary> Поток для бонусов и ловушек </summary>
        Thread threadBonuses = new Thread(new ParameterizedThreadStart(func));
        /// <summary> Функция для потока генерации призов и ловушек </summary>
        /// <param name="gameField"></param>
        static void func(object gameField)
        {
            GameField gf = (GameField)gameField;

            while (true)
            {
                gf.CreateSurprises();
                Thread.Sleep(1000);
            }
        }


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

            alpha = (float)Math.PI * 0.0135f;

            if (_firstRun)
            {
                _firstRun = false;
                deltaTime = _timeHelper.DeltaT / 60;
                gameField.Angle = 2.0f * (float)Math.PI * 0.25f * deltaTime;

                RenderFormResizedCallback(this, new EventArgs());
                hudRacing.Car = car;
                threadBonuses.Start(gameField);
            }

            if (_inputController.KeyboardUpdated)
            {
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

                if (!car.IsDead)
                {

                    if (_inputController.LeftPressed)
                    {
                        car.TurnWheelsLeft(alpha);
                    }
                    if (_inputController.RightPressed)
                    {
                        car.TurnWheelsRight(alpha);
                    }

                    if (_inputController.UpPressed)
                    {
                        car.Accelerate();
                    }
                    else if (_inputController.DownPressed)
                    {
                        car.Brake();
                    }
                    else
                    {
                        car.MoveInertia();
                    }

                    car.MoveProperly();
                }
                
                _character.FollowCar(car);

                sounds.Plays();


                if (_inputController.Space)
                {
                    _character.Position = new Vector3(_character.Position.X, _character.Position.Y + 2, _character.Position.Z);
                    
                    //car.BackWheels();
                }

                // Игровое поле
                gameField.CheckRaceFinish();
                gameField.MoveEnemies();
                gameField.CheckCollisions();
                gameField.CreateSurprises();



                //if (_inputController.Num1Pressed)
                //{
                //}
                //if (_inputController.Num2Pressed)
                //{
                //}


                //if (_inputController.Num4Pressed)
                //{
                //    //box1.RotateY(-alpha);
                //    enemy.TurnWheelsLeft(alpha);
                //}
                //if (_inputController.Num6Pressed)
                //{
                //    //box1.RotateY(alpha);
                //    enemy.TurnWheelsRight(alpha);
                //}
                //if (_inputController.Num8Pressed)
                //{
                //    //box1.MoveForward();
                //    enemy.Accelerate();
                //}
                //else if (_inputController.Num5Pressed)
                //{
                //    //box1.MoveBackward();
                //    enemy.Brake();
                //}
                //else
                //    box1.moveSign = 0;
                //enemy.MoveProperly();



                //if (_inputController.Num7Pressed)
                //{
                //    //var p = box1.Position;
                //    //p.X -= alpha;
                //    //box1.Position = p;
                //}
                //if (_inputController.Num9Pressed)
                //{
                //    //var p = box1.Position;
                //    //p.X += alpha;
                //    //box1.Position = p;
                //}

                hudRacing.Update();

                if (_inputController.Esc)
                {
                    _renderForm.Close();
                }                               // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                // Toggle help by F1.
                if (_inputController.Func[0]) _displayHelp = !_displayHelp;
                // Switch solid and wireframe modes by F2, F3.
                if (_inputController.Func[1]) _directX3DGraphics.RenderMode = DirectX3DGraphics.RenderModes.Solid;
                if (_inputController.Func[2]) _directX3DGraphics.RenderMode = DirectX3DGraphics.RenderModes.Wireframe;
                // Toggle fullscreen mode by F4, F5.
                if (_inputController.Func[3]) _directX3DGraphics.IsFullScreen = false;
                if (_inputController.Func[4])
                    _directX3DGraphics.IsFullScreen = true;
            }

            
            _viewMatrix = _camera.GetViewMatrix();

            _renderer.BeginRender();

            _illumination.EyePosition = (Vector4)_camera.Position;
            LightSource light2 = _illumination[2];
            if (RandomUtil.NextFloat(_random, 0.0f, 1.0f) < 0.2f) light2.Enabled = (1 == light2.Enabled ? 0 : 1);
            _illumination[2] = light2;
            _renderer.UpdateIlluminationProperties(_illumination);

            _renderer.SetPerObjectConstants(_timeHelper.Time, 0);
            //float angle = _timeHelper.Time * 2.0f * (float)Math.PI * 0.25f; // Frequency = 0.25 Hz

            // Render 3D objects.
            for (int i = 0; i <= _meshObjects.Count - 1; i++)
            {
                if (i > 0)
                {
                    _renderer.SetPerObjectConstants(_timeHelper.Time, 0);
                }
                _meshObjects[i].Render(_viewMatrix, _projectionMatrix);
            }

            gameField.Draw(_viewMatrix, _projectionMatrix);

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

                                $"car itrs: {/*car.turnCount*/0} \n" +
                                $"car Position X: {car.Position.X} Y:{car.Position.Y} Z:{car.Position.Z}\n" +
                                $"car Speed: {car.Speed} ups\n\n" +

                                $"enemy Position   X: {enemy1.Position.X} Y:{enemy1.Position.Y} Z:{enemy1.Position.Z}\n" +
                                $"enemy Direction   X: {enemy1.Direction.X} Y:{enemy1.Direction.Y} Z:{enemy1.Direction.Z}\n" +
                                $"enemy Speed: {enemy1.Speed} ups\n" +
                                $"enemy MaxSpeed: {enemy1.MaxSpeed} ups\n" +
                                $"enemy Acceleration: {enemy1.Acceleration} ups\n" +
                                $"Data: {screen}\n";

            if (_displayHelp) text += "\n\n" + _helpString;

            _directX2DGraphics.BeginDraw();
            //_directX2DGraphics.DrawText(text, _HUDResources.textFPSTextFormatIndex, Matrix3x2.Identity,
            //                            _directX2DGraphics.RenderTargetClientRectangle, _HUDResources.textFPSBrushIndex + 1); // 0 - Желтый, 1 - Красный, 2 - Черный

            hudRacing.DrawText();
            hudRacing.DrawBitmaps();

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
