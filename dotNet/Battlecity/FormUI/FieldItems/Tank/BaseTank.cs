using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using API.Components;
using FormUI.FieldItems.Helpers;
using FormUI.Infrastructure;
using Point = API.Components.Point;

namespace FormUI.FieldItems.Tank
{
    public abstract class BaseTank : BaseMobile
    {
        public override bool CanShootThrough => false; 
        public override bool CanMove => false;

        public override int Speed => 1;

        public virtual int Health { get; set; } = 1;

        public abstract int ShotCountdown { get; }
        public int ShotCountdownLeft { get; set; } = 0;

        protected BaseTank(Element element, Point point) : base(element, point)
        {
            SetHealthNote();
        }

        protected virtual void SetHealthNote()
        {
            AddNote(Health, Brushes.Red, NoteType.Health);
        }

        public void SetHealth(int health)
        {
            Health = health;
            SetHealthNote();
        }
    }
}
