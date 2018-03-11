cls

# First functions

function updateNodeVersion
{
    Param ([System.Xml.XmlElement] $versionNode, [string] $filename)

    if ($versionNode -eq $null)
    {
        return $false
    }

    $nodename = $versionNode.Name

     $oldVersion = $versionNode.InnerText
     $version = getUpdatedVersion $oldVersion
        
        if (-not $oldVersion.Equals($version))
        {
            $versionNode.InnerText = $version
            
            Write-Host "$filename : Updating $nodename from $oldVersion to $version"

            return $true
        }
   
   return $false

}

function getUpdatedVersion
{
    Param ([string] $version)
    $versionArray = $version.Split(".")
    if ($versionArray.Length -eq 4)
    {
        $versionArray[3] = ([int]$versionArray[3]) + 1
        $version =  [string]::Join(".",$versionArray)

        return $version        
    }

    return $inputVerison
}


# Update version number

function updateProjectVersionNumber
{
    $updates = 0
    $filter = "*.csproj"

    echo "Scanning $filter to update version..."

    Get-ChildItem -Filter "$filter" | ForEach-Object { 
        updateFileVersion $_.FullName 
        }
}

function updateFileVersion
{
    Param ([string] $filename)

    $content = Get-Content $filename 
    [xml]$xml = $content

    $shortFilename = [System.IO.Path]::GetFileName($filename)

    $wasUpdated = $false

    $propertyGroups = $xml.SelectNodes("//Project/PropertyGroup")

    $updatedVersion = $false
    $updatedFileVersion = $false
    $updatedAssemblyVersion = $false

    $firstNonConditionalPropertyGroup = $null

    foreach ($propertyGroup in $propertyGroups)
    {
        if ($firstNonConditionalPropertyGroup -eq $null)
        {            
            $condition = $propertyGroup.GetAttribute("Condition")
    
            if ($condition -eq "")
            {
                $firstNonConditionalPropertyGroup = $propertyGroup
            }
        }

        $r = updateNodeVersion $propertyGroup.SelectSingleNode("Version") $shortFilename

        if ($r -eq $true)
        {
            $wasUpdated = $true
            $updatedVersion = $true
        }

        $r = updateNodeVersion $propertyGroup.SelectSingleNode("FileVersion") $shortFilename

        if ($r -eq $true)
        {
            $wasUpdated = $true
            $updatedFileVersion = $true
        }

        $r = updateNodeVersion $propertyGroup.SelectSingleNode("AssemblyVersion") $shortFilename

        if ($r -eq $true)
        {
            $wasUpdated = $true
            $updatedAssemblyVersion = $true
        }
    }

    if ($updatedVersion -eq $false)
    {
        Write-Host "Version not found in any propertygroup. Adding Version 0.0.0.1"
        $version =  $xml.CreateElement("Version")
        $version.InnerText = "0.0.0.1"
        $firstNonConditionalPropertyGroup.AppendChild($version)
        $wasUpdated = $true
    }

    if ($updatedFileVersion -eq $false)
    {
        Write-Host "FileVersion not found in any propertygroup. Adding FileVersion 0.0.0.1"
        $version =  $xml.CreateElement("FileVersion")
        $version.InnerText = "0.0.0.1"
        $firstNonConditionalPropertyGroup.AppendChild($version)
        $wasUpdated = $true
    }

    if ($updatedAssemblyVersion -eq $false)
    {
        Write-Host "AssemblyVersion not found in any propertygroup. Adding AssemblyVersion 0.0.0.1"
        $version =  $xml.CreateElement("AssemblyVersion")
        $version.InnerText = "0.0.0.1"
        $firstNonConditionalPropertyGroup.AppendChild($version)
        $wasUpdated = $true
    }

    if ($wasUpdated -eq $true)
    {
        $xml.Save($_.FullName);
    }
}

function pushToNugetServer
{
    Param ([string] $apikey)

    $filter = "bin\release\*.symbols.nupkg"

    Get-ChildItem -Filter "$filter" | ForEach-Object {     
        $filename = $_.FullName

        Write-Host "Copying package to nuget cache directory..."
	    copy-Item $filename "c:\NugetPackages"

        Write-Host "Publishing to nuget..."

        dotnet nuget push $filename --api-key $apiKey --source "https://www.nuget.org"
	    Remove-Item $filename
    }
}

updateProjectVersionNumber

# dotnet pack  -c release --include-symbols --include-source

Write-Host ""

Write-Host "Building and creating nuget package..."

dotnet pack -c release --include-symbols

$apiKey = Get-Content "c:\projects\eb_nuget_apikey.txt"

Write-Host ""
Write-Host "Pushing package to nuget server..."

pushToNugetServer $apiKey

