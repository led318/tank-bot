using System.Drawing;
using FormUI.Infrastructure;

namespace FormUI.FieldItems.Tank
{
    public class MyTank : BaseTank
    {
        public override int ShotCountdown => Settings.Get.TankTicksPerShoot;

        public MyTank()
        {
            BorderColor = Color.Yellow;
        }
    }
}
