﻿using Audio;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    class Sounds
    {
        SharpAudioDevice _device = new SharpAudioDevice();
        /// <summary> Звук холостого хода </summary>
        public SharpAudioVoice Idle;
        /// <summary> Звук при максимальных оборотах </summary>
        public SharpAudioVoice HighRPM;
        /// <summary> Звук разгоняющегося двигателя </summary>
        public SharpAudioVoice Throttle;
        private bool IsThrottled = true;

        public SharpAudioVoice Pickup;
        public SharpAudioVoice Melody;
        /// <summary> Звук столкновения с другой машиной </summary>
        public SharpAudioVoice Crash;
        /// <summary> Звук торможения </summary>
        public SharpAudioVoice Brake;
        /// <summary> Другой звук </summary>
        public SharpAudioVoice Other;

        private List<SharpAudioVoice> sounds;

        InputController inputController;
        Car car;

        public Sounds(InputController inputController, Car car)
        {
            this.inputController = inputController;
            this.car = car;
            this.car.Collied += (c) =>
            {
                Crash.Play();
            };
            this.car.ColliedCheckPoint += () =>
            {
                Pickup.Play();
            };
            sounds = new List<SharpAudioVoice>();
            sounds.Capacity = 7;
        }
        
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
