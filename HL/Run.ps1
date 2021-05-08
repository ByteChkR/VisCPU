. ./vis-make.ps1
$title    = 'Kernel Startup'
$question = 'Use existing Installation?'
$choices  = '&Yes', '&No'

$decision = $Host.UI.PromptForChoice($title, $question, $choices, 1)
if ($decision -eq 0) {
    echo "Skipping Build.."
} else {

    ./Build.ps1

    #Delete Old Disk if any
    echo "Removing Old Disks"
    Remove-Item 'D:\Users\Tim\Documents\viscpu\src\VisCPU.Console\bin\Debug\net5.0\configs\cpu\peripherals\FileDrive' -Recurse
    
    #Initialize Disks
    echo "Creating Partitions..."
    vis project make ./Kernel ReleaseRun >$null 2>&1

}

#Launch into Kernel
echo "Starting Kernel..."
vis project make ./Kernel ReleaseRun -log StackTrace