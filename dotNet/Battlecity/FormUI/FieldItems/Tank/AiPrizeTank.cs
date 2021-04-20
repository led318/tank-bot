using System.Drawing;
using FormUI.Infrastructure;

namespace FormUI.FieldItems.Tank
{
    public class AiPrizeTank : AiTank
    {
        public override int Health { get; set; } = Settings.Get.KillHitsAiPrize;

        public AiPrizeTank()
        {
            BorderColor = Color.Orange;
        }
    }
}
