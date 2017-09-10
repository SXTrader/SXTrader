<?php
include 'UsageDB.php'; 
$ip = isset($_GET["fb"])?$_GET["fb"]:'';

if($ip == '')
{
	$ip = $_SERVER["REMOTE_ADDR"];
}
$sportexchange = $_GET["sx"];

echo $ip;
echo $sportexchange;

$usageDB = new UsageDB();
echo $usageDB->writeUsage($ip, $sportexchange);
