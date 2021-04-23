using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FormUI.FieldItems.Tank;
using FormUI.Infrastructure;

namespace FormUI.Logic
{
    public static class ShotCountdownLogic
    {
        public static void PopulateShotCountdownsFromPrevRound()
        {
            if (!State.HasPrevRound)
            {
                return;
            }

            var allTanks = State.ThisRound.AllTanks;
            var allPrevTanks = State.PrevRound.AllTanks;

            foreach (var tank in allTanks)
            {
                var nearPoints = tank.Point.GetNearPoints(includeThis: true).ToList();

                var nearPrevTanks = allPrevTanks.Where(t => nearPoints.Contains(t.Point)).ToList();

                BaseTank prevTank = null;
                if (nearPrevTanks.Count == 1)
                {
                    prevTank = nearPrevTanks.First();
                }
                else
                {
                    var tankType = tank.GetType();

                    var nearPrevSameTypeTanks = nearPrevTanks.Where(t => t.GetType() == tankType).ToList();

                    if (nearPrevSameTypeTanks.Count == 1)
                    {
                        prevTank = nearPrevSameTypeTanks.First();
                    }
                    else
                    {
                        prevTank = nearPrevSameTypeTanks.FirstOrDefault(t =>
                            t.GetNextPoints(t.Point).Contains(tank.Point));
                    }
                }

                if (prevTank != null)
                {
                    tank.SetShotCountdown(prevTank.ShotCountdownLeft);
                }
            }
        }
    }
}
