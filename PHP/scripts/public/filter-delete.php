<?php
// Filter script is responsible for CRUD of spam filters
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: GET, POST, OPTIONS');
header('Access-Control-Allow-Headers: Content-Type, origin, authorization, accept, client-security-token, host, date, cookie, cookie2');

if ($_SERVER['REQUEST_METHOD'] == 'OPTIONS')
{
	http_response_code(200);
	exit;
}

error_reporting(E_ALL ^ E_DEPRECATED);

require_once __DIR__ . '/../../lib/Auth.php';
require_once __DIR__ . '/../../lib/Rules.php';

\SpamProtector\Auth::verifyAuth();

$rules = new \SpamProtector\Rules(false);

if ($_SERVER['REQUEST_METHOD'] != 'POST')
{
	http_response_code(403);
	exit;
}

$data = json_decode(file_get_contents('php://input'), true);

if (!isset($data['id']))
{
	http_response_code(403);
	exit;
}

try
{
	$rules->delete($data['id']);
}
catch (\Exception $ex)
{
	http_response_code(500);
	exit;
}

http_response_code(200);
exit;
