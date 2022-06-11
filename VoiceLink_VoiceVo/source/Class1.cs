using System;
using System.Collections.Generic;
using System.IO;                  // ファイル読み込みに必要です
using System.Text.Json;
using Newtonsoft.Json;
namespace clsJson
{

    public class VoiceVoData
    {
        public bool talk = false;
        public Dictionary<string, object> FileDic = new Dictionary<string, object> { };
        public Dictionary<string, object> ResDic = new Dictionary<string, object> { };
        public string text;
        public int speaker;
        public VoiceVoData(String filename)
        {
            file(filename);
        }

        public void file(String filename)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                Formatting = Formatting.Indented,
            };
            String data = File.ReadAllText(AppDomain.CurrentDomain.BaseDirectory + "VoiceData\\" + filename + ".json");
            FileDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(data);

            text = FileDic["text"].ToString();
            speaker = int.Parse(FileDic["speaker"].ToString());


            foreach (String key in FileDic.Keys)
            {
                switch (key)
                {
                    case "talk":
                        talk = System.Convert.ToBoolean(FileDic["talk"].ToString().ToLowerInvariant());
                        break;
                }
            }

        }
        public void web(String respons)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat,
                Formatting = Formatting.Indented,
            };
            ResDic = JsonConvert.DeserializeObject<Dictionary<string, object>>(respons);

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
            return JsonConvert.SerializeObject(ResDic);

        }

    }
}


