//DOCS http://soundfile.sapp.org/doc/WaveFormat/

using System.Text;

byte[] data = new byte[250000];
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
        int volume = 10;
        for(int i = 0; i < data.Length; i++)
        {
            data[i] = 128;
        }
        int oneEighth = data.Length / 8, count = 0;
        while(count < oneEighth)
        {
            data[count] = (byte)(data[count] + volume * Math.Sin(count * multiplier * 523.28));
            count++;
        }
        while (count < oneEighth * 2)
        {
            data[count] = (byte)(data[count] + volume * Math.Sin(count * multiplier * 587.36));
            count++;
        }
        while (count < oneEighth * 3)
        {
            data[count] = (byte)(data[count] + volume * Math.Sin(count * multiplier * 659.28));
            count++;
        }
        while (count < oneEighth * 4)
        {
            data[count] = (byte)(data[count] + volume * Math.Sin(count * multiplier * 698.48));
            count++;
        }
        while (count < oneEighth * 5)
        {
            data[count] = (byte)(data[count] + volume * Math.Sin(count * multiplier * 784.0));
            count++;
        }
        while (count < oneEighth * 6)
        {
            data[count] = (byte)(data[count] + volume * Math.Sin(count * multiplier * 880.0));
            count++;
        }
        while (count < oneEighth * 7)
        {
            data[count] = (byte)(data[count] + volume * Math.Sin(count * multiplier * 987.84));
            count++;
        }
        while (count < data.Length)
        {
            data[count] = (byte)(data[count] + volume * Math.Sin(count * multiplier * 1046.56));
            count++;
        }
        bw.Write(data);
    }
}

static byte[] GetBytes(string str)
{
    return Encoding.ASCII.GetBytes(str);
}