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
using StardewValley.Objects;
using SObject = StardewValley.Object;

namespace PrairieKingPrizes
{
    public class ModEntry : Mod, IAssetEditor, IAssetLoader
    {
        int coinsCollected;
        int totalTokens;
        private object LastMinigame;
        bool isNPCLoaded = false;
        int basicItemID = 444;
        int premiumItemID = 445;
        int cancelID = 446;
        NPC tokenNPC { get; set; }


        //[DeluxeGrabber] Map: Saloon
        //[DeluxeGrabber] Tile: {X:36 Y:17}
        //It's a Surprise Tool That Will Help Us Later

        public override void Entry(IModHelper helper)
        {
            GameEvents.UpdateTick += GameEvents_UpdateTick;
            SaveEvents.AfterLoad += AfterSaveLoaded;
            helper.ConsoleCommands.Add("gettokens", "Retrieves the value of your current amount of tokens.", this.GetCoins);
            helper.ConsoleCommands.Add("addtokens", "Gives you tokens. Usage: addtokens <amount>", this.AddCoins);
        }

        private void AddCoins(string command, string[] args)
        {
            if(args.Length > 1)
            {
                return;
            }
            totalTokens += Convert.ToInt32(args[1]);

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
                asset.AsDictionary<string, string>().Data["TokenMachine"] = "adult/shy/outgoing/negative/male/non-datable/null/Town/fall 9/null/Saloon 34 17/Token Machine";
            }

            if (asset.AssetNameEquals("Data/NPCGiftTastes"))
            {
                IDictionary<string, string> NPCGiftTastes = asset.AsDictionary<string, string>().Data;
                NPCGiftTastes["TokenMachine"] = "ERROR: I DO NOT ACCEPT GIFTS//ERROR: I DO NOT ACCEPT GIFTS//ERROR: I DO NOT ACCEPT GIFTS//ERROR: I DO NOT ACCEPT GIFTS/-2 -4 -5 -6 -7 -8 -9 -12 -14 -15 -16 -17 -18 -19 -20 -21 -22 -23 -24 -25 -26 -27 -28 -29 -74 -75 -79 -80 -81 -95 -96 -98 -99/ERROR: I DO NOT ACCEPT GIFTS//";
            }

            if (asset.AssetNameEquals("Characters/Dialogue/rainy"))
            {
                IDictionary<string, string> rainy = asset.AsDictionary<string, string>().Data;
                rainy["TokenMachine"] = "I HOPE I DO NOT GET WET IN THIS RAIN. OH WAIT, I CANNOT MOVE. HA. HA.";
            }
        }

        public bool CanLoad<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Characters/Dialogue/TokenMachine"))
            {
                return true;
            }

            if (asset.AssetNameEquals("Characters/TokenMachine"))
            {
                return true;
            }

            if (asset.AssetNameEquals("Portraits/TokenMachine"))
            {
                return true;
            }

            if (asset.AssetNameEquals("Characters/Schedules/TokenMachine"))
            {
                return true;
            }

