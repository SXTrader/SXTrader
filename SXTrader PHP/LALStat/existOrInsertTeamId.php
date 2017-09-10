<?php

include 'LALStatDatabase.php';

$livetickerId = $_GET["livetickerId"];
$teamId =  isset($_GET["teamId"]) ? $_GET["teamId"]+0:0;


$statisticDb = new LalStatDatabase();

echo $statisticDb->existOrInsertTeam($teamId, $livetickerId);

?>