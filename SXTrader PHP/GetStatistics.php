<?php
// @(#) $Id$
// +-----------------------------------------------------------------------+
// | Copyright (C) 2011, http://sxtrader.net                                   |
// +-----------------------------------------------------------------------+
// |                                                                       |
// +-----------------------------------------------------------------------+
// | Author: mhe                                                |
// +-----------------------------------------------------------------------+
//

//Datenbankverbindung
class LSDatabase
{
	var $db; //Datenbankverbindung
	
	public __construct()
	/*function LSDatabase()*/
	{
		$this->db = new mysqli('localhost', 'user', 'password', 'database');
		if(mysqli_connect_errno())
		{
			printf("Verbindung fehlgeschlagen: %s\n", mysqli_connect_error());
			exit();
		}
	}

	public __destruct()
	{
		$this->db->close();
	}
}

// Enumeration der Begegnisereignisse
class MATCHEVENTTYP
{
	const GOAL = 0;
	const PENALTY = 1;
	const OWNGOAL = 2;
	const YELLOWCARD = 3;
	const REDCARD = 4;
	const YELLOWTORED = 5;
}

// Einzelnes Ereignis in einer Spielbegegnung
class LSHistoryMatchEvent
{
	var $MatchId;
    var $TeamId;    
    var $EventType;    
    var $InfoEvent1;    
    var $InforEvent2;    
    var $EventMinute;    

	
	public __construct /*function LSHistoryMatchEvent*/($matchId, $teamId, $eventType, $infoEvent1, $infoEvent2, $eventMinute)
	{
		$this->MatchId = $matchId;
		$this->TeamId = $teamId;
		$this->EventType = $eventType;
        $this->InfoEvent1 = $infoEvent1;        
       	$this->InfoEvent2 = $infoEvent2;       
		$this->EventMinute = $eventMinute;
	}
}

//
class LSHistoryMatch
{
	var $MatchId;
    var $MatchDate;
}


$lsdb = new LSDatabase();

/*
@$db = new mysqli('localhost', 'user', 'password');

//Verbindung überprüfen
if(mysqli_connect_errno())
{
	printf("Verbindung fehlgeschlagen: %s\n", mysqli_connect_error());
    exit();
}
echo "Verbindung erfolgreich: " . $db->host_info . "\n</br>";
echo "Server: " . $db->server_info . "\n</br>";

// SQL-Befehlt 
$sql_befehl = "SHOW DATABASES";

if($resultat = $db->query($sql_befehl))
{
	// Meldung bei erfolgreicher  Abfrage
	while($daten = $resultat->fetch_object())
	{
		//Namen der Datenbank ausgeben
		echo $daten->Database . "\n</br>";
	}

	// Anzahl der Abfragezeilen
	$anzahl = $resultat->num_rows;

	// Ausgabe
	printf("Abfrage enthält %d Datenbank(en).\n</br>", $anzahl);

	// Speicher freigeben
	$resultat->close();
}
else
{
	// Meldung bei Fehlschlag
	echo "Zugriff fehlgeschlagen!";
}

// Verbindung zum Datenbankserver beenden
$db->close();
*/
?>

		