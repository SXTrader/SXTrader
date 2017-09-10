using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.livescoreparser
{
    public class LivetickerSegment1
    {
        private int _matchId;
        private String _league;
        private ulong _teamAId;
        private ulong _teamBId;
        private String _teamA;
        private String _teamB;

        public int MatchId
        {
            get { return _matchId; }
        }

        public String League
        {
            get { return _league; }
        }

        public ulong TeamAId
        {
            get { return _teamAId; }
        }

        public ulong TeamBId
        {
            get { return _teamBId; }
        }

        public String TeamA
        {
            get { return _teamA; }
        }

        public String TeamB
        {
            get { return _teamB; }
        }


        public LivetickerSegment1(int matchId, String league, ulong teamAId, ulong teamBId, String teamA, String teamB)
        {
            _matchId = matchId;
            _league = league;
            _teamAId = teamAId;
            _teamBId = teamBId;
            _teamA = teamA;
            _teamB = teamB;
        }
    }

    class Segment1List : SortedList<int, LivetickerSegment1> { }

}
