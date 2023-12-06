using Checkers.Models.Players;
using CheckersBase.BrainBase;
using System;
using System.Linq;
using CheckersBase;
using System.Timers;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;

namespace Checkers.Models
{
    public class TSingleton<T> where T : class, new()
    {
        private static T _instance;
        private static object _initLock = new object();

        public static T Instance
        {
            get
            {
                lock (_initLock)
                {
                    if (_instance == null)
                    {
                        _instance = new T();
                    }
                    return _instance;
                }
            }
        }
    }

    public class BrainStats
    {
        public int Wins { get; set; }
        public int Loses { get; set; }
        public int Draws { get; set; }

        public string WinRate 
        { 
            get
            {
                double wins = (double)Wins;
                double totalGames = (double)(Wins + Loses + Draws);
                double winRate = totalGames > 0 ? wins / totalGames : 0;

                return String.Format("{0}%", Math.Round(winRate, 2));
            } 
        }

        public BrainStats(int wins, int loses, int draws)
        {
            Wins = wins;
            Loses = loses;
            Draws = draws;
        }
        public BrainStats() :  this(0,0,0)
        {

        }
    }

    public class Rating : TSingleton<Rating>
    {
        const string DOC_NAME = "stats.data";
        XElement _root;
        public Rating()
        {
            if (!File.Exists(DOC_NAME))
            {
                _root = new XElement("Stats");
                SaveChanges();
            }
            _root = XElement.Load(DOC_NAME);
        }


        public void DispatchWinGameResult(BrainBase winner, BrainBase looser)
        {
            try
            {
                IncreaseValue(FindPlayer(winner), "Wins");
                IncreaseValue(FindPlayer(looser), "Loses");
            }
            catch { }
        }
        public void DispatchDrawGameResult(BrainBase white, BrainBase black)
        {
            try
            {
                IncreaseValue(FindPlayer(white), "Draws");
                IncreaseValue(FindPlayer(black), "Draws");
            }
            catch { }
        }

        private void IncreaseValue(XElement elem, string attrName)
        {
            var attr = elem.Attribute(attrName);
            var val = Int32.Parse(attr.Value);
            val++;
            attr.Value = val.ToString();
        }


        public BrainStats GetBrainStats( BrainBase brain )
        {
            try
            {
                var elem = FindPlayer(brain);

                int wins = Int32.Parse(elem.Attribute("Wins").Value);
                int loses = Int32.Parse(elem.Attribute("Loses").Value);
                int draws = Int32.Parse(elem.Attribute("Draws").Value);

                return new BrainStats(wins, loses, draws);
            }
            catch
            {
                return new BrainStats();
            }
        }

        private XElement FindPlayer(BrainBase brain)
        {
            string name = brain.GetName();
            string student = brain.GetStudent();
            string studentGroup = brain.GetStudentGroup();

            var elems = from el in _root.Elements("Player")
                        where   (string)el.Attribute("Name") == name &&
                                (string)el.Attribute("Student") == student &&
                                (string)el.Attribute("StudentGroup") == studentGroup
                          select el;

            var elem = elems.FirstOrDefault();
            
            if(elem == null)
            {
                elem = new XElement("Player",
                    new XAttribute("Name", name),
                    new XAttribute("Student", student),
                    new XAttribute("StudentGroup", studentGroup),
                    new XAttribute("Wins", 0),
                    new XAttribute("Loses", 0),
                    new XAttribute("Draws", 0));

                _root.Add(elem);
            }

            return elem;
        }


        public void SaveChanges()
        {
            _root.Save(DOC_NAME);
        }

    }
}
