namespace PrairieKingPrizes.Framework
{
    internal class Prize
    {
        public string ItemId { get; set; }
        public int Quantity { get; set; }
        public int? Quality { get; set; } = null;
    }
}
