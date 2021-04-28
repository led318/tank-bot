namespace FormUICore.Infrastructure
{
    public static class Constants
    {
        public static readonly int FieldHeight = 34;
        public static readonly int FieldWidth = 34;
        public static readonly int CellSize = 25;

        static Constants()
        {
            //FieldHeight = AppSettings.IsOldMap ? 34 : 27;
            //FieldWidth = AppSettings.IsOldMap ? 34 : 27;
            //CellSize = AppSettings.IsOldMap ? 25 : 30;
        }
    }
}
