using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersRules
{
    public enum EndGameEnum
    {
        [Description("")]
        EG_NONE,
        [Description("Время на ход вышло")]
        EG_TIMEOUT,
        [Description("Ход не соответствует правилам")]
        EG_INVALID_MOTION,
        [Description("Ничья")]
        EG_DRAW,
        [Description("Черные победили")]
        EG_WIN_BLACK,
        [Description("Белые победили")]
        EG_WIN_WHITE,
    }
}
