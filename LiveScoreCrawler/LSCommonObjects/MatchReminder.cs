using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml;
using System.Diagnostics;

namespace LSCommonObjects
{
    public class MatchReminder
    {
        XDocument _feed;
        public MatchReminder()
        {
            String reminderPath = Environment.CurrentDirectory + @"\MatchReminder.xml";

            _feed = XDocument.Load(reminderPath);            
        }

        public bool isMatchReminded(ulong matchId)
        {

            DateTime startDts = DateTime.Now;

            var reminders = from reminder in _feed.Descendants("Reminder")
                               where reminder.Attribute("matchId").Value.Trim() == matchId.ToString()
                                && reminder.Attribute("version").Value.Trim() == "1"
                               select new
                               {
                                   MatchId = reminder.Attribute("matchId").Value.Trim(),
                               };

            DateTime endDts = DateTime.Now;
            Debug.WriteLine(String.Format("Time expired checked Reminded: {0}", endDts.Subtract(startDts).ToString()));
            foreach (var r in reminders)
            {
                return true;
            }

            return false;
        }

        public void insertMatchReminder(ulong matchId)
        {
            DateTime startDts = DateTime.Now;
            String reminderPath = Environment.CurrentDirectory + @"\MatchReminder.xml";
            XElement element = (XElement)_feed.FirstNode;
            XElement newElement = new XElement("Reminder");
            XAttribute attriMatchId = new XAttribute("matchId",matchId);
            XAttribute attriVersion = new XAttribute("version", 1);
            newElement.Add(attriMatchId);
            newElement.Add(attriVersion);

            element.Add(newElement);

            _feed.Save(reminderPath);
            DateTime endDts = DateTime.Now;
            Debug.WriteLine(String.Format("Time expired insert Reminded: {0}", endDts.Subtract(startDts).ToString()));
        }
    }
}
