. ./vis-make.ps1

echo "Cleaning Kernel Projects..."

echo "[1/13] -> kernel"
vis project clean .\Kernel >$null 2>&1
echo "[2/13] -> alias"
vis project clean .\KernelApps\alias >$null 2>&1
echo "[3/13] -> cfgloader"
vis project clean .\KernelApps\cfgloader >$null 2>&1
echo "[4/13] -> clear"
vis project clean .\KernelApps\clear >$null 2>&1
echo "[5/13] -> console"
vis project clean .\KernelApps\console >$null 2>&1
echo "[6/13] -> consoletools"
vis project clean .\KernelApps\consoletools >$null 2>&1
echo "[7/13] -> copy"
vis project clean .\KernelApps\copy >$null 2>&1
echo "[8/13] -> delete"
vis project clean .\KernelApps\delete >$null 2>&1
echo "[9/13] -> echo"
vis project clean .\KernelApps\echo >$null 2>&1
echo "[10/13] -> kernelloader"
vis project clean .\KernelApps\kernelloader >$null 2>&1
echo "[11/13] -> kerneltools"
vis project clean .\KernelApps\kerneltools >$null 2>&1
echo "[12/13] -> move"
vis project clean .\KernelApps\move >$null 2>&1
echo "[13/13] -> time"
vis project clean .\KernelApps\time >$null 2>&1