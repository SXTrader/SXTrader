<?php
class LalStatDatabase
{
	private $db; //Datenbankverbindung
	
	public function __construct()
	/*function LSDatabase()*/
	{
		$this->db = new mysqli('localhost', 'User', 'password, 'database');
		if(mysqli_connect_errno())
		{
			printf("Verbindung fehlgeschlagen: %s\n",  mysqli_connect_error());
			exit();
		}
	}
	
	
	public function insertDivision($ltId, $ltDivName) 
	{
		//Überprüfen, ob Liveticker bekannt.
		$sql = "SELECT * from database.livetickers WHERE livetickerName = '$ltId'";
		if($result = $this->db->query($sql))
		{
			//Liveticker nicht bekannt
			if($result->num_rows == 0)
			{
				$result->close();
				return -2;
			}
			
			//Liveticker ist bekannt => Weiter
			$datensatz = $result->fetch_array();
			$internalLtId = $datensatz[0];
			$result->close();
			
			//existiert bereits ein Mapping?
			$sql = "SELECT * FROM database.lal_lt_leagueid_mapper 
				WHERE livetickerId = $internalLtId AND livetickerLeagueId = '$ltDivName'";
			if($result = $this->db->query($sql))
			{
				//Es existiert bereits ein Mapping
				if($result->num_rows>0)
				{
					$result->close();
					return 0;
				}
				
				$result->close();
				
				//Neues Mapping wird benötigt
				//eine LAL-LigaId erzeugen
				$sql = "INSERT INTO table.lal_leagues(version) VALUES (2)";
				$stmt = $this->db->prepare($sql);
				
				if(!$stmt->execute())
				{
					//Insert ist fehlgeschlagen
					$stmt->close();
					return -4;
				}
				
				$stmt->close();
				
				
				$sql = "SELECT LAST_INSERT_ID()";
				if($result = $this->db->query( $sql))
				{

					$datensatz = $result->fetch_array();
					$lastId = $datensatz[0];
					$result->close();
					
					//Mapping anlegen
					$sql = "INSERT INTO database.lal_lt_leagueid_mapper(leagueId, livetickerId, livetickerLeagueId, version) VALUES(?,?,?,?)";
					
					$stmt = $this->db->prepare($sql);
					$versionId = 2;
					$stmt->bind_param("iisi", $lastId, $internalLtId, $ltDivName, $versionId);
					
					if(!$stmt->execute())
					{
						//Einfügen fehlgeschlagen
						$stmt->close(); 
						return -5;
					}
					$stmt->close();
					//Erfolg
					return $lastId;
				}
				else
				{
					//Konnte letzte ID nicht lesen
					return -5;
				}
			}
			else
			{
				//Datenbankabfrage fehlgeschlagen
				return -3;				
			}
			
		}	
		else
		{
			//Datenbankabfrage fehlgeschlagen
			return -1;
		}		
	}
	
	public function mapSxNameToLal($sxId, $sxName, $lalId)
	{
		//Überprüfen, ob Wettbörse bekannt
		$sql = "SELECT * from database.sportexchanges WHERE sxName = '$sxId'";
		
		if($result = $this->db->query($sql))
		{
			//Wettbörse nicht bekannt
			if($result->num_rows == 0)
			{
				$result->close();
				return -2;
			}
			
			//Wettbörse ist bekannt => Weiter
			$datensatz = $result->fetch_array();
			$internalSxId = $datensatz[0];
			$result->close();
			//Existiert die LAL-ID?
			$sql = "SELECT * from database.lal_teams WHERE teamId = '$lalId'";
			if($result=$this->db->query($sql))
			{
				//LAL-ID nicht bekannt				
				if($result->num_rows == 0)
				{
					$result->close();
					return -4;
				}
				
				//LAL-ID gib es
				$result->close();
				
				//Überprüfe, ob mapping bereits vorhanden.
				$sql = "SELECT * from database.lal_sx_name_mapper WHERE sxId = '$internalSxId' AND 
					sxTeamName = '$sxName' AND lalTeamId = '$lalId'";
				
				if($result=$this->db->query($sql))
				{
					//Mapping existier bereits
					if($result->num_rows > 0)
					{
						$result->close();
						return 0;
					}
					
					// Mapping anlegen
					$sql = "INSERT INTO database.lal_sx_name_mapper(sxId, sxTeamName, lalTeamId, version) VALUES($internalSxId, '$sxName', $lalId, 2)";
					$this->db->query($sql);
					
					if($this->db->errno > 0)
					{
						echo mysqli_error($this->db);
						return $this->db->errno * (-1);
					}
					
					return $lalId;
				}
				else 
				{
					//Datenbankabfrage fehlgeschlagen
					return -5;
				}
			}
			else 
			{
				//Datenbankabfrage fehlgeschlagen
				return -3;
			}
		}
		else 
		{
			//Datenbankabfrage fehlgeschlagen
			return -1;
		}
	}
	
	public function getLALTeamIDByLTTeamID($idTeam, $liveticker)
	{
		$sql = "SELECT * from database.livetickers WHERE livetickername = '$liveticker'";

		if($result = $this->db->query($sql))
		{
		
			if($result->num_rows == 0)
			{
				// Der Liveticker ist nicht vorhanden
				$result->close();
				return -1;
			}
			
			$datensatz = $result->fetch_array();
			$tickerId = $datensatz[0];
				
			$result->close();
			
			$sql = "SELECT * from database.lal_lt_teamid_mapper WHERE livetickerId = $tickerId AND livetickerTeamId = $idTeam";

			if($result = $this->db->query($sql))
			{
				//Mapping existiert bereits => raus
				if($result->num_rows == 0)
				{
					$result->close();
					return -1;
				}
				
				$datensatz = $result->fetch_array();
				$lalId = $datensatz[0];
				$result->close();
				
				return $lalId;
			}
		}
	}
	
	public function existOrInsertTeam($idTeam, $liveticker)
	{
		$sql = "SELECT * from database.livetickers WHERE livetickername = '$liveticker'";
		if($result = mysqli_query($this->db, $sql))
		{
						
			if($result->num_rows == 0)
			{
				// Der Liveticker ist nicht vorhanden				
				$result->close();
				return -1;
			}
			
			$datensatz = $result->fetch_array();
			$tickerId = $datensatz[0];
			
			$result->close();
			
			//existiert bereits ein Mappingeintrag?
			$sql = "SELECT * from database.lal_lt_teamid_mapper WHERE livetickerId = $tickerId AND livetickerTeamId = $idTeam";
			if($result = mysqli_query($this->db, $sql))
			{				
				//Mapping existiert bereits => raus
				if($result->num_rows > 0)
				{					
					$result->close();
					return -1;
				}
				$result->close();
				
				//eine LAL-TeamId erzeugen
				$sql = "INSERT INTO database.lal_teams(version) VALUES (2)";
				$this->db->query( $sql);
				$sql = "SELECT LAST_INSERT_ID()";
				$result = $this->db->query( $sql);
				//$lastId = $this->$db->insert_id;
				$datensatz = $result->fetch_array();
				$lastId = $datensatz[0];
				$sql = "INSERT INTO database.lal_lt_teamid_mapper(teamId, livetickerId, livetickerTeamId, version) VALUES($lastId, $tickerId, $idTeam, 2)";
				$this->db->query($sql);
				return $lastId;
			}
		}
		echo mysqli_error($this->db);
		return false;
	}
}

?>
