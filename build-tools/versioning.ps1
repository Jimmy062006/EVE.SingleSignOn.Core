# Script for auto-incrementing .NET 5 project versions
# Parameters (all optional):
# -version <string>
# -configDir <string>
# -fileName <string>

param([string]$version = "", [string]$configDir = "", [string]$fileName = "project.json")

# If we don't receive a config directory
# assume that the project.json is in the same directory
# as the powershell script
If ($configDir -eq "") 
{
	$configDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent
}

# Combine the config location with project json
$configDir += "\" + $fileName

# Get JSON contents
$json = Get-Content -Raw -Path $configDir | ConvertFrom-Json

# Increment the patch version number
# only when we don't receive a new version number
If ($version -eq "")
{
	$versionString = $json.version

	# Split away version suffix
	$versionParts = $versionString.Split("-")
	$versionNumber = $versionParts[0]
	$patchInt = [convert]::ToInt32($versionNumber.Split(".")[2], 10)
	[int]$incPatch = $patchInt + 1
	$version = $versionNumber.Split(".")[0] + "." + $versionNumber.Split(".")[1] + "." + ($incPatch -as [string])

	# If there was a suffix, attach it again
	If ($json.version -like '*-*')
	{
		$version += "-" + $versionParts[1]
	}
}

# Save the new version number to the configuration file
$json.version = $version
$json | ConvertTo-Json -depth 100 | Out-File $configDir

# Inform the user what version has been set as
Write-Host "Version updated to: $version"
