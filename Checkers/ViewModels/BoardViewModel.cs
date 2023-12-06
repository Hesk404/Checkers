using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Checkers.Commands;
using Checkers.Models;
using CheckersBase;
using Checkers.Models.Players;
using Point = CheckersBase.Point;

namespace Checkers.ViewModels
{
    class BoardViewModel : ViewModelBase
    {
        public Board board { get; private set; }

        public delegate void SelectCellHandler(CheckersBase.Point point);

        public delegate void BoardChangeCancelHandler();

        public event SelectCellHandler OnSelectCell;
        public event BoardChangeCancelHandler BoardChangeCancel;

        public void OnBoardChangeCancel()
        {
            BoardChangeCancelHandler handler = BoardChangeCancel;
            if (handler != null) handler();
        }

        public ObservableCollection<CheckersBase.Point> AvailableMovesPieces
        {
            get { return _availableMovesPieces; }
            set { 
                _availableMovesPieces = value;
                OnPropertyChanged(() => AvailableMovesPieces);
            }
        }

        #region bool CurrentPlayerWhite
        bool? _currentPlayerWhite;
        public bool? CurrentPlayerWhite
        {
            get { return _currentPlayerWhite; }
            set { _currentPlayerWhite = value; OnPropertyChanged(() => CurrentPlayerWhite); }
        } 
        #endregion

        public BoardViewModel(Board board)
        {
            this.board = board;
            SelectedPiece = null;
            AvailableMovesPieces = new ObservableCollection<Point>();

        }

        private CheckersBase.Point _selectedPiece;
        public CheckersBase.Point SelectedPiece
        {
            get { return _selectedPiece; }
            set
            {
                _selectedPiece = value; 
                OnPropertyChanged(() => SelectedPiece);
            }
        }

        #region bool Frozen
        private bool _frozen;
        public bool Frozen
        {
            get { return _frozen; }
            set { _frozen = value; OnPropertyChanged(() => Frozen); }
        } 
        #endregion

        public IPiece this[int x, int y]
        {
            get
            {
                if (board == null)
                    return new BlankPiece();
                return board[x, y];
            }
            set
            {
                if (board == null) return;
                board[x, y] = value;
                OnPropertyChanged(() => this);
            }
        }

        public void UpdateBoard(Board newBoard)
        {
            for (int i = 0; i < Board.BOARD_SIZE; i++)
            {
                for (int j = 0; j < Board.BOARD_SIZE; j++)
                {
                    this[i, j] = newBoard[i, j];
                    
                }
            }

            // TODO: Не работает через this, делаю через SelectedPiece
            OnPropertyChanged(() => this); 
            OnPropertyChanged(() => SelectedPiece);
            
        }
        public void UpdateUI()
        {
            OnPropertyChanged(() => this);
        }
        public IPiece this[CheckersBase.Point pt]
        {
            get { return board[pt.X, pt.Y]; }
            set
            {
                board[pt.X, pt.Y] = value;
                OnPropertyChanged(() => this);
            }
        }

        #region Commands

        #region MouseDownCommand
        private DelegateCommand<CheckersBase.Point> _mouseDownCommand;
        ObservableCollection<Point> _availableMovesPieces;


        public DelegateCommand<CheckersBase.Point> MouseDownCommand
        {
            get
            {
                if (_mouseDownCommand == null)
                    _mouseDownCommand = new DelegateCommand<CheckersBase.Point>(MouseDown, CanMouseDown);
                return _mouseDownCommand;
            }
        }

        private void MouseDown(CheckersBase.Point pt)
        {
            if (!Frozen)
                OnSelectCell(pt);
        }

        private bool CanMouseDown(CheckersBase.Point pt)
        {
            return true;
        } 
        #endregion

        #endregion

    }
}
