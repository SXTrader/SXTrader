<?php
class UsageDB
{
	private $db; //Datenbankverbindung
	
	public function __construct()
	{
		$this->db = new mysqli('localhost', 'user', 'password', 'database');
		if(mysqli_connect_errno())
		{
			printf("Verbindung fehlgeschlagen: %s\n",  mysqli_connect_error());
			exit();
		}		
	}
	public function writeUsage($ip, $sx)
	{		
		$sql = "SELECT * from `usage` WHERE ip = '$ip' AND sxName = '$sx'";
		if($result = $this->db->query($sql))
		{
			if($result->num_rows ==0)
			{
				$sql = "INSERT INTO `usage`(`ip`, `sxName`) VALUES( '$ip', '$sx')";
				$result = $this->db->query($sql);
			}
			else 
			{
				$sql = "UPDATE `usage` SET lastActivity =  CURRENT_TIMESTAMP() WHERE ip = '$ip' AND sxName = '$sx' ";
				$result = $this->db->query($sql);
			}			
		}
		
	}
}
