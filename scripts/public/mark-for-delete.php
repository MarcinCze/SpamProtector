<?php
// Mark for Delete is responsible to find all mails (in database) that must be removed, marked it and send report by mail

//error_reporting(E_ALL ^ E_DEPRECATED);

header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: GET, POST, OPTIONS');
header('Access-Control-Allow-Headers: Content-Type, origin, authorization, accept, client-security-token, host, date, cookie, cookie2');

require_once __DIR__ . '/../../lib/Report.php';
require_once __DIR__ . '/../../lib/Settings.php';

// Script start
$start_time = microtime(true); 

$output = null;

try 
{
	$report = new \SpamProtector\Report();
	$result = $report->markForDelete();
	
	http_response_code(200);
	$output = array(
		'result' => 'SUCCESS',
		'details' => $result
	);
	
	$settings = new \SpamProtector\Settings();
	$settings->UpdateLastRunMarkForDelete();
}
catch (\Exception $e)
{
	http_response_code(500);
	$output = array(
		'result' => 'ERROR',
		'details' => $e->getMessage()
	);
}

// Script end
$end_time = microtime(true); 
$execution_time = ($end_time - $start_time); 
$output['executionTime'] = $execution_time." sec";
  
echo(json_encode($output));
exit;
