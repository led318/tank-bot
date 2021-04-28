using System.Linq;
using FormUI.FieldItems;
using FormUI.FieldObjects;
using FormUI.Infrastructure;
using FormUI.Predictions;
using FormUICore.FieldItems;
using FormUICore.FieldObjects;
using FormUICore.Infrastructure;
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
            for (var depth = 1; depth <= AppSettings.DangerCellPredictionDepth; depth++)
            {
                CalculateDangerCellForDepth(cell, depth);
            }
        }

        private static void CalculateDangerCellForDepth(Cell cell, int depth)
        {
            var bulletPredictions = cell.Predictions.BulletPredictions.Where(x => x.Depth == depth - 1 || x.Depth == depth).ToList();
            foreach (var bulletPrediction in bulletPredictions)
            {
                var bullet = (Bullet) bulletPrediction.Item;
                if (bullet.IsMyBullet)
                    continue;

                MarkCellAsDangerous(cell, depth, true);
            }

            var aiShotPredictions = cell.Predictions.AiShotPredictions.Where(x => x.Depth == depth).ToList();
            foreach (var aiShotPrediction in aiShotPredictions)
            {
                MarkCellAsDangerous(cell, depth, true);
            }

            var enemyShotPredictions = cell.Predictions.EnemyShotPredictions.Where(x => x.Depth == depth).ToList();
            foreach (var enemyShotPrediction in enemyShotPredictions)
            {
                MarkCellAsDangerous(cell, depth);
            }

            if (depth == 1)
            {
                var enemy = State.ThisRound.EnemyTanks.FirstOrDefault(x => x.Point == cell.Point);
                if (enemy != null && !enemy.IsStuck && enemy.IsShotThisRound)
                {
                    MarkCellAsDangerous(cell, depth);
                }

                var aiTank = State.ThisRound.AiTanks.FirstOrDefault(x => x.Point == cell.Point);
                if (aiTank != null)
                {
                    if (aiTank.IsShotThisRound)
                    {
                        MarkCellAsDangerous(cell, depth, true);
                    }
                }
            }
        }

        private static void MarkCellAsDangerous(Cell cell, int depth, bool isCritical = false)
        {
            var prediction = (DangerCellPrediction)cell.Predictions.Add(PredictionType.DangerCell, depth, cell.Point);
            prediction.IsCritical = isCritical;
        }
    }
}
