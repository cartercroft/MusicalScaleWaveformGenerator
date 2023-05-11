//DOCS http://soundfile.sapp.org/doc/WaveFormat/

using System.Diagnostics;
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

string path = Path.Combine(Environment.CurrentDirectory, "test.wav");
Console.WriteLine($"Saving to {path}");
using(FileStream fs = new FileStream(path, FileMode.Create))
{
    using(BinaryWriter bw = new BinaryWriter(fs))
    {
        for(int i = 0; i < data.Length; i++)        
            data[i] = 128;
        float aNoteFreq = 440.0f;
        GetMajorScaleByteData(aNoteFreq, data);
        WriteWaveformDataToBinaryWriter(bw, data);
    }
    Console.WriteLine("Done writing to file.");
}
Console.WriteLine("Attempting to open file for playback...");
Process.Start("explorer.exe", "/open, " + path);
Thread.Sleep(5500);
static void GetMajorScaleByteData(float baseNoteFreq, byte[] data)
{
    double multiplier = 2.0 * Math.PI / SAMPLERATE;
    int volume = 2;
    int oneEighth = data.Length / 8;
    float[] scaleFrequencies = GetMajorScaleFrequencies(baseNoteFreq);
    int scaleFrequencyReadIndex = 0;
    for (int i = 0; i < data.Length; i++)
    {
        if (i != 0 && i % oneEighth == 0)
            scaleFrequencyReadIndex++;
        data[i] = (byte)(data[i] + volume * Math.Sin(i * multiplier * scaleFrequencies[scaleFrequencyReadIndex]));
    }
}
static void WriteWaveformDataToBinaryWriter(BinaryWriter bw, byte[] data)
{
    WriteWaveformHeader(bw, data);
    bw.Write(data);
}
static void WriteWaveformHeader(BinaryWriter bw, byte[] data)
{
    int subChunk2Size = data.Length * NUMCHANNELS * (BITSPERSAMPLE / 8);
    int chunkSize = 4 + (8 + SUBCHUNKSIZE) + (8 + subChunk2Size);

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
}
static byte[] GetBytes(string str)
{
    return Encoding.ASCII.GetBytes(str);
}
static float[] GetMajorScaleFrequencies(float baseFreq) 
{
    string intervals = "WWHWWWH";
    return GetScaleFrequenciesFromInterval(baseFreq, intervals);
}
static float[] GetMinorScaleFrequencies(float baseFreq)
{
    string intervals = "WHWWHWW";
    return GetScaleFrequenciesFromInterval(baseFreq, intervals);
}
static float[] GetScaleFrequenciesFromInterval(float baseFreq, string intervals)
{
    float[] freqs = new float[8];
    freqs[0] = baseFreq;
    for (int i = 1; i < freqs.Length; i++)
    {
        float factor;

        if (intervals[0] == 'W')
            factor = (float)Math.Pow(2.0, 2 / 12.0f);
        else
            factor = (float)Math.Pow(2.0, 1 / 12.0f);

        freqs[i] = freqs[i - 1] * factor;
        intervals = intervals.Substring(1);
    }
    return freqs;
}
static float[] GetAllBaseNoteFrequencies(float baseFreq)
{
    float[] freqs = new float[12];
    freqs[0] = baseFreq;
    for(int i = 1; i < freqs.Length; i++)
    {
        float factor = (float)Math.Pow(2.0, 1 / 12.0f);
        freqs[i] = freqs[i - 1] * factor;
    }
    return freqs;
}