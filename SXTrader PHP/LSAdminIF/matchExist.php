<?php
/*
 * Created on 02.08.2011
 *
 * To change the template for this generated file go to
 * Window - Preferences - PHPeclipse - PHP - Code Templates
 */
 
   include 'LSDatabase.php';
   
   $matchId = isset($_GET["matchId"]) ? $_GET["matchId"]+0 : 0;
   
   $statisticDb = new LSDatabase(); 
   $result = $statisticDb->matchExists($matchId);
   if($result == true)
   {
   	echo 'true';
   }
   else
   {
   	echo 'false';
   }
?>
