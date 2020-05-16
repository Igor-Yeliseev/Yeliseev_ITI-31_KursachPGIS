using Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Template.Entities.Abstract_Factory;

namespace Template
{
    class Sounds
    {
        SharpAudioDevice _device = new SharpAudioDevice();
        /// <summary> Звук холостого хода </summary>
        private SharpAudioVoice Idle;
        /// <summary> Звук при максимальных оборотах </summary>
        private SharpAudioVoice HighRPM;
        /// <summary> Звук разгоняющегося двигателя </summary>
        private SharpAudioVoice Throttle;
        private SharpAudioVoice Pickup;
        private SharpAudioVoice BonusHealth;
        private SharpAudioVoice TirePuncture;
        private SharpAudioVoice BonusNitro;
        private SharpAudioVoice Melody;
        /// <summary> Звук столкновения с другой машиной </summary>
        public SharpAudioVoice Crash;
        /// <summary> Звук торможения </summary>
        public SharpAudioVoice Brake;
        /// <summary> Другой звук </summary>
        public SharpAudioVoice Other;

        private List<SharpAudioVoice> sounds;

        InputController inputController;
        Car car;
        /// <summary> Задать звуки для машины</summary>
        public Car Car
        {
            set
            {
                car = value;

                car.Collied += (c) =>
                {
                    Crash.Play();
                };
                car.ColliedCheckPoint += () =>
                {
                    Pickup.Play();
                };
            }
        }


        public Sounds(InputController inputController)
        {
            this.inputController = inputController;
            sounds = new List<SharpAudioVoice>();
            sounds.Capacity = 8;
        }


        /// <summary> Добавить звук на сюрприз</summary>
        /// <param name="surprise"></param>
        public void AddSoundBonus(SurPrise surprise)
        {
            switch (surprise.Type)
            {
                case SurpriseType.Health:
                    surprise.OnCatched += (v) => BonusHealth.Play();
                    break;
                case SurpriseType.Tire:
                    surprise.OnCatched += (v) => BonusHealth.Play();
                    break;
                case SurpriseType.Speed:
                    surprise.OnCatched += (v) => BonusNitro.Play();
                    break;
                case SurpriseType.Damage:
                    surprise.OnCatched += (v) => TirePuncture.Play();
                    break;
            }
        }


        /// <summary> Загрузки звука </summary>
        /// <param name="fileName"></param>
        public void Load(string fileName)
        {
            if (fileName.Contains("idle"))
            {
                Idle = new SharpAudioVoice(_device, fileName);
                sounds.Add(Idle);
            }
            else if (fileName.Contains("throttle"))
            {
                Throttle = new SharpAudioVoice(_device, fileName);
                Throttle.Stopped += Throttle_Stopped;
                sounds.Add(Throttle);
            }
            else if (fileName.Contains("high-rpm"))
            {
                HighRPM = new SharpAudioVoice(_device, fileName);
                sounds.Add(HighRPM);
            }
            else if (fileName.Contains("crash"))
            {
                Crash = new SharpAudioVoice(_device, fileName);
                sounds.Add(Crash);
            }
            else if (fileName.Contains("bonus"))
            {
                if (fileName.Contains("nitro"))
                {
                    BonusNitro = new SharpAudioVoice(_device, fileName);
                    sounds.Add(BonusNitro);
                }
                else if(fileName.Contains("health"))
                {
                    BonusHealth = new SharpAudioVoice(_device, fileName);
                    sounds.Add(BonusHealth);
                }
                else if (fileName.Contains("tire-puncture"))
                {
                    TirePuncture = new SharpAudioVoice(_device, fileName);
                    sounds.Add(TirePuncture);
                }
            }
            else if (fileName.Contains("brake"))
            {
                Brake = new SharpAudioVoice(_device, fileName);
                sounds.Add(Brake);
            }
            else if (fileName.Contains("melody"))
            {
                Melody = new SharpAudioVoice(_device, fileName);
                sounds.Add(Melody);
            }
            else if (fileName.Contains("pickup"))
            {
                Pickup = new SharpAudioVoice(_device, fileName);
                Pickup.Stopped += Reverse_Stopped;
                sounds.Add(Pickup);
            }
            else
            {
                if (Other != null)
                    Other.Dispose();

                Other = new SharpAudioVoice(_device, fileName);
                sounds.Add(Other);
            }
        }


        private void Reverse_Stopped(SharpAudioVoice voice)
        {
            offset = 0;
        }

        private void Throttle_Stopped(SharpAudioVoice voice)
        {
            offset = 0;
            thrtlEnd = true;
        }

        public bool thrtlEnd = false;
        private long offset = 0;
        private float vol;

        Stopwatch watch = new Stopwatch();

        private bool firstRun = false;

        public void Plays()
        {
            if (!firstRun)
            {
                //Idle.PlayLoop();
                Melody.Voice.SetVolume(0.75f);
                Melody.PlayLoop();
                firstRun = true;
            }

            //if (inputController.UpPressed)
            //{
            //    //if (IsThrottled)
            //    //{
            //    //    Reverse.Stop(out offset);
            //    //    Throttle.Play(Throttle.Duration - offset);
            //    //    IsThrottled = false;
            //    //}
            //}
            //else if (inputController.DownPressed)
            //{
            //    if (car.Speed > 0)
            //    {
            //        vol = car.Speed / car.MaxSpeed;
            //        Brake.Voice.SetVolume(vol);
            //        Brake.Resume();
            //    }
            //    else
            //        Brake.Stop();
                
            //}
            //else
            //{
            //    //if (!IsThrottled)
            //    //{
            //    //    //HighRPM.StopOnce();
            //    //    Throttle.Stop(out offset);
            //    //    IsThrottled = true;
            //    //    if (thrtlEnd)
            //    //        Reverse.Play(0);
            //    //    else
            //    //    {
            //    //        Reverse.Play(Reverse.Duration - offset);
            //    //    }
            //    //    thrtlEnd = false;
            //    //}
                
            //    Brake.Stop();
            //}

            
        }


        public void Dispose()
        {
            foreach (var sound in sounds)
            {
                if (sound != null)
                {
                    sound.Stop();
                    sound.Dispose();
                }
            }

            _device.Dispose();
        }
        
    }
}
