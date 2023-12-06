using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Checkers.ViewModels;
using CheckersBase;
using Checkers.Models.Players;
using CheckersRules;

namespace Checkers.Models
{
	class Game : IDisposable
	{
		private BoardViewModel _boardViewModel;
		private BasePlayer _whitePlayer, _blackPlayer, _currentPlayer;

		private int _movesWithoutBeats = 0;
		private const int _MOVES_WITHOUT_BEATS_LIMIT = 30;

		public GameOverEventHandler OnGameOver;

		/// <summary>
		/// Связь с GUI доски
		/// </summary>
		public BoardViewModel BoardViewModel { get { return _boardViewModel; } }

		public HistoryViewModel HistoryViewModel { get; set; }

		public BasePlayer WhitePlayer { get { return _whitePlayer; } }

		public BasePlayer BlackPlayer { get { return _blackPlayer; } }

		public Game(BasePlayer white, BasePlayer black)
		{
			_whitePlayer = white;
			_whitePlayer.BroadcastMotion += PlayerPrepareMotion;
			_whitePlayer.BroadcastGameOverILoose += GameOverPlayerLoose;

			_blackPlayer = black;
			_blackPlayer.BroadcastMotion += PlayerPrepareMotion;
			_blackPlayer.BroadcastGameOverILoose += GameOverPlayerLoose;

			_boardViewModel = new BoardViewModel(Board.CreateStartingBoard());

			_currentPlayer = _whitePlayer;
			BoardViewModel.CurrentPlayerWhite = true;


		}

		private void GameOverPlayerLoose(BasePlayer loosePlayer, EndGameEnum type)
		{
			var winner = IsWhite(loosePlayer)
				? BlackPlayer
				: WhitePlayer;

			GameOver(type, winner);
		}

		/// <summary>
		/// Обработка хода игрока. 
		/// Функция вызывается по событию BroadcastMotion, запускаемому игроком
		/// Применяет ход к доске, если правильный - запрашивает ход у другого игрока, если неправильный, делает конец игры
		/// </summary>
		/// <param name="mtn">Ход игрока</param>
		private void PlayerPrepareMotion(Motion mtn)
		{
			bool currentIsWhite = _currentPlayer == _whitePlayer;

			var board = (Board)BoardViewModel.board.Clone();
			// покажем результат хода вне зависимости от его правильности 
			BoardViewModel.UpdateBoard(Rules.ApplyMotion(BoardViewModel.board, mtn, currentIsWhite));

			EndGameEnum endGame = EndGameEnum.EG_NONE;

			//проверяем ход, актуально для игрока-компьютера
			if (ValidateMotion(board, mtn, IsWhite(_currentPlayer)) != MotionValidationEnum.VALID)
			{
				//если все плохо
				endGame = EndGameEnum.EG_INVALID_MOTION;
			}
			else
			{
				bool beated;

				//применяем ход
				var nextBoard = Rules.ApplyMotion(board, mtn, currentIsWhite, out beated);
				BoardViewModel.UpdateBoard(nextBoard);

				var motionInfo = new MotionInfo()
				{
					Motion = mtn,
					PlayerColorWhite = IsWhite(_currentPlayer),
					ResultBoard = new BoardViewModel(nextBoard)
				};
				if (HistoryViewModel != null)
					HistoryViewModel.AddMotion(motionInfo);

				if (beated)
					_movesWithoutBeats = 0;
				else
					_movesWithoutBeats++;

				if (_movesWithoutBeats >= _MOVES_WITHOUT_BEATS_LIMIT)
				{
					endGame = EndGameEnum.EG_DRAW;
				}
				else
				{
					// меняем игрока
					TogglePlayer();

					// смотрим, может ли он ходит - есть ли шашки 
					endGame = Rules.CheckGameOver(BoardViewModel.board);
					if (endGame == EndGameEnum.EG_NONE)
					{
						// или доступные ходы
						if (NoValidMotions(BoardViewModel.board, IsWhite(_currentPlayer)))
							endGame = IsWhite(_currentPlayer) ? EndGameEnum.EG_WIN_BLACK : EndGameEnum.EG_WIN_WHITE;
					}
				}
			}

			switch (endGame)
			{
				// Все нормально, продолжаем игру
				case EndGameEnum.EG_NONE:
					RequestMotion();
					break;

				case EndGameEnum.EG_INVALID_MOTION:
					{
						var motionInfo = new MotionInfo()
						{
							Motion = mtn,
							PlayerColorWhite = IsWhite(_currentPlayer),
							ResultBoard = new BoardViewModel(BoardViewModel.board),
							GameOverMessage = GetEnumDescription(endGame)
						};
						if (HistoryViewModel != null)
							HistoryViewModel.AddMotion(motionInfo);
						var winner = IsWhite(_currentPlayer)
						? BlackPlayer
						: WhitePlayer;
						GameOver(EndGameEnum.EG_INVALID_MOTION, winner);
					}
					
					break;
				case EndGameEnum.EG_WIN_BLACK:
					{
						var motionInfo = new MotionInfo()
						{
							ResultBoard = new BoardViewModel(BoardViewModel.board),
							GameOverMessage = GetEnumDescription(endGame)
						};
						if (HistoryViewModel != null)
							HistoryViewModel.AddMotion(motionInfo);
						GameOver(endGame, BlackPlayer);
						break;
					}
				case EndGameEnum.EG_WIN_WHITE:
					{
						var motionInfo = new MotionInfo()
						{
							ResultBoard = new BoardViewModel(BoardViewModel.board),
							GameOverMessage = GetEnumDescription(endGame)
						};
						if (HistoryViewModel != null)
							HistoryViewModel.AddMotion(motionInfo);
						GameOver(endGame, WhitePlayer);
						break;
					}
				default:
					{
						var motionInfo = new MotionInfo()
						{
							ResultBoard = new BoardViewModel(BoardViewModel.board),
							GameOverMessage = GetEnumDescription(endGame)
						};
						if (HistoryViewModel != null)
							HistoryViewModel.AddMotion(motionInfo);
						GameOver(endGame, null);
					}
					
					break;
			}



		}

