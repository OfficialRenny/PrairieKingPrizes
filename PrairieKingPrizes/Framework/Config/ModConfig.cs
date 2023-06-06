namespace PrairieKingPrizes.Framework.Config
{
    internal class ModConfig
    {
        public bool RequireGameCompletion { get; set; } = false;
        public bool AlternateCoinMethod { get; set; } = false;

        public MachineLocation MachineLocation { get; set; } = new MachineLocation();

        public Lootbox[] Lootboxes { get; set; } = new Lootbox[]
        {
            new Lootbox
            {
                Key = "Basic",
                Name = "Basic Tier",
                Cost = 10,
                PrizeTiers = new PrizeTier[]
                {
                    new PrizeTier
                    {
                        Chance = 0.4,
                        Prizes = Defaults.DefaultCommonItems,
                    },
                    new PrizeTier
                    {
                        Chance = 0.3,
                        Prizes = Defaults.DefaultUncommonItems,
                    },
                    new PrizeTier
                    {
                        Chance = 0.2,
                        Prizes = Defaults.DefaultRareItems,
                    },
                    new PrizeTier
                    {
                        Chance = 0.099,
                        Prizes = Defaults.DefaultCovetedItems,
                    },
                    new PrizeTier
                    {
                        Chance = 0.001,
                        Prizes = Defaults.DefaultLegendaryItems,
                    }
                },
            },
            new Lootbox
            {
                Key = "Premium",
                Name = "Premium Tier",
                Cost = 50,
                PrizeTiers = new PrizeTier[]
                {
                    new PrizeTier
                    {
                        Chance = 0.2,
                        Prizes = Defaults.DefaultCommonItems,
                    },
                    new PrizeTier
                    {
                        Chance = 0.25,
                        Prizes = Defaults.DefaultUncommonItems,
                    },
                    new PrizeTier
                    {
                        Chance = 0.30,
                        Prizes = Defaults.DefaultRareItems,
                    },
                    new PrizeTier
                    {
                        Chance = 0.24,
                        Prizes = Defaults.DefaultCovetedItems,
                    },
                    new PrizeTier
                    {
                        Chance = 0.1,
                        Prizes = Defaults.DefaultLegendaryItems,
                    }
                },
            }
        };
    }
}
