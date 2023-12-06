using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System;

namespace CheckersBase
{
    /// <summary>
    ///  Игровая доска с шашками
    /// </summary>
    public class Board : ICloneable
    {
        public const int BOARD_SIZE = 8;
        private IPiece[,] pieces = new IPiece[BOARD_SIZE, BOARD_SIZE];

        private Board() { }
        public static Board CreateStartingBoard()
        {
            Board ret = new Board();

            for (int x = 0; x < BOARD_SIZE; x++)
                for (int y = 0; y < BOARD_SIZE; y++)
                {
                    int dx = (y + 1) % 2;
                    
                    if((x + dx) % 2 == 0)
                    {
                        if (y < 3)
                            ret.pieces[x, y] = new BlackPawn();
                        else if (y > 4)
                            ret.pieces[x, y] = new WhitePawn();
                        else
                            ret.pieces[x, y] = new BlankPiece();
                    }
                    else
                        ret.pieces[x, y] = new BlankPiece();
                }
            return ret;
        }

        public IPiece this[int x,int y]
        {
            get { return pieces[x, y]; }
            set { pieces[x, y] = value; }
        }

        /// <summary>
        ///  Если шашка черная
        /// </summary>
        public bool IsBlack(int X, int Y)
        {
            return (pieces[X, Y].Type & PieceTypes.Black) != PieceTypes.None; 
        }

        /// <summary>
        ///  Если шашка белая
        /// </summary>
        public bool IsWhite(int X, int Y)
        {
            return (pieces[X, Y].Type & PieceTypes.White) != PieceTypes.None; 
        }

        /// <summary>
        ///  Если здесь нет шашки
        /// </summary>
        public bool IsEmpty(int X, int Y)
        {
            return pieces[X, Y].Type == PieceTypes.None; 
        }

        /// <summary>
        ///  Если это дамка
        /// </summary>
        public bool IsKing(int X, int Y)
        {
            return (pieces[X, Y].Type & PieceTypes.King) != PieceTypes.None;
        }

        public object Clone()
        {
            Board ret = new Board();

            for (int x = 0; x < BOARD_SIZE; x++)
                for (int y = 0; y < BOARD_SIZE; y++)
                {
                    ret[x, y] = (IPiece) this[x, y].Clone();
                }

            return ret;
        }
    }
}
