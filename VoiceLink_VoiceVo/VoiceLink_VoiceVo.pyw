# -*- coding: utf-8 -*-
import sys
import requests
import json
import winsound
import os

from multiprocessing import Process

# VoiceVoxで喋る及びファイルに出力
def talk(filename):

    try:
        winsound.PlaySound(filename,  winsound.SND_MEMORY)
    except:
        print ("なんかエラーだって")
        sys.exit(-4)

def audio_query( filename, json_load,speaker,text,talkFlg):
    #query_payload = json_load
    query_payload =  {"text": text, "speaker": speaker}
    try:
            r = requests.post("http://localhost:50021/audio_query", 
                    params=query_payload, timeout=(10, 30))
            if r.status_code == 200:
                query_data = r.json()
                synth_payload = {"speaker": speaker}    
                print (query_data)
                for key in json_load:
                    if('speedScale' ==key):
                        query_data['speedScale']=json_load[key]
                    elif('pitchScale' ==key):
                        query_data['pitchScale']=json_load[key]
                    elif('volumeScale' ==key):
                        query_data['volumeScale']=json_load[key]
                    elif('prePhonemeLength' ==key):
                        query_data['prePhonemeLength']=json_load[key]
                    elif('postPhonemeLength' ==key):
                        query_data['postPhonemeLength']=json_load[key]
                    elif('outputSamplingRate' ==key):
                        query_data['outputSamplingRate']=json_load[key]
                    elif('outputStereo' ==key):
                        query_data['outputStereo']=json_load[key]
                r = requests.post("http://localhost:50021/synthesis", params=synth_payload, 
                              data=json.dumps(query_data), timeout=(10, 30))
                if r.status_code == 200:
                    with open(filename, "wb") as fp:
                        fp.write(r.content)
                        if(talkFlg==True):
                            talk( r.content)


    except:
        print ("なんかエラーだって")
        sys.exit(-2)

def main():
    if len(sys.argv) > 1:
        try:
            os.chdir('VoiceData/')
            filename = sys.argv[1]+".wav" # outputFileName
            jsonfile = sys.argv[1]+".json"
            json_open = open(jsonfile, 'r', encoding="utf-8")
            json_load = json.load(json_open)

            text = json_load.pop('text')
            sparg = json_load.pop('speaker')
            talkFlg = json_load.pop('talk')


            audio_query(filename,json_load,sparg,text,talkFlg)
        except:
            print ("なんかエラーだって")
            sys.exit(-3)

    else:
        print ("引数不足")
        sys.exit(-1)

if __name__ == '__main__':
    print(sys.getdefaultencoding())
    main()
    sys.exit(0)

