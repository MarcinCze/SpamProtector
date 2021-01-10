<?php
namespace SpamProtector\Mailbox;

require_once 'BaseMailbox.php'; 
require_once __DIR__ . '/../Rules.php';

class MainMailbox extends BaseMailbox
{
	private $rules;
	
	function __construct() 
	{ 
		$this->rules = new \SpamProtector\Rules();
		$this->hostname = \SpamProtector\Configuration::MainMailbox['hostname'];
		$this->username = \SpamProtector\Configuration::MainMailbox['login'];
		$this->password = \SpamProtector\Configuration::MainMailbox['password'];
		$this->mailboxName = 'MARCIN';
		$this->spamDir = '/Junk';
	}
	
	function detectSpam()
	{
		$result = array('mailsMoved' => 0, 'mails' => array());
		$inbox = imap_open($this->hostname, $this->username, $this->password);
		$emails = imap_search($inbox, 'UNSEEN');

		if($emails) 
		{				
			rsort($emails);

			foreach($emails as $email_number) 
			{
				$overview = imap_headerinfo ($inbox, $email_number, 0);
				
				$from = $overview->from[0]->mailbox . '@' . $overview->from[0]->host;
				
				if(property_exists($overview, "subject"))
				{
					$subject = $this->decodeSubject($overview->subject);
				}
				else
				{
					$subject = 'NO SUBJECT';
				}
		
				if($this->isSpam($from, $subject))
				{
					$uid = imap_uid($inbox, $email_number);
					$movingResult = imap_mail_move($inbox, $uid, 'INBOX/Junk', CP_UID);
					
					if($movingResult)
					{
						$result['mails'][] = array('from' => $from, 'subject' => $subject, 'resolution' => 'MOVED');
						$result['mailsMoved'] = $result['mailsMoved'] + 1;
					}
					else
					{
						$result['mails'][] = array('from' => $from, 'subject' => $subject, 'resolution' => 'MOVED FAILURE');
					}
				}
			}
		}
		
		imap_expunge($inbox);
		imap_close($inbox);
		return $result;
	}
	
	function catalogSpam()
	{
		return $this->catalog();
	}
	
	private function isSpam($sender, $subject)
	{
		if($this->rules->isOnSenderBlacklist($sender)) 
		{
			return true;
		}
		
		if($this->rules->isOnDomainBlacklist($sender)) 
		{
			return true;
		}

		if($this->rules->isOnSubjectBlacklist($subject))
		{
			return true;
		}
		
		return false;
	}
	
}