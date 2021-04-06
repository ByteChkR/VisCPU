cd ..
. .\PublishAll.ps1
cd OS

# Build OS Image
echo "Building OS Image..."
cd OS
vis project make . Release >$null 2>&1

cd ..\Installer
# Build and Run Installer
echo "Building Installer Image..."
vis project make . Release >$null 2>&1

cd ..\BootDisk
# Build and Run BootDisk Loader
# Runs the OS Image that was installed onto the disk.
echo "Building BootDisk Image..."
vis project make . Release >$null 2>&1

cd ..\Installer
echo "Running Installer.."
vis project make . Run

cd ..\BootDisk
echo "Running BootDisk.."
vis project make . Run

cd ..