using API.Components;
using FormUI.FieldItems.Helpers;
using System.Drawing;
using FormUI.FieldObjects;
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

        public abstract int ShotCountdownDefault { get; }
        public int ShotCountdownLeft { get; set; } = 0;

        public bool IsShotThisRound => ShotCountdownLeft <= 0;
        //public bool IsShotNextRound => ShotCountdownLeft == 1;

        public bool IsPrize { get; set; }

        public bool IsStuck { get; set; }

        protected BaseTank(Element element, Point point) : base(element, point)
        {
            SetHealthNote();
            InitIsStuck();
        }

        private void InitIsStuck()
        {
            if (!State.HasPrevRound)
                return;

            var prevStepPoint = State.PrevRound.Items[Point.X, Point.Y];

            if (prevStepPoint.Element == Element)
                IsStuck = true;
        }

        protected virtual void SetHealthNote()
        {
            //if (Health != 1)
            //    AddNote(Health, Brushes.Green, NoteType.Health);
        }

        public void SetHealth(int health)
        {
            Health = health;
            SetHealthNote();
        }

        protected virtual void SetShotCountdownNote()
        {
            AddNote(ShotCountdownLeft, Brushes.Red, NoteType.ShotCountdown);

            Field.MarkCellDirty(Point);
        }

        public virtual void SetShotCountdown(int countdown)
        {
            ShotCountdownLeft = countdown;
            SetShotCountdownNote();
        }

        public virtual void Shot()
        {
            ShotCountdownLeft = ShotCountdownDefault - 1;
            SetShotCountdownNote();
        }

        public override void Tick()
        {
            ShotCountdownLeft--;
            SetShotCountdownNote();
        }
    }
}
