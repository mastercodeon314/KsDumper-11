# KsDumper-11
# KsDumper
![Demo](https://i.imgur.com/6XyMDxa.gif)
^New Demo gif comming soon

## Features
- Auto dumping of selected exe.
- Auto Refresh (every 100ms)
- Suspend, resume, kill process
- Dump any process main module using a kernel driver (both x86 and x64)
- Rebuild PE32/PE64 header and sections
- Works on protected system processes & processes with stripped handles (anti-cheats)
- Works on WINDOWS 11, it doesnt crash anymore!
![Dev Channel Insider Build Win 11 Ksdumper](https://cdn.discordapp.com/attachments/1022996250037076047/1066538037154152548/image.png)

**Note**: Import table isn't rebuilt.

## Usage
The old way of loading the unsigned ksDumper.sys kernel driver was to use the capcom exploit to map it, this got patched in windows 11.
This one loads the driver with Kernel Driver Utility, or KDU for short. 
I could not get the main fork of the program to work when being built from source. 

This one does though.
https://github.com/morelli690/KDU_kernel_bypass_/blob/master/Bin/kdu.exe

All driver loading is now automated, i plan on putting in a splash screen till the driver loads.
For now, the client wont open until the driver has been loaded, if it fails, it exits. 
I tried to build a logger to output kdu's console output to a file, however it writes black always. Known issue

**Note**: The driver stays loaded until you reboot, so if you close KsDumperClient.exe, you can just reopen it !  
**Note2**: Even though it can dump both x86 & x64 processes, this has to run on x64 Windows.

## Disclaimer
This project has been made available for informational and educational purposes only.
The driver source is not included because i couldnt ever get it to compile on my system. The source can be found on the original reop. 
Considering the nature of this project, it is highly recommended to run it in a `Virtual Environment`. I am not responsible for any crash or damage that could happen to your system.

**Important**: This tool makes no attempt at hiding itself. If you target protected games, the anti-cheat might flag this as a cheat and ban you after a while. Use a `Virtual Environment` !

## References
- https://github.com/hfiref0x/KDU
- https://github.com/morelli690/KDU_kernel_bypass_/blob/master/Bin/kdu.exe
- https://github.com/not-wlan/drvmap
- https://github.com/Zer0Mem0ry/KernelBhop
- https://github.com/NtQuery/Scylla/
- http://terminus.rewolf.pl/terminus/
- https://www.unknowncheats.me/

## Compile Yourself
- Requires Visual Studio 2022
- Requires .NET 4.8