<?php
namespace SpamProtector;

require_once __DIR__ . '/Config.php';

class Settings
{
	function __construct() 
	{ }
	
	public function UpdateLastRunScan()
	{
		$this->UpdateLastRun('last_run_scan');
	}
	
	public function UpdateLastRunCatalogMain()
	{
		$this->UpdateLastRun('last_run_catalog_main');
	}
	
	public function UpdateLastRunCatalogSpam()
	{
		$this->UpdateLastRun('last_run_catalog_spam');
	}
	
	public function UpdateLastRunMarkForDelete()
	{
		$this->UpdateLastRun('last_run_mark_for_delete');
	}
	
	public function UpdateLastRunRemoveMain()
	{
		$this->UpdateLastRun('last_run_remove_main');
	}
	
	public function UpdateLastRunRemoveSpam()
	{
		$this->UpdateLastRun('last_run_remove_spam');
	}
	
	private function UpdateLastRun($key)
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

		$stmt = $conn->prepare("UPDATE `spamprotector_settings` SET `value`=NOW() WHERE `name` = '".$key."'");
		$stmt->execute();
		$stmt->close();
		
		$conn->close();
	}
}