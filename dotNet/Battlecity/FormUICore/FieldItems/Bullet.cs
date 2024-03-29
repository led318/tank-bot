﻿using System.Drawing;
using API.Components;
using FormUI.FieldItems;
using FormUI.Infrastructure;
using FormUICore.Infrastructure;
using Point = API.Components.Point;

namespace FormUICore.FieldItems
{
    public class Bullet : BaseMobile
    {
        public static int DefaultSpeed => 2;

        //public override bool CanShootThrough => false; //todo: maybe true

        public override int Speed => DefaultSpeed;

        public bool IsMyBullet { get; set; }

        public Bullet(Element element, Point point) : base(element, point)
        {
            if (AppSettings.DrawBaseBorders)
                BorderColor = Color.Red;

            //AddNote("0", Brushes.Red);
        }

        protected override void SetDirection()
        {
            //todo: calculate direction
        }
    }
}
