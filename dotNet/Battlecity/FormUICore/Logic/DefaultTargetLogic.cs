using System;
using System.Collections.Generic;
using System.Linq;
using API.Components;
using FormUI.FieldObjects;
using FormUI.Predictions;
using FormUICore.FieldObjects;
using FormUICore.Infrastructure;

// ReSharper disable InconsistentNaming
namespace FormUICore.Logic
{
    public static class DefaultTargetLogic
    {
        private static readonly int _chunksPerLine = 3;
        private static readonly Random _random = new Random();

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
            var myTankIsOnDefaultPoint = State.ThisRound.MyTank.Point == _currentDefaultTargetPoint;
            var prevStepIsToDefaultPoint = State.PrevRound is { CurrentMoveIsToDefaultTarget: true };

            if (myTankIsOnDefaultPoint || !prevStepIsToDefaultPoint)
                InitNewDefaultTargetPoint();
        }

        private static void InitNewDefaultTargetPoint()
        {
            if (State.ThisRound.MyTank == null)
                return;

            var aiTanks = State.ThisRound.AiTanks;
            var chunkSize = State.ThisRound.Board.Size / _chunksPerLine;
            var chunks = new List<Chunk>();

            for (var i = 0; i < _chunksPerLine; i++)
            {
                for (var j = 0; j < _chunksPerLine; j++)
                {
                    var start = new Point(i * chunkSize, j * chunkSize);
                    var end = new Point(((i + 1) * chunkSize) - 1, ((j + 1) * chunkSize) - 1);

                    var chunk = new Chunk
                    {
                        X = i,
                        Y = j,
                        Start = start,
                        End = end,
                        AiTanks = aiTanks.Where(x => x.Point.IsInArea(start, end)).ToList()
                    };
                    chunks.Add(chunk);
                }
            }

            var maxChunkPopulation = chunks.Max(x => x.AiTanks.Count);
            var mostPopulatedChunks = chunks.Where(x => x.AiTanks.Count == maxChunkPopulation).ToList();

            //var targetTanks = mostPopulatedChunks.SelectMany(x => x.AiTanks).ToList();

            //var targetTanksMyMovePredictions = new List<BasePrediction>();
            //foreach (var targetTank in targetTanks)
            //{
            //    var cell = Field.GetCell(targetTank.Point);

            //    var nearestMyMovePrediction = cell.Predictions.MyMovePredictions.OrderBy(x => x.Depth).FirstOrDefault();
            //    if (nearestMyMovePrediction != null)
            //        targetTanksMyMovePredictions.Add(nearestMyMovePrediction);
            //}

            //var nearestAiTankMyMovePrediction = targetTanksMyMovePredictions.OrderBy(x => x.Depth).FirstOrDefault();
            //if (nearestAiTankMyMovePrediction != null)
            //{
            //    _currentDefaultTargetPoint = nearestAiTankMyMovePrediction.Point;
            //    return;
            //}

            //var mostPopulatedChunks
            if (InitRandomEmptyCell(mostPopulatedChunks))
                return;

            var allMapChunk = new Chunk
            {
                Start = new Point(1, 1),
                End = new Point(State.ThisRound.Board.Size - 2, State.ThisRound.Board.Size - 2)
            };

            InitRandomEmptyCell(new[] {allMapChunk});
        }

        private static bool InitRandomEmptyCell(IEnumerable<Chunk> mostPopulatedChunks)
        {
            var myTank = State.ThisRound.MyTank;
            var emptyItems = State.ThisRound.EmptyItems
                .Where(x => mostPopulatedChunks.Any(c => x.Point.IsInArea(c.Start, c.End)))
                .ToList();

            var notNearEmptyItems = emptyItems.Where(x => x.Point.DistantionTo(myTank.Point) > 10).ToList();
            var tries = 0;

            while (tries < 100)
            {
                var randomEmptyItem = notNearEmptyItems[_random.Next(notNearEmptyItems.Count - 1)];
                var emptyCell = Field.GetCell(randomEmptyItem.Point);

                var isDangerous = emptyCell.Predictions.DangerCellPredictions.Any(x => x.Depth != 1);
                if (!isDangerous)
                {
                    var isReachable = emptyCell.Predictions.MyMovePredictions.Any();
                    if (isReachable)
                    {
                        _currentDefaultTargetPoint = emptyCell.Point;
                        return true;
                    }
                }

                tries++;
            }

            return false;
        }
    }
}
