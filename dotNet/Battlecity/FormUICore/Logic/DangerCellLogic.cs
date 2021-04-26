﻿using System.Linq;
using FormUI.FieldObjects;
using FormUICore.FieldObjects;
using FormUICore.Predictions;

namespace FormUICore.Logic
{
    public static class DangerCellLogic
    {
        public static void CalculateDangerCells()
        {
            var allNotWallCells = Field.AllCells.Where(x => !x.IsWall).ToList();

            foreach (var cell in allNotWallCells)
            {
                CalculateDangerCell(cell);
            }
        }

        private static void CalculateDangerCell(Cell cell)
        {
            var bulletPredictions = cell.Predictions.BulletPredictions.Where(x => x.Depth == 0 || x.Depth == 1).ToList();
            foreach (var bulletPrediction in bulletPredictions)
            {
                cell.Predictions.Add(PredictionType.DangerCell, 1, cell.Point);
            }

            var aiShotPredictions = cell.Predictions.AiShotPredictions.Where(x => x.Depth == 1).ToList();
            foreach (var aiShotPrediction in aiShotPredictions)
            {
                cell.Predictions.Add(PredictionType.DangerCell, 1, cell.Point);
            }

            var enemyShotPredictions = cell.Predictions.EnemyShotPredictions.Where(x => x.Depth == 1).ToList();
            foreach (var enemyShotPrediction in enemyShotPredictions)
            {
                cell.Predictions.Add(PredictionType.DangerCell, 1, cell.Point);
            }
        }
    }
}