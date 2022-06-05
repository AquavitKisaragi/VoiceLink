using System;
using System.Collections.Generic;
using System.IO;                  // ファイル読み込みに必要です
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace clsJson
{

    public class AQTK_VOICEdata
    {
        public bool talk = false;
        public string text = "";
        public string koe = "";
        public int bas = 0;    // 基本素片 F1E/F2E/M1E (0/1/2)
        public int spd = 100;    // 話速     50-300 default:100
        public int vol = 100;    // 音量     0-300 default:100
        public int pit = 100;    // 高さ     20-200 default:基本素片に依存
        public int acc = 100;    // アクセント 0-200 default:基本素片に依存
        public int lmd = 100;    // 音程１  0-200 default:100
        public int fsc = 100;   // 音程２(サンプリング周波数) 50-200 default:100
        public Dictionary<string, object> Dic = new Dictionary<string, object> { };

        public AQTK_VOICEdata()
        {
        }
        public AQTK_VOICEdata(String filename)
        {
            jsonGet(filename);
        }

        public Dictionary<string, object> jsonGet(String filename)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                Formatting = Formatting.Indented,
            };
            String data = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "VoiceData\\" +  filename + ".json");
            //            var result = JsonConvert.DeserializeObject<List<AQTK_VOICEdata>>(data, settings);
            Dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);
            //            var status = result[0].text;


            foreach(String key in Dic.Keys)
            {
                switch (key)
                {
                    case "text":
                        text = ""+Dic[key].ToString();
                        break;
                    case "talk":
                        talk = System.Convert.ToBoolean(Dic[key].ToString().ToLowerInvariant());
                        break;
                    case "bas":
                        bas = Int32.Parse(Dic[key].ToString());
                        break;
                    case "spd":
                        spd = Int32.Parse(Dic[key].ToString());
                        break;
                    case "vol":
                        vol = Int32.Parse(Dic[key].ToString());
                        break;
                    case "pit":
                        pit = Int32.Parse(Dic[key].ToString());
                        break;
                    case "acc":
                        acc = Int32.Parse(Dic[key].ToString());
                        break;
                    case "lmd":
                        lmd = Int32.Parse(Dic[key].ToString());
                        break;
                    case "fsc":
                        fsc = Int32.Parse(Dic[key].ToString());
                        break;
                }

            }
            return Dic;

        }
    }

}


