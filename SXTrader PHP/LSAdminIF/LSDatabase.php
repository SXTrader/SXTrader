<?php
/*
 * Created on 24.07.2011
 *
 * To change the template for this generated file go to
 * Window - Preferences - PHPeclipse - PHP - Code Templates
 */
 
 class LSDatabase
{
	private $db; //Datenbankverbindung
	
	public function __construct()
	/*function LSDatabase()*/
	{
		$this->db = new mysqli('localhost', 'user', 'password', 'database');
		if(mysqli_connect_errno())
		{
			printf("Verbindung fehlgeschlagen: %s\n",  mysqli_connect_error());
			exit();
		}
	}


	public function getTeamNames($teamId)
	{
		$nameList = array();
		$sql = "SELECT * FROM teamnames where idTeam=$teamId";
		if($result = mysqli_query($this->db, $sql))
		{
			while($datensatz = $result->fetch_array())
			{
				$nameTeam = $datensatz[1];
				$nameList[] = $nameTeam;
			}
			$result->close();
		}		
		return $nameList;
	}
	
	public function insertMatchEvent($matchId, $teamId, $eventType, $eventMinute, $infoEvent1, $infoEvent2)
	{
		$versionId = 1;
		$sql = "INSERT INTO matchevent(matchId, teamId, typeEvent, eventMinute, infoEvent1, infoEvent2, version) VALUES( $matchId, $teamId, $eventType, $eventMinute, '$infoEvent1', '$infoEvent2', $versionId)";
        //echo $sql;
		mysqli_query($this->db, $sql);
		echo mysqli_error($this->db);
	}
	
	public function insertMatch($matchId, $matchDate, $teamAId, $teamBId, $devisionId, $scoreTeamA, $scoreTeamB, $halftimeScore)
	{
		$versionId = 1;
		$sql = "INSERT INTO database.match VALUES( $matchId, '$matchDate', $teamAId, $teamBId, '$devisionId', $scoreTeamA, $scoreTeamB, '$halftimeScore', $versionId)";
		mysqli_query($this->db, $sql);
		echo mysqli_error($this->db);
	}
	
	public function insertDevision($devisionId, $devisionName)
	{
		//$devisionId = str_replace('\'', '', $devisionId);
		//$devisionName = str_replace('\'', '', $devisionName);
		if(empty($devisionName))
		{
			$devisionName = "NULL";
		}
		$versionId = 1;
		$sql = "INSERT INTO devision VALUES('$devisionId', $devisionName, $versionId)";		
		mysqli_query($this->db, $sql);
		echo mysqli_error($this->db);
	}
	
	public function insertTeamName($teamId, $teamName)
	{
		$versionId = 1;
		$sql = "INSERT INTO teamnames VALUES($teamId, '$teamName', $versionId)";
		mysqli_query($this->db, $sql);
		echo mysqli_error($this->db);
	}
	
	public function insertTeam($teamId)
	{
		$versionId = 1;
		$sql = "INSERT INTO team VALUES($teamId, $versionId)";
		mysqli_query($this->db, $sql);
		echo mysqli_error($this->db);
	}
	
