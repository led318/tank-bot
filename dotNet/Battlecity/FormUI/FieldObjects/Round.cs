using System.Collections.Generic;
using System.Linq;
using API;
using API.Components;
using FormUI.FieldItems;
using FormUI.FieldItems.Tank;
using FormUI.Infrastructure;

namespace FormUI.FieldObjects
{
    public class Round
    {
        public BaseItem[,] Items = new BaseItem[Constants.FieldWidth, Constants.FieldHeight];

        public List<AiTank> AiTanks { get; set; } = new List<AiTank>();
        public List<AiTank> AiPrizeTanks { get; set; } = new List<AiTank>();
        public List<EnemyTank> EnemyTanks { get; set; } = new List<EnemyTank>();
        public MyTank MyTank { get; set; }
        public IEnumerable<BaseTank> AllTanks => GetAllTanks();

        public List<Bullet> Bullets { get; set; } = new List<Bullet>();
        public List<Bullet> MyBullets { get; set; } = new List<Bullet>();


        public Round(Board board)
        {
            Field.Reset();

            for (var i = 0; i < Constants.FieldWidth; i++)
            {
                for (var j = 0; j < Constants.FieldHeight; j++)
                {
                    var point = new Point(i, j);
                    var item = ItemFactory.GetItem(board.GetAt(point), point);

                    PopulateState(item);

                    Items[i, j] = item;

                    Field.Cells[i, j].Items.Clear();
                    Field.Cells[i, j].Items.Add(item);


                }
            }
        }

        public void AddMyBullet(Bullet bullet)
        {
            bullet.IsMyBullet = true;
            MyBullets.Add(bullet);
        }

        public void AddAiPrizeTank(AiTank aiTank, AiTank prevRoundAiPrizeTank)
        {
            aiTank.SetHealth(prevRoundAiPrizeTank.Health);
            aiTank.BorderColor = prevRoundAiPrizeTank.BorderColor;
            AiPrizeTanks.Add(aiTank);
        }

        private void PopulateState(BaseItem item)
        {
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
