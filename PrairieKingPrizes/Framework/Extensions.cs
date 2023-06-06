using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrairieKingPrizes.Framework
{
    internal static class Extensions
    {
        internal static PrizeTier PickPrizeTier(this Random random, IEnumerable<PrizeTier> prizeTiers)
        {
            var totalSum = prizeTiers.Sum(x => x.Chance);
            var randomNumber = random.NextDouble() * totalSum;

            double sum = 0;
            foreach (var prizeTier in prizeTiers)
            {
                if (randomNumber <= (sum += prizeTier.Chance))
                    return prizeTier;
            }

            return prizeTiers.Last();
        }
    }
}
