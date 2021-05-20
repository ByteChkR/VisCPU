. ./vis-make.ps1

echo "Cleaning Kernel Projects..."

echo "[1/18] -> kernel"
vis project clean .\Kernel >$null 2>&1
echo "[2/18] -> alias"
vis project clean .\KernelApps\alias >$null 2>&1
echo "[3/18] -> cfgloader"
vis project clean .\KernelLibs\cfgloader >$null 2>&1
echo "[4/18] -> clear"
vis project clean .\KernelApps\clear >$null 2>&1
echo "[5/18] -> console"
vis project clean .\KernelApps\console >$null 2>&1
echo "[6/18] -> consoletools"
vis project clean .\KernelLibs\consoletools >$null 2>&1
echo "[7/18] -> copy"
vis project clean .\KernelApps\copy >$null 2>&1
echo "[8/18] -> delete"
vis project clean .\KernelApps\delete >$null 2>&1
echo "[9/18] -> echo"
vis project clean .\KernelApps\echo >$null 2>&1
echo "[10/18] -> kernelloader"
vis project clean .\KernelApps\kernelloader >$null 2>&1
echo "[11/18] -> kerneltools"
vis project clean .\KernelApps\kerneltools >$null 2>&1
echo "[12/18] -> move"
vis project clean .\KernelApps\move >$null 2>&1
echo "[11/18] -> netecho"
vis project clean ./KernelApps/netecho >$null 2>&1
echo "[12/18] -> netwrite"
vis project clean ./KernelApps/netwrite >$null 2>&1
echo "[15/18] -> time"
vis project clean .\KernelLibs\time >$null 2>&1
echo "[16/18] -> networking"
vis project clean ./KernelLibs/networking >$null 2>&1
echo "[17/18] -> networking.dns"
vis project clean ./KernelLibs/networking.dns >$null 2>&1
echo "[18/18] -> dnsreg"
vis project clean ./KernelApps/dnsreg >$null 2>&1