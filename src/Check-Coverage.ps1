[CmdletBinding()]
param(
    [Parameter(Mandatory)][string]$Report,
    [ValidateRange(0, 100)][double]$MinimumLineCoverage = 90,
    [ValidateRange(0, 100)][double]$MinimumBranchCoverage = 80
)
$ErrorActionPreference = 'Stop'
[xml]$coverage = Get-Content -LiteralPath $Report -Raw
$expected = @('DocuNet.Core', 'DocuNet.Rendering', 'DocuNet.Cli')
$culture = [Globalization.CultureInfo]::InvariantCulture
$failures = @()
foreach ($name in $expected) {
    $packages = @($coverage.coverage.packages.package | Where-Object { $_.name -eq $name })
    if ($packages.Count -ne 1) { throw "Expected exactly one coverage entry for $name." }
    $line = 100 * [double]::Parse($packages[0].GetAttribute('line-rate'), $culture)
    $branch = 100 * [double]::Parse($packages[0].GetAttribute('branch-rate'), $culture)
    Write-Output ("{0}: lines {1:F2}%, branches {2:F2}%" -f $name, $line, $branch)
    if ($line -lt $MinimumLineCoverage -or $branch -lt $MinimumBranchCoverage) {
        $failures += "$name is below line/branch thresholds ($MinimumLineCoverage%/$MinimumBranchCoverage%)."
    }
}
if ($failures.Count) { throw ($failures -join [Environment]::NewLine) }
