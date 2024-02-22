# KsDumper-11
https://github.com/mastercodeon314/KsDumper-11/assets/78676320/9452970c-27cf-44df-b091-0d18a07937e5

## Whats new v1.3.4
+ Added new feature Anti Anti Debugging Tools Detection
	Randomized MainWindow Title, most Control Titles, and the exe file name during runtime.
	The process name is reverted to KsDumper11.exe upon program closing.
	Enable Anti Anti Debugging Tools Detection check box setting added
	This feature was added in hopes to make KsDumper 11 more stealthy when trying to dump programs that have more rudimentary Anti Debugging techniques implemented.
+ Lots of source code cleanup
+ Fixed Easter Egg window that would not close upon clicking of the close button
+ Changed all labels in every form to be manually drawn to get around label text being changed when Anti Anti Debugging Tools Detection feature is enabled
+ Migrated from Application Settings to custom Settings.json for saving and loading of settings.

## Whats new v1.3.3
+ Updated KDU to v1.4.1
	New providers were added, see KDU patch notes on latest release.

## Whats new v1.3.2
+ Provider selction window now has a button to reset or wipe provider settings.
	This means that all the providers will be reset to needing to be tested, and the default provider will be reset.
+ Fixed a bug in the provider selection window that would prevent it from being closed when opened from the main Dumper window.
![image](https://github.com/mastercodeon314/KsDumper-11/assets/78676320/9ffeb3a7-86c6-40ef-95f7-cd140b20143d)

## Whats new v1.3.1
+ Updated KDU to v1.4.0! Provider count is now 44

## Whats new v1.3
+ Updated KDU to KDU V1.3.4! Over 40 different providers are now available!
+ Removed the old auto detection of working providers and replaced it with a new provider selector. Users can now select which provider they want to use to load the driver. As well as test providers to see if they work on your system!
+ Testing some Providers may BSOD crash the system, KsDumper now has support for being ran again after a crash and will mark the last checked provider as non-working!
+ Anytime kdu loads and it detects a saved providers list, it will try to load the KsDumper driver using the default provider
+ Providers list and selected default provider are now saved as JSON files!
+ Updated to .NET Framework v4.8

![KsDumper v1.3 Provider Selector window](https://github.com/mastercodeon314/KsDumper-11/assets/78676320/c683b753-774b-49f0-81ca-76ed2f4dd09b)

## Whats new v1.2
+ KsDumper will now try and start the driver using the default kdu exploit provider #1 (RTCore64.sys)
+ If the default provider does not work, KsDumper will scan all kdu providers and save each one that works into a list.
+ Anytime kdu loads and it detects a saved providers list, it will try to load the KsDumper driver using each saved provider until one works.
+ This technique should increase the amount of systems that the driver will be able to be loaded on. 

## Support
You can join the official KsDumper 11 discord server where I will be managing ongoing issues. 
For those of you who find that ksDumper won't start on their system, please join the server and post your logs in the support channel. 
Please keep in mind that until others volunteer to help in development of this tool, I am only one person with a finite amount of knowledge. 
https://discord.gg/6kfWU3Ckya

## Features
- Selection of working kdu exploit providers.
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
![Canary Channel Insider Build Win 11 Ksdumper](https://github.com/mastercodeon314/KsDumper-11/assets/78676320/12b05290-8856-48c6-ae03-90733c8db392)

**Note**: Import table isn't rebuilt.

## Usage
The old way of loading the unsigned ksDumper.sys kernel driver was to use the capcom exploit to map it, this got patched in windows 11.
This one loads the driver with Kernel Driver Utility, or KDU for short. 

Loading of the driver is handled by the Provider Selector now. Simply select a provider from the list, click Test Driver, and if it works, then you can click Set Default provider and it will use the selected provider to load the KsDumper driver with. 

**Note2**: Even though it can dump both x86 & x64 processes, this has to run on x64 Windows.

## Disclaimer
The new kdu provider selector can and WILL crash windows if a bad provider is tested. As such, I have implimented functionality to allow KsDumper to be ran again after a crash, and it will mark the last tested provider as non-working. This way, users will be prevented from testing that provider again and less crashes should result from general usage of KsDumper 11.
Please do beware that it can sometimes crash the OS even still. I do not take any responsibility for any damage that may occur to your system from using this tool.

Due to the nature of how KDU works to map the kernel driver, it is unknown if the system you run this on 
will have a exploitable driver according to kdu providers.
If you try to boot KsDumper 11 and it fails to start the driver, trying again as administrator.
If it still fails post the log. There is a manualloader.bat you can try as well to see the output directly.
You MUST run KsDumper at least once for the kdu.exe file and its dlls to be self extracted for the ManualLoader.bat to work.

This project has been made available for informational and educational purposes only.
Considering the nature of this project, it is highly recommended to run it in a `Virtual Environment`. I am not responsible for any crash or damage that could happen to your system.

**Important**: This tool makes no attempt at hiding itself. If you target protected games, the anti-cheat might flag this as a cheat and ban you after a while. Use a `Virtual Environment` !

## Donation links
Anything is super helpful! Anything donated helps me keep developing this program and others!
- https://www.paypal.com/paypalme/lifeline42
- https://cash.app/$Mastercodeon3142

## References
- https://github.com/EquiFox/KsDumper
- https://github.com/hfiref0x/KDU
- https://github.com/not-wlan/drvmap
- https://github.com/Zer0Mem0ry/KernelBhop
- https://github.com/NtQuery/Scylla/
- http://terminus.rewolf.pl/terminus/
- https://www.unknowncheats.me/

## Compile Yourself
- Requires Visual Studio 2022 (must use 2019 for compiling the driver, and 2019 wdk)
- Requires .NET 4.8
- Window Driver Framework (WDK)