		private bool IsWhite(BasePlayer player)
		{
			return _whitePlayer == player;
		}

		private static MotionValidationEnum ValidateMotion(Board board, Motion mtn, bool isWhite)
		{
			var validator = Rules.FindValidMotions(board, isWhite);
			return validator.ValidateMotion(mtn);
		}

		private static bool NoValidMotions(Board board, bool isWhite)
		{
			var validator = Rules.FindValidMotions(board, isWhite);
			return validator.NoValidMotions();
		}

		private void GameOver(EndGameEnum type, BasePlayer winner)
		{
			if (OnGameOver != null)
			{
                OnGameOver(this, new GameOverEventArgs(type, winner, WhitePlayer, BlackPlayer));
			}

            DipatchResult(WhitePlayer, BlackPlayer, winner);
		}

        private static void DipatchResult(BasePlayer white, BasePlayer black, BasePlayer winner)
        {
            var whiteAI = white as AIPlayer;
            var blackAI = black as AIPlayer;

            if (whiteAI == null || blackAI == null)
                return;

            if (winner == null)
            {
                Rating.Instance.DispatchDrawGameResult(white == winner ? whiteAI.Brain : blackAI.Brain,
                                             white == winner ? blackAI.Brain : whiteAI.Brain);
            }
            else
                Rating.Instance.DispatchWinGameResult(whiteAI.Brain, blackAI.Brain);
        }

		public static string GetEnumDescription(Enum value)
		{
			FieldInfo fi = value.GetType().GetField(value.ToString());

			DescriptionAttribute[] attributes =
				(DescriptionAttribute[])fi.GetCustomAttributes(
				typeof(DescriptionAttribute),
				false);

			if (attributes != null &&
				attributes.Length > 0)
				return attributes[0].Description;
			else
				return value.ToString();
		}
		public void Start()
		{
			RequestMotion();
		}

		private void RequestMotion()
		{
			var secondPlayer = _currentPlayer == _blackPlayer ? _whitePlayer : _blackPlayer;

			BoardViewModel.OnSelectCell -= secondPlayer.OnSelectCell;
			BoardViewModel.OnSelectCell += _currentPlayer.OnSelectCell;

			BoardViewModel.BoardChangeCancel += _currentPlayer.OnBoardChangeCancel;
			BoardViewModel.BoardChangeCancel -= secondPlayer.OnBoardChangeCancel;

			_currentPlayer.RequestMotion(BoardViewModel, IsWhite(_currentPlayer));
		}

		private void TogglePlayer()
		{
			_currentPlayer = (_currentPlayer == _whitePlayer ? _blackPlayer : _whitePlayer);
			BoardViewModel.CurrentPlayerWhite = !BoardViewModel.CurrentPlayerWhite.Value;
		}



		public void Dispose()
		{
			_boardViewModel = null;
			_blackPlayer.BroadcastMotion -= PlayerPrepareMotion;
			_whitePlayer.BroadcastMotion -= PlayerPrepareMotion;
			_blackPlayer.BroadcastGameOverILoose -= GameOverPlayerLoose;
			_whitePlayer.BroadcastGameOverILoose -= GameOverPlayerLoose;


		}


	}

	delegate void GameOverEventHandler(Game sender, GameOverEventArgs eventArgs);

	class GameOverEventArgs : EventArgs
	{
        public EndGameEnum Reason { get; set; }
		public BasePlayer Winner { get; set; }
        public BasePlayer WhitePlayer { get; set; }
        public BasePlayer BlackPlayer { get; set; }
		public GameOverEventArgs(EndGameEnum reason,BasePlayer winner, BasePlayer white, BasePlayer black )
		{
			Reason = reason;
			Winner = winner;
            WhitePlayer = white;
            BlackPlayer = black;
		}

        public override string ToString()
        {
            return GetGameInfo() + GetGameResult();
        }

        private string GetGameInfo()
        {
            return String.Format("Партия [{0} против {1}] закончена. ", WhitePlayer.Name, BlackPlayer.Name);
        }

        private string GetGameResult()
        {
            if (Reason == EndGameEnum.EG_DRAW)
                return String.Format("Результат: {0}",Game.GetEnumDescription(Reason));
            else
                return String.Format("Результат: выиграл {0} : {1}",Winner.Name, Game.GetEnumDescription(Reason));
        }


	}
}
