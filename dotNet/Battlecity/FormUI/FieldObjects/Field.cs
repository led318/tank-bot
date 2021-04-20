using System.Collections.Generic;
using System.Linq;
using FormUI.FieldItems;
using FormUI.Infrastructure;

namespace FormUI.FieldObjects
{
    public static class Field
    {
        public static Cell[,] Cells = new Cell[Constants.FieldWidth, Constants.FieldHeight];

        public static IEnumerable<Cell> AllCells => Flatten(Cells);

        public static IEnumerable<BaseItem> AllItems => AllCells.Select(x => x.Item).ToList();
        

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

        public static void Reset()
        {
            foreach (var cell in AllCells)
            {
                cell.Reset();
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

    }
}
