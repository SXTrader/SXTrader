<?php
/*
 * Created on 02.08.2011
 *
 * To change the template for this generated file go to
 * Window - Preferences - PHPeclipse - PHP - Code Templates
 */
 
 include 'LSDatabase.php';
 
 $matchId = isset($_GET["matchId"]) ? $_GET["matchId"]+0 : 0;
 $matchDate = $_GET["matchDate"];
 $teamAId = isset($_GET["teamAId"]) ? $_GET["teamAId"]+0:0;
 $teamBId = isset($_GET["teamBId"]) ? $_GET["teamBId"]+0:0;
 $devisionId = $_GET["devisionId"];
 $scoreA = isset($_GET["scoreTeamA"]) ? $_GET["scoreTeamA"]+0:0;
 $scoreB = isset($_GET["scoreTeamB"]) ? $_GET["scoreTeamB"]+0:0;
 $halfTimeScore = $_GET["halftimeScore"];
 
 $statisticDb = new LSDatabase();
 
 $statisticDb->insertMatch($matchId, $matchDate, $teamAId, $teamBId, $devisionId, $scoreA, $scoreB, $halfTimeScore);
?>
