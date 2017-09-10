<?php
/*
 * Created on 24.07.2011
 *
 * To change the template for this generated file go to
 * Window - Preferences - PHPeclipse - PHP - Code Templates
 */
 
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
class LSHistoricMatchEvent
{
	private $_matchId;
    private $_teamId;    
    private $_eventType;    
    private $_infoEvent1;    
    private $_infoEvent2;    
    private $_eventMinute;    

	
	public function __construct /*function LSHistoryMatchEvent*/($matchId, $teamId, $eventType, $infoEvent1, $infoEvent2, $eventMinute)
	{
		$this->_matchId = $matchId;
		$this->_teamId = $teamId;
		$this->_eventType = $eventType;
        $this->_infoEvent1 = $infoEvent1;        
       	$this->_infoEvent2 = $infoEvent2;       
		$this->_eventMinute = $eventMinute;
	}
	
	public function __get($key)
    {
        	if(strcasecmp($key, 'MatchId')==0)
        	{
        		return $this->_matchId;
        	}
        	elseif(strcasecmp($key, 'TeamId')==0)
        	{
        		return $this->_teamId;
        	}
        	elseif(strcasecmp($key, 'EventType')== 0)
        	{
        		return $this->_eventType;
        	}
        	elseif(strcasecmp($key, 'InfoEvent1')==0)
        	{
        		return $this->_infoEvent1;
        	}
        	elseif(strcasecmp($key, 'InfoEvent2')==0)
        	{
        		return $this->_infoEvent2;
        	}
        	elseif(strcasecmp($key, 'EventMinute')==0)
        	{
        		return $this->_eventMinute;
        	}        	
        	else
        	{
        		return null;
        	}
        	
        }
}

?>
