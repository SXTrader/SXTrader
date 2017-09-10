using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveScoreCrawlerGen2
{
    public class LiveScoreMatchData
    {
        private String _teamA;
        private String _teamB;
        private long _teamAId;
        private long _teamBId;

        private String _devision;
        private long _matchId;

        private String _matchDate;

        private int _scoreTeamA;
        private int _scoreTeamB;

        private String _halftimeScore;

        //private MatchEventList _matchEvents;


        public String TeamA { get { return _teamA; } }
        public String TeamB { get { return _teamB; } }
        public long TeamAId { get { return _teamAId; } }
        public long TeamBId { get { return _teamBId; } }

        public String Devision { get { return _devision; } }
        public long MatchId { get { return _matchId; } }

        public String MatchDate { get { return _matchDate; } }

        public int ScoreTeamA { get { return _scoreTeamA; } set { _scoreTeamA = value; } }
        public int ScoreTeamB { get { return _scoreTeamB; } set { _scoreTeamB = value; } }

        public String HalftimeScore { get { return _halftimeScore; } set { _halftimeScore = value; } }

        //public MatchEventList MatchEvents { get { return _matchEvents; } set { _matchEvents = value; } }

        public LiveScoreMatchData(String teamA, long teamAId, String teamB, long teamBId, String devision, long matchId, String matchDate)
        {
            _teamA = teamA;
            _teamB = teamB;
            _teamAId = teamAId;
            _teamBId = teamBId;
            _devision = devision;
            _matchId = matchId;
            _matchDate = matchDate;
            //_matchEvents = new MatchEventList();
        }

        public LiveScoreMatchData(String teamA, long teamAId, String teamB, long teamBId, String devision, long matchId)
        {
            _teamA = teamA;
            _teamB = teamB;
            _teamAId = teamAId;
            _teamBId = teamBId;
            _devision = devision;
            _matchId = matchId;
        }
    }
}
