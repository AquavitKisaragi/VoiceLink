using System;
using System.Media;
using System.Runtime.InteropServices;
using System.IO;
using clsJson;

namespace aquestalk
{
    static class Program
    {
        //DLL定義
        //漢字から読み仮名に変換
        const string kanadllname = "AqKanji2Koe.dll";
        [DllImport(kanadllname)]
        extern static IntPtr AqKanji2Koe_Create(byte[] pathDic, ref int pErr);
        [DllImport(kanadllname)]
        extern static int AqKanji2Koe_Convert_utf8(IntPtr hAqKanji2Koe, byte[] kanji, byte[] koe, int nBufKoe);
        [DllImport(kanadllname)]
        extern static void AqKanji2Koe_Release(IntPtr hAqKanji2Koe);

        //読み仮名から声に変換
        const string dllname = "AquesTalk.dll";
        [DllImport(dllname)]
        extern static IntPtr AquesTalk_Synthe_Utf8(ref AQTK_VOICE pParam, byte[] koe, ref int size);

        [DllImport(dllname)]
        extern static void AquesTalk_FreeWave(IntPtr wav);

        [StructLayout(LayoutKind.Sequential, Pack = 4)]
        struct AQTK_VOICE
        {
            public int bas;    // 基本素片 F1E/F2E/M1E (0/1/2)
            public int spd;    // 話速     50-300 default:100
            public int vol;    // 音量     0-300 default:100
            public int pit;    // 高さ     20-200 default:基本素片に依存
            public int acc;    // アクセント 0-200 default:基本素片に依存
            public int lmd;    // 音程１  0-200 default:100
            public int fsc;    // 音程２(サンプリング周波数) 50-200 default:100
            public void Init(int lbas, int lspd, int lvol, int lpit, int lacc, int llmd, int lfsc)
            {
                bas = lbas; spd = lspd; vol = lvol; pit = lpit; acc = lacc; lmd = llmd; fsc = lfsc;
            }
        }

        [STAThread]
        static int Main(string[] args)
        {
            //初期チェック
            if (args.Length != 1) { return -1; }
            string fileName = args[0];
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "VoiceData\\" + fileName + ".json")== false) { return -2; }
            string dicdir = AppDomain.CurrentDomain.BaseDirectory + "aq_dic\\";
            if (File.Exists(dicdir+"aqdic.bin")==false) { return -3; }
            
            AQTK_VOICEdata jsondata = new AQTK_VOICEdata(fileName);

            //漢字→よみがな変換開始
            System.Text.Encoding utf8Enc = System.Text.Encoding.GetEncoding("UTF-8");
            int refint = 0;
            byte[] utfdicdir =  utf8Enc.GetBytes(dicdir);
            IntPtr hAqKanji2Koe = AqKanji2Koe_Create(utfdicdir,ref refint);
            if (hAqKanji2Koe == IntPtr.Zero) { return -4; }

            string KanjiMoji = jsondata.text;
            refint = 0;
            byte[] utfKanjiMoji = utf8Enc.GetBytes(KanjiMoji);
            byte[] returnKoe = new byte[4096];

            refint = AqKanji2Koe_Convert_utf8(hAqKanji2Koe, utfKanjiMoji, returnKoe, 4096);

            AqKanji2Koe_Release(hAqKanji2Koe);

            string yomigana = System.Text.Encoding.UTF8.GetString(returnKoe);


            //よみがな→音声変換開始
            string koe = yomigana.Remove(yomigana.IndexOf('\0'));
            IntPtr aqtk_p = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(AQTK_VOICE)));
            AQTK_VOICE aqtk_voice = (AQTK_VOICE)Marshal.PtrToStructure(aqtk_p, typeof(AQTK_VOICE));
            aqtk_voice.Init(jsondata.bas, jsondata.spd, jsondata.vol, jsondata.pit, jsondata.acc, jsondata.lmd, jsondata.fsc);

            byte[] koeUtfBytes = utf8Enc.GetBytes(koe);

            int size = 0;
            IntPtr wavData = AquesTalk_Synthe_Utf8(ref aqtk_voice, koeUtfBytes, ref size);
            
            if (wavData == IntPtr.Zero) { return-5; }
                        
            byte[] wav_data = new byte[size];
            Marshal.Copy(wavData, wav_data, 0, size);
            AquesTalk_FreeWave(wavData);

            using (var ms = new MemoryStream(wav_data))
            using (var sp = new SoundPlayer(ms))
            {
                using (var writer = new BinaryWriter(new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\VoiceData\\" + fileName +".wav", FileMode.Create)))
                {
                    //書き込む処理
                    writer.Write(wav_data);
                }
                if (jsondata.talk == true)
                {
                    sp.Play();
                    sp.PlaySync();
                }
            }
            return 0;
        }
    }
}