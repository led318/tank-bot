using API;
using API.Components;
using FormUI.FieldItems;
using FormUI.Infrastructure;

namespace FormUI.FieldObjects
{
    public class Round
    {
        public BaseItem[,] Items = new BaseItem[Constants.FieldWidth, Constants.FieldHeight];

        public Round(Board board)
        {
            for (var i = 0; i < Constants.FieldWidth; i++)
            {
                for (var j = 0; j < Constants.FieldHeight; j++)
                {
                    var point = new Point(i, j);
                    Items[i, j] = ItemFactory.GetItem(board.GetAt(point), point);

                    Field.Cells[i, j].Items.Clear();
                    Field.Cells[i, j].Items.Add(Items[i, j]);
                }
            }
        }
    }
}
