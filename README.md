# Locknote

Locknote is a secure note-taking application that stores your files encrypted locally. For more information visit https://locknoteapp.xyz

## Building Locknote
### Windows
* Requirements
     * Visual Studio 2017 with Xamarin for mobile development
     * Git for Windows
* Building
     1. Clone this repository (from git bash)
          * `git clone https://github.com/borishonman/Locknote.git`
     2. Get the submodules (from git bash)
          * `cd Locknote && git submodule init && git submodule update`
     3. Open Locknote.sln in VS
     4. Build away!
### Mac - untested
* Requirements
     * Visual Studio for Mac with Xamarin for mobile development
* Building
     1. Clone this repository
          * `git clone https://github.com/borishonman/Locknote.git`
     2. Get the submodules
          * `cd Locknote && git submodule init && git submodule update`
     3. Open Locknote.sln in VS
     4. Build away!
### Linux (Debian/Ubuntu)
* Requirements
     * [My Xamarin.Android linux build](https://github.com/borishonman/xamarin-android-linux/releases) (no binary builds available at the moment, you need to build from source, follow the instructions in the readme in the linked repo)
     * git
          * `sudo apt install git`
     * nuget
          * `sudo apt install nuget`
* Building
     1. Install the deb from the above link
     2. Clone this repository
          * `git clone https://github.com/borishonman/Locknote.git`
     3. Get the submodules
          * `cd Locknote && git submodule init && git submodule update`
     4. Change to the Android directory
          * `cd Locknote/Locknote.Android`
     5. Get all nuget packages
          * `nuget restore packages.config -PackagesDirectory ../../packages`
     6. Build the project
          * `xabuild Locknote.Android.csproj`
     7. Build the apk
          * `xabuild /t:SignAndroidPackage Locknote.Android.csproj`
### Linux (other) - untested
* Requirements
     * [My Xamarin.Android linux build scripts](https://github.com/borishonman/xamarin-android-linux)
     * git
     * nuget
* Building
     1. Build Xamarin.Android using the instructions in the readme from the link above
     2. Clone this repository
          * `git clone https://github.com/borishonman/Locknote.git`
     3. Get the submodules
          * `cd Locknote && git submodule init && git submodule update`
     4. Change to the Android directory
          * `cd Locknote/Locknote.Android`
     5. Get all nuget packages
          * `nuget restore packages.config -PackagesDirectory ../../packages`
     6. Build the project
          * `/path/to/xamarin-android-linux/checkout/xamarin-android/tools/scripts/xabuild Locknote.Android.csproj`
     7. Build the apk
          * `/path/to/xamarin-android-linux/checkout/xamarin-android/tools/scripts/xabuild /t:SignAndroidPackage Locknote.Android.csproj`
