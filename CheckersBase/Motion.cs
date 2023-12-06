using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersBase
{
    /// <summary>
    ///  Позиция на доске
    /// </summary>
    public class Point
    {
        public Point()
            : this(0, 0)
        { }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Point(int index)
        {
            X = index/Board.BOARD_SIZE;
            Y = index%Board.BOARD_SIZE;
        }
        /// <summary>
        /// Строка
        /// </summary>
        public int X { get; set; }
        /// <summary>
        /// Столбец
        /// </summary>
        public int Y { get; set; }

        public override bool Equals(object obj)
        {
            var other = obj as Point;

            if (other == null)
                return false;

            return this.X == other.X && this.Y == other.Y;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    /// <summary>
    ///  Список шагов игрока в рамках хода
    /// </summary>
    public class Motion
    {
        public List<Point> Moves { get; set; }

        public Motion() 
        {
            Moves = new List<Point>();
        }

        public Motion(params Point[] points)
        {
            Moves = new List<Point>(points);
        }

        public bool IsEmpty()
        {
            return Moves.Count == 0;
        }
       
    }
}
