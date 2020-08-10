<?php
// Filter script is responsible for CRUD of spam filters

error_reporting(E_ALL ^ E_DEPRECATED);

require_once __DIR__ . '/../../lib/Rules.php';

$rules = new \SpamProtector\Rules(false);

if ($_SERVER['REQUEST_METHOD'] != 'POST')
{
	http_response_code(403);
	exit;
}

$data = json_decode(file_get_contents('php://input'), true);

if (!isset($data['type']) || !isset($data['value']))
{
	http_response_code(403);
	exit;
}

try
{
	$rules->add($data['type'], $data['value']);
}
catch (\Exception $ex)
{
	http_response_code(500);
	exit;
}

header('Content-Type: application/json');
http_response_code(200);
exit;
