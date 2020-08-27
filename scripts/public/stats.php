<?php
// Scan SPAM script is responsible for scanning main mailbox, check spam rules and move spam if detected

//error_reporting(E_ALL ^ E_DEPRECATED);

header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: GET, POST, OPTIONS');
header('Access-Control-Allow-Headers: Content-Type, origin, authorization, accept, client-security-token, host, date, cookie, cookie2');

try 
{
	$result = array(
		'scripts' => array(
			'scan' => '2020-08-25 14:45:00',
			'catalog' => '2020-08-25 11:00:00'
		),
		'rules' => array(
			'domain' => array('count' => 0, 'used' => 0),
			'sender' => array('count' => 5, 'used' => 15),
			'subject' => array('count' => 29, 'used' => 1478),
		),
		'catalog' => array(
			'totalEntries' => 560,
			'readyToRemove' => 250,
			'removed' => 300,
			'new' => 10
		)
	);
	
	http_response_code(200);
	echo(json_encode(array(
		'result' => 'SUCCESS',
		'details' => $result
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
