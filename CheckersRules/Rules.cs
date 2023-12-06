using CheckersBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersRules
{
    // Играем по правилам русских шашек: 
    // http://www.gambler.ru/%CF%F0%E0%E2%E8%EB%E0_%F0%F3%F1%F1%EA%E8%F5_%F8%E0%F8%E5%EA

    /// <summary>
    ///  This class performs an very important function. Very.
    /// </summary>
    public static class Rules
    {
        private static int[,] _moveDirections = new int[,] { { 1, 1 }, { -1, 1 }, { -1, -1 }, { 1, -1 } };

        #region Pawn
        static class Pawn
        {
            private static int[] dX = new int[2] { 1, -1 };
            internal static void FindMotions(Board board, bool isWhite, int x, int y, 
                ref  List<List<Point>> moves, 
                ref  BeatNode kills)
            {
                FindMoves(board, isWhite, x, y, ref moves);
                FindKills(board, isWhite, x, y, ref kills);
            }

            internal static void FindMoves(Board board, bool isWhite, int x, int y,
                ref  List<List<Point>> moves)
            {
                int dY = isWhite ? -1 : 1;
                int yN = y + dY;

                for (int i = 0; i < dX.Length; i++)
                {
                    int xN = x + dX[i];

                    if (CanMove(board, xN, yN))
                    {
                        moves.Add(new List<Point>() { new Point(x, y), new Point(xN, yN) });
                    }
                }
            }

            internal static bool CanMove(Board board, int xN, int yN)
            {
                if (!InBounds(xN) || !InBounds(yN))
                    return false;

                return board[xN, yN].Type == PieceTypes.None;
            }


            private static void FindKills(Board board, bool isWhite, int x, int y, ref BeatNode kills)
            {
                for(int i = 0; i < 4; i++)
                {
                    int xN = x + _moveDirections[i,0];
                    int yN = y + _moveDirections[i, 1];
                    int xN2 = x + 2 * _moveDirections[i, 0];
                    int yN2 = y + 2 * _moveDirections[i, 1];

                    if (!InBounds(xN) || !InBounds(yN) || !InBounds(xN2) || !InBounds(yN2))
                        continue;

                    if (board[xN2, yN2].Type != PieceTypes.None)
                        continue;

                    if( CheckersHasDifferentColor(board, x, y, xN, yN))
                    {
                        var beated = new Point(xN,yN);

                        if(kills.BranchContaintsValue(beated))
                            continue;

                        if(kills.Move == null)
                        {
                            kills.Move = new Point(x, y);
                        }
                        
                        var beat = new BeatNode
                        {
                            Move = new Point(xN2,yN2),
                            Beated = beated
                        };
                                                
                        kills.AddChild(ref beat);

                        var boardCopy = (Board) board.Clone();
                        
                        boardCopy[xN2, yN2] = boardCopy[x, y];
                        boardCopy[x, y] = new BlankPiece();

                        if (ShouldBecomeKing(yN2, isWhite))
                            King.FindKills(boardCopy, isWhite, xN2, yN2, ref beat);
                        else
                            FindKills(boardCopy, isWhite, xN2, yN2, ref beat);
                    }
                }
            }

        }

        #endregion

        #region King
        static class King
        {
            internal static void FindMotions(Board board, bool isWhite, int i, int j, ref List<List<Point>> moves, ref BeatNode kill)
            {
                FindMoves(board, i,j, ref moves);
                FindKills(board, isWhite, i, j, ref kill);
                kill.FilterForKingKills();
            }


            internal static void FindMoves(Board board, int x, int y, ref List<List<Point>> moves)
            {
                for(int i = 0; i < 4; i++)
                {
                    int xN = x + _moveDirections[i, 0];
                    int yN = y + _moveDirections[i, 1];

                    while(InBounds(xN) && InBounds(yN) && board.IsEmpty(xN,yN))
                    {
                        moves.Add(new List<Point>() { new Point(x, y), new Point(xN, yN) });

                        xN += _moveDirections[i, 0];
                        yN += _moveDirections[i, 1];
                    }
                }
            }

            internal static void FindKills(Board board, bool isWhite, int x, int y, ref BeatNode kills)
            {
                for (int i = 0; i < 4; i++)
                {
                    int dX = _moveDirections[i, 0];
                    int dY = _moveDirections[i, 1];

                    int xN = x + dX;
                    int yN = y + dY;

                    //пропускаем все пустые клетки
                    while (InBounds(xN) && InBounds(yN) && board.IsEmpty(xN, yN))
                    {
                        xN += dX;
                        yN += dY;
                    }

                    if (!InBounds(xN) || !InBounds(yN) || CheckersHasSameColor(board,x,y,xN,yN))
                        continue;

                    //теперь [xN,yN] - шашка противника

                    int xN2 = xN + dX;
                    int yN2 = yN + dY;

                    while (InBounds(xN2) && InBounds(yN2) && board.IsEmpty(xN2, yN2))
                    {
                        var beated = new Point(xN, yN);

                        if (kills.BranchContaintsValue(beated))
                        {
                            xN2 += dX;
                            yN2 += dY;
                            continue;
                        }
                            
                        if (kills.Move == null)
                        {
                            kills.Move = new Point(x, y);
                        }

                        var beat = new BeatNode
                        {
                            Move = new Point(xN2, yN2),
                            Beated = beated
                        };

                        kills.AddChild(ref beat);

                        var boardCopy = (Board)board.Clone();

                        boardCopy[xN2, yN2] = boardCopy[x, y];
                        boardCopy[x, y] = new BlankPiece();

                        King.FindKills(boardCopy, isWhite, xN2, yN2, ref beat);

                        xN2 += dX;
                        yN2 += dY;
                    }

                }
            }


        }

        #endregion
        /// <summary>
        /// Найти доступные ходы для данной ситуации
        /// </summary>
        /// <param name="board">Доска</param>
        /// <param name="isWhite">Играем белыми?</param>
        /// <returns>MotionValidator для данной ситуации</returns>
        public static MotionValidator FindValidMotions(Board board, bool isWhite)
        {
            List<List<Point>> moves = new List<List<Point>>();
            List<BeatNode> kills = new List<BeatNode>();
            

            Func<int, int, bool> colorValidator;

            if (isWhite)
                colorValidator = board.IsWhite;
            else
                colorValidator = board.IsBlack;

            for (int i = 0; i < Board.BOARD_SIZE; i++)
                for (int j = 0; j < Board.BOARD_SIZE; j++ )
                {
                    if (colorValidator(i, j))
                    {
                        var kill = new BeatNode();

                        if (board.IsKing(i, j))
                            King.FindMotions(board, isWhite, i, j, ref moves, ref kill);
                        else
                            Pawn.FindMotions(board, isWhite, i, j, ref moves, ref kill);
                        
                        if (kill.Move != null) // значит кого-то убили
                            kills.Add(kill);
                    }
                }

            var resultKills = new List<List<Point>>();

            kills.ForEach(pk => 
            {
                resultKills.AddRange(pk.SplitToBranches()); 
                pk.Dispose();
            });
           
            return new MotionValidator(resultKills, moves);
        }

        public static Tuple<int,int> GetMotionsCount(Board board)
        {
            int whiteMotions = FindValidMotions(board, true).GetOnlyMotionsCount();
            int blackMotions = FindValidMotions(board, false).GetOnlyMotionsCount();

            return new Tuple<int,int>(whiteMotions,blackMotions);
        }

        public static EndGameEnum CheckGameOver(Board board)
        {
            int blackPawns = 0, blackKings = 0, whitePawns = 0, whiteKings = 0;
            
            for (int i = 0; i < Board.BOARD_SIZE; i++)
            {
                for (int j = 0; j < Board.BOARD_SIZE; j++)
                {
                    switch(board[i,j].Type)
                    {
                        case PieceTypes.BlackPawn:
                            blackPawns++;
                            break;
                        case PieceTypes.BlackKing:
                            blackKings++;
                            break;
                        case PieceTypes.WhitePawn:
                            whitePawns++;
                            break;
                        case PieceTypes.WhiteKing:
                            whiteKings++;
                            break;
                    }
                }
            }

            int totalBlacks = blackKings + blackPawns;
            int totalWhites = whiteKings + whitePawns;

            if (totalBlacks == 0 && totalWhites > 0) return EndGameEnum.EG_WIN_WHITE;
            if (totalBlacks > 0 && totalWhites == 0) return EndGameEnum.EG_WIN_BLACK;

            if (blackPawns == 0 && whitePawns == 0 && blackKings == 1 && whiteKings == 1)
                return EndGameEnum.EG_DRAW;

            return EndGameEnum.EG_NONE;
        }


        public static Board ApplyMotion(Board board, Motion mtn, bool IsWhite)
        {
            if (mtn.IsEmpty()) return board;

            bool beated;
            var newBoard = (Board)board.Clone();
            ApplyMotion(ref newBoard, mtn, IsWhite, out beated);

            return newBoard;
        }


        public static Board ApplyMotion(Board board, Motion mtn, bool IsWhite, out bool beated)
        {
            beated = false;

            if (mtn.IsEmpty()) return board;

            var newBoard = (Board)board.Clone();
            ApplyMotion(ref newBoard, mtn, IsWhite,out beated);

            return newBoard;
        }

        private static void ApplyMotion(ref Board newBoard, Motion mtn, bool IsWhite, out bool beated)
        {
            beated = false;

            var steps = mtn.Moves.ToArray();
            var last = steps.GetUpperBound(0);

            int oldX, oldY, newX, newY;

            oldX = steps[0].X;
            oldY = steps[0].Y;

            newX = steps[last].X;
            newY = steps[last].Y;

            var movedChecker = newBoard[oldX, oldY];
            newBoard[oldX, oldY] = new BlankPiece();
            newBoard[newX, newY] = movedChecker;

            for (int i = 1; i <= steps.GetUpperBound(0); i++)
            {
                if (ShouldBecomeKing(steps[i].Y, IsWhite))
                {
                    if (IsWhite)
                        newBoard[newX, newY] = new WhiteKing();
                    else
                        newBoard[newX, newY] = new BlackKing();
                }
            }

            for (int i = 0; i < last; i++)
            {
                int dX = steps[i].X > steps[i + 1].X ? -1 : 1;
                int dY = steps[i].Y > steps[i + 1].Y ? -1 : 1;

                int dist = Math.Abs(steps[i].X - steps[i + 1].X);

                for (int j = 1; j < dist; j++)
                {
                    int currX = steps[i].X + dX * j;
                    int currY = steps[i].Y + dY * j;

                    if (!newBoard.IsEmpty(currX, currY))
                    {
                        newBoard[currX, currY] = new BlankPiece();
                        beated = true;
                    }
                }
            }

        }
       
        private static bool InBounds(int val)
        {
            return val >= 0 && val < Board.BOARD_SIZE;
        }

        private static bool ShouldBecomeKing(int y, bool isWhite)
        {
            return y == 7 && !isWhite || y == 0 && isWhite;
        }

        private static bool CheckersHasSameColor(Board board, int x, int y, int xN, int yN)
        {
            return (board.IsWhite(x, y) && board.IsWhite(xN, yN) ||
                (board.IsBlack(x, y) && board.IsBlack(xN, yN)));
        }

        private static bool CheckersHasDifferentColor(Board board, int x, int y, int xN, int yN)
        {
            return (board.IsWhite(x,y) && board.IsBlack(xN, yN) || 
                (board.IsBlack(x,y) && board.IsWhite(xN, yN)));
        }

    }
}
