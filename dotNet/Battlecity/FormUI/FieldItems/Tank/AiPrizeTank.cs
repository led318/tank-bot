using System.Drawing;
using API.Components;
using FormUI.Infrastructure;
using Point = API.Components.Point;

namespace FormUI.FieldItems.Tank
{
    public class AiPrizeTank : AiTank
    {
        //public static Color Color => Color.Orange;

        public override int Health { get; set; } = Settings.Get.KillHitsAiPrize;

        public AiPrizeTank(Element element, Point point) : base(element, point)
        {
            //if (AppSettings.DrawBaseBorders)
                BorderColor = Color.Orange;
        }

        protected override void SetDirection()
        {
            //if (Element == Element.AI_TANK_PRIZE)
            //    return;

            base.SetDirection();
        }
    }
}
