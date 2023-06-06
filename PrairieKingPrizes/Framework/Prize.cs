namespace PrairieKingPrizes.Framework
{
    internal class Prize
    {
        public int ItemId { get; set; }
        public int Quantity { get; set; }
        public int? Quality { get; set; } = null;
    }
}
