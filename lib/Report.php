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
		  die("Connection failed: " . $conn->connect_error);
		}

		$stmt = $conn->prepare("UPDATE `spamprotector_mails` SET `toBeRemovedOn`=NOW() WHERE `toBeRemovedOn` IS NULL AND `removedOn` IS NULL");
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

			<title>SpamProtector Report XX-XX-XXXX</title>
		  </head>
		  <body>
			<h2>SpamProtector Report</h2>
			<h3>XX-XX-XXXX</h3>
			
			<p>todo</p>

			<!-- Optional JavaScript -->
			<!-- jQuery first, then Popper.js, then Bootstrap JS -->
			<script src="https://code.jquery.com/jquery-3.5.1.slim.min.js" integrity="sha384-DfXdz2htPH0lsSSs5nCTpuj/zy4C+OGpamoFVy38MVBnE+IbbVYUew+OrCXaRkfj" crossorigin="anonymous"></script>
			<script src="https://cdn.jsdelivr.net/npm/popper.js@1.16.1/dist/umd/popper.min.js" integrity="sha384-9/reFTGAW83EW2RDu2S0VKaIzap3H66lZH81PoYlFhbGU+6BZp6G7niu735Sk7lN" crossorigin="anonymous"></script>
			<script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js" integrity="sha384-B4gt1jrGC7Jh4AgTPSdUtOBvfO8shuf57BaghqFfPlYxofvL8/KUEfYiJOMMV+rV" crossorigin="anonymous"></script>
		  </body>
		</html>
		';

		// To send HTML mail, the Content-type header must be set
		$headers[] = 'MIME-Version: 1.0';
		$headers[] = 'Content-type: text/html; charset=utf-8';

		// Additional headers
		$headers[] = 'To: Marcin <mar.czernecki@gmail.com>';
		$headers[] = 'From: SpamProtector <no-reply@mczernecki.pl>';

		// Mail it
		$result = mail($to, $subject, $message, implode("\r\n", $headers));
		return array('sent' => $result, 'sentTo' => $to);
	}
}