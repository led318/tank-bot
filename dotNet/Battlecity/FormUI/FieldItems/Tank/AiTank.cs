using System.Drawing;
using FormUI.FieldItems.Helpers;
using FormUI.Infrastructure;

namespace FormUI.FieldItems.Tank
{
    public class AiTank : BaseTank
    {
        public override int ShotCountdown => Settings.Get.AiTicksPerShoot;

        public AiTank()
        {
            BorderColor = Color.DarkGray;

            AddNote("0", Brushes.DarkGreen);
            AddNote("1", Brushes.Red);
        }
    }
}
