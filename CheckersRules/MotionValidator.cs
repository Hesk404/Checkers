using CheckersBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheckersRules
{

    public enum MotionValidationEnum
    {
        VALID,
        INVALID,
        MUST_BEAT
    }

    /// <summary>
    ///  Проверяет правильность ходов для текущей ситуации и предоставляет список позиций для формирования хода игроком-человеком
    /// </summary>
    public class MotionValidator
    {
        List<List<Point>> _kills;
        List<List<Point>> _moves;

        public MotionValidator(List<List<Point>> kills, List<List<Point>> moves)
        {
            _kills = kills;
            _moves = moves;
        }

        public MotionValidationEnum ValidateMotion(Motion mtn)
        {
            bool mustBeat = false;
            bool canMove = false;

            if(_kills.Count > 0)
            {
                if (_kills.Any(k => Enumerable.SequenceEqual(k,mtn.Moves)))
                    return MotionValidationEnum.VALID;
                
                mustBeat = true;
            }

            canMove = _moves.Any(m => Enumerable.SequenceEqual(m, mtn.Moves));
            
            if(canMove)
            {
                if (mustBeat)
                    return MotionValidationEnum.MUST_BEAT;
                else
                    return MotionValidationEnum.VALID;
            }
            
            return MotionValidationEnum.INVALID;
        }

        /// <summary>
        /// Найти доступные следующие ходы
        /// </summary>
        /// <param name="mtn">Первые действия текущего хода</param>
        /// <returns>Варианты продолжения текущего хода</returns>
        public List<Point> FindValidPoints(Motion mtn)
        {
            List<Point> ret = new List<Point>();

            var container = (_kills.Count > 0 ? _kills : _moves);

            if (mtn.IsEmpty())
            {
                ret.AddRange(SlicePointsAt(container, 0));
            }
            else
            {
                var killsOrMoves = container.Where(k => k.StartsFrom(mtn.Moves)).ToList();
                ret.AddRange(SlicePointsAt(killsOrMoves, mtn.Moves.Count));
            }

            return ret;
        }

        /// <summary>
        /// Получаем Н-ные шаги из ходов
        /// </summary>
        /// <param name="container">Список ходов</param>
        /// <param name="index">Глубина среза</param>
        private static List<Point> SlicePointsAt(List<List<Point>> container, int index)
        {
            List<Point> ret = new List<Point>();
                        
            foreach(var list in container)
               ret.Add(list[index]);
            
            return ret;
        }

        public bool NoValidMotions()
        {
            return (_kills.Count == 0 && _moves.Count == 0);
        }

        public List<Motion> GetAllMotions()
        {
            var list = _kills.Count > 0 ? _kills : _moves;

            var ret = list.Select(m => new Motion(m.ToArray())).ToList();

            return ret;
        }

        public int GetOnlyMotionsCount()
        {
            return _kills.Count > 0 ? 0 : _moves.Count;
        }
    }
}
