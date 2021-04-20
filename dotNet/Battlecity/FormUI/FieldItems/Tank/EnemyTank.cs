using System.Drawing;
using FormUI.Infrastructure;

namespace FormUI.FieldItems.Tank
{
    public class EnemyTank : BaseTank
    {
        public override int ShotCountdown => Settings.Get.TankTicksPerShoot;

        public EnemyTank()
        {
            BorderColor = Color.Green;
        }
    }
}
