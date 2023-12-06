using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Threading;
using Checkers.Models;
using Checkers.Models.Players;
using CheckersBase;
using CheckersBase.BrainBase;
using System.Threading;

namespace Checkers.ViewModels
{
	internal class TournamentViewModel : ViewModelBase
	{
		public TournamentSettingsViewModel TournamentSettingsVM { get; set; }
		private string _logText;

		public List<Game> PendingGames { get; set; }

		public ObservableCollection<Game> FinishedGames { get; set; }

		public List<Game> RunningGames { get; set; }

		public ObservableCollection<Round> Rounds { get; set; }

		public Dictionary<BrainBase, int> Wins { get; set; }

		public string LogText
		{
			get { return _logText; }
			set { _logText = value; OnPropertyChanged(() => LogText); }
		}

		private Dispatcher UIDispatcher { get; set; }

		public double Progress
		{
			get
			{
				var totalGames = (double)PendingGames.Count + RunningGames.Count + FinishedGames.Count;
				var finishedGames = (double)FinishedGames.Count;
				if (Math.Abs(totalGames) < 0.000001) return 100;
				return Math.Round(100 * finishedGames / totalGames, 2);

			}
		}
		public TournamentViewModel()
		{
			PendingGames = new List<Game>();
			FinishedGames = new ObservableCollection<Game>();
			RunningGames = new List<Game>();
			FinishedGames.CollectionChanged += (sender, args) => OnPropertyChanged(() => Progress);
			Rounds = new ObservableCollection<Round>();
			taskFactory = new TaskFactory();
			UIDispatcher = Dispatcher.CurrentDispatcher;
		}
		public void Stop()
		{
			PendingGames.Clear();
			RunningGames.ForEach(g => g.Dispose());
			RunningGames.Clear();
		}
		TaskFactory taskFactory;

        const string TOURNAMENT_BEGAN = "Турнир начался"; // It has begun!
        const string TOURNAMENT_FINISHED = "Турнир окончен";
		public void Start()
		{
            Log(TOURNAMENT_BEGAN);

			Wins = new Dictionary<BrainBase, int>();
			TournamentSettingsVM.SelectedBots.ToList().ForEach(brainVm => Wins.Add(brainVm.Brain, 0));

			for (int i = 0; i < TournamentSettingsVM.SelectedBots.Count - 1; i++)
			{
				for (int j = i + 1; j < TournamentSettingsVM.SelectedBots.Count; j++)
				{
					for (int gameInd = 1; gameInd <= TournamentSettingsVM.NumberOfGamesPerFight; gameInd++)
					{
						var firstPlayerInd = gameInd % 2 == 0 ? i : j;
						var secondPlayerInd = gameInd % 2 == 0 ? j : i;
						var game = new Game(new AIPlayer(TournamentSettingsVM.SelectedBots[firstPlayerInd].Brain),
							new AIPlayer(TournamentSettingsVM.SelectedBots[secondPlayerInd].Brain));

						PendingGames.Add(game);
						game.OnGameOver += (g, gameResult) =>
						{
							currentRunningThreads--;
							FinishedGames.Add(game);
							RunningGames.Remove(game);
							var winner = gameResult.Winner as AIPlayer;
							if (winner != null)
							{
								UIDispatcher.Invoke(() => Rounds.Add(new Round()
								                                     {
									                                     BlackPlayer = (game.BlackPlayer as AIPlayer),
									                                     WhitePlayer = (game.WhitePlayer as AIPlayer),
									                                     Winner = winner
								                                     }));
								
							}
							
                            Log(gameResult.ToString());

							if (PendingGames.Count == 0 && RunningGames.Count == 0)
							{
                                Log(TOURNAMENT_FINISHED);
                                Rating.Instance.SaveChanges();
								ShowResults();
							}
							else
							{
								StartPendingGames();
							}
						};
					}

				}
			}

			StartPendingGames();
		}

		private int currentRunningThreads = 0;
		private void StartPendingGames()
		{
			if (currentRunningThreads < TournamentSettingsVM.MaxThreads || PendingGames.Count > 0)
			{
				while (currentRunningThreads < TournamentSettingsVM.MaxThreads)
				{
					if (PendingGames.Count == 0) break;

					var gameToRun = PendingGames.First();

					RunningGames.Add(gameToRun);
					Log("Начало партии (" + RunningGames.Count + " игр запушено) ");
					taskFactory.StartNew(PendingGames[0].Start);
					currentRunningThreads++;
					PendingGames.Remove(gameToRun);


				}
			}
		}
		private void ShowResults()
		{
			Log("");
			Log("Результаты турнира: ");
			var allBots = TournamentSettingsVM.SelectedBots.Select(b => b.Brain);
			Dictionary<BrainBase, int> botWins = new Dictionary<BrainBase, int>();

			foreach (var currentBot in allBots)
			{
				BrainBase bot = currentBot;
				var roundsForThisBot = Rounds.Where(r => r.BlackPlayer.Brain == bot || r.WhitePlayer.Brain == bot);
				List<AIPlayer> usedAnotherPlayers = new List<AIPlayer>();
				foreach (var round in roundsForThisBot)
				{
					var anotherPlayer = round.BlackPlayer.Brain == bot ? round.WhitePlayer : round.BlackPlayer;
					if (usedAnotherPlayers.Contains(anotherPlayer)) continue;
					var roundsForThisPair =
						roundsForThisBot.Where(
							r =>
								(r.BlackPlayer.Brain == bot && r.WhitePlayer == anotherPlayer) ||
								(r.BlackPlayer == anotherPlayer && r.WhitePlayer.Brain == bot));
					bool didCurrentBotWin = roundsForThisPair.Count(r => r.Winner.Brain == bot) >
											roundsForThisPair.Count(r => r.Winner == anotherPlayer);
					if (didCurrentBotWin)
						Wins[bot]++;
					usedAnotherPlayers.Add(anotherPlayer);

				}
			}

			int counter = 1;
			foreach (var win in Wins.OrderBy(w => -w.Value))
			{
                Log((counter++) + ") " + win.Key.GetName() + "(" + win.Key.GetStudent() + ") - " + win.Value + " побед.");
			}
		}

		private void Log(string message)
		{
			LogText += message + Environment.NewLine;
		}

	}

	class Round : ViewModelBase
	{
		public AIPlayer WhitePlayer { get; set; }

		public AIPlayer BlackPlayer { get; set; }
		public AIPlayer Winner { get; set; }
	}
}
