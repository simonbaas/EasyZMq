# SetAssemblyInfoVersion.ps1
# Based on https://gist.github.com/derekgates/4678882

function Help {
    "Sets the AssemblyVersion and AssemblyFileVersion of AssemblyInfo.cs files`n"
    ".\SetAssemblyVersion.ps1 [VersionNumber] -path [SearchPath]`n"
    "   [VersionNumber]     The version number to set, for example: 1.1.9301.0"
    "   [SearchPath]        The path to search for AssemblyInfo files.`n"
}

function Update-SourceVersion
{
    Param ([string]$Version)
    $NewVersion = 'AssemblyVersion("' + $Version + '")';
    $NewFileVersion = 'AssemblyFileVersion("' + $Version + '")';

    foreach ($o in $input) 
    {
        Write-Host "Updating  '$($o.FullName)' -> $Version"
    
        $assemblyVersionPattern = 'AssemblyVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
        $fileVersionPattern = 'AssemblyFileVersion\("[0-9]+(\.([0-9]+|\*)){1,3}"\)'
        $assemblyVersion = 'AssemblyVersion("' + $version + '")';
        $fileVersion = 'AssemblyFileVersion("' + $version + '")';
        
        (Get-Content $o.FullName) | ForEach-Object  { 
           % {$_ -replace $assemblyVersionPattern, $assemblyVersion } |
           % {$_ -replace $fileVersionPattern, $fileVersion }
        } | Out-File $o.FullName -encoding UTF8 -force
    }
}

function Update-AllAssemblyInfoFiles ( $version )
{
    Write-Host "Searching '$path'"
   foreach ($file in "AssemblyInfo.cs", "AssemblyInfo.vb" ) 
   {
        get-childitem $path -recurse |? {$_.Name -eq $file} | Update-SourceVersion $version ;
   }
}

# validate arguments 
if ($args -ne $null) {
    $version = $args[0]
    if (($version -eq '/?') -or ($version -notmatch "[0-9]+(\.([0-9]+|\*)){1,3}")) {
        Help
        exit 1;
    }
} else {
    Help
    exit 1;
}

Update-AllAssemblyInfoFiles $version

