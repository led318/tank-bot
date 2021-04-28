using System;

// ReSharper disable InconsistentNaming

namespace FormUICore.Predictions
{
    public enum PredictionType
    {
        //[IsDefaultSelected(Selected = true)]
        DangerCell,

        //[IsDefaultSelected(Selected = true)]
        MyKill,
        //[IsDefaultSelected(Selected = true)]
        MyShot,
        //[IsDefaultSelected(Selected = true)]
        MyMove,
        //[IsDefaultSelected(Selected = true)]
        AiShot,
        //[IsDefaultSelected(Selected = true)]
        Bullet,
        //[IsDefaultSelected(Selected = true)]
        AiMove,
        //[IsDefaultSelected(Selected = true)]
        EnemyShot,
        //[IsDefaultSelected(Selected = true)]
        EnemyMove
    }

    public class IsDefaultSelectedAttribute : Attribute
    {
        public bool Selected { get; set; }
    }
}
