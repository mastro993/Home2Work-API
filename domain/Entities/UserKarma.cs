using System;

namespace domain.Entities
{
    [Serializable]
    public class UserKarma
    {
        public int Level { get; set; }
        public long Amount { get; set; }
        public double CurrentLvLKarma { get; set; }
        public double NextLvlKarma { get; set; }
        public int MonthKarma { get; set; }

        private const double Threshold = 50.0;

        public UserKarma(long amount)
        {
            Amount = amount;
            Level = GetLevel(amount);
            CurrentLvLKarma = ForLevel(Level);
            NextLvlKarma = ForLevel(Level + 1);
        }

        private static int GetLevel(long karma)
        {
            var level = (1.0 + Math.Sqrt(1.0 + 8.0 * (karma / Threshold))) / 2;
            return (int) level;
        }

        private static int ForLevel(int targetLevel)
        {
            var exp = (Math.Pow(targetLevel, 2) - targetLevel) * Threshold / 2;
            return (int) exp;
        }
    }
}