<?php
namespace SpamProtector\Mailbox;

require_once __DIR__ . '/../Config.php'; 

abstract class BaseMailbox
{
	protected $hostname;
	protected $username;
	protected $password;
	
	function __construct() 
	{ 
		
	}
}