	public function teamExists($teamId)
	{
		$sql = "SELECT count(*) FROM team WHERE idTeam=$teamId";
		if($resultat = mysqli_query($this->db, $sql))
		{
			if($datenstatz = $resultat->fetch_array())
			{
				if($datenstatz[0] > 0)
					return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
		
		return false;
	}
	
	public function matchExists($matchId)
	{
		$sql = "SELECT count(*) from database.match where idMatch=$matchId";
		if($resultat = mysqli_query($this->db, $sql))
		{
			if($datenstatz = $resultat->fetch_array())
			{
				if($datenstatz[0] > 0)
					return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
		return false;
	}
	
	public function devisionExists($devisionId)
	{
		$sql = "SELECT count(*) FROM devision WHERE idDevision='$devisionId'";
		if($resultat = mysqli_query($this->db, $sql))
		{
			if($datenstatz = $resultat->fetch_array())
			{
				if($datenstatz[0] > 0)
					return true;
			}
			else
			{
				return false;
			}
		}
		else
		{
			return false;
		}
		return false;
	}
/*	
	public function getStatistics($teamAId, $teamBId)
	{
		//$directList = buildDirectList($teamAId, $teamBId);
		$directList = $this->buildDirectList($teamAId, $teamBId);
		$teamAList = $this->buildTeamList($teamAId);
		$teamBList = $this->buildTeamList($teamBId);
		
		$statistic = new LSHistoricDataStatistic($directList, $teamAList, $teamBList);
		return $statistic;
	}

	private function buildMatchEventList($matchId)
	{
		$eventList = array();
		$sql = "SELECT * FROM matchevent where matchId=$matchId";
		if($result = mysqli_query($this->db, $sql))
		{			
			while($datensatz = $result->fetch_array())
			{				
				$idMatch = $datensatz[1];
				$idTeam = $datensatz[2];
				$eventType = $datensatz[3];
				$eventMinute = $datensatz[4];
                $infoEvent1 = $datensatz[5];
                $infoEvent2 = $datensatz[6];

				$matchEvent = new LSHistoricMatchEvent($idMatch, $idTeam, $eventType, $infoEvent1, $infoEvent2, $eventMinute);
				$eventList[] = $matchEvent;				
			}
			$result->close();
		}
		return $eventList;
	}
	
	private function buildTeamList($idTeam)
	{
		$list = array();
		$sql = "SELECT * FROM database.match WHERE idTeamA = $idTeam OR idTeamB = $idTeam ORDER BY matchDate DESC LIMIT 30";
		if($result = mysqli_query($this->db, $sql))
		{
			while($datensatz = $result->fetch_array())
			{
				$idMatch = $datensatz[0];
				$dateMatch = $datensatz[1];
				$idTeamA = $datensatz[2];
				$idTeamB = $datensatz[3];
				$idDevision = $datensatz[4];
				$scoreA = $datensatz[5];
				$scoreB = $datensatz[6];
				$htScore = $datensatz[7];
				$teamA = $this->getTeamName($idTeamA);
				$teamB = $this->getTeamName($idTeamB);

				$eventList = $this->buildMatchEventList($datensatz[0]);
				$match = new LSHistoricMatch($idMatch, $dateMatch, $idTeamA, $idTeamB, $teamA, $teamB,
        			$idDevision, $scoreA, $scoreB, $htScore, $eventList);
        			
        	    $list[]=$match;
			}	
			$result->close();			
		}
		return $list;
	}
	
	private function buildDirectList($teamAId, $teamBId )
	{
		$list = array();
		$sql = "SELECT * FROM database.match WHERE (idTeamA = $teamAId AND idTeamB = $teamBId) OR (idTeamA= $teamBId AND idTeamB =$teamAId) ORDER BY matchDate DESC LIMIT 30";
		if($result = mysqli_query($this->db, $sql))
		{			
			while($datensatz = $result->fetch_array())
			{
				$idMatch = $datensatz[0];
				$dateMatch = $datensatz[1];
				$idTeamA = $datensatz[2];
				$idTeamB = $datensatz[3];
				$idDevision = $datensatz[4];
				$scoreA = $datensatz[5];
				$scoreB = $datensatz[6];
				$htScore = $datensatz[7];
				$teamA = $this->getTeamName($idTeamA);
				$teamB = $this->getTeamName($idTeamB);

				$eventList = $this->buildMatchEventList($datensatz[0]);
				$match = new LSHistoricMatch($idMatch, $dateMatch, $idTeamA, $idTeamB, $teamA, $teamB,
        			$idDevision, $scoreA, $scoreB, $htScore, $eventList);
        			
        	    $list[]=$match;
			}	
			$result->close();
			// Ergebnis schliessen
		}
		
		return $list;
	}
	
	private function getTeamName($idTeam)
	{
		$sql =  "SELECT * from database.teamnames WHERE idTeam = $idTeam LIMIT 1";
		if($result = mysqli_query($this->db, $sql))
		{
			while($datensatz = $result->fetch_array())
			{
				return $datensatz[1];
			}
			
			$result->close();
		}
	}
*/
	public function __destruct()
	{
		$this->db->close();
	}
}
?>
