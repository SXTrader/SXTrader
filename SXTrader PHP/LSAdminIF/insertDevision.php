<?php
/*
 * Created on 02.08.2011
 *
 * To change the template for this generated file go to
 * Window - Preferences - PHPeclipse - PHP - Code Templates
 */
 
  include 'LSDatabase.php';
  
  $devisionId = $_GET["devisionId"];
  $devisionName = $_GET["devisionName"];
  
  $statisticDb = new LSDatabase();
  $statisticDb->insertDevision($devisionId, $devisionName); 
?>
