using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.sxtrader.bftradingstrategies.lsparserinterfaces;
using System.Collections;
using System.Globalization;

namespace net.sxtrader.bftradingstrategies.livescoreparser
{
    public enum UPDATEEVENT { UNASSIGNED, GOALTEAMA, GOALTEAMB, GOALBACKA, GOALBACKB, GAMEENDED, REDCARDA, REDCARDB };
    public class LivetickerSegment2
    {
        public event EventHandler<EventArgs> GameStateChangedEvent;

        private int _id;
        private LIVESTATES _state;
        private int _scoreA;
        private int _scoreB;
        private int _redA;
        private int _redB;
        private DateTime _dts;
        private String _halftimeScore;


        public int ID { get { return _id; } }
        public LIVESTATES GameState { get { return _state; } }
        public int ScoreA { get { return _scoreA; } }
        public int ScoreB { get { return _scoreB; } }
        public int RedA { get { return _redA; } }
        public int RedB { get { return _redB; } }
        public DateTime StartDTS { get { return _dts; } }
        public String HalftimeScore { get { return _halftimeScore; } }

        public LivetickerSegment2(int id, String state, String scoreA, String scoreB, String redA, String redB, String dts1, String halftimeScore, String dts2)
        {
            _id = id;
            // Spielstatus als Enum
            //_state = state;
            _state = (LIVESTATES)Int16.Parse(state,CultureInfo.InvariantCulture);
            if (!Int32.TryParse(scoreA, out _scoreA))
            {
                ;
            }

            if (!Int32.TryParse(scoreB, out _scoreB))
            {
                ;
            }

            if (Int32.TryParse(redA, out _redA))
            {
                ;
            }

            if (Int32.TryParse(redB, out _redB))
            {
                ;
            }

            if (!String.IsNullOrEmpty(dts1))
                _dts = buildDTS(dts1);

            if (!String.IsNullOrEmpty(dts2) && _dts == DateTime.MinValue)
                _dts = buildDTS(dts2);

            _halftimeScore = halftimeScore;
        }

        public UPDATEEVENT[] update(LivetickerSegment2 segment2)
        {
            ArrayList updateEvents = new ArrayList();

            if (segment2.ScoreA > this.ScoreA ) 
            {
                updateEvents.Add(UPDATEEVENT.GOALTEAMA);
                _scoreA = segment2.ScoreA;               
            }

            if (segment2.ScoreB > this.ScoreB)
            {
                updateEvents.Add(UPDATEEVENT.GOALTEAMB);
                _scoreB = segment2.ScoreB;  
            }

            if (segment2.ScoreA < this.ScoreA ) 
            {
                updateEvents.Add(UPDATEEVENT.GOALBACKA);
                _scoreA = segment2.ScoreA;
            }

            if (segment2.ScoreB < this.ScoreB)
            {
                updateEvents.Add(UPDATEEVENT.GOALBACKB);
                _scoreB = segment2.ScoreB;
            }

            if (segment2.RedA != this.RedA) 
            {
                updateEvents.Add(UPDATEEVENT.REDCARDA);
                _redA = segment2.RedA;                
            }

            if (segment2.RedB != this.RedB)
            {
                updateEvents.Add(UPDATEEVENT.REDCARDB);
                _redB = segment2.RedB;
            }

            if (segment2.StartDTS != this.StartDTS)
            {
                _dts = segment2.StartDTS;
            }

            if (segment2.HalftimeScore != this.HalftimeScore)
            {
                _halftimeScore = segment2.HalftimeScore;
            }

            if (segment2.GameState != this.GameState)
            {
                _state = segment2.GameState;

                EventHandler<EventArgs> handler = GameStateChangedEvent;
                if (handler != null)
                {                    
                    handler(this, new EventArgs());
                }
            }



            return (UPDATEEVENT[])updateEvents.ToArray(typeof(UPDATEEVENT));
        }

        private static DateTime buildDTS(String dts)
        {
            dts = dts.Replace("'", String.Empty);
            String[] seps = dts.Split(new char[] { ',' });

            if (seps.Length < 5)
                throw new FormatException("Falsche DTS Format");

            DateTime d = new DateTime(Int32.Parse(seps[0], CultureInfo.InvariantCulture), Int32.Parse(seps[1], CultureInfo.InvariantCulture),
                Int32.Parse(seps[2], CultureInfo.InvariantCulture), Int32.Parse(seps[3], CultureInfo.InvariantCulture),
                Int32.Parse(seps[4], CultureInfo.InvariantCulture), 0);

            String id = "China Standard Time";
            //String id = "Indochina Time";
            TimeZoneInfo tziChina;
            tziChina = TimeZoneInfo.FindSystemTimeZoneById(id);

            TimeZoneInfo tziLocal = TimeZoneInfo.Local;


            d = TimeZoneInfo.ConvertTime(d, tziChina, tziLocal);

            return d;
        }

    }

    class Segment2List : SortedList<int, LivetickerSegment2> { }

}
