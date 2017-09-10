using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSCommonObjects
{
    public class LiveScoreMatchData
    {
        private String _teamA;
        private String _teamB;
        private ulong _teamAId;
        private ulong _teamBId;

        private String _devision;
        private ulong _matchId;

        private String _matchDate;

        private uint _scoreTeamA;
        private uint _scoreTeamB;

        private String _halftimeScore;

        private MatchEventList _matchEvents;


        public String TeamA { get { return _teamA; } }
        public String TeamB { get { return _teamB; } }
        public ulong TeamAId { get { return _teamAId; } }
        public ulong TeamBId { get { return _teamBId; } }

        public String Devision { get { return _devision; } }
        public ulong MatchId { get { return _matchId; } }

        public String MatchDate { get { return _matchDate; } }

        public uint ScoreTeamA { get { return _scoreTeamA; } set { _scoreTeamA = value; } }
        public uint ScoreTeamB { get { return _scoreTeamB; } set { _scoreTeamB = value; } }

        public String HalftimeScore { get { return _halftimeScore; } set { _halftimeScore = value; } }

        public MatchEventList MatchEvents { get { return _matchEvents; } set { _matchEvents = value; } }

        public LiveScoreMatchData(String teamA, ulong teamAId, String teamB, ulong teamBId, String devision, ulong matchId, String matchDate)
        {
            _teamA = teamA;
            _teamB = teamB;
            _teamAId = teamAId;
            _teamBId = teamBId;
            _devision = devision;
            _matchId = matchId;
            _matchDate = matchDate;
            _matchEvents = new MatchEventList();
        }

        public LiveScoreMatchData(String teamA, ulong teamAId, String teamB, ulong teamBId, String devision, ulong matchId)
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
