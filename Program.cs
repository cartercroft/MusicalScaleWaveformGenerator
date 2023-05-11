//DOCS http://soundfile.sapp.org/doc/WaveFormat/

using System.Text;

//Every 100,000 bytes = 1 second
const float SECONDS = 3.5f;
byte[] data = new byte[(int)(SECONDS * 100000)];
const int SUBCHUNKSIZE = 16;
const short AUDIOFORMAT = 1;
const short BITSPERSAMPLE = 16; //2 bytes per sample
const short NUMCHANNELS = 1;
const int SAMPLERATE = 44100;
const int BYTERATE = SAMPLERATE * NUMCHANNELS * (BITSPERSAMPLE / 8);
const short BLOCKALIGN = (short)(NUMCHANNELS * (BITSPERSAMPLE / 8));
int subChunk2Size = data.Length * NUMCHANNELS * (BITSPERSAMPLE / 8);

int chunkSize = 4 + (8 + SUBCHUNKSIZE) + (8 + subChunk2Size);

string path = Path.Combine(Directory.GetCurrentDirectory(), "test.wav");
Console.WriteLine($"Saving to {path}");
using(FileStream fs = new FileStream(path, FileMode.Create))
{
    using(BinaryWriter bw = new BinaryWriter(fs))
    {
        //File Header
        bw.Write(GetBytes("RIFF"));
        bw.Write(chunkSize);
        bw.Write(GetBytes("WAVE"));
        bw.Write(GetBytes("fmt"));
        bw.Write((byte)32);
        bw.Write(SUBCHUNKSIZE);
        bw.Write(AUDIOFORMAT);
        bw.Write(NUMCHANNELS);
        bw.Write(SAMPLERATE);
        bw.Write(BYTERATE);
        bw.Write(BLOCKALIGN);
        bw.Write(BITSPERSAMPLE);
        bw.Write(GetBytes("data"));
        bw.Write(subChunk2Size);
        //Done writing file header

        double multiplier = 2.0 * Math.PI / SAMPLERATE;
        int volume = 2;
        for(int i = 0; i < data.Length; i++)
        {
            data[i] = 128;
        }
        int oneEighth = data.Length / 8, count = 0;
        float baseFreq = 532.28f;

        float[] scaleFrequencies = GetMajorScaleFrequencies(baseFreq);
        int scaleFrequencyReadIndex = 0;
        for(int i = 0; i < data.Length; i++)
        {
            if(i != 0 && i % oneEighth == 0)
                scaleFrequencyReadIndex++;
            data[i] = (byte)(data[count] + volume * Math.Sin(i * multiplier * scaleFrequencies[scaleFrequencyReadIndex]));
        }
        bw.Write(data);
    }
}

static byte[] GetBytes(string str)
{
    return Encoding.ASCII.GetBytes(str);
}

static float[] GetMajorScaleFrequencies(float baseFreq) 
{
    string intervals = "WWHWWWH";
    float[] freqs = new float[8];
    freqs[0] = baseFreq;
    for (int i = 1; i < freqs.Length; i++)
    {
        float factor;
        
        if (intervals[0] == 'W')
            factor = (float)Math.Pow(2.0, 2/12.0f);
        else
            factor = (float)Math.Pow(2.0, 1 /12.0f);
        
        freqs[i] = freqs[i - 1] * factor;
        intervals = intervals.Substring(1);
    }

    return freqs;
}