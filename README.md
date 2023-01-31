# KsDumper-11
https://user-images.githubusercontent.com/78676320/213967527-ba0d435d-9d92-467d-bd9f-4e85f947dfa0.mp4

## Whats new v1.2
- KsDumper will now try and start the driver using the default kdu exploit provider #1 (RTCore64.sys)
- If the default provider does not work, KsDumper will scan all kdu providers and save each one that works into a list.
- Anytime kdu loads and it detects a saved providers list, it will try to load the KsDumper driver using each saved provider until one works.
- This technique should increase the amount of systems that the driver will be able to be loaded on. 

## Support
You can join the official KsDumper 11 discord server where I will be managing ongoing issues. 
I am starting to see multiple people who's system the Rtcore64 exploit fails to work on.
For those of you who find that ksDumper won't start on their system, please join the server and post your logs in the support channel. 
Please keep in mind that until others volunteer to help in development of this tool, I am only one person with a finite amount of knowledge. 
https://discord.gg/6kfWU3Ckya

## Features
- Auto detection of working kdu exploit providers.
- Auto dumping of selected exe.
- Unloading the KsDumper kernel driver is now supported! An option was added to unload on program exit, or system shutdown/restart.
- Splash screen for when driver is being loaded
- Auto Refresh (every 100ms)
- Suspend, resume, kill process
- Dump any process main module using a kernel driver (both x86 and x64)
- Rebuild PE32/PE64 header and sections
- ^ This can be defeated by stripping pe headers. Once pe headers are stripped, it cant dump.
- Works on protected system processes & processes with stripped handles (anti-cheats)
- Works on Windows 11, it doesnt crash anymore!
![Dev Channel Insider Build Win 11 Ksdumper](https://cdn.discordapp.com/attachments/1022996250037076047/1066538037154152548/image.png)

**Note**: Import table isn't rebuilt.

## Usage
The old way of loading the unsigned ksDumper.sys kernel driver was to use the capcom exploit to map it, this got patched in windows 11.
This one loads the driver with Kernel Driver Utility, or KDU for short. 
I could not get the main fork of the program to work when being built from source. 

This one does though.
https://github.com/morelli690/KDU_kernel_bypass_/blob/master/Bin/kdu.exe

All driver loading is now automated, The splash screen is done, and the driver loader log now works!
If the splash screen shuts down saying it failed to start the driver, please post the log file as an issue on the repo.

**Note2**: Even though it can dump both x86 & x64 processes, this has to run on x64 Windows.

## Disclaimer
Due to the nature of how KDU works to map the kernel driver, it is unknown if the system you run this on 
will have a exploitable driver according to kdu providers.
If you try to boot KsDumper 11 and it fails to start the driver, trying again as administrator.
If it still fails post the log. There is a manualloader.bat you can try as well to see the output directly. 
I will be working on making a selector that will get the correct provider for your system, or detect if none are available.

This project has been made available for informational and educational purposes only.
Considering the nature of this project, it is highly recommended to run it in a `Virtual Environment`. I am not responsible for any crash or damage that could happen to your system.

**Important**: This tool makes no attempt at hiding itself. If you target protected games, the anti-cheat might flag this as a cheat and ban you after a while. Use a `Virtual Environment` !

## References
- https://github.com/EquiFox/KsDumper
- https://github.com/hfiref0x/KDU
- https://github.com/morelli690/KDU_kernel_bypass_/blob/master/Bin/kdu.exe
- https://github.com/not-wlan/drvmap
- https://github.com/Zer0Mem0ry/KernelBhop
- https://github.com/NtQuery/Scylla/
- http://terminus.rewolf.pl/terminus/
- https://www.unknowncheats.me/

## Compile Yourself
- Requires Visual Studio 2022
- Requires .NET 4.6.1