using clsJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace VoiceLink_VoiceVo2
{

    internal class Program
    {
        static int Main(string[] args)
        {
            //初期チェック
            if (args.Length != 1) { return -1; }
            string fileName = args[0];
            if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "VoiceData\\" + fileName + ".json") == false) { return -2; }
            VoiceVoData jsonData = new VoiceVoData(fileName);

            String requestEndPoint = "http://localhost:50021/audio_query?text=" + jsonData.text + "&speaker=" + jsonData.speaker;
            //var content = new StringContent("this is content", new UTF8Encoding(), "application/x-www-form-urlencoded");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestEndPoint);
            string resBodyStr;
            HttpStatusCode resStatusCoode = HttpStatusCode.NotFound;
            Task<HttpResponseMessage> response;
            try
            {
                HttpClient httpClient = new HttpClient();
                response = httpClient.SendAsync(request);
                resBodyStr = response.Result.Content.ReadAsStringAsync().Result;
                resStatusCoode = response.Result.StatusCode;
            }
            catch (HttpRequestException e)
            {
                // UNDONE: 通信失敗のエラー処理
                return -3;
            }

            if (!resStatusCoode.Equals(HttpStatusCode.OK))
            {
                // UNDONE: レスポンスが200 OK以外の場合のエラー処理
                return -4;
            }
            if (String.IsNullOrEmpty(resBodyStr))
            {
                // UNDONE: レスポンスのボディが空の場合のエラー処理
                return -5;
            }


            jsonData.web(resBodyStr);
            
            requestEndPoint = "http://localhost:50021/synthesis?text=" + jsonData.text + "&speaker=" + jsonData.speaker;
            request = new HttpRequestMessage(HttpMethod.Post, requestEndPoint);
            var content = new StringContent(jsonData.merge(), Encoding.UTF8, @"application/json");
            request.Content = content;
            request.Headers.Add("Accept", "application/json");


           
            resStatusCoode = HttpStatusCode.NotFound;
            byte[] wav_data;
            try
            {
                HttpClient httpClient = new HttpClient();
                response = httpClient.SendAsync(request);
                wav_data = response.Result.Content.ReadAsByteArrayAsync().Result;
                resStatusCoode = response.Result.StatusCode;
            }
            catch (HttpRequestException e)
            {
                // UNDONE: 通信失敗のエラー処理
                return -6;
            }

            if (!resStatusCoode.Equals(HttpStatusCode.OK))
            {
                // UNDONE: レスポンスが200 OK以外の場合のエラー処理
                return -7;
            }
            if (String.IsNullOrEmpty(resBodyStr))
            {
                // UNDONE: レスポンスのボディが空の場合のエラー処理
                return -8;
            }

            using (var ms = new MemoryStream(wav_data))
            using (var sp = new SoundPlayer(ms))
            {
                using (var writer = new BinaryWriter(new FileStream(AppDomain.CurrentDomain.BaseDirectory + "\\VoiceData\\" + fileName + ".wav", FileMode.Create)))
                {
                    //書き込む処理
                    writer.Write(wav_data);
                }
                if (jsonData.talk == true)
                {
                    sp.Play();
                    sp.PlaySync();
                }
            }
            return 0;
        }

        
    }
}
