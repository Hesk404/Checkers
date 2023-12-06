using System.Diagnostics;
using System.Threading;
using Checkers.Properties;
using Checkers.ViewModels;
using CheckersBase;
using CheckersBase.BrainBase;
using CheckersRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Models.Players
{
    class AIPlayer : BasePlayer
    {
        private BrainBase _brain;

        public BrainBase Brain { get { return _brain; } }

        public AIPlayer(BrainBase brain) : base()
        {
            _brain = brain;
           
        }

        public override string Name { get { return (_brain != null ? _brain.GetName() : "AIPlayer");} }

        public override void RequestMotion(BoardViewModel boardViewModel, bool isWhite)
        {
            RequestMotionAsync(boardViewModel.board, isWhite);
        }

        private async void RequestMotionAsync(Board board, bool isWhite)
        {
            var mtn = await FindMotion(board, isWhite);

            if (mtn != null)
                BoardcastMotion(mtn);
            else
                OnBroadcastGameOver(EndGameEnum.EG_TIMEOUT);
        }

        private Task<Motion> FindMotion(Board board, bool isWhite)
        {
	        bool hasTimeout = Settings.Default.BotTimeLimit != 0;
            var timeout = TimeSpan.FromMilliseconds(Settings.Default.BotTimeLimit);

            return Task.Run(() =>
            {
                var longRunningTask = new Task<Motion>(() =>
                {
                    return _brain.FindMotion(board, isWhite);
                }, TaskCreationOptions.LongRunning);

                longRunningTask.Start();
	            if (!hasTimeout)
	            {
		            longRunningTask.Wait();
		            return longRunningTask.Result;
	            }

	            if (longRunningTask.Wait(timeout))
		            return longRunningTask.Result;

	            return null;
            });

        }


       
    }
}
