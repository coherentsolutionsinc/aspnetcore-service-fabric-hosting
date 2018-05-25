[CmdletBinding()]
param
(
    [Parameter(Position = 0)]
    [ValidateSet("Release", "Debug")]
    [string] $Configuration = 'Debug',
    [switch] $RunTests
)

Function Use-Invocation
{
    [CmdletBinding()]
    param
    (
        [Parameter(Mandatory=$true)]
        [scriptblock] $Invocation,
        [Parameter(Mandatory=$true)]
        [string] $Output,
        [string] $OperationText = $null,
        [string] $OnSuccessText = $null,
        [string] $OnFailureText = $null,
        [Parameter(Mandatory=$true, ParameterSetName='PassThru')]
        [switch] $PassThru,
        [Parameter(Mandatory=$true, ParameterSetName='AsBool')]
        [switch] $AsBool
    )
    $consoleWidth = $Host.UI.RawUI.WindowSize.Width
    $consoleCursorPosition = 0

    if ($OperationText -ne $null)
    {
        $statusWidth = 0
        if ($OnSuccessText -eq $null)
        {
            $OnSuccessText = '[O]'
        }
        if ($OnFailureText -eq $null)
        {
            $OnFailureText = '[F]'
        }
        if ($OnFailureText.Length -gt $OnSuccessText.Length)
        {
            $statusWidth = $OnFailureText.Length
            $OnSuccessText = "{0}{1,$($OnFailureText.Length)}" -f '', $OnSuccessText
        }
        else 
        {
            $statusWidth = $OnSuccessText.Length
            $OnFailureText = "{0}{1,$($OnSuccessText.Length)}" -f '', $OnFailureText
        }
        $operationWidth = $consoleWidth - $statusWidth - 2
        if ($operationWidth -gt 0)
        {
            $width = $operationWidth
            if ($OperationText.Length -lt $operationWidth)
            {
                Write-Host $OperationText -NoNewline

                $width -= $OperationText.Length
            }
            else
            {
                $OperationText -split ' ' `
                    | ForEach-Object `
                    {
                        if ($width -lt ($_.Length + 1))
                        {
                            Write-Host
                            $width = $operationWidth
                        }

                        Write-Host "$_ " -NoNewline

                        $width -= $_.Length + 1
                    }
            }

            Write-Host $("{0, $width}" -f '') -NoNewline

            $consoleCursorPosition = $host.UI.RawUI.CursorPosition

            Write-Host
        }
    }

    $MyInvocation | Out-File $Output -Append

    try 
    {
        try
        {
            $result = Invoke-Command -ScriptBlock $Invocation -NoNewScope -ErrorAction Stop
        }
        finally
        {
            $consoleCurrentCursorPosition = $host.UI.RawUI.CursorPosition
            $host.UI.RawUI.CursorPosition = $consoleCursorPosition
        }
    }
    catch 
    {
        if ($OperationText -ne $null)
        {
            Write-Host $OnFailureText -ForegroundColor Red
            
            $host.UI.RawUI.CursorPosition = $consoleCurrentCursorPosition
        }


        $_ | Out-File $Output -Append

        if ($AsBool)
        {
            return $false
        }

        throw
    }

    $failure = -not $?
    
    $result | Out-File $Output -Append

    if ($failure)
    {
        if ($OperationText -ne $null)
        {
            Write-Host $OnFailureText -ForegroundColor Red

            $host.UI.RawUI.CursorPosition = $consoleCurrentCursorPosition
        }

        Write-Error "Cannot execute command. Please see '$Output' for details."

        return
    }
    if ($PassThru)
    {
        if ($OperationText -ne $null)
        {
            Write-Host $OnSuccessText -ForegroundColor Green

            $host.UI.RawUI.CursorPosition = $consoleCurrentCursorPosition
        }

        return $result
    }
    if ($AsBool)
    {
        if (($result -eq $null) -or (([string]$result).ToLower() -eq 'false'))
        {
            if ($OperationText -ne $null)
            {
                Write-Host $OnFailureText -ForegroundColor Red

                $host.UI.RawUI.CursorPosition = $consoleCurrentCursorPosition
            }

            return $false
        }
    }

    if ($OperationText -ne $null)
    {
        Write-Host $OnSuccessText -ForegroundColor Green

        $host.UI.RawUI.CursorPosition = $consoleCurrentCursorPosition
    }

    return $true
}

if (-not (Get-Command 'dotnet' -ErrorAction SilentlyContinue))
{
    Write-Error 'Could not find dotnet'
    return
}
Write-Verbose -Message "dotnet --version: $(& dotnet --version)"

$root = $PSScriptRoot
$build_log = Join-Path -Path $root -ChildPath "build.log" -ErrorAction Stop
$tests_log = Join-Path -Path $root -ChildPath "tests.log" -ErrorAction Stop
$command_log = Join-Path -Path $root -ChildPath "command.log" -ErrorAction Stop

if (Test-Path -Path $build_log)
{
    Remove-Item -Path $build_log -Force -ErrorAction Stop
}
if (Test-Path -Path $tests_log)
{
    Remove-Item -Path $tests_log -Force -ErrorAction Stop
}
if (Test-Path -Path $command_log)
{
    Remove-Item -Path $command_log -Force -ErrorAction Stop
}

Write-Verbose -Message "location (root): $root"
Write-Verbose -Message "location (build log): $build_log"
Write-Verbose -Message "location (tests log): $tests_log"
Write-Verbose -Message "location (command log): $command_log"

$result = Use-Invocation `
    -OperationText 'Building Sources' `
    -OnSuccessText '[DONE]' `
    -OnFailureText '[FAILED]' `
    -Invocation `
        { 
            $working_path = Join-Path -Path $root -ChildPath 'src'

            Push-Location $working_path

            & dotnet build --configuration "$Configuration" | Out-File -FilePath $build_log

            if (-not $?)
            {
                return $false
            }

            Pop-Location

            return $true
        } `
    -Output $command_log `
    -AsBool

if (-not $result)
{
    Write-Error "The solution build ended with errors. Please see '$build_log' for details."
    return
}

if ($RunTests)
{
    $result = Use-Invocation `
        -OperationText 'Running Tests' `
        -OnSuccessText '[DONE]' `
        -OnFailureText '[FAILED]' `
        -Invocation `
            { 
                $working_path = Join-Path -Path $root -ChildPath 'src/CoherentSolutions.AspNetCore.ServiceFabric.Hosting.Tests'

                Push-Location $working_path

                & dotnet test --no-build --no-restore | Out-File -FilePath $tests_log

                if (-not $?)
                {
                    return $false
                }

                Pop-Location

                return $true
            } `
        -Output $command_log `
        -AsBool

    if (-not $result)
    {
        Write-Error "Service Fabric application packaging ended with errors. Please see '$build_log' for details."
        return
    }
}