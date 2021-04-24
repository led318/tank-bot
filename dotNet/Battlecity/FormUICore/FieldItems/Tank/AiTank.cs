using System.Collections.Generic;
using System.Drawing;
using API.Components;
using FormUI.Infrastructure;
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
                    direction = Direction.Down;
                    break;
                case Element.AI_TANK_UP:
                    direction = Direction.Up;
                    break;
                case Element.AI_TANK_LEFT:
                    direction = Direction.Left;
                    break;
                case Element.AI_TANK_RIGHT:
                    direction = Direction.Right;
                    break;

                default:
                    direction = Direction.Down;
                    break;
            }

            CurrentDirection = direction;
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
