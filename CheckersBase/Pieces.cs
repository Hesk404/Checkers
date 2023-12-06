using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersBase
{
    [Flags]
    public enum PieceTypes
    {
        None = 0,
        WhitePawn = 1,
        BlackPawn = 2,
        Pawn = 3,
        WhiteKing = 4,
        BlackKing = 8,
        White = 5,
        Black = 10,
        King = 12
    }

    public interface IPiece : ICloneable
    {
        PieceTypes Type { get; }
    }

    public class BlankPiece : IPiece
    {
        public PieceTypes Type
        {
            get { return PieceTypes.None; }
        }

        public object Clone()
        {
            return new BlankPiece();
        }
    }

    public class BlackPawn : IPiece
    {
        public PieceTypes Type
        {
            get { return PieceTypes.BlackPawn; }
        }

        public object Clone()
        {
            return new BlackPawn();
        }
    }

    public class BlackKing : IPiece
    {
        public PieceTypes Type
        {
            get { return PieceTypes.BlackKing; }
        }
        public object Clone()
        {
            return new BlackKing();
        }
    }

    public class WhitePawn : IPiece
    {
        public PieceTypes Type
        {
            get { return PieceTypes.WhitePawn; }
        }

        public object Clone()
        {
            return new WhitePawn();
        }
    }

    public class WhiteKing : IPiece
    {
        public PieceTypes Type
        {
            get { return PieceTypes.WhiteKing; }
        }

        public object Clone()
        {
            return new WhiteKing();
        }
    }
}
