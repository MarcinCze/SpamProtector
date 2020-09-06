<?php
namespace SpamProtector;

require_once __DIR__ . '/Config.php';

class Report
{
	function __construct() 
	{ }
	
	function markForDelete()
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

		$stmt = $conn->prepare("UPDATE `spamprotector_mails` SET `toBeRemovedOn`=NOW() + INTERVAL 3 DAY WHERE `toBeRemovedOn` IS NULL AND `removedOn` IS NULL");
		$stmt->execute();
		$stmt->close();
		
		$sql = "SELECT * from `spamprotector_mails` WHERE `catalogedOn` IS NOT NULL AND `toBeRemovedOn` IS NOT NULL AND `removedOn` IS NULL";
		$result = $conn->query($sql);

		$mailsForRemoval = array();
		
		if ($result->num_rows > 0) 
		{
			while($row = $result->fetch_assoc()) 
			{
				$mailsForRemoval[] = $row;
			}
		} 
		
		$conn->close();
		
		return array('reportOverallResult' => $this->SendMail($mailsForRemoval));
	}
	
	private function SendMail($mailsForRemoval)
	{
		// Recipients
		$to = 'mar.czernecki@gmail.com';

		// Subject
		$subject = 'SpamProtector Report';

		// Message
		$message = '
		<!doctype html>
		<html lang="en">
		  <head>
			<!-- Required meta tags -->
			<meta charset="utf-8">
			<meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">

			<!-- Bootstrap CSS -->
			<link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css" integrity="sha384-JcKb8q3iqJ61gNV9KGb8thSsNjpSL0n8PARn9HuZOnIxN0hoP+VmmDGMN5t9UJ0Z" crossorigin="anonymous">

			<title>SpamProtector Report %%DATE%%</title>
		  </head>
		  <body>
			<h2>SpamProtector Report</h2>
			<h3>%%DATE%%</h3>
			
			%%CONTENT%%

			<!-- Optional JavaScript -->
			<!-- jQuery first, then Popper.js, then Bootstrap JS -->
			<!-- <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js" integrity="sha384-DfXdz2htPH0lsSSs5nCTpuj/zy4C+OGpamoFVy38MVBnE+IbbVYUew+OrCXaRkfj" crossorigin="anonymous"></script> -->
			<!-- <script src="https://cdn.jsdelivr.net/npm/popper.js@1.16.1/dist/umd/popper.min.js" integrity="sha384-9/reFTGAW83EW2RDu2S0VKaIzap3H66lZH81PoYlFhbGU+6BZp6G7niu735Sk7lN" crossorigin="anonymous"></script> -->
			<!-- <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js" integrity="sha384-B4gt1jrGC7Jh4AgTPSdUtOBvfO8shuf57BaghqFfPlYxofvL8/KUEfYiJOMMV+rV" crossorigin="anonymous"></script> -->
		  </body>
		</html>
		';
		
		$contentTemplate = '
			<br><br>
			<table style="width:100%">
			  <tr><th>Id</th><td>%%ID%%</td></tr>
			  <tr><th>Mailbox</th><td>%%MAILBOX%%</td></tr>
			  <tr><th>UID</th><td>%%UID%%</td></tr>
			  <tr><th>Date</th><td>%%DATE%%</td></tr>
			  <tr><th>Sender</th><td>%%SENDER%%</td></tr>
			  <tr><th>Recipient</th><td>%%RECIPIENT%%</td></tr>
			  <tr><th>Subject</th><td>%%SUBJECT%%</td></tr>
			  <tr><th>Catalog time</th><td>%%CATALOG_TIME%%</td></tr>
			  <tr><th>Remove time</th><td>%%REMOVE_TIME%%</td></tr>
			</table>
			<br><br>
			<hr>
		';
		
		// [id] => 889
		// [mailbox] => MARCIN
		// [imapUid] => 1421
		// [sentDateTime] => 2020-08-27 12:48:06
		// [sender] => annadzthhhx@gastecity.com
		// [recipient] => marcin@mczernecki.pl
		// [subject] => [SPAM] Oto jak odnowić tępy nóż lub inne narzędzie tnące: 3-fazowa ostrzałka do noży
		// [content] => 
		// [catalogedOn] => 2020-08-27 14:48:55
		// [toBeRemovedOn] => 2020-08-27 15:18:27
		// [removedOn] => 
		
		$content = '';
		foreach($mailsForRemoval as $mail)
		{
			$mailTemplate = $contentTemplate;
			$mailTemplate = str_replace("%%ID%%", $mail['id'], $mailTemplate);
			$mailTemplate = str_replace("%%MAILBOX%%", $mail['mailbox'], $mailTemplate);
			$mailTemplate = str_replace("%%UID%%", $mail['imapUid'], $mailTemplate);
			$mailTemplate = str_replace("%%DATE%%", $mail['sentDateTime'], $mailTemplate);
			$mailTemplate = str_replace("%%SENDER%%", $mail['sender'], $mailTemplate);
			$mailTemplate = str_replace("%%RECIPIENT%%", $mail['recipient'], $mailTemplate);
			$mailTemplate = str_replace("%%SUBJECT%%", $mail['subject'], $mailTemplate);
			$mailTemplate = str_replace("%%CATALOG_TIME%%", $mail['catalogedOn'], $mailTemplate);
			$mailTemplate = str_replace("%%REMOVE_TIME%%", $mail['toBeRemovedOn'], $mailTemplate);
			$content = $content . $mailTemplate;
		}
		
		$date = new \DateTime();
		$message = str_replace("%%DATE%%", $date->format('Y-m-d'), $message);
		$message = str_replace("%%CONTENT%%", $content, $message);

		// To send HTML mail, the Content-type header must be set
		$headers[] = 'MIME-Version: 1.0';
		$headers[] = 'Content-type: text/html; charset=utf-8';

		// Additional headers
		//$headers[] = 'To: Marcin <mar.czernecki@gmail.com>';
		$headers[] = 'From: SpamProtector <no-reply@mczernecki.pl>';

		// Mail it
		$result = mail($to, $subject, $message, implode("\r\n", $headers));
		return array('sent' => $result, 'sentTo' => $to);
	}
}