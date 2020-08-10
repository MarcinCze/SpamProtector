<?php
namespace SpamProtector;

require_once __DIR__ . '/Config.php';

class Rules
{
	private $rulesSubject;
	private $rulesSender;
	private $rulesDomain;
	
	function __construct($loadRules = true) 
	{ 
		if ($loadRules)
			$this->LoadRules();
	}
	
	function isOnSenderBlacklist($sender)
	{
		return in_array($sender, $this->rulesSender, false);
	}
	
	function isOnDomainBlacklist($sender)
	{
		$domain = substr($sender, strrpos($sender, '@') + 1);
		return in_array($domain, $this->rulesDomain, false);
	}
	
	function isOnSubjectBlacklist($subject)
	{
		foreach ($this->rulesSubject as $rule)
		{				
			if (stripos($subject, $rule) !== false) 
			{
				return true;
			}
		}
				
		return false;
	}
	
	function getSubjects()
	{
		$this->loadRules();
		return $this->rulesSubject;
	}
	
	function getSenders()
	{
		$this->loadRules();
		return $this->rulesSender;
	}
	
	function getDomains()
	{
		$this->loadRules();
		return $this->rulesDomain;
	}
	
	function add($type, $value)
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

		$stmt = $conn->prepare("INSERT INTO spamprotector_rules (field, value, isActive) VALUES (?, ?, 1)");
		$stmt->bind_param("ss", $type, $value);
		$stmt->execute();
		
		$stmt->close();
		$conn->close();
	}
	
	function delete($id)
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

		$stmt = $conn->prepare("DELETE FROM spamprotector_rules WHERE id = ?");
		$stmt->bind_param("i", $id);
		$stmt->execute();
		
		$stmt->close();
		$conn->close();
	}
	
	function edit($id, $value, $isActive)
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

		$stmt = $conn->prepare("UPDATE spamprotector_rules SET value = ?, isActive = ? WHERE id = ?");
		$stmt->bind_param("sii", $value, $isActive, $id);
		$stmt->execute();
		
		$stmt->close();
		$conn->close();
	}
	
	private function LoadRules() 
	{
		$this->rulesDomain = array();
		$this->rulesSender = array();
		$this->rulesSubject = array();
		
		$servername = \SpamProtector\Configuration::Database['server'];
		$username = \SpamProtector\Configuration::Database['username'];
		$password = \SpamProtector\Configuration::Database['password'];
		$dbname = \SpamProtector\Configuration::Database['db'];

		$conn = new \mysqli($servername, $username, $password, $dbname);
		
		if ($conn->connect_error) {
		  die("Connection failed: " . $conn->connect_error);
		}

		$sql = "SELECT field, value FROM spamprotector_rules WHERE isActive = 1";
		$conn->set_charset("utf8");
		$result = $conn->query($sql);

		if ($result->num_rows > 0) 
		{
			while($row = $result->fetch_assoc()) 
			{
				switch($row['field'])
				{
					case 'sender':
					{
						$this->rulesSender[] = $row['value'];
						break;
					}
					case 'subject':
					{
						$this->rulesSubject[] = $row['value'];
						break;
					}
					case 'domain':
					{
						$this->rulesDomain[] = $row['value'];
						break;
					}
				}
			}
		} 
		
		$conn->close();
		
		// var_dump($this->rulesSender); echo('<br><br><br>'); 
		// var_dump($this->rulesDomain); echo('<br><br><br>');
		// var_dump($this->rulesSubject); echo('<br><br><br>');exit;
	}
}