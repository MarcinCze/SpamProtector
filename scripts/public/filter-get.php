<?php
// Filter script is responsible for CRUD of spam filters

error_reporting(E_ALL ^ E_DEPRECATED);

require_once __DIR__ . '/../../lib/Auth.php';
require_once __DIR__ . '/../../lib/Rules.php';

\SpamProtector\Auth::verifyAuth();

$rules = new \SpamProtector\Rules(false);

$output = null;

if ($_SERVER['REQUEST_METHOD'] != 'POST')
{
	http_response_code(403);
	exit;
}

$data = json_decode(file_get_contents('php://input'), true);

if (!isset($data['type']))
{
	http_response_code(403);
	exit;
}

switch($data['type'])
{
	case 'subjects':
		$output = $rules->getSubjects();
		break;
	case 'senders':
		$output = $rules->getSenders();
		break;
	case 'domains':
		$output = $rules->getDomains();
		break;
}

header('Content-Type: application/json');

if ($output == null)
{
	http_response_code(404);
}
else 
{
	echo(json_encode($output));	
}

exit;
