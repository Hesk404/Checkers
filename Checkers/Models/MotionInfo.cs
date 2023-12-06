using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;
using Checkers.Models.Players;
using Checkers.ViewModels;
using CheckersBase;

namespace Checkers.Models
{
    static class MotionInfoConverter
    {
        static string[] _letters = new string[] { "A", "B", "C", "D", "E", "F", "G", "H" };

        public static string ToString(bool? isWhite, Motion mtn, string gameOverMessage)
        {
            string ret = GetColorPart(isWhite);

            if (!String.IsNullOrEmpty(gameOverMessage))
                ret += gameOverMessage;

            ret += GetMotionPart(mtn);

            return ret;
        }

        private static string GetColorPart(bool? isWhite)
        {
            if (isWhite == null)
                return String.Empty;

            return isWhite.Value ? "Белые: " : "Черные: ";
        }

        private static string GetMotionPart(Motion mtn)
        {
            string ret = String.Empty;

            if (mtn == null)
                return ret;

            foreach(var m in mtn.Moves)
            {
                ret += String.Format(" {0}{1} ", _letters[m.X], m.Y + 1);
            }
            return ret;
        }

    }

    internal class MotionInfo
    {
        public Motion Motion { get; set; }
        public BoardViewModel ResultBoard { get; set; }
        public bool? PlayerColorWhite { get; set; }

        public string GameOverMessage { get; set; }

        public override string ToString()
        {
            return MotionInfoConverter.ToString(PlayerColorWhite,Motion,GameOverMessage);
        }
    }
}
