using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Minigames;

namespace PrairieKingPrizes
{
    public class ModEntry : Mod
    {
        int coinsCollected;
        int totalTokens;
        private object LastMinigame;

        //[DeluxeGrabber] Map: Saloon
        //[DeluxeGrabber] Tile: {X:34 Y:17}
        //It's a Surprise Tool That Will Help Us Later

        public override void Entry(IModHelper helper)
        {
            var savedData = this.Helper.ReadJsonFile<SavedData>($"data/{Constants.SaveFolderName}.json") ?? new SavedData();
            totalTokens = savedData.totalTokens;
            GameEvents.UpdateTick += GameEvents_UpdateTick;
            SaveEvents.AfterLoad += AfterSaveLoaded;
            helper.ConsoleCommands.Add("getcoins", "Retrieves the value of your current amount of coins.", this.GetCoins);
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data/NPCDispositions"))
                return true;

            if (asset.AssetNameEquals("Data/NPCGiftTastes"))
                return true;

            if (asset.AssetNameEquals("Characters/Dialogue/rainy"))
                return true;

            return false;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/NPCDispositions"))
            {
                asset.AsDictionary<string, string>().Data["Token Machine"] = "adult/shy/outgoing/negative/male/non-datable/null/Town/fall 9/null/Saloon 34 17/Token Machine";
            }

            if (asset.AssetNameEquals("Data/NPCGiftTastes"))
            {
                IDictionary<string, string> NPCGiftTastes = asset.AsDictionary<string, string>().Data;
                NPCGiftTastes["Token Machine"] = "ERROR: I DO NOT ACCEPT GIFTS//ERROR: I DO NOT ACCEPT GIFTS//ERROR: I DO NOT ACCEPT GIFTS//ERROR: I DO NOT ACCEPT GIFTS/-2 -4 -5 -6 -7 -8 -9 -12 -14 -15 -16 -17 -18 -19 -20 -21 -22 -23 -24 -25 -26 -27 -28 -29 -74 -75 -79 -80 -81 -95 -96 -98 -99/ERROR: I DO NOT ACCEPT GIFTS//";
            }

            if (asset.AssetNameEquals("Characters/Dialogue/rainy"))
            {
                IDictionary<string, string> rainy = asset.AsDictionary<string, string>().Data;
                rainy["Token Machine"] = "I HOPE I DO NOT GET WET IN THIS RAIN. OH WAIT, I CANNOT MOVE. HA. HA.";
            }
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Characters/Dialogue/Token Machine"))
            {
                return true;
            }

            if (asset.AssetNameEquals("Characters/Token Machine"))
            {
                return true;
            }

            if (asset.AssetNameEquals("Portraits/Token Machine"))
            {
                return true;
            }

            if (asset.AssetNameEquals("Characters/Schedules/Token Machine"))
            {
                return true;
            }

            return false;
        }

        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Characters/Dialogue/Token Machine"))
            {
                return Helper.Content.Load<T>("assets/tokenDialogue.xnb", ContentSource.ModFolder);
            }

            if (asset.AssetNameEquals("Characters/Schedules/Token Machine"))
            {
                return Helper.Content.Load<T>("assets/tokenSchedule.xnb", ContentSource.ModFolder);
            }

            if (asset.AssetNameEquals("Characters/Token Machine"))
            {
                return Helper.Content.Load<T>("assets/tokenMachine.png", ContentSource.ModFolder);
            }

            if (asset.AssetNameEquals("Portraits/Token Machine"))
            {
                return Helper.Content.Load<T>("assets/portrait.png", ContentSource.ModFolder);
            }

            throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        }

        private void GetCoins(string command, string[] args)
        {
            this.Monitor.Log($"You currently have {totalTokens} coins.");
        }


        private void AfterSaveLoaded(object sender, EventArgs args)
        {
            Texture2D portrait = Helper.Content.Load<Texture2D>("assets/portrait.png", ContentSource.ModFolder);
            NPC tokenNPC = new NPC(null, new Vector2(34, 17), "Saloon", 3, "Token Machine", false, null, portrait);
            Monitor.Log("Created Token Machine NPC.");
            Game1.getLocationFromName("Saloon").addCharacter(tokenNPC);
            Monitor.Log($"The token machine should have spawned at {tokenNPC.Position.X},{tokenNPC.Position.Y}");
        }

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (Game1.currentMinigame != null && "AbigailGame".Equals(Game1.currentMinigame.GetType().Name))
            {
                Type minigameType = Game1.currentMinigame.GetType();
                coinsCollected = Convert.ToInt32(minigameType.GetField("coins").GetValue(Game1.currentMinigame));
                LastMinigame = Game1.currentMinigame.GetType().Name;
            }

            if(Game1.currentMinigame == null && "AbigailGame".Equals(LastMinigame))
            {
                totalTokens += coinsCollected;
                coinsCollected = 0;
                var savedData = this.Helper.ReadJsonFile<SavedData>($"data/{Constants.SaveFolderName}.json") ?? new SavedData();
                savedData.totalTokens = totalTokens;
            }

