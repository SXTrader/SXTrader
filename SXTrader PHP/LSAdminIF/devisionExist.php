<?php
/*
 * Created on 02.08.2011
 *
 * To change the template for this generated file go to
 * Window - Preferences - PHPeclipse - PHP - Code Templates
 */
 include 'LSDatabase.php';
 
 $devisionId = $_GET["devisionId"];
 
 $statisticDb = new LSDatabase();
 $result = $statisticDb->devisionExists($devisionId);
   if($result == true)
   {
   	echo 'true';
   }
   else
   {
   	echo 'false';
   }
?>
