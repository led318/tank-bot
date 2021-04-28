using System.Collections.Generic;
using System.Drawing;
using API.Components;
using FormUI.Infrastructure;
using FormUICore.Infrastructure;
using Point = API.Components.Point;

namespace FormUI.FieldItems.Tank
{
    public class AiTank : BaseTank
    {
        public override int ShotCountdownDefault => Settings.Get.AiTicksPerShoot;

        private readonly HashSet<Element> _aiTankElements = new HashSet<Element>
        {
            Element.AI_TANK_DOWN,
            Element.AI_TANK_LEFT,
            Element.AI_TANK_RIGHT,
            Element.AI_TANK_UP,
            Element.AI_TANK_PRIZE
        };

        public AiTank(Element element, Point point) : base(element, point)
        {
            if (AppSettings.DrawBaseBorders)
                BorderColor = Color.DarkGray;
        }

        protected override void SetDirection()
        {
            Direction direction;

            switch (Element)
            {
                case Element.AI_TANK_DOWN:
                    direction = API.Components.Direction.Down;
                    break;
                case Element.AI_TANK_UP:
                    direction = API.Components.Direction.Up;
                    break;
                case Element.AI_TANK_LEFT:
                    direction = API.Components.Direction.Left;
                    break;
                case Element.AI_TANK_RIGHT:
                    direction = API.Components.Direction.Right;
                    break;

                default:
                    direction = API.Components.Direction.Down;
                    break;
            }

            Direction = direction;
        }

        public override void Tick()
        {
            base.Tick();

            if (ShotCountdownLeft < 0)
                Shot();
        }

        protected override void InitIsStuck()
        {
            if (!State.HasPrevRound)
                return;

            var prevStepPoint = State.PrevRound.Items[Point.X, Point.Y];

            if (_aiTankElements.Contains(prevStepPoint.Element))
                IsStuck = true;
        }
    }
}
