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
            switch (Type)
            {
                case PredictionType.MyShot:
                    return Color.DeepPink;
                case PredictionType.AiShot:
                    return Color.Red;
                case PredictionType.Bullet:
                    return Color.LightCoral;
                case PredictionType.EnemyShot:
                    return Color.DarkOrchid;
                case PredictionType.AiTank:
                    return Color.Aqua;
                case PredictionType.EnemyTank:
                    return Color.Green;
            }

            return null;
        }

        public Brush GetTextColor()
        {
            switch (Type)
            {
                case PredictionType.MyShot:
                    return Brushes.DeepPink;
                case PredictionType.AiShot:
                    return Brushes.Red;
                case PredictionType.Bullet:
                    return Brushes.LightCoral;
                case PredictionType.EnemyShot:
                    return Brushes.DarkOrchid;
                case PredictionType.AiTank:
                    return Brushes.Aqua;
                case PredictionType.EnemyTank:
                    return Brushes.Green;
            }

            return null;
        }
    }

    public enum PredictionType
    {
        MyShot,
        AiShot,
        Bullet,
        EnemyShot,
        AiTank,
        EnemyTank
    }
}
