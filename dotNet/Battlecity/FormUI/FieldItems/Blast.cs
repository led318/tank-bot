﻿using System.Drawing;
using API.Components;
using FormUI.Infrastructure;

namespace FormUI.FieldItems
{
    public class Blast : BaseItem
    {
        public Blast(Element element, API.Components.Point point) : base(element, point)
        {
            if (AppSettings.DrawBaseBorders)
                BorderColor = Color.Brown;
        }
    }
}
