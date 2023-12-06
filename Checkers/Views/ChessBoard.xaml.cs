using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using Checkers.Commands;
using Checkers.Models;
using Checkers.ViewModels;
using CheckersBase;

namespace Checkers
{
    public partial class ChessBoard : UserControl
    {
        public ChessBoard()
        {
            InitializeComponent();

            

            for (int i = 0; i < 64; i++)
            {
                Grid cell = new Grid();
                grdChessBoard.Children.Add(cell);
            }

            SetDefaultColors();

            this.DataContextChanged += OnDataContextChanged;
            
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var boardVM = DataContext as BoardViewModel;
            Debug.WriteLine("DataContextChanged " + ((boardVM == null)? "null" : boardVM.ToString()));
            if (boardVM == null) return;
            boardVM.PropertyChanged += (o, args) => UpdateCells();

            this.MouseRightButtonDown += (o, args) => boardVM.OnBoardChangeCancel();
            UpdateCells();
            
        }

        private void SetDefaultColors()
        {
            SolidColorBrush defaultBrush = new SolidColorBrush(Color.FromRgb(240, 208, 133));
            SolidColorBrush alternateBrush = new SolidColorBrush(Color.FromRgb(17, 17, 17));


            for (int i = 0; i < 64; i++)
            {
                var cell = grdChessBoard.Children[i] as Grid;
                if ((i + i / 8) % 2 == 0)
                {
                    cell.Background = defaultBrush;
                }
                else
                {
                    cell.Background = alternateBrush;
                }
            }
        }
        private void UpdateCells()
        {
            Debug.WriteLine("Обновляем UI доски...");
            var borders = grdChessBoard.Children;
            var boardVM = DataContext as BoardViewModel;
            SetDefaultColors();

            for (int i = 0; i < Board.BOARD_SIZE; i++)
            {
                for (int j = 0; j < Board.BOARD_SIZE; j++)
                {
                    var grid = borders[i * 8 + j] as Grid;
                    if (boardVM.AvailableMovesPieces.Any(p => p.X == j && p.Y == i))
                        grid.Background = new SolidColorBrush(Colors.Chocolate);
                    // транспонирование необходимо, чтобы проще интерпретировать доску с шашками в логике
                    if (boardVM.SelectedPiece != null && boardVM.SelectedPiece.X == j && boardVM.SelectedPiece.Y == i)
                    {
                        grid.Background = new SolidColorBrush(Colors.GreenYellow);
                    }
                    var cp = new ContentPresenter()
                    {
                        Content = boardVM[j, i]
                    };
                    MouseBehavior.SetMouseDownCommand(grid, boardVM.MouseDownCommand, new CheckersBase.Point(j,i));
                    grid.Children.Clear();
                    grid.Children.Add(cp);

                }
            }
        }
    }
}
