using System.Drawing;
using FormUI.Infrastructure;

namespace FormUI.FieldItems.Tank
{
    public class AiTank : BaseTank
    {
        public override int ShotCountdown => Settings.Get.AiTicksPerShoot;

        public AiTank()
        {
            BorderColor = Color.DarkGray;
        }
    }
}
