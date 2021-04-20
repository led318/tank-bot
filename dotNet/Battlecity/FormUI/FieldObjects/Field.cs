using FormUI.Infrastructure;

namespace FormUI.FieldObjects
{
    public static class Field
    {
        public static Cell[,] Cells = new Cell[Constants.FieldWidth, Constants.FieldHeight];

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

    }
}
