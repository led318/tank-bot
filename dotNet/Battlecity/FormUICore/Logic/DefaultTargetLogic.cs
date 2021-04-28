using System;
using System.Collections.Generic;
using System.Linq;
using API.Components;
using FormUI.FieldItems.Tank;
using FormUI.FieldObjects;
using FormUI.Predictions;
using FormUICore.Infrastructure;
using FormUICore.Predictions;

// ReSharper disable InconsistentNaming
namespace FormUICore.Logic
{
    public static class DefaultTargetLogic
    {
        private static readonly int _chunksPerLine = 4;

        private static Point _currentDefaultTargetPoint;

        static DefaultTargetLogic()
        {
            InitNewDefaultTargetPoint();
        }

        public static List<MyMovePrediction> GetDefaultTargetMovePredictions()
        {
            RecalculateDefaultTargetPoint();
            var defaultCell = Field.GetCell(_currentDefaultTargetPoint);
            var defaultCellNearestMovePredictions = defaultCell.Predictions.MyMovePredictions.ToList();

            return defaultCellNearestMovePredictions;
        }

        private static void RecalculateDefaultTargetPoint()
        {
            if (State.ThisRound.MyTank == null)
                return;

            InitNewDefaultTargetPoint();
        }

        private static void InitNewDefaultTargetPoint()
        {
            var aiTanks = State.ThisRound.AiTanks;
            var chunkSize = State.ThisRound.Board.Size / _chunksPerLine;
            var chunks = new Dictionary<Tuple<int, int>, List<AiTank>>();

            for (var i = 0; i < _chunksPerLine; i++)
            {
                for (var j = 0; j < _chunksPerLine; j++)
                {
                    var start = new Point(i * chunkSize, j * chunkSize);
                    var end = new Point(((i + 1) * chunkSize) - 1, ((j + 1) * chunkSize) - 1);

                    var chunkAiTanks = aiTanks.Where(x => x.Point.IsInArea(start, end)).ToList();
                    chunks[new Tuple<int, int>(i, j)] = chunkAiTanks;
                }
            }

            var maxChunkPopulation = chunks.Max(x => x.Value.Count);
            var mostPopulatedChunks = chunks.Where(x => x.Value.Count == maxChunkPopulation).ToList();

            var targetTanks = mostPopulatedChunks.SelectMany(x => x.Value).ToList();

            var targetTanksMyMovePredictions = new List<BasePrediction>();
            foreach (var targetTank in targetTanks)
            {
                var cell = Field.GetCell(targetTank.Point);

                var nearestMyMovePrediction = cell.Predictions.MyMovePredictions.OrderBy(x => x.Depth).FirstOrDefault();
                if (nearestMyMovePrediction != null)
                    targetTanksMyMovePredictions.Add(nearestMyMovePrediction);
            }

            var nearestAiTankMyMovePrediction = targetTanksMyMovePredictions.OrderBy(x => x.Depth).FirstOrDefault();
            if (nearestAiTankMyMovePrediction != null)
                _currentDefaultTargetPoint = nearestAiTankMyMovePrediction.Point;
        }
    }
}
