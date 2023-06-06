using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using PrairieKingPrizes.Framework;
using PrairieKingPrizes.Framework.Config;
using xTile.Layers;
using xTile.Tiles;
using System.Linq;
using Microsoft.Xna.Framework.Graphics;

namespace PrairieKingPrizes
{
    public class ModEntry : Mod
    {
        private int _coinsCollected;
        private int _totalTokens;
        private object _lastMinigame;
        private ModConfig _config;
        private Random _random;
        IDictionary<int, string> _objectData;

        public override void Entry(IModHelper helper)
        {
            _config = Helper.ReadConfig<ModConfig>();
            _random = new Random();

            //Events
            helper.Events.GameLoop.UpdateTicked += GameEvents_UpdateTick;
            helper.Events.GameLoop.SaveLoaded += AfterSaveLoaded;
            helper.Events.Input.ButtonPressed += CheckAction;
            helper.Events.GameLoop.Saved += UpdateSavedData;

            // asset placing
            helper.Events.Content.AssetReady += Asset_Ready;

            //Custom Commands
            helper.ConsoleCommands.Add("gettokens", "Retrieves the value of your current amount of tokens.", GetCoins);
            helper.ConsoleCommands.Add("orange", "Debug stuff, outputs a list of all items in the loot pool. Needs 3 special words in order to activate.", OrangeMonkeyEagle);
            
        }

        private void Asset_Ready(object sender, AssetReadyEventArgs e)
        {
            if (e.NameWithoutLocale.BaseName == "Data/ObjectInformation")
            {
                _objectData = Helper.GameContent.Load<Dictionary<int, string>>(e.NameWithoutLocale);
            }
        }

        private void GetCoins(string command, string[] args)
        {
            Monitor.Log($"You currently have {_totalTokens} coins.");
        }

        private void OrangeMonkeyEagle(string command, string[] args)
        {
            if (args.Length == 2)
            {
                if (args[0].ToLower() == "monkey" && args[1].ToLower() == "eagle")
                {
                    foreach (var lootbox in _config.Lootboxes)
                    {
                        Monitor.Log($"--- {lootbox.Name} - {lootbox.PrizeTiers.Length} Prize Tiers ---");
                        for (int i = 0; i < lootbox.PrizeTiers.Length; i++)
                        {
                            var prizeTier = lootbox.PrizeTiers[i];
                            Monitor.Log($"--- #{i} Prize Tier - Chance/Weight: {prizeTier.Chance} ---");
                            foreach (var item in prizeTier.Prizes)
                            {
                                if (_objectData.TryGetValue(item.ItemId, out string entry))
                                {
                                    string[] fields = entry.Split('/');
                                    string name = fields[0];
                                    Monitor.Log($"Prize ID {item.ItemId} gives you a {item.Quantity}x {name}.");
                                }
                            }
                            Monitor.Log($"--- End of Prize Tier #{i} ---");
                        }
                        Monitor.Log($"--- End of {lootbox.Name} ---");
                    }
                }
            }
            else
            {
                Monitor.Log("Invalid Arguments");
            }
        }

        private void AfterSaveLoaded(object sender, SaveLoadedEventArgs args)
        {
            var savedData = Helper.Data.ReadJsonFile<SavedData>($"data/{Constants.SaveFolderName}.json") ?? new SavedData();
            _totalTokens = savedData.TotalTokens;

            var tilesheetPath = Helper.ModContent.GetInternalAssetName("assets/z_extraSaloonTilesheet2.png");

            GameLocation location = Game1.getLocationFromName(_config.MachineLocation.LocationName);
            if (location != null)
            {
                TileSheet tilesheet = new TileSheet("z_extraSaloonTilesheet2", location.map, tilesheetPath.Name, new xTile.Dimensions.Size(48, 16), new xTile.Dimensions.Size(16, 16));

                location.map.AddTileSheet(tilesheet);
                location.map.LoadTileSheets(Game1.mapDisplayDevice);

                Layer layerBack = location.map.GetLayer("Back");
                Layer layerFront = location.map.GetLayer("Front");
                Layer layerBuildings = location.map.GetLayer("Buildings");

                location.removeTile(_config.MachineLocation.X, _config.MachineLocation.Y + 1, "Back");
                TileSheet customTileSheet = location.map.GetTileSheet("z_extraSaloonTilesheet2");
                layerFront.Tiles[_config.MachineLocation.X, _config.MachineLocation.Y - 1] = new StaticTile(layerFront, customTileSheet, BlendMode.Alpha, 0);
                layerBuildings.Tiles[_config.MachineLocation.X, _config.MachineLocation.Y] = new StaticTile(layerBuildings, customTileSheet, BlendMode.Alpha, 1);
                layerBack.Tiles[_config.MachineLocation.X, _config.MachineLocation.Y + 1] = new StaticTile(layerBack, customTileSheet, BlendMode.Alpha, 2);

                location.setTileProperty(_config.MachineLocation.X, _config.MachineLocation.Y, "Buildings", "Action", "TokenMachine");
            }
        }

