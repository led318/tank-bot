using System;
using System.Collections.Generic;
using System.Drawing;
using API.Components;
using FormUI.FieldItems;

namespace FormUI.FieldObjects
{
    public class Prediction
    {
        public int Depth { get; set; }
        public PredictionType Type { get; set; }
        public BaseItem Item { get; set; }

        public List<Direction> Command { get; set; } = new List<Direction>();

        public Color? GetBorderColor()
        {
            return GetBorderColor(Type);
        }

        public static Color? GetBorderColor(PredictionType type)
        {
            switch (type)
            {
                case PredictionType.MyShot:
                    return Color.DeepPink;
                case PredictionType.AiShot:
                    return Color.Red;
                case PredictionType.Bullet:
                    return Color.LightCoral;
                case PredictionType.EnemyShot:
                    return Color.DarkOrchid;
                case PredictionType.AiTankMove:
                    return Color.Aqua;
                case PredictionType.EnemyTankMove:
                    return Color.Green;
            }

            return null;
        }

        public Brush GetTextColor()
        {
            return GetTextColor(Type);
        }

        public static Brush GetTextColor(PredictionType type)
        {
            switch (type)
            {
                case PredictionType.MyShot:
                    return Brushes.DeepPink;
                case PredictionType.AiShot:
                    return Brushes.Red;
                case PredictionType.Bullet:
                    return Brushes.LightCoral;
                case PredictionType.EnemyShot:
                    return Brushes.DarkOrchid;
                case PredictionType.AiTankMove:
                    return Brushes.Aqua;
                case PredictionType.EnemyTankMove:
                    return Brushes.Green;
            }

            return null;
        }
    }

    public enum PredictionType
    {
        [IsDefaultSelected(Selected = true)]
        MyShot,
        [IsDefaultSelected(Selected = true)]
        AiShot,
        [IsDefaultSelected(Selected = true)]
        Bullet,
        [IsDefaultSelected(Selected = true)]
        EnemyShot,
        [IsDefaultSelected(Selected = true)]
        AiTankMove,
        [IsDefaultSelected(Selected = true)]
        EnemyTankMove
    }

    public class IsDefaultSelectedAttribute : Attribute
    {
        public bool Selected { get; set; }
    }
}