            int basicItemID = 1101;
            int premiumItemID = 1102;
            int cancelID = 1103;

            foreach(int i in Game1.player.dialogueQuestionsAnswered)
            {
                if(i == basicItemID)
                {
                    Game1.player.dialogueQuestionsAnswered.Remove(basicItemID);
                    givePlayerBasicItem();
                }
                if(i == premiumItemID)
                {
                    Game1.player.dialogueQuestionsAnswered.Remove(basicItemID);
                    givePlayerPremiumItem();
                }
                if(i == cancelID)
                {
                    Game1.player.dialogueQuestionsAnswered.Remove(cancelID);
                    return;
                }
            }
        }

        private void givePlayerBasicItem()
        {
            int[] common = { 495, 496, 497, 498 };
            int[] uncommon = { 88, 301, 302, 431, 433, 453, 472, 473, 473, 475, 475, 477, 478, 479, 480, 481, 482, 483, 484, 485, 487, 488, 489, 490, 491, 492, 493, 494 };
            int[] rare = { 72, 337, 417, 434, 305 };
            int[] coveted = { 499, 486, 347, 163, 166, 107 };
            int[] legendary = { 74 };

            if (Game1.player.isInventoryFull() == true)
            {
                return;
            }

            Random random = new Random();
            double diceRoll = random.NextDouble();

            if(diceRoll <= 0.01)
            {
                //give legendary item
                Game1.player.items.Add((Item)new StardewValley.Object(74, 1));
            }
            if(diceRoll > 0.01 && diceRoll <= 0.1)
            {
                //give coveted item
                Random rnd = new Random();
                int r = rnd.Next(coveted.Length);
                Game1.player.items.Add((Item)new StardewValley.Object(r, 2));
            }
            if(diceRoll > 0.1 && diceRoll <= 0.3)
            {
                //give rare item
                Random rnd = new Random();
                int r = rnd.Next(rare.Length);
                Game1.player.items.Add((Item)new StardewValley.Object(r, 3));
            }
            if(diceRoll > 0.3 && diceRoll <= 0.6)
            {
                //give uncommon item
                Random rnd = new Random();
                int r = rnd.Next(uncommon.Length);
                Game1.player.items.Add((Item)new StardewValley.Object(r, 10));
            }
            if(diceRoll > 0.6 && diceRoll <= 1)
            {
                //give common item
                Random rnd = new Random();
                int r = rnd.Next(common.Length);
                Game1.player.items.Add((Item)new StardewValley.Object(r, 15));
            }

        }
        private void givePlayerPremiumItem()
        {
            int[] common = { 495, 496, 497, 498 };
            int[] uncommon = { 88, 301, 302, 431, 433, 453, 472, 473, 473, 475, 475, 477, 478, 479, 480, 481, 482, 483, 484, 485, 487, 488, 489, 490, 491, 492, 493, 494 };
            int[] rare = { 72, 337, 417, 434, 305 };
            int[] coveted = { 499, 486, 347, 163, 166, 107 };
            int[] legendary = { 74 };

            if (Game1.player.isInventoryFull() == true)
            {
                return;
            }
            Random random = new Random();
            double diceRoll = random.NextDouble();
            if (diceRoll <= 0.05)
            {
                //give legendary premium item
                Game1.player.items.Add((Item)new StardewValley.Object(74, 2));
            }
            if (diceRoll > 0.05 && diceRoll <= 0.25)
            {
                //give coveted premium item
                Random rnd = new Random();
                int r = rnd.Next(coveted.Length);
                Game1.player.items.Add((Item)new StardewValley.Object(r, 5));
            }
            if (diceRoll > 0.25 && diceRoll <= 0.45)
            {
                //give rare premium item
                Random rnd = new Random();
                int r = rnd.Next(rare.Length);
                Game1.player.items.Add((Item)new StardewValley.Object(r, 10));
            }
            if (diceRoll > 0.45 && diceRoll <= 0.8)
            {
                //give uncommon premium item
                Random rnd = new Random();
                int r = rnd.Next(uncommon.Length);
                Game1.player.items.Add((Item)new StardewValley.Object(r, 15));
            }
            if (diceRoll > 0.8 && diceRoll <= 1)
            {
                //give common premium item
                Random rnd = new Random();
                int r = rnd.Next(common.Length);
                Game1.player.items.Add((Item)new StardewValley.Object(r, 25));
            }

        }
    }

    //Secret Hacks - Basic Item
    //    Common - 40%
    //    Uncommon - 30%
    //    Rare - 20%
    //    Coveted - 9%
    //    Legendary - 1%
    //Secret Hacks - Premium Item
    //    Common - 20%
    //    Uncommon - 25%
    //    Rare - 30%
    //    Coveted - 20%
    //    Legendary - 5%

    internal class SavedData
    {
        public int totalTokens { get; set; }
    }
}
