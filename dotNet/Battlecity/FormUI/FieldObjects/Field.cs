using System;
using System.Collections.Generic;
using System.Linq;
using API.Components;
using FormUI.FieldItems;
using FormUI.Infrastructure;
using FormUI.Predictions;

namespace FormUI.FieldObjects
{
    public static class Field
    {
        public static Cell[,] Cells = new Cell[Constants.FieldWidth, Constants.FieldHeight];

        public static IEnumerable<Cell> AllCells => Flatten(Cells);

        public static IEnumerable<BaseItem> AllItems => AllCells.Select(x => x.Item).ToList();

        public static IDictionary<int, List<MyMovePrediction>> AllMyMoveDepthPredictions { get; set; } =
            new Dictionary<int, List<MyMovePrediction>>();
        

        static Field()
        {
            for (int i = 0; i < Constants.FieldWidth; i++)
            {
                for (int j = 0; j < Constants.FieldHeight; j++)
                {
                    Cells[i, j] = new Cell(i, j);
                }
            }
        }

        public static Cell GetCell(Point point)
        {
            return Cells[point.X, point.Y];
        }

        public static bool IsOutOfField(Point point)
        {
            return point.X < 0 || point.Y < 0 || point.X >= Constants.FieldWidth || point.Y >= Constants.FieldHeight;
        }

        public static void Reset()
        {
            foreach (var cell in AllCells)
            {
                cell.Reset();
            }

            foreach (var depthPredictions in AllMyMoveDepthPredictions)
            {
                depthPredictions.Value.Clear();
            }
        }

        private static IEnumerable<T> Flatten<T>(T[,] map)
        {
            for (var row = 0; row < map.GetLength(0); row++)
            {
                for (var col = 0; col < map.GetLength(1); col++)
                {
                    yield return map[row, col];
                }
            }
        }

        public static void MarkCellDirty(Point point)
        {
            GetCell(point).IsDirty = true;
        }

        public static List<BasePrediction> GetPredictions(Func<PredictionAggregate, IEnumerable<BasePrediction>> func)
        {
            return AllCells.SelectMany(x => func(x.Predictions)).ToList();
        }

        public static int GetPredictionsCount(Func<PredictionAggregate, IEnumerable<BasePrediction>> func)
        {
            return AllCells.Sum(x => func(x.Predictions).Count());
        }

        public static int GetMyMovePredictionsCount()
        {
            return GetPredictionsCount(x => x.MyMovePredictions);
        }

        public static int GetMyShotPredictionsCount()
        {
            return GetPredictionsCount(x => x.MyShotPredictions);
        }

        public static void AddMyMoveDepthPredictions(int depth, MyMovePrediction prediction)
        {
            if (!AllMyMoveDepthPredictions.ContainsKey(depth))
            {
                AllMyMoveDepthPredictions[depth] = new List<MyMovePrediction>();
            }

            AllMyMoveDepthPredictions[depth].Add(prediction);
        }

        public static List<MyMovePrediction> GetMyMoveDepthPredictions(int depth)
        {
            if (!AllMyMoveDepthPredictions.ContainsKey(depth))
                return new List<MyMovePrediction>();

            var depthPredictions = AllMyMoveDepthPredictions[depth];
            if (!depthPredictions.Any())
                return new List<MyMovePrediction>();

            return depthPredictions;
        }
    }
}
