namespace FormUI.Infrastructure
{
    public static class Constants
    {
        public static readonly int FieldHeight;
        public static readonly int FieldWidth;
        public static readonly int CellSize = 25;

        static Constants()
        {
            FieldHeight = AppSettings.IsOldMap ? 34 : 27;
            FieldWidth = AppSettings.IsOldMap ? 34 : 27;

        }
    }
}
