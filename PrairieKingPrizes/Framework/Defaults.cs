using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrairieKingPrizes.Framework
{
    internal static class Defaults
    {
        public static Prize[] DefaultCommonItems { get; } = {
            new() { ItemId = "495", Quantity = 5 },
            new() { ItemId = "496", Quantity = 5 },
            new() { ItemId = "497", Quantity = 5 },
            new() { ItemId = "498", Quantity = 5 },
            new() { ItemId = "390", Quantity = 30 },
            new() { ItemId = "388", Quantity = 30 },
            new() { ItemId = "441", Quantity = 5 },
            new() { ItemId = "463", Quantity = 5 },
            new() { ItemId = "464", Quantity = 5 },
            new() { ItemId = "465", Quantity = 5 },
            new() { ItemId = "535", Quantity = 5 },
            new() { ItemId = "709", Quantity = 15 }
        };

        public static Prize[] DefaultUncommonItems { get; } = {
            new() { ItemId = "88", Quantity = 3 },
            new() { ItemId = "301", Quantity = 3 },
            new() { ItemId = "302", Quantity = 3 },
            new() { ItemId = "431", Quantity = 3 },
            new() { ItemId = "453", Quantity = 3 },
            new() { ItemId = "472", Quantity = 3 },
            new() { ItemId = "473", Quantity = 3 },
            new() { ItemId = "475", Quantity = 3 },
            new() { ItemId = "477", Quantity = 3 },
            new() { ItemId = "478", Quantity = 3 },
            new() { ItemId = "479", Quantity = 3 },
            new() { ItemId = "480", Quantity = 3 },
            new() { ItemId = "481", Quantity = 3 },
            new() { ItemId = "482", Quantity = 3 },
            new() { ItemId = "483", Quantity = 3 },
            new() { ItemId = "484", Quantity = 3 },
            new() { ItemId = "485", Quantity = 3 },
            new() { ItemId = "487", Quantity = 3 },
            new() { ItemId = "488", Quantity = 3 },
            new() { ItemId = "489", Quantity = 3 },
            new() { ItemId = "490", Quantity = 3 },
            new() { ItemId = "491", Quantity = 3 },
            new() { ItemId = "492", Quantity = 3 },
            new() { ItemId = "493", Quantity = 3 },
            new() { ItemId = "494", Quantity = 3 },
            new() { ItemId = "466", Quantity = 3 },
            new() { ItemId = "340", Quantity = 3 },
            new() { ItemId = "724", Quantity = 3 },
            new() { ItemId = "725", Quantity = 3 },
            new() { ItemId = "726", Quantity = 3 },
            new() { ItemId = "536", Quantity = 3 },
            new() { ItemId = "537", Quantity = 3 },
            new() { ItemId = "335", Quantity = 3 }
        };

        public static Prize[] DefaultRareItems { get; } = {
            new() { ItemId = "72", Quantity = 2 },
            new() { ItemId = "337", Quantity = 2 },
            new() { ItemId = "417", Quantity = 2 },
            new() { ItemId = "305", Quantity = 2 },
            new() { ItemId = "308", Quantity = 2 },
            new() { ItemId = "336", Quantity = 2 },
            new() { ItemId = "787", Quantity = 2 },
            new() { ItemId = "710", Quantity = 2 },
            new() { ItemId = "413", Quantity = 2 },
            new() { ItemId = "430", Quantity = 2 },
            new() { ItemId = "433", Quantity = 2 },
            new() { ItemId = "437", Quantity = 2 },
            new() { ItemId = "444", Quantity = 2 },
            new() { ItemId = "446", Quantity = 2 },
            new() { ItemId = "439", Quantity = 2 },
            new() { ItemId = "680", Quantity = 2 },
            new() { ItemId = "749", Quantity = 2 },
            new() { ItemId = "797", Quantity = 2 },
            new() { ItemId = "486", Quantity = 2 },
            new() { ItemId = "681", Quantity = 2 },
            new() { ItemId = "690", Quantity = 2 },
            new() { ItemId = "688", Quantity = 2 },
            new() { ItemId = "689", Quantity = 2 },
        };

        public static Prize[] DefaultCovetedItems { get; set; } = {
            new() { ItemId = "499", Quantity = 1 },
            new() { ItemId = "347", Quantity = 1 },
            new() { ItemId = "417", Quantity = 1 },
            new() { ItemId = "163", Quantity = 1 },
            new() { ItemId = "166", Quantity = 1 },
            new() { ItemId = "107", Quantity = 1 },
            new() { ItemId = "341", Quantity = 1 },
            new() { ItemId = "645", Quantity = 1 },
            new() { ItemId = "789", Quantity = 1 },
            new() { ItemId = "520", Quantity = 1 },
            new() { ItemId = "682", Quantity = 1 },
            new() { ItemId = "585", Quantity = 1 },
            new() { ItemId = "586", Quantity = 1 },
            new() { ItemId = "587", Quantity = 1 },
            new() { ItemId = "373", Quantity = 1 },
        };

        public static Prize[] DefaultLegendaryItems { get; set; } = {
            new() { ItemId = "74", Quantity = 1 },
        };
    }
}
