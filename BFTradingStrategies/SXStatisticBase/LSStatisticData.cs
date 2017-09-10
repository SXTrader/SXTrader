using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace net.sxtrader.bftradingstrategies.sxstatisticbase
{
   
        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://sxtrader.net/")]
        public class LSHistoricDataStatistic : object, System.ComponentModel.INotifyPropertyChanged
        {

            private LSHistoricMatch[] directField;

            private LSHistoricMatch[] teamAField;

            private LSHistoricMatch[] teamBField;

            /// <remarks/>
            //[System.Xml.Serialization.XmlArrayAttribute(Order = 0)]
            public LSHistoricMatch[] Direct
            {
                get
                {
                    return this.directField;
                }
                set
                {
                    this.directField = value;
                    this.RaisePropertyChanged("Direct");
                }
            }

            /// <remarks/>
            
            [System.Xml.Serialization.XmlArrayAttribute(Order = 1)]
            public LSHistoricMatch[] TeamA
            {
                get
                {
                    return this.teamAField;
                }
                set
                {
                    this.teamAField = value;
                    this.RaisePropertyChanged("TeamA");
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayAttribute(Order = 2)]
            public LSHistoricMatch[] TeamB
            {
                get
                {
                    return this.teamBField;
                }
                set
                {
                    this.teamBField = value;
                    this.RaisePropertyChanged("TeamB");
                }
            }

            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

            protected void RaisePropertyChanged(string propertyName)
            {
                System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
                if ((propertyChanged != null))
                {
                    propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://sxtrader.net/")]
        public class LSHistoricMatch : object, System.ComponentModel.INotifyPropertyChanged
        {

            private LSHistoricMatchEvent[] eventsField;

            private ulong matchIdField;

            private System.DateTime matchDateField;

            private string teamAField;

            private ulong teamAIdField;

            private string teamBField;

            private ulong teamBIdField;

            private string devisionField;

            private uint scoreAField;

            private uint scoreBField;

            private string halftimeScoreField;

            private int firstGoalMinuteField;

            /// <remarks/>
            [System.Xml.Serialization.XmlArrayAttribute(Order = 0)]
            public LSHistoricMatchEvent[] Events
            {
                get
                {
                    return this.eventsField;
                }
                set
                {
                    this.eventsField = value;
                    this.RaisePropertyChanged("Events");
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public ulong MatchId
            {
                get
                {
                    return this.matchIdField;
                }
                set
                {
                    this.matchIdField = value;
                    this.RaisePropertyChanged("MatchId");
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public System.DateTime MatchDate
            {
                get
                {
                    return this.matchDateField;
                }
                set
                {
                    this.matchDateField = value;
                    this.RaisePropertyChanged("MatchDate");
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string TeamA
            {
                get
                {
                    return this.teamAField;
                }
                set
                {
                    this.teamAField = value;
                    this.RaisePropertyChanged("TeamA");
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public ulong TeamAId
            {
                get
                {
                    return this.teamAIdField;
                }
                set
                {
                    this.teamAIdField = value;
                    this.RaisePropertyChanged("TeamAId");
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string TeamB
            {
                get
                {
                    return this.teamBField;
                }
                set
                {
                    this.teamBField = value;
                    this.RaisePropertyChanged("TeamB");
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public ulong TeamBId
            {
                get
                {
                    return this.teamBIdField;
                }
                set
                {
                    this.teamBIdField = value;
                    this.RaisePropertyChanged("TeamBId");
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string Devision
            {
                get
                {
                    return this.devisionField;
                }
                set
                {
                    this.devisionField = value;
                    this.RaisePropertyChanged("Devision");
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public uint ScoreA
            {
                get
                {
                    return this.scoreAField;
                }
                set
                {
                    this.scoreAField = value;
                    this.RaisePropertyChanged("ScoreA");
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public uint ScoreB
            {
                get
                {
                    return this.scoreBField;
                }
                set
                {
                    this.scoreBField = value;
                    this.RaisePropertyChanged("ScoreB");
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string HalftimeScore
            {
                get
                {
                    return this.halftimeScoreField;
                }
                set
                {
                    this.halftimeScoreField = value;
                    this.RaisePropertyChanged("HalftimeScore");
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public int FirstGoalMinute
            {
                get
                {
                    return this.firstGoalMinuteField;
                }
                set
                {
                    this.firstGoalMinuteField = value;
                    this.RaisePropertyChanged("FirstGoalMinute");
                }
            }

            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

            protected void RaisePropertyChanged(string propertyName)
            {
                System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
                if ((propertyChanged != null))
                {
                    propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
        [System.SerializableAttribute()]
        [System.Diagnostics.DebuggerStepThroughAttribute()]
        [System.ComponentModel.DesignerCategoryAttribute("code")]
        [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://sxtrader.net/")]
        public class LSHistoricMatchEvent : object, System.ComponentModel.INotifyPropertyChanged
        {

            private ulong matchIdField;

            private ulong teamIdField;

            private MATCHEVENTTYPE eventTypeField;

            private string infoEvent1Field;

            private string infoEvent2Field;

            private int eventMinuteField;

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public ulong MatchId
            {
                get
                {
                    return this.matchIdField;
                }
                set
                {
                    this.matchIdField = value;
                    this.RaisePropertyChanged("MatchId");
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public ulong TeamId
            {
                get
                {
                    return this.teamIdField;
                }
                set
                {
                    this.teamIdField = value;
                    this.RaisePropertyChanged("TeamId");
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public MATCHEVENTTYPE EventType
            {
                get
                {
                    return this.eventTypeField;
                }
                set
                {
                    this.eventTypeField = value;
                    this.RaisePropertyChanged("EventType");
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string InfoEvent1
            {
                get
                {
                    return this.infoEvent1Field;
                }
                set
                {
                    this.infoEvent1Field = value;
                    this.RaisePropertyChanged("InfoEvent1");
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public string InfoEvent2
            {
                get
                {
                    return this.infoEvent2Field;
                }
                set
                {
                    this.infoEvent2Field = value;
                    this.RaisePropertyChanged("InfoEvent2");
                }
            }

            /// <remarks/>
            [System.Xml.Serialization.XmlAttributeAttribute()]
            public int EventMinute
            {
                get
                {
                    return this.eventMinuteField;
                }
                set
                {
                    this.eventMinuteField = value;
                    this.RaisePropertyChanged("EventMinute");
                }
            }

            public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

            protected void RaisePropertyChanged(string propertyName)
            {
                System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
                if ((propertyChanged != null))
                {
                    propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
                }
            }
        }

        /// <remarks/>
        [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Xml", "2.0.50727.3082")]
        [System.SerializableAttribute()]
        [System.Xml.Serialization.XmlTypeAttribute(Namespace = "http://sxtrader.net/")]
        public enum MATCHEVENTTYPE
        {

            /// <remarks/>
            GOAL,

            /// <remarks/>
            PENALTY,

            /// <remarks/>
            OWNGOAL,

            /// <remarks/>
            YELLOWCARD,

            /// <remarks/>
            REDCARD,

            /// <remarks/>
            YELLOWTORED,
        }       
}
