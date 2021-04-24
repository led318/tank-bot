using System.Drawing;
using API.Components;
using Point = API.Components.Point;

namespace FormUI.FieldItems
{
    public class Bullet : BaseMobile
    {
        public static int DefaultSpeed => 2;

        //public override bool CanShootThrough => false; //todo: maybe true

        public override int Speed => DefaultSpeed;

        //public bool IsMyBullet { get; set; }

        public Bullet(Element element, Point point) : base(element, point)
        {
            BorderColor = Color.Red;

            //AddNote("0", Brushes.Red);
        }

        protected override void SetDirection()
        {
            //todo: calculate direction
        }
    }
}
