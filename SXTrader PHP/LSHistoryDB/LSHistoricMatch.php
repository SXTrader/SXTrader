<?php
/*
 * Created on 24.07.2011
 *
 * To change the template for this generated file go to
 * Window - Preferences - PHPeclipse - PHP - Code Templates
 */
 
 class LSHistoricMatch
 {
 	    private /*ulong*/ $_matchId;
        private /*DateTime*/ $_matchDate;
        private /*ulong*/ $_teamAId;
        private /*ulong*/ $_teamBId;
        private /*String*/ $_teamA;
        private /*String*/ $_teamB;
        private /*String*/ $_devision;
        private /*uint*/ $_scoreA;
        private /*uint*/ $_scoreB;
        private /*String*/ $_halfTimeScore;
        private /*LSHistoricMatchEventList*/ $_eventList = array();
        
        public function __construct($matchId, $matchDate, $teamAId, $teamBId, $teamA, $teamB,
        	$devision, $scoreA, $scoreB, $halfTimeScore, $eventList)
        	{
        		$this->_matchId = $matchId;
        		$this->_matchDate = $matchDate;
        		$this->_teamAId = $teamAId;
        		$this->_teamBId = $teamBId;
        		$this->_teamA = $teamA;
        		$this->_teamB = $teamB;
        		$this->_devision = $devision;
        		$this->_scoreA = $scoreA;
        		$this->_scoreB = $scoreB;
        		$this->_halfTimeScore = $halfTimeScore;
        		$this->_eventList = $eventList;
        	}
        	
        public function __get($key)
        {
        	if(strcasecmp($key, 'MatchId')==0)
        	{
        		return $this->_matchId;
        	}
        	elseif(strcasecmp($key, 'MatchDate')==0)
        	{
        		return $this->_matchDate;
        	}
        	elseif(strcasecmp($key, 'TeamAId')== 0)
        	{
        		return $this->_teamAId;
        	}
        	elseif(strcasecmp($key, 'TeamBId')==0)
        	{
        		return $this->_teamBId;
        	}
        	elseif(strcasecmp($key, 'TeamA')==0)
        	{
        		return $this->_teamA;
        	}
        	elseif(strcasecmp($key, 'TeamB')==0)
        	{
        		return $this->_teamB;
        	}
        	elseif(strcasecmp($key, 'Devision')==0)
        	{
        		return $this->_devision;
        	}
        	elseif(strcasecmp($key, 'ScoreA')==0)
        	{
        		return $this->_scoreA;
        	}
        	elseif(strcasecmp($key, 'ScoreB')==0)
        	{
        		return $this->_scoreB;
        	}
        	elseif(strcasecmp($key, 'HalfTimeScore')==0)
        	{
        		return $this->_halfTimeScore;
        	}
        	elseif(strcasecmp($key, 'MatchEventList')==0)
        	{
        		return $this->_eventList;
        	}
        	else
        	{
        		return null;
        	}
        	
        }
 }
?>
