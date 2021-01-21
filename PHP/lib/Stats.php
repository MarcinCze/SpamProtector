<?php
namespace SpamProtector;

require_once __DIR__ . '/Config.php';

class Stats
{
	function __construct() 
	{ }
	
	function getStats()
	{
		$results = $this->extractFromDb();

		return array(
		'scripts' => array(
			'scan' => $results['scan'],
			'catalogMain' => $results['catalogMain'],
			'catalogSpam' => $results['catalogSpam'],
			'markForDelete' => $results['markForDelete'],
			'removeMain' => $results['removeMain'],
			'removeSpam' => $results['removeSpam']
		),
		'rules' => array(
			'domain' => array('count' => $results['domainCount'], 'used' => $results['domainUsed']),
			'sender' => array('count' => $results['senderCount'], 'used' => $results['senderUsed']),
			'subject' => array('count' => $results['subjectCount'], 'used' => $results['subjectUsed']),
		),
		'catalog' => array(
			'totalEntries' => $results['catalogTotalEntries'],
			'readyToRemove' => $results['catalogReadyToRemove'],
			'removed' => $results['catalogRemoved'],
			'new' => $results['catalogNew']
		));
	}
	
	private function extractFromDb()
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

		$sql = "
			SELECT 
				(SELECT value FROM spamprotector_settings WHERE name = 'last_run_scan') as scan,
				(SELECT value FROM spamprotector_settings WHERE name = 'last_run_catalog_main') as catalogMain,
				(SELECT value FROM spamprotector_settings WHERE name = 'last_run_catalog_spam') as catalogSpam,
				(SELECT value FROM spamprotector_settings WHERE name = 'last_run_mark_for_delete') as markForDelete,
				(SELECT value FROM spamprotector_settings WHERE name = 'last_run_remove_main') as removeMain,
				(SELECT value FROM spamprotector_settings WHERE name = 'last_run_remove_spam') as removeSpam,
				(SELECT count(*) FROM spamprotector_rules WHERE field = 'domain') as domainCount,
				(SELECT SUM(used) FROM spamprotector_rules WHERE field = 'domain') as domainUsed,
				(SELECT count(*) FROM spamprotector_rules WHERE field = 'sender') as senderCount,
				(SELECT SUM(used) FROM spamprotector_rules WHERE field = 'sender') as senderUsed,
				(SELECT count(*) FROM spamprotector_rules WHERE field = 'subject') as subjectCount,
				(SELECT SUM(used) FROM spamprotector_rules WHERE field = 'subject') as subjectUsed,
				(SELECT count(*) from spamprotector_mails) as catalogTotalEntries,
				(SELECT count(*) from spamprotector_mails WHERE removedOn IS NULL AND toBeRemovedOn IS NOT NULL) as catalogReadyToRemove,
				(SELECT count(*) from spamprotector_mails WHERE removedOn IS NOT NULL) as catalogRemoved,
				(SELECT count(*) from spamprotector_mails WHERE removedOn IS NULL AND tobeRemovedOn IS NULL) as catalogNew
		";
		
		$result = $conn->query($sql);
		$data = null;
		if ($result->num_rows > 0) 
		{
			while($row = $result->fetch_assoc()) 
			{
				$data = $row;
			}
		} 
		
		$conn->close();
		
		return $data;
	}
}


// Array
// (
    // [scan
// ] => 2020-09-06 16: 40: 35
    // [catalogMain
// ] => 2020-09-06 16: 28: 43
    // [catalogSpam
// ] => 2020-09-06 16: 08: 19
    // [markForDelete
// ] => 2020-09-06 16: 30: 20
    // [removeMain
// ] => 2020-09-06 16: 30: 54
    // [removeSpam
// ] => 2020-09-06 16: 09: 47
    // [rulesCount
// ] => 2
    // [rulesUsed
// ] => 4
    // [senderCount
// ] => 5
    // [senderUsed
// ] => 23
    // [subjectCount
// ] => 19
    // [subjectUsed
// ] => 766
    // [catalogTotalEntries
// ] => 3
    // [catalogReadyToRemove
// ] => 2
    // [catalogRemoved
// ] => 1
    // [catalogNew
// ] => 0
// )
