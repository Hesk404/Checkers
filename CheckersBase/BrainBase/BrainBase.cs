using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersBase.BrainBase
{
    /// <summary>
    ///  Базовый класс мозга для игрока-компьютера
    /// </summary>
    public abstract class BrainBase
    {
        public abstract Motion FindMotion(Board Board, bool isWhite);
    }
}
