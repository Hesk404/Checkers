using System.Collections.ObjectModel;
using System.Diagnostics;
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
    class HumanPlayer : BasePlayer
    {
        MotionValidator _validator;
        Motion _motion;
        List<Point> _validPoints;

        /// <summary>
        /// Храним состояние доски на момент начала хода для возможности отмены действия
        /// </summary>
        private Board initialBoard;

        /// <summary>
        /// Текущее временное состояние доски
        /// </summary>
        private BoardViewModel currentBoard;

        public override string Name { get { return "Человек"; } }

        public override void RequestMotion(BoardViewModel boardViewModel, bool isWhite)
        {
            initialBoard = (Board) boardViewModel.board.Clone();
            currentBoard = boardViewModel;
            //_active = true;
            _motion = new Motion();

            // создаем валидатор для текущего состояния
            _validator = Rules.FindValidMotions(boardViewModel.board, isWhite);

            // ищем доступные ходы
            _validPoints = _validator.FindValidPoints(_motion);
            currentBoard.AvailableMovesPieces = new ObservableCollection<Point>(_validPoints);
        }

        internal override void OnSelectCell(Point selection)
        {
            currentBoard.AvailableMovesPieces.Clear();
            //if (!_active)
            //    return;


            // Кликнули на точку, которая не соответствует правилам
            if(!_validPoints.Contains(selection))
                return;

            if (_motion.Moves.Any())
                currentBoard.SelectedPiece = _motion.Moves.Last();

            // Не выбирать первую ячейку, если она пустая
            if (currentBoard.SelectedPiece == null && currentBoard[selection].Type == PieceTypes.None) return;

            if (currentBoard.SelectedPiece != null && currentBoard[currentBoard.SelectedPiece].Type != PieceTypes.None)
            {
                if (currentBoard.SelectedPiece.Equals(selection))
                    return;
                currentBoard[selection] = currentBoard[currentBoard.SelectedPiece];
                currentBoard[currentBoard.SelectedPiece] = new BlankPiece();
            }
            currentBoard.SelectedPiece = selection;

            _motion.Moves.Add(selection);
            _validPoints = _validator.FindValidPoints(_motion);
            currentBoard.AvailableMovesPieces = new ObservableCollection<Point>(_validPoints);
            
            // конец хода
            if(_validPoints.Count == 0)
            {
                currentBoard.SelectedPiece = null;
                currentBoard.UpdateBoard(initialBoard);
                initialBoard = null;
                BoardcastMotion(_motion);
                //_active = false;
                
            }
        }

        internal override void OnBoardChangeCancel()
        {
            //if (!_active)
            //    return;
            _motion.Moves.Clear();
            currentBoard.SelectedPiece = null;
            currentBoard.UpdateBoard(initialBoard);
            _validPoints = _validator.FindValidPoints(new Motion());
            currentBoard.AvailableMovesPieces = new ObservableCollection<Point>(_validPoints);
        }

       
    }
}
