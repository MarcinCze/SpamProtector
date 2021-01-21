<?php
// Scan SPAM script is responsible for scanning main mailbox, check spam rules and move spam if detected

//error_reporting(E_ALL ^ E_DEPRECATED);

header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: GET, POST, OPTIONS');
header('Access-Control-Allow-Headers: Content-Type, origin, authorization, accept, client-security-token, host, date, cookie, cookie2');

require_once __DIR__ . '/../../lib/Stats.php';

try 
{
	$stats = new \SpamProtector\Stats();
	
	http_response_code(200);
	echo(json_encode(array(
		'result' => 'SUCCESS',
		'details' => $stats->getStats()
	)));
}
catch (\Exception $e)
{
	http_response_code(500);
	echo(json_encode(array(
		'result' => 'ERROR',
		'details' => $e->getMessage()
	)));
}

exit;
