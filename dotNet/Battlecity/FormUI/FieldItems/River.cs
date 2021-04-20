﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using API.Components;
using FormUI.Infrastructure;

namespace FormUI.FieldItems
{
    public class River : BaseItem
    {
        public override bool CanMove => false;

        public River(Element element, API.Components.Point point) : base(element, point)
        {
            if (AppSettings.DrawBaseBorders)
                BorderColor = Color.Blue;
        }

    }
}
