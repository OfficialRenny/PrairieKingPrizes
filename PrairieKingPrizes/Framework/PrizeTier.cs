namespace PrairieKingPrizes.Framework
{
    internal class PrizeTier
    {
        public string Name { get; set; } = "";
        public double Chance { get; set; }
        public Prize[] Prizes { get; set; }
    }
}
