<?php
include 'LALStatDatabase.php';

$sxId = isset($_GET["sxId"])?$_GET["sxId"]:'';
$sxName = isset($_GET["sxName"]) ? $_GET["sxName"] : '';
$lalId =  isset($_GET["lalId"]) ? $_GET["lalId"]+0:0;

$statisticDb = new LalStatDatabase();

echo $statisticDb->mapSxNameToLal($sxId, $sxName, $lalId)
?>