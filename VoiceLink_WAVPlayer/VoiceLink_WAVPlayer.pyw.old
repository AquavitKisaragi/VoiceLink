# -*- coding: utf-8 -*-
import sys
import winsound
import os
from datetime import datetime
from multiprocessing import Process

# ファイルから再生
def talk(filename):
    try:
        if (os.path.isfile(filename)):
            winsound.PlaySound(filename,  winsound.SND_FILENAME)
        else:
            print("ファイルがない")
            exit(-1)
    except:
        print ("なんかエラーだって")
        sys.exit(-2)

        
def main():
    os.chdir('VoiceData/')
    if len(sys.argv) > 1:
        LinkFileName = sys.argv[1]    # LinkFileName
        filename = LinkFileName + ".wav"
        talk(filename)
    
    else:
        print ("引数不足")
        sys.exit(-1)

if __name__ == '__main__':
    main()
    sys.exit(0)
