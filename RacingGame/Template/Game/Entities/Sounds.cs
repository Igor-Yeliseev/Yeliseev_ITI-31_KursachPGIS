using Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Template
{
    class Sounds
    {
        SharpAudioDevice device = new SharpAudioDevice();
        /// <summary> Звук разгоняющегося двигателя </summary>
        public SharpAudioVoice Throttle;
        private bool IsThrottled = true;
        /// <summary> Звук останавливающегося двигателя </summary>
        public SharpAudioVoice Reverse;
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
            sounds = new List<SharpAudioVoice>();
            sounds.Capacity = 4;
        }
        
        public void Load(string fileName)
        {
            if (fileName.Contains("throttle"))
            {
                Throttle = new SharpAudioVoice(device, fileName);
                Throttle.Stopped += Throttle_Stopped;
                sounds.Add(Throttle);
            }   
            else if (fileName.Contains("crash"))
            {
                Crash = new SharpAudioVoice(device, fileName);
                sounds.Add(Crash);
            }
            else if (fileName.Contains("brake"))
            {
                Brake = new SharpAudioVoice(device, fileName);
                sounds.Add(Brake);
            }
            else if (fileName.Contains("reverse"))
            {
                Reverse = new SharpAudioVoice(device, fileName);
                Reverse.Stopped += Reverse_Stopped;
                sounds.Add(Reverse);
            }
            else
            {
                if (Other != null)
                    Other.Dispose();

                Other = new SharpAudioVoice(device, fileName);
                sounds.Add(Other);
            }
        }


        private void Reverse_Stopped(SharpAudioVoice voice)
        {
            offset = 0;
        }

        private void Throttle_Stopped(SharpAudioVoice voice)
        {
            IsThrottled = true;
            offset = 0;
        }

        private long offset = 0;

        public void Plays()
        {
            //if (inputController.UpPressed)
            //{
            //    if (IsThrottled)
            //    {
            //        Reverse.Stop(out offset);
            //        //Throttle.Voice.SetFrequencyRatio(1.0f);
            //        Throttle.Play(Throttle.Duration - offset);
            //        IsThrottled = false;
            //    }
            //}
            //else
            //{
            //    if (!IsThrottled)
            //    {
            //        Throttle.Stop(out offset);
            //        IsThrottled = true;
            //        Reverse.Play(Reverse.Duration - offset);
            //    }
            //}


            //if (inputController.DownPressed)
            //{
            //    if (car.Speed > 0)
            //    {
            //        Brake.Resume();
            //        vol = car.Speed / car.MaxSpeed;
            //        Brake.Voice.SetVolume(vol);
            //    }
            //    else
            //        Brake.Stop();

            //}
            //else
            //{
            //    //Brake.Pause();
            //    Brake.Stop();
            //}
        }

        float vol;


        public void Dispose()
        {
            foreach (var sound in sounds)
            {
                if(sound != null)
                {
                    sound.Stop();
                    sound.Dispose();
                }
            }

        }
        
    }
}
