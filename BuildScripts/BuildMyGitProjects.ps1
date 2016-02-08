[CmdletBinding()]
param()

$dnxPackages="C:\Users\mika\.dnx\packages"
$mygit = "C:\MyGit"


$projectJsonPaths = "C:\MyGit\Common\src\Common.Core",
                    "C:\MyGit\Common\src\Common.Mailer",
                    "C:\MyGit\Common\src\Common.ProcessExecution",
                    "C:\MyGit\CodingCode\src\CodingCode.Model",
                    "C:\MyGit\CodingCode\src\CodingCode.ViewModel",
                    "C:\MyGit\CodingCode\src\CodingCode.Abstraction",
                    "C:\MyGit\CodingCode\src\CodingCode.Services"
    
function Main()
{
    foreach($directoryToRunFrom in $projectJsonPaths)
    {
        Write-Host "cd to $directoryToRunFrom" -BackgroundColor Green
        cd $directoryToRunFrom
        dnu restore
        $dllName = Split-Path $directoryToRunFrom -leaf
        $NugetPath = Join-Path $dnxPackages $dllName
        if(Test-Path $NugetPath){
            Remove-Item $NugetPath -Recurse
            Write-Host "Removing $NugetPath" -BackgroundColor Green
        }
        Write-Host "publishing $dllName to $dnxPackages" -BackgroundColor Green
        $outPath = Join-Path (Join-Path $dnxPackages "approot\packages") $dllName
        dnu publish --no-source --out $dnxPackages; 
        Copy-Item  $outPath $dnxPackages -Recurse
        $publishedPath = Join-Path $dnxPackages $dllName
        Write-Host "publish to $publishedPath completed" -BackgroundColor Green
        if(Test-Path $publishedPath){
            Write-Host "Testing path $publishedPath" -BackgroundColor Green
        }
        cd $myGit
    }
}

Main