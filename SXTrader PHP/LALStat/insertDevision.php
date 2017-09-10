<?php
include 'LALStatDatabase.php';

$ltId = isset($_GET["ltId"])?$_GET["ltId"]:'';
$ltDivName = isset($_GET["ltDivName"]) ? $_GET["ltDivName"] : '';

$statisticDb = new LalStatDatabase();
echo $statisticDb->insertDivision($ltId, $ltDivName);
?>