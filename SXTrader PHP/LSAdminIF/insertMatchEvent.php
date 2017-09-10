<?php
/*
 * Created on 02.08.2011
 *
 * To change the template for this generated file go to
 * Window - Preferences - PHPeclipse - PHP - Code Templates
 */
 
  include 'LSDatabase.php';
  
  $teamId = isset($_GET["teamId"]) ? $_GET["teamId"]+0 : 0;
  $matchId = isset($_GET["matchId"]) ? $_GET["matchId"]+0 : 0;
  $eventType = isset($_GET["eventType"]) ? $_GET["eventType"]+0 : 0;
  $eventMinute = isset($_GET["eventMinute"]) ? $_GET["eventMinute"]+0 : 0;
  $infoEvent1 = $_GET["infoEvent1"];
  $infoEvent2 = $_GET["infoEvent2"]; 
  
  $statisticDb = new LSDatabase();
  $statisticDb->insertMatchEvent($matchId, $teamId, $eventType, $eventMinute, $infoEvent1, $infoEvent2);
?>
