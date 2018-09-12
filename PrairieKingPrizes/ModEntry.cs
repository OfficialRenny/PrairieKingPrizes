using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using xTile.Layers;
using xTile.Tiles;

namespace PrairieKingPrizes
{
    public class ModEntry : Mod
    {
        private int coinsCollected;
        private int totalTokens;
        private object LastMinigame;
        private NPC TokenNPC = null;
        private bool isNPCLoaded = false;

        //[DeluxeGrabber] Map: Saloon
        //[DeluxeGrabber] Tile: {X:36 Y:17}
        //It's a Surprise Tool That Will Help Us Later

        public override void Entry(IModHelper helper)
        {
            GameEvents.UpdateTick += GameEvents_UpdateTick;
            SaveEvents.AfterLoad += AfterSaveLoaded;
            helper.ConsoleCommands.Add("gettokens", "Retrieves the value of your current amount of tokens.", this.GetCoins);
            helper.ConsoleCommands.Add("settokens", "Sets tokens", this.SetCoins);
            //SaveEvents.BeforeSave += DeleteNPC_BeforeSave;
            //SaveEvents.AfterSave += AddNPC_AfterSave;
            InputEvents.ButtonPressed += CheckAction;
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

        //public void Edit<T>(IAssetData asset)
        // {
        //    if (asset.AssetNameEquals("Data/NPCDispositions"))
        //    {
        //         asset.AsDictionary<string, string>().Data["TokenMachine"] = "adult/shy/outgoing/negative/male/non-datable/null/Town/fall 9/null/Saloon -1000 -1000/Token Machine";
        //     }
        //
        //    if (asset.AssetNameEquals("Data/NPCGiftTastes"))
        //    {
        //        IDictionary<string, string> NPCGiftTastes = asset.AsDictionary<string, string>().Data;
        //        NPCGiftTastes["TokenMachine"] = "ERROR: I DO NOT ACCEPT GIFTS//ERROR: I DO NOT ACCEPT GIFTS//ERROR: I DO NOT ACCEPT GIFTS//ERROR: I DO NOT ACCEPT GIFTS/-2 -4 -5 -6 -7 -8 -9 -12 -14 -15 -16 -17 -18 -19 -20 -21 -22 -23 -24 -25 -26 -27 -28 -29 -74 -75 -79 -80 -81 -95 -96 -98 -99/ERROR: I DO NOT ACCEPT GIFTS//";
        //    }

        //    if (asset.AssetNameEquals("Characters/Dialogue/rainy"))
        //   {
        //      IDictionary<string, string> rainy = asset.AsDictionary<string, string>().Data;
        //      rainy["TokenMachine"] = "I HOPE I DO NOT GET WET IN THIS RAIN. OH WAIT, I CANNOT MOVE. HA. HA.";
        //   }
        //  }

        // public bool CanLoad<T>(IAssetInfo asset)
        // {
        //       if (asset.AssetNameEquals("Characters/Dialogue/TokenMachine"))
        //       {
        //           return true;
        //       }

        //        if (asset.AssetNameEquals("Characters/TokenMachine"))
        //        {
        //            return true;
        //        }

        //        if (asset.AssetNameEquals("Portraits/TokenMachine"))
        //        {
        //            return true;
        //        }

        //        if (asset.AssetNameEquals("Characters/Schedules/TokenMachine"))
        //        {
        //            return true;
        //        }

        //   return false;
        //}

        //public T Load<T>(IAssetInfo asset)
        //{
        //        if (asset.AssetNameEquals("Characters/Dialogue/TokenMachine"))
        //        {
        //            return Helper.Content.Load<T>("assets/tokenDialogue.xnb", ContentSource.ModFolder);
        //        }

        //        if (asset.AssetNameEquals("Characters/Schedules/TokenMachine"))
        //        {
        //            return Helper.Content.Load<T>("assets/tokenSchedule.xnb", ContentSource.ModFolder);
        //        }

        //        if (asset.AssetNameEquals("Characters/TokenMachine"))
        //        {
        //            return Helper.Content.Load<T>("assets/tokenMachine.png", ContentSource.ModFolder);
        //        }

        //        if (asset.AssetNameEquals("Portraits/TokenMachine"))
        //        {
        //            return Helper.Content.Load<T>("assets/portrait.png", ContentSource.ModFolder);
        //        }

        //  throw new InvalidOperationException($"Unexpected asset '{asset.AssetName}'.");
        //}

        private void GetCoins(string command, string[] args)
        {
            this.Monitor.Log($"You currently have {totalTokens} coins.");
        }

        public void SetCoins(string command, string[] args)
        {
            int tokensToAdd = 0;
            if( args.Length > 1)
            {
                this.Monitor.Log("Please input only a single number as an argument.");
                return;
            }
            if (Int32.TryParse(args[0], out tokensToAdd))
            {
                totalTokens += tokensToAdd;
                this.Monitor.Log($"{tokensToAdd} tokens added.");
                tokensToAdd = 0;
            } else { this.Monitor.Log("NaN!"); }
        }

        private void AfterSaveLoaded(object sender, EventArgs args)
        {
            var savedData = this.Helper.ReadJsonFile<SavedData>($"data/{Constants.SaveFolderName}.json") ?? new SavedData();
            totalTokens = savedData.totalTokens;

            string tilesheetPath = this.Helper.Content.GetActualAssetKey("assets/z_extraSaloonTilesheet2.png", ContentSource.ModFolder);

            GameLocation location = Game1.getLocationFromName("Saloon");
            TileSheet tilesheet = new TileSheet(
               id: "z_extraSaloonTilesheet2",
               map: location.map,
               imageSource: tilesheetPath,
               sheetSize: new xTile.Dimensions.Size(48, 16),
               tileSize: new xTile.Dimensions.Size(16, 16)
            );
            location.map.AddTileSheet(tilesheet);
            location.map.LoadTileSheets(Game1.mapDisplayDevice);

            Layer layerBack = location.map.GetLayer("Back");
            Layer layerFront = location.map.GetLayer("Front");
            Layer layerBuildings = location.map.GetLayer("Buildings");

            location.removeTile(34, 18, "Back");
            TileSheet customTileSheet = location.map.GetTileSheet("z_extraSaloonTilesheet2");
            layerFront.Tiles[34, 16] = new StaticTile(layerFront, customTileSheet, BlendMode.Alpha, 0);
            layerBuildings.Tiles[34, 17] = new StaticTile(layerBuildings, customTileSheet, BlendMode.Alpha, 1);
            layerBack.Tiles[34, 18] = new StaticTile(layerBack, customTileSheet, BlendMode.Alpha, 2);

            location.setTileProperty(34, 17, "Buildings", "Action", "TokenMachine");

            Texture2D portrait = Helper.Content.Load<Texture2D>("assets/portrait.png", ContentSource.ModFolder);

            //if (this.TokenNPC == null)
            //{
            //    this.TokenNPC = new NPC(null, new Vector2(-1000f, -1000f), "Saloon", 3, "TokenMachine", false, null, Helper.Content.Load<Texture2D>("assets/portrait.png", ContentSource.ModFolder));
            //    Game1.locations[Game1.locations.Count - 1].addCharacter(this.TokenNPC);
            //    isNPCLoaded = true;
            //}

            //int basicItemID = 444;
            //int premiumItemID = 445;
            //int cancelID = 446;

            //foreach (int num in game1.player.dialoguequestionsanswered)
            //{
            //    if (num == basicitemid)
            //    {
            //        game1.player.dialoguequestionsanswered.remove(basicitemid);
            //        this.monitor.log($"{basicitemid}/basicitemid was a valid dialogue answer and has been removed.");
            //    }
            //    if (num == premiumitemid)
            //    {
            //        game1.player.dialoguequestionsanswered.remove(premiumitemid);
            //        this.monitor.log($"{premiumitemid}/premiumitemid was a valid dialogue answer and has been removed.");
            //    }
            //    if (num == cancelid)
            //    {
            //        game1.player.dialoguequestionsanswered.remove(cancelid);
            //        this.monitor.log($"{cancelid}/cancelid was a valid dialogue answer and has been removed.");
            //    }
            //}
        }

        private void CheckAction(object sender, EventArgsInput e)
        {
            if (Context.IsPlayerFree && e.IsActionButton)
            {
                Vector2 grabTile = new Vector2((float)(Game1.getOldMouseX() + Game1.viewport.X), (float)(Game1.getOldMouseY() + Game1.viewport.Y)) / (float)Game1.tileSize;
                if (!Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
                {
                    grabTile = Game1.player.GetGrabTile();
                }
                xTile.Tiles.Tile tile = Game1.currentLocation.map.GetLayer("Buildings").PickTile(new xTile.Dimensions.Location((int)grabTile.X * Game1.tileSize, (int)grabTile.Y * Game1.tileSize), Game1.viewport.Size);
                xTile.ObjectModel.PropertyValue propertyValue = null;
                if (tile != null)
                {
                    tile.Properties.TryGetValue("Action", out propertyValue);
                }
                if (propertyValue != null)
                {
                    if (propertyValue == "TokenMachine")
                    {
                        //this.Monitor.Log("Player has clicked on the Token Machine");
                        //NPC TokenMachine = Game1.getCharacterFromName("TokenMachine");
                        //Game1.drawDialogue(TokenMachine);

                        Response basic = new Response("Basic", "Basic Tier (10 Tokens)");
                        Response premium = new Response("Premium", "Premium Tier (50 Tokens)");
                        Response cancel = new Response("Cancel", "Cancel");
                        Response[] answers = { basic, premium, cancel };

                        
                        Game1.player.currentLocation.createQuestionDialogue("Would you like to spend your tokens to receive a random item?", answers, AfterQuestion, null);
                        
                    }
                }
            }
        }

        private void AfterQuestion(Farmer who, string whichAnswer)
        {
            this.Monitor.Log("Successfully called the AfterQuestion method");
            if (whichAnswer == "Basic")
            {
                givePlayerBasicItem();
            } else if (whichAnswer == "Premium")
            {
                givePlayerPremiumItem();
            } else { return;  }
        }

        //private void DeleteNPC_BeforeSave(object sender, EventArgs args)
        //{
        //    if (!Context.IsWorldReady) { return; }

        //    //if (isNPCLoaded == true)
        //    //{
        //    //    Game1.removeThisCharacterFromAllLocations(tokenNPC);
        //    //    isNPCLoaded = false;
        //    //    foreach (GameLocation location in Game1.locations)
        //    //    {
        //    //        NPC[] matches = location.characters.Where(p => p.Name == "TokenMachine").ToArray();
        //    //        if (matches.Any())
        //    //            Monitor.Log($"Found the NPC in {location.Name}.", LogLevel.Warn);

        //    //    }
        //    //}
        //    //else { return; }
        //}

        //private void AddNPC_AfterSave(object sender, EventArgs args)
        //{
            //Texture2D portrait = Helper.Content.Load<Texture2D>("assets/portrait.png", ContentSource.ModFolder);

            //if (isNPCLoaded == false)
            //{
            //    NPC tokenNPC = new NPC(null, new Vector2(34f, 17f), "Saloon", 3, "TokenMachine", false, null, portrait);
            //    isNPCLoaded = true;
            //}
            //else { return; }
        //}

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

            //int basicItemID = 444;
            //int premiumItemID = 445;
            //int cancelID = 446;

            //foreach (int num in Game1.player.dialogueQuestionsAnswered)
            //{
            //    if (num == basicItemID)
            //    {
            //        givePlayerBasicItem();
            //        Game1.player.dialogueQuestionsAnswered.Remove(basicItemID);
            //        this.Monitor.Log($"{basicItemID}/basicItemID was a valid dialogue answer and has been removed.");
            //    }
            //    if (num == premiumItemID)
            //    {
            //        givePlayerPremiumItem();
            //        Game1.player.dialogueQuestionsAnswered.Remove(premiumItemID);
            //        this.Monitor.Log($"{premiumItemID}/premiumItemID was a valid dialogue answer and has been removed.");
            //    }
            //    if (num == cancelID)
            //    {
            //        Game1.player.dialogueQuestionsAnswered.Remove(cancelID);
            //        this.Monitor.Log($"{cancelID}/cancelID was a valid dialogue answer and has been removed.");
            //        return;
            //    }
            //}
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
                totalTokens -= 10;
                var savedData = this.Helper.ReadJsonFile<SavedData>($"data/{Constants.SaveFolderName}.json") ?? new SavedData();
                savedData.totalTokens = totalTokens;


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

            Random random = new Random();
            double diceRoll = random.NextDouble();

            totalTokens -= 50;
            var savedData = this.Helper.ReadJsonFile<SavedData>($"data/{Constants.SaveFolderName}.json") ?? new SavedData();
            savedData.totalTokens = totalTokens;
            Game1.addHUDMessage(new HUDMessage($"Your current Token balance is now {totalTokens}.", 2));

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
            }
            else
            {
                Game1.addHUDMessage(new HUDMessage($"Your current Token balance is {totalTokens}.", 2));
                Game1.addHUDMessage(new HUDMessage($"You do not have enough Tokens.", 3));
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