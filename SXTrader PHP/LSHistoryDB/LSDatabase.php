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
	
	public function getMatchesByLeague($league, $from, $to)
	{
		$list = array();
		$sql = "SELECT * FROM `match` WHERE `idDevision`= '$league' AND `matchDate`BETWEEN '$from' AND '$to' ORDER BY `matchDate`";
		
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
				
				$match = new LSHistoricMatch($idMatch, $dateMatch, $idTeamA, $idTeamB, $teamA, $teamB,
						$idDevision, $scoreA, $scoreB, $htScore, null);								
				$list[]=$match;
			}
			$result->close();
		}
		else
		{
				
		}
		
		$matchList = new LSSimpleMatchList($list);
		return $matchList;
	}
	
	public function getStatistics($teamAId, $teamBId, $league, $count, $age)
	{
		//$directList = buildDirectList($teamAId, $teamBId);
		$directList = $this->buildDirectList($teamAId, $teamBId, $league, $count, $age);
		$teamAList = $this->buildTeamList($teamAId, $league, $count, $age);
		$teamBList = $this->buildTeamList($teamBId, $league, $count, $age);
		
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
	
	private function buildTeamList($idTeam, $league, $count, $age)
	{
		$list = array();
		$sql = "";
		

		
		if(empty($league))
		{
			if($age == 0)
			{
				$sql = "SELECT * FROM database.match WHERE ( idTeamA = $idTeam OR idTeamB = $idTeam ) ORDER BY matchDate DESC LIMIT $count";				
			}
			else
			{
				
				$sql = "SELECT * FROM database.match WHERE ( idTeamA = $idTeam OR idTeamB = $idTeam ) AND matchDate >= '$age' ORDER BY matchDate DESC LIMIT $count";
			}
		}
		else
		{
			if($age == 0)
			{
				$sql = "SELECT * FROM database.match WHERE (idTeamA = $idTeam OR idTeamB = $idTeam) AND idDevision = '$league' ORDER BY matchDate DESC LIMIT $count";
			}
			else
			{
				$sql = "SELECT * FROM database.match WHERE (idTeamA = $idTeam OR idTeamB = $idTeam) AND idDevision = '$league' AND matchDate >= '$age' ORDER BY matchDate DESC LIMIT $count";
			}
		}
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
	
	private function buildDirectList($teamAId, $teamBId, $league, $count, $age )
	{
		$list = array();
		$sql = "";
		if(empty($league))
		{
			if($age == 0)
			{
				$sql = "SELECT * FROM database.match WHERE (idTeamA = $teamAId AND idTeamB = $teamBId) OR (idTeamA= $teamBId AND idTeamB =$teamAId) ORDER BY matchDate DESC LIMIT $count";
			}
			else
			{
				$sql = "SELECT * FROM database.match WHERE ((idTeamA = $teamAId AND idTeamB = $teamBId) OR (idTeamA= $teamBId AND idTeamB =$teamAId)) AND matchDate >= '$age' ORDER BY matchDate DESC LIMIT $count";
			}
		}
		else
		{
			if($age == 0)
				$sql = "SELECT * FROM database.match WHERE ( (idTeamA = $teamAId AND idTeamB = $teamBId) OR (idTeamA= $teamBId AND idTeamB =$teamAId) ) AND idDevision = '$league' ORDER BY matchDate DESC LIMIT $count";
			else
				$sql = "SELECT * FROM database.match WHERE ( (idTeamA = $teamAId AND idTeamB = $teamBId) OR (idTeamA= $teamBId AND idTeamB =$teamAId) ) AND idDevision = '$league' AND matchDate >= '$age' ORDER BY matchDate DESC LIMIT $count";
		}
		
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
	
	public function getTeamIdByTeamName($nameTeam)
	{
		$sql = "SELECT * from database.teamnames WHERE teamName = '$nameTeam'";
		if($result = $this->db->query($sql, MYSQLI_USE_RESULT))
		{
			while($datensatz=$result->fetch_array())
			{
				echo $datensatz[0];
				echo ';';
			}
			
			$result->close();
		}
	}
	
	
	public function getAllDevisions()
	{
		$sql ="SELECT idDevision from database.devision";
		
		if($result = $this->db->query($sql, MYSQLI_USE_RESULT)) 
		{
			while($datensatz = $result->fetch_array())
			{
				echo $datensatz[0];
				echo ';';
			}
					
			$result->close();
		}
	
	}
	
	public function getAllTeamIds()
	{
		$list = array();
		$sql = "SELECT * from database.team";
		if($result = $this->db->query($sql,MYSQLI_USE_RESULT))
		{
			while($datensatz = $result->fetch_array())
			{
				echo $datensatz[0];
				echo ';';
			}
			
			$result->close();
		}

	}

	public function __destruct()
	{
		$this->db->close();
	}
}
?>
