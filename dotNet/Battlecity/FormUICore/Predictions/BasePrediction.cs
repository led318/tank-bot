using System;
using System.Collections.Generic;
using System.Drawing;
using API.Components;
using FormUI.FieldItems;
using FormUI.Infrastructure;
using FormUI.Predictions;
using Point = API.Components.Point;

namespace FormUICore.Predictions
{
    public abstract class BasePrediction
    {
        public Point Point { get; set; }
        public int Depth { get; set; }
        public abstract PredictionType Type { get; }
        public Color? BorderColor => GetBorderColor();
        public Brush TextColor => GetTextColor();

        public BaseItem Item { get; set; }

        //[Obsolete]
        public List<Direction> Commands { get; set; } = new List<Direction>();

        public string CommandsText => Commands.CommandsToString();

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
                case PredictionType.MyMove:
                    return Color.LawnGreen;
                case PredictionType.AiShot:
                    return Color.Coral;
                case PredictionType.Bullet:
                    return Color.LightCoral;
                case PredictionType.EnemyShot:
                    return Color.DarkOrchid;
                case PredictionType.AiMove:
                    return Color.Aqua;
                case PredictionType.EnemyMove:
                    return Color.Green;
                case PredictionType.MyKill:
                    return Color.Red;
                case PredictionType.DangerCell:
                    return Color.DarkRed;
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
                case PredictionType.MyMove:
                    return Brushes.LawnGreen;
                case PredictionType.AiShot:
                    return Brushes.Coral;
                case PredictionType.Bullet:
                    return Brushes.LightCoral;
                case PredictionType.EnemyShot:
                    return Brushes.DarkOrchid;
                case PredictionType.AiMove:
                    return Brushes.Aqua;
                case PredictionType.EnemyMove:
                    return Brushes.Green;
                case PredictionType.MyKill:
                    return Brushes.Red;
                case PredictionType.DangerCell:
                    return Brushes.DarkRed;
            }

            return null;
        }
    }
}
