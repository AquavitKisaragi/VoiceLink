using System.Media;
using System.IO;
using System;

class Program
{
    static int Main(string[] args)
    {
        //初期チェック
        if (args.Length != 1) { return -1; }
        string fileName = args[0];
        string path = @""+ AppDomain.CurrentDomain.BaseDirectory + "\\VoiceData\\" + fileName + ".wav";
        if (File.Exists(path) == false) { return -1; }
        SoundPlayer wavePlayer = new SoundPlayer(path);
        wavePlayer.Play();
        wavePlayer.PlaySync();
        return 0;
    }
}