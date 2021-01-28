Write-Host "=============== CHANGING ASSEMBLY VERSION SCRIPT ==============="

$buildNumber = $env:BUILD_NUMBER
$workspace = $env:WORKSPACE
Write-Host "Build number: $buildNumber"
Write-Host "Workspace: $workspace"
$pathPrefix = "$workspace\Dotnet\SpamProtector"

function ModifyVersion
{
  param ($line)
  
  $newLine = $line.substring(0, $line.lastIndexOf(".")+1)
  $newLine += $buildNumber
  $newLine += $line.substring($line.lastIndexOf("</"))
  
  return $newLine
}

function Analyze-File 
{
	param ($fileName)
    Write-Host "Analyzing file $file"
  
    $stream_reader = New-Object System.IO.StreamReader($file)
    $line_number = 1
    $content = @()
    
    while (($current_line =$stream_reader.ReadLine()) -ne $null)
    {
        $content += $current_line
        $line_number++
    }
    
    $stream_reader.Close();
    $stream_reader.Dispose();
    $stream_writer = New-Object System.IO.StreamWriter($file)
    
    foreach ($line in $content) 
    {
      if ($line -Match "<Version>" -or $line -Match "<FileVersion>")
      {
        $line = ModifyVersion($line)
      }
      
      $stream_writer.WriteLine($line)
    }
    
    $stream_writer.Close()
    $stream_writer.Dispose()  
}

$files = @()
$files += "$pathPrefix\CatalogMainService\CatalogMainService.csproj"
$files += "$pathPrefix\CatalogSpamService\CatalogSpamService.csproj"
$files += "$pathPrefix\ScanService\ScanService.csproj"
$files += "$pathPrefix\ProtectorLib\ProtectorLib.csproj"
$files += "$pathPrefix\DeleteMainService\DeleteMainService.csproj"
$files += "$pathPrefix\DeleteSpamService\DeleteSpamService.csproj"
$files += "$pathPrefix\MarkingService\MarkingService.csproj"

foreach ($file in $files)
{
  Analyze-File($file)
}

Write-Host "=============== END OF CHANGING ASSEMBLY VERSION SCRIPT ==============="