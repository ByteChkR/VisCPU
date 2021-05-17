. ./vis-make.ps1
$title    = 'Kernel Startup'
$question = 'Use existing Installation?'
$choices  = '&Yes', '&No'

$decision = $Host.UI.PromptForChoice($title, $question, $choices, 1)
if ($decision -eq 0) {
    echo "Skipping Build.."
} else {

    ./BuildRaw.ps1

    ./DeleteFileDrive.ps1
}

#Launch into Kernel
echo "Starting Kernel..."
vis project make ./Kernel ReleaseRun -log StackTrace Network