            return false;
        }

        public T Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Characters/Dialogue/TokenMachine"))
            {
                return Helper.Content.Load<T>("assets/tokenDialogue.xnb", ContentSource.ModFolder);
            }

            if (asset.AssetNameEquals("Characters/Schedules/TokenMachine"))
            {
                return Helper.Content.Load<T>("assets/tokenSchedule.xnb", ContentSource.ModFolder);
            }

            if (asset.AssetNameEquals("Characters/TokenMachine"))
            {
                return Helper.Content.Load<T>("assets/tokenMachine.png", ContentSource.ModFolder);
            }

            if (asset.AssetNameEquals("Portraits/TokenMachine"))
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
            var savedData = this.Helper.ReadJsonFile<SavedData>($"data/{Constants.SaveFolderName}.json") ?? new SavedData();
            totalTokens = savedData.totalTokens;

            Texture2D portrait = Helper.Content.Load<Texture2D>("assets/portrait.png", ContentSource.ModFolder);

            NPC tokenNPC = new NPC(null, new Vector2(36f, 17f), "Saloon", 3, "TokenMachine", false, null, portrait);

            Monitor.Log("Created Token Machine NPC.");
            Monitor.Log($"The token machine should have spawned at {tokenNPC.Position.X},{tokenNPC.Position.Y}");


            foreach (int i in Game1.player.dialogueQuestionsAnswered)
            {
                if (i == basicItemID)
                {
                    Game1.player.dialogueQuestionsAnswered.Remove(basicItemID);
                }
                if (i == premiumItemID)
                {
                    Game1.player.dialogueQuestionsAnswered.Remove(premiumItemID);
                }
                if (i == cancelID)
                {
                    Game1.player.dialogueQuestionsAnswered.Remove(cancelID);
                }
            }

        }

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            bool needToUpdateSavedData = false;

            if (Game1.currentMinigame != null && "AbigailGame".Equals(Game1.currentMinigame.GetType().Name))
            {
                Type minigameType = Game1.currentMinigame.GetType();
                coinsCollected = Convert.ToInt32(minigameType.GetField("coins").GetValue(Game1.currentMinigame));
                LastMinigame = Game1.currentMinigame.GetType().Name;
            }

            if (Game1.currentMinigame == null && "AbigailGame".Equals(LastMinigame))
            {
                totalTokens += coinsCollected;
                needToUpdateSavedData = true;
            }

            if (needToUpdateSavedData)
            {
                var savedData = this.Helper.ReadJsonFile<SavedData>($"data/{Constants.SaveFolderName}.json") ?? new SavedData();
                savedData.totalTokens = totalTokens;
                needToUpdateSavedData = false;
                coinsCollected = 0;
            }


            foreach (int i in Game1.player.dialogueQuestionsAnswered)
            {
                if (i == basicItemID)
                {
                    Game1.player.dialogueQuestionsAnswered.Remove(basicItemID);
                    givePlayerBasicItem();
                    tokenNPC.resetCurrentDialogue();
                }
                if (i == premiumItemID)
                {
                    Game1.player.dialogueQuestionsAnswered.Remove(premiumItemID);
                    givePlayerPremiumItem();
                    tokenNPC.resetCurrentDialogue();

                }
                if (i == cancelID)
                {
                    Game1.player.dialogueQuestionsAnswered.Remove(cancelID);
                    tokenNPC.resetCurrentDialogue();
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



            Random random = new Random();
            double diceRoll = random.NextDouble();

            if (totalTokens >= 10)
            {

                if (diceRoll <= 0.01)
                {
                    //give legendary item
                    this.Monitor.Log($"Attempting to give player an item with the ID of 74.");
                    Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(74, 1, false, -1, 0));


                }
                if (diceRoll > 0.01 && diceRoll <= 0.1)
                {
                    //give coveted item
                    Random rnd = new Random();
                    int r = rnd.Next(coveted.Length);
                    this.Monitor.Log($"Attempting to give player an item with the ID of {coveted[r]}.");
                    Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(coveted[r], 1, false, -1, 0));
                }
                if (diceRoll > 0.1 && diceRoll <= 0.3)
                {
                    //give rare item
                    Random rnd = new Random();
                    int r = rnd.Next(rare.Length);
                    this.Monitor.Log($"Attempting to give player an item with the ID of {rare[r]}.");
                    Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(rare[r], 1, false, -1, 0));
                }
                if (diceRoll > 0.3 && diceRoll <= 0.6)
                {
                    //give uncommon item
                    Random rnd = new Random();
                    int r = rnd.Next(uncommon.Length);
                    this.Monitor.Log($"Attempting to give player an item with the ID of {uncommon[r]}.");
                    Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(uncommon[r], 1, false, -1, 0));
                }
                if (diceRoll > 0.6 && diceRoll <= 1)
                {
                    //give common item
                    Random rnd = new Random();
                    int r = rnd.Next(common.Length);
                    this.Monitor.Log($"Attempting to give player an item with the ID of {common[r]}.");
                    Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(common[r], 1, false, -1, 0));
                }
                totalTokens -= 10;
                var savedData = this.Helper.ReadJsonFile<SavedData>($"data/{Constants.SaveFolderName}.json") ?? new SavedData();
                savedData.totalTokens = totalTokens;
            }
            else
            {
                Game1.addHUDMessage(new HUDMessage($"You do not have enough Tokens.", 3));
                Game1.addHUDMessage(new HUDMessage($"Your current Token balance is {totalTokens}.", 2));
                return;
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

            if (totalTokens >= 50)
            {
                if (diceRoll <= 0.05)
                {
                    //give legendary premium item
                    this.Monitor.Log($"Attempting to give player an item with the ID of 74.");
                    Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(74, 2, false, -1, 0));
                }
                if (diceRoll > 0.05 && diceRoll <= 0.25)
                {
                    //give coveted premium item
                    Random rnd = new Random();
                    int r = rnd.Next(coveted.Length);
                    this.Monitor.Log($"Attempting to give player an item with the ID of {coveted[r]}.");
                    Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(coveted[r], 5, false, -1, 0));
                }
                if (diceRoll > 0.25 && diceRoll <= 0.45)
                {
                    //give rare premium item
                    Random rnd = new Random();
                    int r = rnd.Next(rare.Length);
                    this.Monitor.Log($"Attempting to give player an item with the ID of {rare[r]}.");
                    Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(rare[r], 10, false, -1, 0));
                }
                if (diceRoll > 0.45 && diceRoll <= 0.8)
                {
                    //give uncommon premium item
                    Random rnd = new Random();
                    int r = rnd.Next(uncommon.Length);
                    this.Monitor.Log($"Attempting to give player an item with the ID of {uncommon[r]}.");
                    Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(uncommon[r], 15, false, -1, 0));
                }
                if (diceRoll > 0.8 && diceRoll <= 1)
                {
                    //give common premium item
                    Random rnd = new Random();
                    int r = rnd.Next(common.Length);
                    this.Monitor.Log($"Attempting to give player an item with the ID of {common[r]}.");
                    Game1.player.addItemByMenuIfNecessary((Item)new StardewValley.Object(common[r], 25, false, -1, 0));
                }
                totalTokens -= 50;
                var savedData = this.Helper.ReadJsonFile<SavedData>($"data/{Constants.SaveFolderName}.json") ?? new SavedData();
                savedData.totalTokens = totalTokens;
            } else
            {
                Game1.addHUDMessage(new HUDMessage($"You do not have enough Tokens.", 3));
                Game1.addHUDMessage(new HUDMessage($"Your current Token balance is {totalTokens}.", 2));
                return;
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
