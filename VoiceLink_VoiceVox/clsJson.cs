using System;
using System.Collections.Generic;
using System.IO;                  // ファイル読み込みに必要です
using System.Text.Json;
namespace clsJson
{

    public class VoiceVoData
    {
        public Dictionary<string, object> FileDic = new Dictionary<string, object> { };
        public Dictionary<string, object> ResDic = new Dictionary<string, object> { };
        public string text ="";
        public int speaker =0;
        public VoiceVoData(String filename)
        {
            file(filename);
        }

        public void file(String filename)
        {
            try { 
                String data = File.ReadAllText( filename + ".json");
                FileDic = JsonSerializer.Deserialize<Dictionary<string, object>>(data);

                text = FileDic["text"].ToString();
                speaker = int.Parse(FileDic["speaker"].ToString());
            }
            catch
            {
                Environment.Exit(-1000);
            }
        }
        public void web(String respons)
        {
            ResDic = JsonSerializer.Deserialize<Dictionary<string, object>>(respons);

        }

        public String merge()
        {
            foreach (String key in FileDic.Keys)
            {
                switch (key)
                {
                    case "speedScale":
                        ResDic["speedScale"] = FileDic[key];
                        break;
                    case "pitchScale":
                        ResDic["pitchScale"] = FileDic[key];
                        break;
                    case "volumeScale":
                        ResDic["volumeScale"] = FileDic[key];
                        break;
                    case "prePhonemeLength":
                        ResDic["prePhonemeLength"] = FileDic[key];
                        break;
                    case "postPhonemeLength":
                        ResDic["postPhonemeLength"] = FileDic[key];
                        break;
                    case "outputSamplingRate":
                        ResDic["outputSamplingRate"] = FileDic[key];
                        break;
                    case "outputStereo":
                        ResDic["outputStereo"] = FileDic[key];
                        break;
                }

            }
            return JsonSerializer.Serialize(ResDic);

        }

    }
}


