. ./vis-make.ps1

./Clean.ps1

echo "Building Kernel Projects..."
echo "[1/18] -> kernel"
vis project make ./Kernel Release >$null 2>&1
echo "[2/18] -> alias"
vis project make ./KernelApps/alias Release >$null 2>&1
echo "[3/18] -> clear"
vis project make ./KernelApps/clear Release >$null 2>&1
echo "[4/18] -> console"
vis project make ./KernelApps/console Release >$null 2>&1
echo "[5/18] -> copy"
vis project make ./KernelApps/copy Release >$null 2>&1
echo "[6/18] -> delete"
vis project make ./KernelApps/delete Release >$null 2>&1
echo "[7/18] -> echo"
vis project make ./KernelApps/echo Release >$null 2>&1
echo "[8/18] -> kernelloader"
vis project make ./KernelApps/kernelloader Release >$null 2>&1
echo "[9/18] -> kerneltools"
vis project make ./KernelApps/kerneltools Release >$null 2>&1
echo "[10/18] -> move"
vis project make ./KernelApps/move Release >$null 2>&1
echo "[11/18] -> netecho"
vis project make ./KernelApps/netecho Release >$null 2>&1
echo "[12/18] -> netwrite"
vis project make ./KernelApps/netwrite Release >$null 2>&1
echo "[13/18] -> cfgloader"
vis project make ./KernelLibs/cfgloader Release >$null 2>&1
echo "[14/18] -> consoletools"
vis project make ./KernelLibs/consoletools Release >$null 2>&1
echo "[15/18] -> time"
vis project make ./KernelLibs/time Release >$null 2>&1
echo "[16/18] -> networking"
vis project make ./KernelLibs/networking Release >$null 2>&1
echo "[17/18] -> networking.dns"
vis project make ./KernelLibs/networking.dns Release >$null 2>&1
echo "[18/18] -> dnsreg"
vis project make ./KernelApps/dnsreg Release >$null 2>&1