        private void UpdateSavedData(object sender, SavedEventArgs args)
        {
            var savedData = Helper.Data.ReadJsonFile<SavedData>($"data/{Constants.SaveFolderName}.json") ?? new SavedData();
            savedData.TotalTokens = _totalTokens;
            Helper.Data.WriteJsonFile($"data/{Constants.SaveFolderName}.json", savedData);
        }

        private void CheckAction(object sender, ButtonPressedEventArgs e)
        {
            if (Context.IsPlayerFree && e.Button.IsActionButton())
            {
                Vector2 grabTile = new Vector2(Game1.getOldMouseX() + Game1.viewport.X, Game1.getOldMouseY() + Game1.viewport.Y) / Game1.tileSize;
                if (!Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
                {
                    grabTile = Game1.player.GetGrabTile();
                }
                Tile tile = Game1.currentLocation.map.GetLayer("Buildings").PickTile(new xTile.Dimensions.Location((int)grabTile.X * Game1.tileSize, (int)grabTile.Y * Game1.tileSize), Game1.viewport.Size);
                xTile.ObjectModel.PropertyValue propertyValue = null;
                tile?.Properties.TryGetValue("Action", out propertyValue);
                if (propertyValue != null)
                {
                    if (propertyValue == "TokenMachine")
                    {
                        List<Response> responses = new List<Response>();
                        responses.AddRange(
                            _config.Lootboxes
                            .Select(x => new Response(x.Key, $"{x.Name} ({x.Cost} Tokens)"))
                        );
                        responses.Add(new Response("Cancel", "Cancel"));

                        Game1.player.currentLocation.createQuestionDialogue($"Would you like to spend your tokens to receive a random item? You currently have {_totalTokens} tokens.", responses.ToArray(), AfterQuestion);
                    }
                }
            }
        }

        private void AfterQuestion(Farmer who, string whichAnswer)
        {
            var lootbox = _config.Lootboxes.FirstOrDefault(x => x.Key == whichAnswer);

            if (lootbox == null)
                return;

            if (lootbox.Cost > _totalTokens)
            {
                Game1.addHUDMessage(new HUDMessage("You do not have enough Tokens.", 3));
                Game1.addHUDMessage(new HUDMessage($"Your current Token balance is {_totalTokens}.", 2));
                Game1.playSound("cancel");
            }

            GivePlayerItem(lootbox);
        }

        int _coinStorage;
        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (_config.RequireGameCompletion && !Game1.player.mailReceived.Contains("Beat_PK")) return;
            if (Game1.currentMinigame != null && "AbigailGame".Equals(Game1.currentMinigame.GetType().Name))
            {
                Type minigameType = Game1.currentMinigame.GetType();
                _coinsCollected = Convert.ToInt32(minigameType.GetField("coins").GetValue(Game1.currentMinigame));
                if (_config.AlternateCoinMethod) {
                    if (_coinsCollected > _coinStorage) _coinStorage = _coinsCollected;
                } else {
                    _coinStorage = _coinsCollected;
                }
                _lastMinigame = Game1.currentMinigame.GetType().Name;
            }

            if (Game1.currentMinigame == null && "AbigailGame".Equals(_lastMinigame))
            {
                _totalTokens += _coinStorage;
                _coinsCollected = 0;
                _coinStorage = 0;
            }
        }

        private void GivePlayerItem(Lootbox lootbox)
        {
            var prizeTier = _random.PickPrizeTier(lootbox.PrizeTiers);
            if (prizeTier == null)
                return;

            var prize = prizeTier.Prizes[_random.Next(prizeTier.Prizes.Length)];

            if (prize == null)
                return;

            _totalTokens -= lootbox.Cost;
            Game1.addHUDMessage(new HUDMessage($"Your current Token balance is now {_totalTokens}.", 2));
            Game1.playSound("purchase");

            Game1.player.addItemByMenuIfNecessary(new StardewValley.Object(prize.ItemId, prize.Quantity, quality: prize.Quality ?? 0));
        }

        //Secrets - Basic Item
        //    Common - 40% | Quantity: 5
        //    Uncommon - 30% | Quantity: 3
        //    Rare - 20% | Quantity: 2
        //    Coveted - 9.9% | Quantity: 1
        //    Legendary - 0.1% | Quantity: 1
        //Secrets - Premium Item
        //    Common - 20% | Quantity: 25
        //    Uncommon - 25% | Quantity: 15
        //    Rare - 30% | Quantity: 10
        //    Coveted - 24% | Quantity: 5
        //    Legendary - 1% | Quantity: 2
    }
}