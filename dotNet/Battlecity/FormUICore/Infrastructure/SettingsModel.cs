namespace FormUI.Infrastructure
{
    public class SettingsModel
    {
        /// <summary>
        /// Затримка між пострілами звичайних танків
        /// </summary>
        public int TankTicksPerShoot { get; set; } = 4;

        /// <summary>
        /// Кількість тіків, які танк ковзає по кризі
        /// </summary>
        public int Slipperiness { get; set; } = 3;

        /// <summary>
        /// ???
        /// </summary>
        public int AiPrizeLimit { get; set; } = 3;

        /// <summary>
        /// Кількість штрафних тіків-затримки на воді
        /// </summary>
        public int PenaltyWalkingOnWater { get; set; } = 2;

        /// <summary>
        /// Бачите ви свій танк поверх дерев, чи ні
        /// </summary>
        public bool ShowMyTankUnderTree { get; set; } = true;

        /// <summary>
        /// Штрафні бали, коли гине ваш танк
        /// </summary>
        public int KillYourTankPenalty { get; set; } = 0;

        /// <summary>
        /// Бали, які ви заробляєтете, знищуючи інших ботів
        /// </summary>
        public int KillOtherHeroTankScore { get; set; } = 50;

        /// <summary>
        /// Бали за знищення AI-ботів
        /// </summary>
        public int KillOtherAiTankScore { get; set; } = 25;

        /// <summary>
        /// ???
        /// </summary>
        public int SpawnAiPrize { get; set; } = 4;

        /// <summary>
        /// Кількість пострілів, які потрібно зробити по призовому танку
        /// </summary>
        public int KillHitsAiPrize { get; set; } = 3;

        /// <summary>
        /// Час існування призу на полі
        /// </summary>
        public int PrizeOnField { get; set; } = 50;

        /// <summary>
        /// Час впливу призу на танк
        /// </summary>
        public int PrizeWorking { get; set; } = 30;

        /// <summary>
        /// Затримка між пострілами для AI-танків
        /// </summary>
        public int AiTicksPerShoot { get; set; } = 10;

    }

    /*
[
    {
        "tankTicksPerShoot": 4,
        "slipperiness": 3,
        "aiPrizeLimit": 3,
        "penaltyWalkingOnWater": 2,
        "showMyTankUnderTree": true,
        "killYourTankPenalty": 0,
        "killOtherHeroTankScore": 50,
        "killOtherAiTankScore": 25,
        "spawnAiPrize": 4,
        "killHitsAiPrize": 3,
        "prizeOnField": 50,
        "prizeWorking": 30,
        "aiTicksPerShoot": 10
    }
]
     
     */
}
