using System.Drawing;
using API.Components;
using FormUI.FieldItems.Tank;
using FormUICore.Infrastructure;
using Point = API.Components.Point;

namespace FormUICore.FieldItems.Tank
{
    public class AiPrizeTank : AiTank
    {
        public override int Health { get; set; } = ServerSettings.Settings.KillHitsAiPrize;

        public AiPrizeTank(Element element, Point point) : base(element, point)
        {
            IsPrize = true;

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
