using System;

namespace CauldronOnlineServer.Services
{
    public class RNGService : WorldService
    {
        public const string NAME = "RNG";
        private static RNGService _instance = null;

        public override string Name => NAME;

        private Random _seed = null;

        public override void Start()
        {
            if (_instance == null)
            {
                _instance = this;
                _seed = new Random((int)DateTime.UtcNow.Ticks);
                base.Start();
            }
        }

        public static int Range(int min, int max)
        {
            return _instance._seed.Next(min, max);
        }

        public static bool Roll(float chanceToSucceed = .5f)
        {
            return _instance._seed.Next(0, 101) / 100f <= chanceToSucceed;
        }
    }
}