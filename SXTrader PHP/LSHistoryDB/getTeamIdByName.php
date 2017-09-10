<?php
include 'LSDatabase.php';

$teamName = isset($_GET["teamName"]) ? $_GET["teamName"] : '';
 
$statisticDb = new LSDatabase();

$result = $statisticDb->getTeamIdByTeamName($teamName);