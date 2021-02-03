Write-Host "=============== CHANGING CONFIG SCRIPT ==============="

$DBConnectionString = ""
$MainBoxUrl = ""
$MainBoxPort = 0
$MainBoxUser = ""
$MainBoxPassword = ""
$SpamBoxUrl = ""
$SpamBoxPort = 0
$SpamBoxUser = ""
$SpamBoxPassword = ""

function Analyze-Config 
{
	param ($pathToJson)
    Write-Host "Analyzing file $file"
  
    $a = Get-Content $pathToJson | ConvertFrom-Json
  
  	#DATABASE
    $a.ConnectionStrings.'SpamProtectorDBContext' = $DBConnectionString
  	
  	#MAILBOX - MAIN
  	$a.Mailboxes.'MainBox'.'Url' = $MainBoxUrl
  	$a.Mailboxes.'MainBox'.'Port' = $MainBoxPort
  	$a.Mailboxes.'MainBox'.'UserName' = $MainBoxUser
  	$a.Mailboxes.'MainBox'.'Password' = $MainBoxPassword
  
  	#MAILBOX - SPAM
  	$a.Mailboxes.'SpamBox'.'Url' = $SpamBoxUrl
  	$a.Mailboxes.'SpamBox'.'Port' = $SpamBoxPort
  	$a.Mailboxes.'SpamBox'.'UserName' = $SpamBoxUser
  	$a.Mailboxes.'SpamBox'.'Password' = $SpamBoxPassword
  
    $a | ConvertTo-Json -depth 4 | set-content $pathToJson  
}

$workspace = $env:WORKSPACE
$pathPrefix = "$workspace\Dotnet\SpamProtector"

$files = @()
$files += "$pathPrefix\Shared\appsettings.json"
#$files += "$pathPrefix\CatalogService\appsettings.json"
#$files += "$pathPrefix\ScanService\appsettings.json"
#$files += "$pathPrefix\DeleteService\appsettings.json"
#$files += "$pathPrefix\MarkingService\appsettings.json"

foreach ($file in $files)
{
  Analyze-Config($file)
}


Write-Host "=============== END OF CHANGING CONFIG SCRIPT ==============="