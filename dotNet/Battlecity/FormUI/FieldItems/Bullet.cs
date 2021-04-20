using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Components;
using FormUI.FieldItems.Helpers;

namespace FormUI.FieldItems
{
    public class Bullet : BaseMobile
    {

        public override bool CanShootThrough => false; //todo: maybe true

        public override int Speed => 2;

        public bool IsMyBullet { get; set; }

        public Bullet(Element element, API.Components.Point point) : base(element, point)
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
