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
	
	protected function catalogDirectory($directory, $mailbox)
	{
		$result = array('cataloged' => 0);
		$mailDetails = array();
		
		$inbox = imap_open($this->hostname . $directory, $this->username, $this->password);
		$emails = imap_search($inbox, 'ALL');
		
		if($emails) 
		{				
			rsort($emails);

			foreach($emails as $email_number) 
			{
				$overview = imap_header($inbox, $email_number, 0);
				
				$date = new \DateTime();
				$date->setTimestamp($overview->udate);
				$sentDt = $date->format('Y-m-d H:i:s P');
				$content = null;
				
				$mailDetails[] = array(
					'from' => $overview->from[0]->mailbox . '@' . $overview->from[0]->host,
					'to'   => $overview->to[0]->mailbox . '@' . $overview->to[0]->host,
					'uid'  => imap_uid($inbox, $email_number),
					'subject' => $this->decodeSubject($overview->subject),
					'content' => null,
					'sentDt'  => $sentDt,
					'mailbox' => $mailbox
				);
			}
		}
		
		imap_close($inbox);
		
		if (count($mailDetails) > 0)
		{
			foreach ($mailDetails as $mail)
			{
				if(!$this->isAlreadyInCatalog($mail['mailbox'], $mail['uid'], $mail['sentDt'], $mail['from'], $mail['to']))
				{
					$this->saveCatalogEntry($mail['mailbox'], $mail['uid'], $mail['sentDt'], $mail['from'], $mail['to'], $mail['subject'], $mail['content']);
					$result['cataloged'] = $result['cataloged'] + 1;
				}
			}
		}
		
		return $result;
	}
	
	protected function saveCatalogEntry($mailbox, $uid, $sentDt, $sender, $recipient, $subject, $content)
	{
		$servername = \SpamProtector\Configuration::Database['server'];
		$username = \SpamProtector\Configuration::Database['username'];
		$password = \SpamProtector\Configuration::Database['password'];
		$dbname = \SpamProtector\Configuration::Database['db'];

		$conn = new \mysqli($servername, $username, $password, $dbname);
		$conn->set_charset("utf8");
		
		if ($conn->connect_error) {
			throw new \Exception($conn->connect_error);
		}

		$stmt = $conn->prepare("INSERT INTO spamprotector_mails (mailbox, imapUid, sentDateTime, sender, recipient, subject, content, catalogedOn, toBeRemovedOn, removedOn) 
								VALUES (?, ?, ?, ?, ?, ?, ?, NOW(), NULL, NULL)"); 
		$stmt->bind_param("sisssss", $mailbox, $uid, $sentDt, $sender, $recipient, $subject, $content);
		$stmt->execute();
		
		$stmt->close();
		$conn->close();
	}
	
	protected function isAlreadyInCatalog($mailbox, $uid, $sentDt, $sender, $recipient)
	{
		$servername = \SpamProtector\Configuration::Database['server'];
		$username = \SpamProtector\Configuration::Database['username'];
		$password = \SpamProtector\Configuration::Database['password'];
		$dbname = \SpamProtector\Configuration::Database['db'];

		$conn = new \mysqli($servername, $username, $password, $dbname);
		
		if ($conn->connect_error) {
		  throw new \Exception($conn->connect_error);
		}

		$stmt = $conn->prepare("SELECT id FROM spamprotector_mails WHERE mailbox = ? AND imapUid = ? AND sentDateTime = ? AND sender = ? AND recipient = ?");
		$stmt->bind_param("sisss", $mailbox, $uid, $sentDt, $sender, $recipient);
		$stmt->execute();
		$result = $stmt->get_result();
		$exist = $result->num_rows > 0;
		$stmt->close();
		// var_dump($uid);  var_dump($exist); exit;
		return $exist;
	}
	
	protected function decodeSubject($subject)
	{
		$decoded = imap_mime_header_decode($subject);
		$output = '';
		
		if (!isset($decoded))
		{
			return $output;
		}
			
		foreach($decoded as $part)
		{
			$output = $output . $part->text;
		}
			
		return $output;
	}
}