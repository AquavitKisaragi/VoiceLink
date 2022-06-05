# -*- coding: utf-8 -*-
import sys
import requests
import json
import winsound
import os
from datetime import datetime
from multiprocessing import Process

# VoiceVoxで喋る及びファイルに出力
def talk(filename):

    try:
        winsound.PlaySound(filename,  winsound.SND_MEMORY)
    except:
        print ("なんかエラーだって")
        sys.exit(-4)

def audio_query(text, filename, speaker,talkFlg):
    query_payload = {"text": text, "speaker": speaker}
    try:
            r = requests.post("http://localhost:50021/audio_query", 
                    params=query_payload, timeout=(10, 30))
            if r.status_code == 200:
                query_data = r.json()
            synth_payload = {"speaker": speaker}    
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
            outputType = int(2) # 0:voice onry,1:file onry,2,Voice and File
            filename = sys.argv[1]+".wav" # outputFileName
            jsonfile = sys.argv[1]+".json"
            json_open = open(jsonfile, 'r', encoding="utf-8")
            json_load = json.load(json_open)

            text = json_load['text']
            sparg = json_load['sperker']
            talkFlg = json_load['talk']
            
            audio_query(text,filename,sparg,talkFlg)
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

