using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormUI.FieldItems;

namespace FormUI.FieldObjects
{
    public class Prediction
    {
        public int Depth { get; set; }
        public PredictionType Type { get; set; }
        public BaseItem Item { get; set; }


        public Color? GetBorderColor()
        {
            switch (Type)
            {
                case PredictionType.Bullet:
                    return Color.LightCoral;
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
                case PredictionType.Bullet:
                    return Brushes.LightCoral;
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
        MyBullet,
        Bullet,
        AiTank,
        EnemyTank
    }
}
