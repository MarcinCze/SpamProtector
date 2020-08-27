<?php
namespace SpamProtector;

class Configuration
{
	public const MainMailbox = array(
		'login' => 'marcin@mczernecki.pl',
		'password' => 'htoLMebaHN',
		'hostname' => '{ssl0.ovh.net:993/imap/ssl}INBOX',
	);
	
	public const SpamMailbox = array(
		'login' => 'spam@mczernecki.pl',
		'password' => '4y1UMx2tEd',
		'hostname' => '{ssl0.ovh.net:993/imap/ssl}INBOX',
	);
	
	public const Database = array(
		'server' => 'mczernecmain.mysql.db',
		'db' => 'mczernecmain',
		'username' => 'mczernecmain',
		'password' => 'WoHFYb7btB3B'
	);
	
	public function __construct() { }
}