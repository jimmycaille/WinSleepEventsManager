# WinSleepEventsManager : Windows tool to read various system events

## Screenshots
![Screenshot 1](https://raw.githubusercontent.com/jimmycaille/WinSleepEventsManager/master/Screenshots/readme.png "Screenshot 1")

## Features
Current events are :
* Application
  * ID 1001 : Wininit : CheckDisk report during boot
  * ID 26212 : Chkdsk : CheckDisk report during session
* Security
  * ID 4800 : Auditing : Windows locked
  * ID 4801 : Auditing : Windows unlocked
* Setup
  * None
* System
  * ID 1 : Power-Troubleshooter : Windows out of sleep
  * ID 42 : Kernel-Power : Windows goes to sleep
  * ID 1001 : Windows Error reporting : Crash/BSOD
  * ID 6005 : EventLog : Service started (boot)
  * ID 6006 : EventLog : Service stopped (shutdown)
  * ID 6008 : EventLog : Forced shutdown
* ForwardedEvents
  * None

## Project structure
- README.md                 -> that the file you are reading now
- WinSleepEventsManager.sln -> Visual studio project
- WinSleepEventsManager\bin\Debug\WinSleepEventsManager.exe -> last working program

## Log
### TODO

## Known issues
### Reverse chronological doesn't work

## Prerequisites (to run Visual Studio project)
### Visual Studio Community (free) can be found here :
https://visualstudio.microsoft.com

## Developper infos
- Author : Jimmy Caille
- Email  : jimmy.caille.dev@gmail.com
