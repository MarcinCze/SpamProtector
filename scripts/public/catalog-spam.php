<?php
// Catalog SPAM script is responsible for scanning main mailbox, check spam rules and move spam if detected

//error_reporting(E_ALL ^ E_DEPRECATED);

header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: GET, POST, OPTIONS');
header('Access-Control-Allow-Headers: Content-Type, origin, authorization, accept, client-security-token, host, date, cookie, cookie2');

require_once __DIR__ . '/../../lib/Mailbox/SpamMailbox.php';

// Script start
$start_time = microtime(true); 

$output = null;

try 
{
	$mainMailbox = new \SpamProtector\Mailbox\SpamMailbox();
	$result = $mainMailbox->catalogSpam();
	
	http_response_code(200);
	$output = array(
		'result' => 'SUCCESS',
		'details' => $result
	);
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
