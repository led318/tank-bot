using API;
using API.Components;
using FormUI.Infrastructure;

namespace FormUI.FieldItems
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
                }
            }
        }
    }
}
