. ./vis-make.ps1

./Clean.ps1

echo "Building Kernel Projects..."
echo "[1/13] -> kernel"
vis project make ./Kernel Release >$null 2>&1
echo "[2/13] -> alias"
vis project make ./KernelApps/alias Release >$null 2>&1
echo "[3/13] -> clear"
vis project make ./KernelApps/clear Release >$null 2>&1
echo "[4/13] -> console"
vis project make ./KernelApps/console Release >$null 2>&1
echo "[5/13] -> copy"
vis project make ./KernelApps/copy Release >$null 2>&1
echo "[6/13] -> delete"
vis project make ./KernelApps/delete Release >$null 2>&1
echo "[7/13] -> echo"
vis project make ./KernelApps/echo Release >$null 2>&1
echo "[8/13] -> kernelloader"
vis project make ./KernelApps/kernelloader Release >$null 2>&1
echo "[9/13] -> kerneltools"
vis project make ./KernelApps/kerneltools Release >$null 2>&1
echo "[10/13] -> move"
vis project make ./KernelApps/move Release >$null 2>&1
echo "[11/13] -> cfgloader"
vis project make ./KernelLibs/cfgloader Release >$null 2>&1
echo "[12/13] -> consoletools"
vis project make ./KernelLibs/consoletools Release >$null 2>&1
echo "[13/13] -> time"
vis project make ./KernelLibs/time Release >$null 2>&1

