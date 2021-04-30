using System.Collections.Generic;
using API;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Tank;
using FormUI.FieldObjects;
using FormUICore.FieldItems;
using FormUICore.FieldItems.Tank;
using FormUICore.Infrastructure;
using FormUICore.Predictions;

namespace FormUICore.FieldObjects
{
    public class Round
    {
        public Board Board { get; set; }

        public BaseItem[,] Items = new BaseItem[Constants.FieldWidth, Constants.FieldHeight];


        public List<AiTank> AiTanks { get; } = new List<AiTank>();
        public List<AiTank> AiPrizeTanks { get; } = new List<AiTank>();
        public List<EnemyTank> EnemyTanks { get; } = new List<EnemyTank>();
        public MyTank MyTank { get; set; }
        public IEnumerable<BaseTank> AllTanks => GetAllTanks();

        public List<Bullet> Bullets { get; } = new List<Bullet>();

        public List<Empty> EmptyItems { get; } = new List<Empty>();
        public List<River> Rivers { get; } = new List<River>();
        public List<Ice> Ice { get; } = new List<Ice>();
        public List<Tree> Trees { get; } = new List<Tree>();

        public BasePrediction CurrentMoveSelectedPrediction { get; set; }
        public List<Direction> CurrentMoveCommands { get; } = new List<Direction>();
        public string CurrentMoveCommandString => string.Join(",", CurrentMoveCommands);
        public bool CurrentMoveIsToDefaultTarget { get; set; }

        public bool IsInDeadZone { get; set; }

        public bool GameIsRunning { get; set; }

        public Round(Board board)
        {
            Board = board;

            Field.Reset();

            for (var i = 0; i < Constants.FieldWidth; i++)
            {
                for (var j = 0; j < Constants.FieldHeight; j++)
                {
                    var point = new Point(i, j);
                    var item = ItemFactory.GetItem(board.GetAt(point), point);

                    PopulateState(item);

                    Items[i, j] = item;

                    Field.GetCell(point).Items.Clear();
                    Field.GetCell(point).Items.Add(item);


                }
            }
        }

        public void AddAiPrizeTank(AiTank aiTank, AiTank prevRoundAiPrizeTank)
        {
            aiTank.SetHealth(prevRoundAiPrizeTank.Health);
            aiTank.BorderColor = prevRoundAiPrizeTank.BorderColor;
            AiPrizeTanks.Add(aiTank);
        }

        private void PopulateState(BaseItem item)
        {
            if (item is Empty empty)
            {
                EmptyItems.Add(empty);
                return;
            }

            if (item is AiTank aiTank)
            {
                AiTanks.Add(aiTank);

                if (item is AiPrizeTank aiPrizeTank)
                {
                    AiPrizeTanks.Add(aiPrizeTank);
                }

                return;
            }

            if (item is EnemyTank enemyTank)
            {
                EnemyTanks.Add(enemyTank);
                return;
            }

            if (item is MyTank myTank)
            {
                MyTank = myTank;
                return;
            }

            if (item is Bullet bullet)
            {
                Bullets.Add(bullet);
                return;
            }

            if (item is River river)
            {
                Rivers.Add(river);
                return;
            }

            if (item is Tree tree)
            {
                Trees.Add(tree);
                return;
            }

            if (item is Ice ice)
            {
                Ice.Add(ice);
                return;
            }
        }

        private List<BaseTank> GetAllTanks()
        {
            var result = new List<BaseTank>();

            result.AddRange(AiTanks);
            result.AddRange(EnemyTanks);

            if (MyTank != null)
            {
                result.Add(MyTank);
            }

            return result;
        }
    }
}
