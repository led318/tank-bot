using System.Collections.Generic;
using System.Linq;
using API.Components;

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

        public static bool IsSameTargetMultipleRounds(Point point)
        {
            var targetsToScan = 10;
            var threshold = 8;

            var lastXTargets = _targets.TakeLast(targetsToScan).ToList();

            return lastXTargets.Count(x => x == point) >= threshold;
        }
    }
}
