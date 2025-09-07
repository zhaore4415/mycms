param(
    [string]$Path = ".",
    [string]$OutputFile = "README.md"
)

# 强制控制台输出为 UTF-8
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

$ExcludeNames = @(
    'bin', 'obj', 'Debug', 'Release', '.vs', 'packages', 'node_modules',
    '.nuget', 'logs', 'temp', 'tmp', 'Backup', '_ReSharper', '.dotnet'
)

function Should-Exclude {
    param([string]$Name)
    foreach ($ex in $ExcludeNames) {
        if ($Name -like "$ex*" -or $Name -eq $ex) { return $true }
    }
    return $false
}

function Write-Tree {
    param([System.IO.DirectoryInfo]$Dir, [string]$Indent = "", [bool]$IsLast = $true)
    
    $Connector = if ($IsLast) { "`- " } else { "+- " }
    $Result = $Indent + $Connector + $Dir.Name + "/"

    $Children = @($Dir.GetDirectories() | Where-Object { -not (Should-Exclude $_.Name) })
    for ($i = 0; $i -lt $Children.Length; $i++) {
        $IsLastChild = ($i -eq ($Children.Length - 1))
        $Prefix = if ($IsLast) { "   " } else { "|  " }
        $ChildResult = Write-Tree -Dir $Children[$i] -Indent ($Indent + $Prefix) -IsLast:$IsLastChild
        $Result += "`n" + $ChildResult
    }
    return $Result
}

try {
    $RootDir = Get-Item $Path
    $OutputLines = @()
    $OutputLines += "## Project Directory Structure"
    $OutputLines += ""
    $OutputLines += '```tree'
    $OutputLines += "$($RootDir.Name)/"

    $SubDirs = @($RootDir.GetDirectories() | Where-Object { -not (Should-Exclude $_.Name) })
    for ($i = 0; $i -lt $SubDirs.Length; $i++) {
        $IsLast = ($i -eq ($SubDirs.Length - 1))
        $SubTree = Write-Tree -Dir $SubDirs[$i] -Indent "" -IsLast:$IsLast
        if ($SubTree) { $OutputLines += $SubTree }
    }

    $OutputLines += '```'

    # ✅ 关键：使用 UTF-8 with BOM
    $Utf8WithBom = New-Object System.Text.UTF8Encoding($true)
    $FullPath = Join-Path (Get-Location) $OutputFile

    # ✅ 使用 .NET 方法写文件
    [System.IO.File]::WriteAllLines($FullPath, $OutputLines, $Utf8WithBom)

    Write-Host "✅ 文件已保存：" $FullPath -ForegroundColor Green

} catch {
    Write-Error ("❌ 脚本失败: " + $_.Exception.Message)
}