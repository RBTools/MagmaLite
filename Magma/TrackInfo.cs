using System;
using System.IO;

namespace MagmaC3
{
    public class TrackInfo
    {
        private string mStrType;
        private float panLeft;
        private float panRight;
        private float volLeft;
        private float volRight;

        public int Length { get; private set; }

        public bool Enabled { get; set; }

        public string Filename { get; private set; }

        public int NumChannels { get; private set; }

        public int SampleRate { get; private set; }

        public int BitsPerSample { get; private set; }

        public string TrackType
        {
            get
            {
                return mStrType;
            }
            set
            {
                mStrType = value;
            }
        }

        public float PanLeft
        {
            get
            {
                return panLeft;
            }
            set
            {
                panLeft = value;
            }
        }

        public float PanRight
        {
            get
            {
                return panRight;
            }
            set
            {
                panRight = value;
            }
        }

        public float VolLeft
        {
            get
            {
                return volLeft;
            }
            set
            {
                volLeft = value;
            }
        }

        public float VolRight
        {
            get
            {
                return volRight;
            }
            set
            {
                volRight = value;
            }
        }

        public TrackInfo()
        {
            Enabled = false;
            Filename = "";
            NumChannels = 0;
            SampleRate = 0;
            mStrType = "guitar";
            SetDefaultPan();
            SetDefaultVol();
            Length = 0;
            BitsPerSample = 0;
        }

        public TrackInfo(string strFilename, string type, float pan, float attenuation, bool enabled)
        {
            mStrType = type;
            Enabled = enabled;
            Filename = strFilename;
            NumChannels = 0;
            SampleRate = 0;
            Length = 0;
            panLeft = pan;
            volLeft = attenuation;
            if (!File.Exists(strFilename) || !strFilename.EndsWith(".wav", StringComparison.CurrentCultureIgnoreCase))
                return;
            var waveInfo1 = new Wrapper.WaveInfo();
            FileStatus waveInfo2;
            try
            {
                waveInfo2 = Wrapper.GetWaveInfo(strFilename, waveInfo1);
            }
            catch (Exception)
            {
                return;
            }
            if (waveInfo2 != FileStatus.kSuccess)
                return;
            if (waveInfo1.mIsCompressed != 0)
                throw new MagmaException("Wave file " + strFilename + " is compressed.");
            Filename = strFilename;
            NumChannels = waveInfo1.mNumChannels;
            SampleRate = waveInfo1.mSampleRate;
            Length = WavUtl.GetWavLength(waveInfo1);
            BitsPerSample = waveInfo1.mBitsPerSample;
            SetAttenuation(attenuation);
            SetPan(pan);
        }

        public void SetDefaultPan()
        {
            panLeft = 0.0f;
            panRight = 0.0f;
        }

        public void SetDefaultVol()
        {
            volLeft = 0.0f;
            volRight = 0.0f;
        }

        public void SetAttenuation(float attenuation)
        {
            if (NumChannels == 2)
            {
                volLeft = attenuation;
                volRight = attenuation;
            }
            else
                volLeft = attenuation;
        }

        public void SetPan(float pan)
        {
            if (NumChannels == 2)
            {
                panLeft = -1f;
                panRight = 1f;
            }
            else
                panLeft = pan;
        }

        public bool IsValid()
        {
            return Filename.Length != 0 && File.Exists(Filename) && (mStrType != "dry_vocals" || BitsPerSample == 16) && (BitsPerSample == 16 || BitsPerSample == 24);
        }

        public void InitFromFile(string strFilename, string type)
        {
            Filename = strFilename;
            if (!File.Exists(strFilename) || !strFilename.EndsWith(".wav", StringComparison.CurrentCultureIgnoreCase))
                return;
            var waveInfo1 = new Wrapper.WaveInfo();
            FileStatus waveInfo2;
            try
            {
                waveInfo2 = Wrapper.GetWaveInfo(strFilename, waveInfo1);
            }
            catch (Exception)
            {
                return;
            }
            if (waveInfo2 != FileStatus.kSuccess)
                return;
            if (waveInfo1.mIsCompressed != 0)
                throw new MagmaException("Wave file " + strFilename + " is compressed.");
            Length = WavUtl.GetWavLength(waveInfo1);
            NumChannels = waveInfo1.mNumChannels;
            SampleRate = waveInfo1.mSampleRate;
            mStrType = type;
            BitsPerSample = waveInfo1.mBitsPerSample;
        }
    }
}
