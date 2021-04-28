using System.Collections.Generic;
using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Tank;
using FormUICore.Predictions;

namespace FormUICore.Infrastructure
{
    public static class TargetLog
    {
        private static List<Point> _targets = new List<Point>();

        public static void Add(Point point)
        {
            _targets.Add(point);
        }

        public static void Clear()
        {
            _targets.Clear();
        }

        public static bool IsSameTargetMultipleRounds(BasePrediction prediction)
        {
            var targetsToScan = 10;
            var threshold = 8;

            var lastXTargets = _targets.TakeLast(targetsToScan).ToList();

            if (prediction.Item != null)
            {
                if (prediction.Item is EnemyTank enemyTank)
                {
                    if (enemyTank.IsStuck && !enemyTank.IsShoting)
                    {
                        return false;
                    }
                }
            }

            return lastXTargets.Count(x => x == prediction.Point) >= threshold;
        }
    }
}
