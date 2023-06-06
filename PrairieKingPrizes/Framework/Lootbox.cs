namespace PrairieKingPrizes.Framework
{
    internal class Lootbox
    {
        public string Key { get; set; }
        public string Name { get; set; }
        public int Cost { get; set; }
        public PrizeTier[] PrizeTiers { get; set; }
    }
}
