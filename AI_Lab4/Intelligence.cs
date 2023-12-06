using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheckersBase;
using CheckersBase.BrainBase;
using CheckersRules;
using Matrix;

namespace AI_Lab4
{
    public class Intelligence : Matrix.CheckersIntelligence
    {
        public Intelligence(int koeffKings, int koeffPawns, int koeffMoves, int maxDeep) : base (koeffKings, koeffPawns, koeffMoves, maxDeep)
        {
        }

        protected override int Eval(Board board)
        {
            int pawsNearDam = 0;
            if (_isWhiteTurn)
            {
                for (int i = 0; i < Board.BOARD_SIZE; i++)
                    if (board[i, 2].Type == PieceTypes.WhitePawn)
                        pawsNearDam++;
            }
            else
            {
                for (int i = 0; i < Board.BOARD_SIZE; i++)
                    if (board[i, 5].Type == PieceTypes.BlackPawn)
                        pawsNearDam++;
            }

            int whiteSafePawns = 0, blackSafePawns = 0;
            int whiteSafeKings = 0, blackSafeKings = 0;
            int whiteCentralPawns = 0, blackCentralPawns = 0;



            CheckersInfo info = new CheckersInfo();

            for (int i = 0; i < Board.BOARD_SIZE; i++)
            {
                for (int j = 0; j < Board.BOARD_SIZE; j++)
                {
                    switch (board[i, j].Type)
                    {
                        case PieceTypes.BlackPawn:
                            info.Black.Pawns++;
                            break;
                        case PieceTypes.BlackKing:
                            info.Black.Kings++;
                            break;
                        case PieceTypes.WhitePawn:
                            info.White.Pawns++;
                            break;
                        case PieceTypes.WhiteKing:
                            info.White.Kings++;
                            break;
                    }

                    if (i == 0 || j == 0 || i == Board.BOARD_SIZE - 1 || j == Board.BOARD_SIZE - 1)
                    {
                        switch (board[i, j].Type)
                        {
                            case PieceTypes.BlackPawn:
                                blackSafePawns++;
                                break;
                            case PieceTypes.BlackKing:
                                blackSafeKings++;
                                break;
                            case PieceTypes.WhitePawn:
                                whiteSafePawns++;
                                break;
                            case PieceTypes.WhiteKing:
                                whiteSafeKings++;
                                break;
                        }
                    }

                    if (j == 3 || j == 4)
                    {
                        switch (board[i, j].Type)
                        {
                            case PieceTypes.BlackPawn:
                                blackCentralPawns++;
                                break;
                            case PieceTypes.WhitePawn:
                                whiteCentralPawns++;
                                break;
                        }
                    }
                }
            }

            var moves = Rules.GetMotionsCount(board);
            int wC = moves.Item1;
            int bC = moves.Item2;

            int f = 0;

            PlayersCheckers a, b;

            a = _isWhiteTurn ? info.White : info.Black;
            b = _isWhiteTurn ? info.Black : info.White;

            int aCount = _isWhiteTurn ? wC : bC;
            int bCount = _isWhiteTurn ? bC : wC;

            int advKings = a.Kings - b.Kings;
            int advPawns = a.Pawns - b.Pawns;
            int advMoves = aCount - bCount;


            int advSafePawns = _isWhiteTurn ? whiteSafePawns - blackSafePawns : blackSafePawns - whiteSafePawns;
            int advSafeKings = _isWhiteTurn ? whiteSafeKings - blackSafeKings : blackSafeKings - whiteSafeKings;
            int advCentralPawns = _isWhiteTurn ? whiteCentralPawns - blackCentralPawns : blackCentralPawns - whiteCentralPawns;
            int safePawnsKoef = 8;
            int safeKingKoef = 10;
            int centralPawnsKoef = 4;
            int nearDamKoef = 10;

            f = _koeffKings * advKings + _koeffPawns * advPawns + _koeffMoves * advMoves
                + advSafePawns * safePawnsKoef + advSafeKings * safeKingKoef + advCentralPawns * centralPawnsKoef
                + pawsNearDam * nearDamKoef;

            return f;
        }
    }

    [BrainInfo(BrainName = "Агент Шашка", Student = "Матрица", StudentGroup = "/dev/null")]
    public class AgentChecker : BrainBase
    {
        Intelligence intelligence = new Intelligence(6, 3, 1, 5);
        public override Motion FindMotion(Board Board, bool isWhite)
        {
            return intelligence.FindMotion(Board, isWhite);
        }
    }
}
