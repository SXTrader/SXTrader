using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LSCommonObjects
{
    /*
    var d_pi = [0,0,0,0,0,0];
    var d_pn = ['','','','','',''];
    var d_lx = [0,0,0,4,0,0];
    var d_tm = [15,45,69,77,84,90];
    var d_sx = [0,0,0,-1,-1,0];
    var d_bf = ['1-0','2-0','3-0','','3-1','4-1'];
    var d_mc = '0542B0';
    var d_grade = 4;
    var d_mi = 728;
    var d_mn = 'Korea National League Cup';
    var d_ai = 5890;
    var d_bi = 5921;
    var d_ta = 'Busan Transpor Tation';
    var d_tb = 'Gangneung';
    var d_upi = [];
    var d_upn = [];
    var d_dpi = [];
    var d_dpn = [];
    var d_stm = [];
    var d_ssx = [];

    */

    // Feld d_lx gibt art des Ereignis an 0 = TOR;1=Elfmetertor;2=Eigentor?; 3=Gelbe Karte; 4 = Rote Karte; 5 = Gelb/Rot?
// Feld d_sx gibt an welches Team das Ereignis hatte: 0 = TeamA; -1 = TeamB
// Feld d_pn gibt den Spielernamen an, auf den sich das Ereignis bezieht
// Feld d_pi gibt die Spieler-ID an
// Felder d_ai und d_bi sind die Team-Ids
    public enum MATCHEVENTTYPE { GOAL=0, PENALTY, OWNGOAL, YELLOWCARD, REDCARD, YELLOWTORED};
    public class MatchEvent
    {
        private ulong _matchId;
        private ulong _teamId;
        private MATCHEVENTTYPE _eventType;
        private uint _eventMinute;
        private String _infoEvent1;
        private String _infoEvent2;

        public ulong MatchId { get { return _matchId; } }
        public ulong TeamId { get { return _teamId; } }
        public MATCHEVENTTYPE EventType { get { return _eventType; } }
        public uint EventMinute { get { return _eventMinute; } }
        public String InfoEvent1 { get { return _infoEvent1; } }
        public String InfoEvent2 { get { return _infoEvent2; } }

        public MatchEvent(ulong matchId, ulong teamId, MATCHEVENTTYPE eventType, uint eventMinute, String infoEvent1, String infoEvent2)
        {
            _matchId = matchId;
            _teamId = teamId;
            _eventType = eventType;
            _eventMinute = eventMinute;
            _infoEvent1 = infoEvent1;
            _infoEvent2 = infoEvent2;
        }


    }

    public class MatchEventList : List<MatchEvent> { }
}
