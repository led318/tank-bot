using System.Collections.Generic;
using System.Linq;
using API.Components;
using FormUI.FieldObjects;
using FormUI.Infrastructure;
using FormUI.Predictions;
using FormUICore.Infrastructure;

// ReSharper disable InconsistentNaming

namespace FormUICore.Logic
{
    public static class DefaultTargetLogic
    {
        private static int _currentDefaultTargetPointIndex;
        private static readonly List<Point> _defaultTargetPoints = new List<Point>();
        private static Point _currentDefaultTargetPoint => _defaultTargetPoints[_currentDefaultTargetPointIndex];

        static DefaultTargetLogic()
        {
            if (AppSettings.IsOldMap)
            {
                _defaultTargetPoints.Add(new Point(3, 31));
                _defaultTargetPoints.Add(new Point(31, 31));
            }
            else
            {
                _defaultTargetPoints.Add(new Point(2, 9));
                _defaultTargetPoints.Add(new Point(24, 9));
            }
        }

        public static List<MyMovePrediction> GetDefaultTargetMovePredictions()
        {
            CheckAndChangeDefaultTargetPoint();
            var defaultCell = Field.GetCell(_currentDefaultTargetPoint);
            var defaultCellNearestMovePredictions = defaultCell.Predictions.MyMovePredictions.ToList();

            return defaultCellNearestMovePredictions;
        }

        public static bool ProcessDefaultTarget()
        {
            CheckAndChangeDefaultTargetPoint();
            var defaultCell = Field.GetCell(_currentDefaultTargetPoint);
            var defaultCellNearestMovePrediction =
                defaultCell.Predictions.MyMovePredictions.OrderBy(x => x.Depth).FirstOrDefault();

            if (defaultCellNearestMovePrediction == null)
                return false;

            NextCommandCalculationLogic.SetCurrentMove(defaultCellNearestMovePrediction);
            return true;
        }

        private static void CheckAndChangeDefaultTargetPoint()
        {
            if (State.ThisRound.MyTank == null)
                return;

            if (State.ThisRound.MyTank.Point == _currentDefaultTargetPoint)
                _currentDefaultTargetPointIndex = (_currentDefaultTargetPointIndex + 1) % _defaultTargetPoints.Count;
        }
    }
}
