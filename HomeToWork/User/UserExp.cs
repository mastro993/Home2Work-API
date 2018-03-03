using System;

namespace HomeToWork.User
{
    [Serializable]
    public class UserExp
    {
        public int Level { get; set; }
        public long Amount { get; set; }
        public double CurrentLvLExp { get; set; }
        public double NextLvlExp { get; set; }

        private const double Threshold = 50.0;

        public UserExp(long amount)
        {
            Amount = amount;
            Level = GetLevel(amount);
            CurrentLvLExp = ForLevel(Level);
            NextLvlExp = ForLevel(Level + 1);
        }

        private int GetLevel(long expAmount)
        {
            var level = (1.0 + Math.Sqrt(1.0 + 8.0 * (expAmount / Threshold))) / 2;
            return (int) level;
        }

        private int ForLevel(int targetLevel)
        {
            var exp = (Math.Pow(targetLevel, 2) - targetLevel) * Threshold / 2;
            return (int) exp;
        }
    }
}