<?php
namespace SpamProtector\Mailbox;

require_once 'BaseMailbox.php'; 
require_once __DIR__ . '/../Rules.php';

class SpamMailbox extends BaseMailbox
{
	private $rules;
	
	function __construct() 
	{ 
		$this->rules = new \SpamProtector\Rules();
		$this->hostname = \SpamProtector\Configuration::SpamMailbox['hostname'];
		$this->username = \SpamProtector\Configuration::SpamMailbox['login'];
		$this->password = \SpamProtector\Configuration::SpamMailbox['password'];
		$this->mailboxName = 'SPAM';
		$this->spamDir = '/';
	}
	
	function catalogSpam()
	{
		return $this->catalog();
	}
}