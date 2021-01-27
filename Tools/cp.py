#! py -3

import os
import time
import ctypes
import shutil

SF = "./FancyServer/bin/Debug/FancyServer.exe"
TF = "./bin/FancyServer.exe"

idx = 0
chl = ['\\', '/']
player = ctypes.windll.kernel32

while True:
    try:
        if os.path.exists(SF):
            shutil.copyfile(SF, TF)
            os.remove(SF)
            player.Beep(1250, 500)
            print("\ncopied {sf} to {tf}.".format(sf=SF, tf=TF))
        else:
            print("\r {ch} FancyServer.exe not found.".format(
                ch=chl[idx & 1]), end="")
    except Exception as e:
        print(e.args)
    time.sleep(1)
    idx += 1

