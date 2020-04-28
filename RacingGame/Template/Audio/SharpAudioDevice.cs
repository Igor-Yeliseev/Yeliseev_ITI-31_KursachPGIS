using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.XAudio2;
using SharpDX.Multimedia;
using System.IO;
using System.Threading;

namespace Audio
{

    /// <summary>
    /// Delegate for sound stop
    /// </summary>
    /// <param name="voice">Voice that generate event</param>
    public delegate void SoundStop(SharpAudioVoice voice);

    /// <summary>
    /// Audio Voice
    /// </summary>
    public class SharpAudioVoice : IDisposable
    {
        SourceVoice _voice;
        AudioBuffer _buffer;
        SoundStream _stream;
        Thread _checkThread;
        
        public int Duration { get; }

        /// <summary>
        /// Voice
        /// </summary>
        public SourceVoice Voice
        {
            get { return _voice; }
        }

        /// <summary>
        /// Raise event when stopped
        /// </summary>
        public event SoundStop Stopped;

        SharpAudioDevice _device;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="device">Device</param>
        /// <param name="filename">Filename</param>
        public SharpAudioVoice(SharpAudioDevice device, string filename)
        {
            _stream = new SoundStream(File.OpenRead(filename));
            Duration = (int)_stream.Length / 4;
            
            _buffer = new AudioBuffer
            {
                Stream = _stream.ToDataStream(),
                AudioBytes = (int)_stream.Length,
                Flags = BufferFlags.EndOfStream
            };

            _device = device;

            var waveFormat = _stream.Format;
            _voice = new SourceVoice(_device.Device, waveFormat);
        }

        long samplesPlayed = 0;
        long startPos = 0;

        /// <summary>
        /// Play
        /// </summary>
        public void Play()
        {
            if (_checkThread != null)
            {
                if (_checkThread.ThreadState == ThreadState.Running)
                    return;
            }
            
            _voice.SubmitSourceBuffer(_buffer, _stream.DecodedPacketsInfo);
            _voice.Start();
            _checkThread = new Thread(new ThreadStart(Check));
            _checkThread.Start();
        }

        public void PlayLoop()
        {
            _buffer.LoopCount = XAudio2.MaximumLoopCount;
            _voice.SubmitSourceBuffer(_buffer, _stream.DecodedPacketsInfo);
            _voice.Start();

            _checkThread = new Thread(new ThreadStart(Check));
            _checkThread.Start();
        }

        /// <summary>
        /// Play file
        /// </summary>
        /// <param name="playBegin"> Место с которого начинать проигрывать файл (номер семпла)</param>
        public void Play(long playBegin)
        {
            //if (_checkThread != null)
            //    return;

            startPos = (playBegin == Duration) ? 0 : playBegin;
            _buffer.PlayBegin = (int)startPos; //540602;
            
            _voice.SubmitSourceBuffer(_buffer, _stream.DecodedPacketsInfo); // 2162412
            _voice.Start();

            _checkThread = new Thread(new ThreadStart(Check));
            _checkThread.Start();

        }

        private bool _onPause;

        public void Resume()
        {
            if (!_onPause)
            {
                if (_checkThread != null)
                {
                    if(_checkThread.ThreadState == ThreadState.Running)
                        return;
                }
            }
            
            _onPause = false;
            _voice.SubmitSourceBuffer(_buffer, _stream.DecodedPacketsInfo);
            _voice.Start();
            _checkThread = new Thread(new ThreadStart(Check));
            _checkThread.Start();
        }

        //check voice status
        private void Check()
        {
            try
            {
                while (Voice.State.BuffersQueued > 0)
                {
                    samplesPlayed = Voice.State.SamplesPlayed;
                    //Thread.Sleep(10);
                }
                _voice.Stop();
                Stopped?.Invoke(this);
            }
            catch
            {

            }
        }
        

        /// <summary>
        /// Stop audio
        /// </summary>
        public void Stop()
        {
            if (_checkThread == null)
                return;

            _voice.Stop();
            _voice.FlushSourceBuffers();
            _checkThread.Abort();
            Stopped?.Invoke(this);
        }

        /// <summary>
        /// Stop audio
        /// </summary>
        public void StopOnce()
        {
            if (_checkThread == null)
                return;
            if (_checkThread.ThreadState == ThreadState.Aborted)
                return;

            _voice.Stop();
            _voice.FlushSourceBuffers();
            _checkThread.Abort();
            Stopped?.Invoke(this);
        }

        /// <summary>
        /// Stop audio
        /// </summary>
        public void Stop(out long offset)
        {
            if (_checkThread == null) {
                offset = 0;
                return;
            }

            _voice.SetVolume(0.0f);
            _voice.Stop();
            //_voice.FlushSourceBuffers();
            _checkThread.Abort();
            _checkThread = null;
            _voice.Dispose();
            _voice = new SourceVoice(_device.Device, _stream.Format);
            offset = startPos + samplesPlayed;
        }

        public void Off()
        {
            if (_checkThread == null)
                return;

            _voice.Stop();
            _voice.FlushSourceBuffers();
            _checkThread.Abort();
            _voice = new SourceVoice(_device.Device, _stream.Format);
        }

        /// <summary>
        /// Pause audio
        /// </summary>
        public void Pause()
        {
            if (_checkThread == null)
                return;

            _onPause = true;
            _voice.Stop();
            _checkThread.Abort();
            Stopped?.Invoke(this);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _voice.DestroyVoice();
            _voice.Dispose();
            _stream.Dispose();
            _buffer.Stream.Dispose();
        }
    }
}
