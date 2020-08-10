<?php
// Scan SPAM script is responsible for scanning main mailbox, check spam rules and move spam if detected

error_reporting(E_ALL ^ E_DEPRECATED);

require_once __DIR__ . '/../../lib/Mailbox/MainMailbox.php';

$mailbox = new \SpamProtector\Mailbox\MainMailbox();
$mailbox->detectSpam();
exit;
