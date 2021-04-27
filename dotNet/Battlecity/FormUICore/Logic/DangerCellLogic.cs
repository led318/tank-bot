using System.Linq;
using FormUI.FieldItems;
using FormUI.FieldObjects;
using FormUI.Infrastructure;
using FormUI.Predictions;
using FormUICore.FieldItems;
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
                var bullet = (Bullet) bulletPrediction.Item;
                if (bullet.IsMyBullet)
                    continue;

                MarkCellAsDangerous(cell, true);
            }

            var aiShotPredictions = cell.Predictions.AiShotPredictions.Where(x => x.Depth == 1).ToList();
            foreach (var aiShotPrediction in aiShotPredictions)
            {
                MarkCellAsDangerous(cell, true);
            }

            var enemyShotPredictions = cell.Predictions.EnemyShotPredictions.Where(x => x.Depth == 1).ToList();
            foreach (var enemyShotPrediction in enemyShotPredictions)
            {
                MarkCellAsDangerous(cell);
            }

            var enemy = State.ThisRound.EnemyTanks.FirstOrDefault(x => x.Point == cell.Point);
            if (enemy != null)
            {
                MarkCellAsDangerous(cell);
            }

            var aiTank = State.ThisRound.AiTanks.FirstOrDefault(x => x.Point == cell.Point);
            if (aiTank != null)
            {
                if (aiTank.IsShotThisRound)
                {
                    MarkCellAsDangerous(cell, true);
                }
            }
        }

        private static void MarkCellAsDangerous(Cell cell, bool isCritical = false)
        {
            var prediction = (DangerCellPrediction)cell.Predictions.Add(PredictionType.DangerCell, 1, cell.Point);
            prediction.IsCritical = isCritical;
        }
    }
}
