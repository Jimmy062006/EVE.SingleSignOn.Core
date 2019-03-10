param([string]$publishKey = "", [string]$projectName = "")

# Validate that we have a publish key and
# that a project name has been set
If ($publishKey -ne "" -And $projectName -ne "") 
{
	# Search for a NuGet package matching the project name
	# ignoring the .symbols.nupkg debug file
	$file = Get-ChildItem -Path nupkg/ -Filter $projectName*.nupkg -Recurse | Where-Object { $_.name -NotLike "*.symbols.nupkg" }
	
	# If we find a file, try and publish it
	# to NuGet.org
	If ($file)
	{
		nuget push nupkg/$file $publishKey -Source https://www.nuget.org/api/v2/package

		# Clean up all NuGet packages for this project
		Write-Host "Cleaning up NuGet packages"
		Remove-Item nupkg/$projectName*.nupkg
	}
	Else
	{
		# No NuGet packages were found
		Write-Host "No NuGet package found for $projectName"
	}
}
Else
{
	Write-Host "Publish key or project name parameters are missing"
}
