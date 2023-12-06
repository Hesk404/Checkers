using Checkers.ViewModels;
using CheckersBase;
using CheckersRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Models.Players
{
    abstract class BasePlayer
    {
        public abstract void RequestMotion(BoardViewModel boardViewModel, bool isWhite);

        public abstract string Name { get; }

        public delegate void MotionHandler(Motion data);
        public event MotionHandler BroadcastMotion;

		public delegate void GameOverILooseHandler(BasePlayer sender, EndGameEnum type);
        public event GameOverILooseHandler BroadcastGameOverILoose;

        public BasePlayer() { }

        protected void BoardcastMotion(Motion mtn)
        {
            if (BroadcastMotion != null)
                BroadcastMotion(mtn);
        }

        protected void OnBroadcastGameOver(EndGameEnum type)
        {
            if (BroadcastGameOverILoose != null)
                BroadcastGameOverILoose(this,type);
        }

        internal virtual void OnSelectCell(Point point) { }

        internal virtual void OnBoardChangeCancel()  {  }
    }
}
