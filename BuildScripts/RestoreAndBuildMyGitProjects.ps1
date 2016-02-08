[CmdletBinding()]
param()

$mygit = "C:\MyGit"

$projectJsonPaths = "C:\MyGit\Common\src\Common.Core",
                    "C:\MyGit\Common\src\Common.Mailer",
                    "C:\MyGit\Common\src\Common.ProcessExecution",
                    "C:\MyGit\CodingCode\src\CodingCode.Model",
                    "C:\MyGit\CodingCode\src\CodingCode.ViewModel",
                    "C:\MyGit\CodingCode\src\CodingCode.Abstraction",
                    "C:\MyGit\CodingCode\src\CodingCode.Services",
                    "C:\MyGit\CodingCode\src\CodingCode.Web",
                    "C:\MyGit\CodingCode\test\CodingCode.IntegrationTest",
                    "C:\MyGit\ContinuousIntegration\ContinuousIntegration.TestRunner"
                    
function Main()
{
    foreach($directoryToRunFrom in $projectJsonPaths)
    {
        Write-Host "cd to $directoryToRunFrom" -BackgroundColor Green
        cd $directoryToRunFrom
        dnu restore
        dnu build
        cd $myGit
    }
}

Main