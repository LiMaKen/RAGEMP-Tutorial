using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using GTANetworkAPI;
using MySql.Data.MySqlClient;
using Tutorial.Controllers;

namespace Tutorial
{
    class Commands : Script
    {        
        [Command("taixiuon", "/taixiuon để bật tài xỉu")]
        public void Cmd_TaiXiuOn(Player player)
        {
            player.TriggerEvent("Client:OpenTaiXiu");   
        }
        [Command("veh", "/veh để tạo ra một chiếc xe")]
        public void cmd_veh(Player player, string vehname, int color1, int color2)
        {
            Accounts account = player.GetData<Accounts>(Accounts.Account_Key);
            if (!account.IstSpielerAdmin((int)Accounts.AdminRanks.Supporter))
            {
                player.SendChatMessage("~r~Cấp quản trị viên của bạn quá thấp!");
                return;
            }
            uint vehash = NAPI.Util.GetHashKey(vehname);
            if (vehash <= 0)
            {
                player.SendChatMessage("~r~Xe không hợp lệ!");
                return;
            }
            Vehicle veh = NAPI.Vehicle.CreateVehicle(vehash, player.Position, player.Heading, color1, color2);
            veh.NumberPlate = "Tutorial";
            veh.Locked = true;
            veh.EngineStatus = true;
            player.SetIntoVehicle(veh, (int)VehicleSeat.Driver);
            Utils.sendNotification(player, "Xe đã tạo thành công!", "fas fa-car");
        }

        [Command("vehspawner", "/vehspawner để mở menu xe!")]
        public void cmd_vehspawner(Player player)
        {
            Accounts account = player.GetData<Accounts>(Accounts.Account_Key);
            if (!account.IstSpielerAdmin((int)Accounts.AdminRanks.Supporter))
            {
                player.SendChatMessage("~r~Cấp quản trị viên của bạn quá thấp!");
                return;
            }
            NAPI.ClientEvent.TriggerClientEvent(player, "vSpawner");
        }

        [Command("playmusic", "/playmusic để chơi nhạc!")]
        public void cmd_playmusic(Player player, string music)
        {
            Accounts account = player.GetData<Accounts>(Accounts.Account_Key);
            if (!account.IstSpielerAdmin((int)Accounts.AdminRanks.Supporter))
            {
                player.SendChatMessage("~r~Cấp quản trị viên của bạn quá thấp!");
                return;
            }
            NAPI.ClientEvent.TriggerClientEvent(player, "PlayMusic", music);
        }

        [Command("freeze", "/freeze đóng băng")]
        public void CMD_FreezePlayer(Player player, Player target, bool freezestatus)
        {
            Accounts account = player.GetData<Accounts>(Accounts.Account_Key);
            if (!account.IstSpielerAdmin((int)Accounts.AdminRanks.Supporter))
            {
                player.SendChatMessage("~r~Cấp quản trị viên của bạn quá thấp!");
                return;
            }
            NAPI.ClientEvent.TriggerClientEvent(target, "PlayerFreeze", freezestatus);
            string freezeText = (freezestatus) ? "freeze" : "unfreeze";
            target.SendChatMessage($"Bạn đã bị {freezeText}!");
        }

        [Command("cayo", "/cayo để dịch chuyển bạn đến Cayo Perico")]
        public void CMD_cayo(Player player)
        {
            Accounts account = player.GetData<Accounts>(Accounts.Account_Key);
            if (!account.IstSpielerAdmin((int)Accounts.AdminRanks.Supporter))
            {
                player.SendChatMessage("~r~Cấp quản trị viên của bạn quá thấp!");
                return;
            }
            player.Position = new Vector3(4840.571, -5174.425, 2.0);
            player.SendChatMessage("Bạn đã dịch chuyển đến Cayo Perico!");
        }

        [Command("einreise", "/einreise cho phép người chơi vào quốc gia")]
        public void CMD_einreise(Player player, string playertarget)
        {
            Accounts account = player.GetData<Accounts>(Accounts.Account_Key);
            if (!account.IstSpielerAdmin((int)Accounts.AdminRanks.Moderator))
            {
                player.SendChatMessage("~r~Cấp quản trị viên của bạn quá thấp!");
                return;
            }
            Player target = Utils.GetPlayerByNameOrID(playertarget);
            if (target == null)
            {
                player.SendChatMessage("~r~Trình phát không hợp lệ");
                return;
            }
            Accounts accountTarget = player.GetData<Accounts>(Accounts.Account_Key);
            if (accountTarget.Einreise == 0)
            {
                accountTarget.Einreise = 1;
                player.Position = new Vector3(-420.678, 1182.97, 325.642);
                player.SendChatMessage("~g~Nhập thành công!");
                Utils.sendNotification(player, "Nhập thành công!", "fas fa-user");
                target.SendChatMessage("~g~Nhập thành công!");
                Utils.sendNotification(target, "Đăng nhập thành công!", "fas fa-user");
                Datenbank.AccountSpeichern(target);
            }
            else
            {
                player.SendChatMessage("~r~ Người chơi không còn phải vào nước nữa!");
            }
        }

        [Command("telexyz", "/telexyz [X] [Y] [Z]")]
        public void CMD_telexyz(Player player, float x, float y, float z)
        {
            Accounts account = player.GetData<Accounts>(Accounts.Account_Key);
            if (!account.IstSpielerAdmin((int)Accounts.AdminRanks.Administrator))
            {
                player.SendChatMessage("~r~Cấp quản trị viên của bạn quá thấp!");
                return;
            }
            Vector3 position = new Vector3(x, y, z + 0.2);
            player.Position = position;
            player.SendChatMessage("Bạn đã dịch chuyển thành công");
            return;
        }

        [Command("testcloth", "Lệnh: /testcloths [Thành phần-ID] [Có thể vẽ] [Màu*]")]
        public void CMD_testcloths(Player player, int componentid, int drawable, int color = 0)
        {
            NAPI.Task.Run(() =>
            {
                Accounts account = player.GetData<Accounts>(Accounts.Account_Key);
                if (!account.IstSpielerAdmin((int)Accounts.AdminRanks.Administrator))
                {
                    player.SendChatMessage("~r~Cấp quản trị của bạn quá thấp!");
                    return;
                }
                if (componentid == 0)
                {
                    NAPI.Player.SetPlayerAccessory(player, 0, drawable, color);
                }
                else
                {
                    NAPI.Player.SetPlayerClothes(player, componentid, drawable, color);
                }
                player.SendChatMessage("Bộ quần áo thử nghiệm!");
                return;
            });
        }

        [Command("me", "/me [Tin nhắn]", GreedyArg = true)]
        public void CMD_me(Player player, string nachricht)
        {
            if (!Accounts.IstSpielerEingeloggt(player)) return;
            if (nachricht.Length > 0)
            {
                Utils.SendRadiusMessage("!{#EE82EE}* " + player.Name + " " + nachricht, 8, player);
            }
        }

        [Command("save", "/save [Vị trí]", GreedyArg = true)]
        public void CMD_save(Player player, string position)
        {
            if (!Accounts.IstSpielerEingeloggt(player)) return;

            string status = (player.IsInVehicle) ? "Trong xe" : "Bằng chân";
            Vector3 pos = (player.IsInVehicle) ? player.Vehicle.Position : player.Position;
            Vector3 rot = (player.IsInVehicle) ? player.Vehicle.Rotation : player.Rotation;

            string message =
            $"{status} -> {position}: {pos.X.ToString(new CultureInfo("en-US")):N3}, {pos.Y.ToString(new CultureInfo("en-US")):N3}, {pos.Z.ToString(new CultureInfo("en-US")):N3}, {rot.X.ToString(new CultureInfo("en-US")):N3}, {rot.Y.ToString(new CultureInfo("en-US")):N3}, {rot.Z.ToString(new CultureInfo("en-US")):N3}";

            player.SendChatMessage(message);

            using (StreamWriter file = new StreamWriter(@"./serverdata/savedpositions.txt", true))
            {
                file.WriteLine(message);
            }
        }

        [Command("createhouse", "/createhouse để tạo ra một ngôi nhà!")]
        public void CMD_createhouse(Player player, int preis, int ipl)
        {
            Accounts account = player.GetData<Accounts>(Accounts.Account_Key);
            if (!account.IstSpielerAdmin((int)Accounts.AdminRanks.Administrator))
            {
                player.SendChatMessage("~r~Cấp độ quản trị viên của bạn quá thấp!");
                return;
            }
            HausModel house = HausController.holeHausInReichweite(player);
            if (house != null)
            {
                player.SendChatMessage("~r~ Ở đây đã có nhà rồi!");
                return;
            }
            string hausLabel = string.Empty;
            house = new HausModel();
            house.ipl = HausInterior.Interior_Liste[ipl].ipl;
            house.position = player.Position;
            house.preis = preis;
            house.besitzer = "Không có";
            house.status = false;
            house.abgeschlossen = false;

            Task.Factory.StartNew(() =>
            {
                NAPI.Task.Run(() =>
                {
                    house.id = Datenbank.ErstelleHaus(house);
                    house.hausLabel = NAPI.TextLabel.CreateTextLabel($"Căn nhà này được rao bán với giá {preis}$, hãy sử dụng /buyhouse để mua nó!", new Vector3(house.position.X, house.position.Y, house.position.Z + 0.8), 5.0f, 0.75f, 4, new Color(255, 255, 255));
                    if (house.status == false)
                    {
                        house.hausMarker = NAPI.Marker.CreateMarker(1, new Vector3(house.position.X, house.position.Y, house.position.Z - 1.1), house.position, new Vector3(), 1.0f, new Color(38, 230, 0), false);
                        house.hausBlip = NAPI.Blip.CreateBlip(40, house.position, 1.0f, 2);
                    }
                    else
                    {
                        house.hausMarker = NAPI.Marker.CreateMarker(1, new Vector3(house.position.X, house.position.Y, house.position.Z - 1.1), house.position, new Vector3(), 1.0f, new Color(255, 255, 255), false);
                        house.hausBlip = NAPI.Blip.CreateBlip(40, house.position, 1.0f, 1);
                    }
                    NAPI.Blip.SetBlipName(house.hausBlip, "số nhà: " + house.id);
                    NAPI.Blip.SetBlipShortRange(house.hausBlip, true);

                    HausController.hausListe.Add(house);

                    player.SendChatMessage("~g~ Ngôi nhà đã được xây dựng thành công!");
                });

            });
        }

        [Command("enter", "/enter để vào một ngôi nhà!")]
        public void CMD_enter(Player player)
        {
            foreach (HausModel house in HausController.hausListe)
            {
                if (player.Position.DistanceTo(house.position) <= 2.5f)
                {
                    if (!HausController.HatSpielerSchluessel(player, house) && house.abgeschlossen)
                    {
                        player.SendChatMessage("~r~Cửa bị khóa!");
                    }
                    else
                    {
                        player.Position = HausInterior.GetHausAusgang(house.ipl);
                        player.SetData("Haus_ID", house.id);
                        player.Dimension = (uint)house.id;
                        player.SendNotification("~g~Vào nhà !");
                    }
                }
            }
        }
       
        [Command("exit", "/exit rời khỏi một ngôi nhà!")]
        public void CMD_exit(Player player)
        {
            foreach (HausModel house in HausController.hausListe)
            {
                if (player.Position.DistanceTo(HausInterior.GetHausAusgang(house.ipl)) <= 2.5f)
                {
                    if (!HausController.HatSpielerSchluessel(player, house) && house.abgeschlossen)
                    {
                        player.SendChatMessage("~r~Cửa bị khóa!");
                    }
                    else
                    {
                        player.Position = house.position;
                        player.SetData("Haus_ID", -1);
                        player.Dimension = 0;
                        player.SendNotification("~r~Rời khỏi nhà!");
                    }
                }
            }
        }

        [Command("lock", "/lock để mở/đóng một ngôi nhà!")]
        public void CMD_lock(Player player)
        {
            if (!Accounts.IstSpielerEingeloggt(player)) return;
            HausModel house = null;
            house = HausController.holeHausInReichweite(player);
            if (house != null)
            {
                house = HausController.holeHausInReichweite(player);
            }
            else
            {
                house = HausController.HoleHausMitID(player.GetData<int>("Haus_ID"));
            }
            if (house != null)
            {
                if (HausController.HatSpielerSchluessel(player, house))
                {
                    if (house.abgeschlossen == false)
                    {
                        house.abgeschlossen = true;
                        player.SendNotification("~r~nhà đã hoàn thiện");
                    }
                    else
                    {
                        house.abgeschlossen = false;
                        player.SendNotification("~g~Nhà mở");
                    }
                }
            }
            else
            {
                player.SendChatMessage("~r~Bạn không đứng ở gần một ngôi nhà!");
            }
        }
        [Command("buyhouse", "/buyhouse um ein Haus zu kaufen!")]
        public void CMD_buyhouse(Player player)
        {
            if (!Accounts.IstSpielerEingeloggt(player)) return;
            Accounts account = player.GetData<Accounts>(Accounts.Account_Key);
            HausModel house = HausController.holeHausInReichweite(player);
            if (house == null || house.besitzer != "Keiner")
            {
                player.SendChatMessage("~r~ Bạn không đưng ở gần một ngôi nhà trống!");
                return;
            }
            if (account.Geld < house.preis)
            {
                player.SendChatMessage("~r~ Bạn không có đủ tiền bên người!");
                return;
            }
            account.Geld -= house.preis;
            house.status = true;
            house.besitzer = player.Name;

            NAPI.Entity.DeleteEntity(house.hausMarker);
            NAPI.Entity.DeleteEntity(house.hausBlip);
            NAPI.Entity.DeleteEntity(house.hausLabel);

            house.hausLabel = NAPI.TextLabel.CreateTextLabel($"Ngôi nhà này thuộc về {house.besitzer}, sử dụng /enter để đi vào nhà!", new Vector3(house.position.X, house.position.Y, house.position.Z + 0.8), 5.0f, 0.75f, 4, new Color(255, 255, 255));
            if (house.status == false)
            {
                house.hausMarker = NAPI.Marker.CreateMarker(1, new Vector3(house.position.X, house.position.Y, house.position.Z - 1.1), house.position, new Vector3(), 1.0f, new Color(38, 230, 0), false);
                house.hausBlip = NAPI.Blip.CreateBlip(40, house.position, 1.0f, 2);
            }
            else
            {
                house.hausMarker = NAPI.Marker.CreateMarker(1, new Vector3(house.position.X, house.position.Y, house.position.Z - 1.1), house.position, new Vector3(), 1.0f, new Color(255, 255, 255), false);
                house.hausBlip = NAPI.Blip.CreateBlip(40, house.position, 1.0f, 1);
            }
            NAPI.Blip.SetBlipName(house.hausBlip, "Hausnummer: " + house.id);
            NAPI.Blip.SetBlipShortRange(house.hausBlip, true);

            player.SendChatMessage("~g~Mua nhà thành công!");

            Task.Factory.StartNew(() =>
            {
                Datenbank.HausSpeichern(house);
            });
        }

        [Command("sellhouse", "/sellhouse để bán nhà!")]
        public void CMD_sellhouse(Player player)
        {
            if (!Accounts.IstSpielerEingeloggt(player)) return;
            Accounts account = player.GetData<Accounts>(Accounts.Account_Key);
            HausModel house = HausController.holeHausInReichweite(player);
            if (house == null)
            {
                player.SendChatMessage("~r~Bạn không ở gần một ngôi nhà!");
                return;
            }
            if (house.besitzer != player.Name)
            {
                player.SendChatMessage("~r~Ngôi nhà này không thuộc về bạn!");
                return;
            }
            account.Geld += house.preis / 2;
            house.status = false;
            house.abgeschlossen = false;
            house.besitzer = "Keiner";

            NAPI.Entity.DeleteEntity(house.hausMarker);
            NAPI.Entity.DeleteEntity(house.hausBlip);
            NAPI.Entity.DeleteEntity(house.hausLabel);

            house.hausLabel = NAPI.TextLabel.CreateTextLabel($"Ngôi nhà này có giá {house.preis}$, sử dụng /buyhouse để mua nhà!", new Vector3(house.position.X, house.position.Y, house.position.Z + 0.8), 5.0f, 0.75f, 4, new Color(255, 255, 255));
            if (house.status == false)
            {
                house.hausMarker = NAPI.Marker.CreateMarker(1, new Vector3(house.position.X, house.position.Y, house.position.Z - 1.1), house.position, new Vector3(), 1.0f, new Color(38, 230, 0), false);
                house.hausBlip = NAPI.Blip.CreateBlip(40, house.position, 1.0f, 2);
            }
            else
            {
                house.hausMarker = NAPI.Marker.CreateMarker(1, new Vector3(house.position.X, house.position.Y, house.position.Z - 1.1), house.position, new Vector3(), 1.0f, new Color(255, 255, 255), false);
                house.hausBlip = NAPI.Blip.CreateBlip(40, house.position, 1.0f, 1);
            }
            NAPI.Blip.SetBlipName(house.hausBlip, "Hausnummer: " + house.id);
            NAPI.Blip.SetBlipShortRange(house.hausBlip, true);

            player.SendChatMessage("~g~Bạn đã bán nhà thành công!");

            Task.Factory.StartNew(() =>
            {
                Datenbank.HausSpeichern(house);
            });
        }

        [Command("lockpicking", "/lockpicking để mở phương tiện!")]
        public void CMD_lockpicking(Player player)
        {
            Vehicle vehicle = Utils.GetClosestVehicle(player);
            if (vehicle != null)
            {
                if (vehicle.Locked == true)
                {
                    NAPI.ClientEvent.TriggerClientEvent(player, "showLockpicking");
                }
                else
                {
                    player.SendChatMessage("~r~Phương tiện đã mở rồi!");
                }
            }
            else
            {
                player.SendChatMessage("~r~Bạn không ở gần một phương tiện!");
            }
        }

        //Nicht in der Tutorialreihe erstellt
        [Command("setskin", "/setskin tạo skin!")]
        public void CMD_setskin(Player player, string model)
        {
            Accounts account = player.GetData<Accounts>(Accounts.Account_Key);
            if (!account.IstSpielerAdmin((int)Accounts.AdminRanks.Administrator))
            {
                player.SendChatMessage("~r~Quyền quản trị viên không đủ!");
                return;
            }
            uint skinhash = NAPI.Util.GetHashKey(model);
            NAPI.Player.SetPlayerSkin(player, skinhash);
            player.SendChatMessage("~g~Đặt skin thành công!");
        }

        [Command("loadipl", "/loadipl để load một IPL!")]
        public void CMD_loadipl(Player player, string ipl)
        {
            Accounts account = player.GetData<Accounts>(Accounts.Account_Key);
            if (!account.IstSpielerAdmin((int)Accounts.AdminRanks.Administrator))
            {
                player.SendChatMessage("~r~Quyền quản trị viên không đủ!");
                return;
            }
            player.SendChatMessage("~g~Load IPL thành công!");
        }

        [Command("fraktionsinfo", "/fraktionsinfo để mô tả một tổ chức")]
        public void CMD_fraktionsinfo(Player player)
        {
            Accounts account = player.GetData<Accounts>(Accounts.Account_Key);
            player.SendChatMessage($"Bạn ở trong tổ chức {account.HoleFraktionsName()} và có chức vụ là {account.HoleRangName()}!");
        }

        [Command("makeleader", "/makeleader biến một người thành người đứng đầu tổ chức!")]
        public void CMD_makeleader(Player player, String playertarget, int frak)
        {
            Accounts account = player.GetData<Accounts>(Accounts.Account_Key);
            if (!account.IstSpielerAdmin((int)Accounts.AdminRanks.Administrator))
            {
                player.SendChatMessage("~r~Quyền quản trị không đủ!");
                return;
            }
            Player target = Utils.GetPlayerByNameOrID(playertarget);
            Accounts accounttarget = target.GetData<Accounts>(Accounts.Account_Key);
            if (accounttarget != null && frak < 0 || frak > Accounts.FraktionsDaten.Length)
            {
                player.SendChatMessage("~r~Tổ chức không hợp lệ!");
                return;
            }
            accounttarget.Fraktion = frak;
            accounttarget.Rang = 10;
            player.SendChatMessage($"Bạn đã để {target.Name} làm người đứng đầu {Accounts.FraktionsDaten[frak, 0]} !");
            target.SendChatMessage($"Bạn được {player.Name} để làm người đứng đầu {Accounts.FraktionsDaten[frak, 0]} !");
        }

        [Command("whitelist", "/whitelist đưa người chơi vào whitelist")]
        public void CMD_whitelist(Player player, ulong socialclubid)
        {
            bool found = false;
            Accounts account = player.GetData<Accounts>(Accounts.Account_Key);
            if (!account.IstSpielerAdmin((int)Accounts.AdminRanks.Administrator))
            {
                player.SendChatMessage("~r~Quyền quản trị viên không đủ!");
                return;
            }
            if (socialclubid < 10000)
            {
                player.SendChatMessage("~r~Socialclubid không hợp lệ!");
                return;
            }
            MySqlCommand command = Datenbank.Connection.CreateCommand();
            command.CommandText = "SELECT id from whitelist WHERE socialclubid=@socialclubid LIMIT 1";
            command.Parameters.AddWithValue("socialclubid", socialclubid);
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    found = true;
                }
            }
            if (found == true)
            {
                MySqlCommand command2 = Datenbank.Connection.CreateCommand();
                command2.CommandText = "DELETE FROM whitelist WHERE socialclubid=@socialclubid LIMIT 1";
                command2.Parameters.AddWithValue("socialclubid", socialclubid);
                command2.ExecuteNonQuery();
                player.SendChatMessage("~g~Whitelisteintrag entfernt!");
            }
            else
            {
                MySqlCommand command3 = Datenbank.Connection.CreateCommand();
                command3.CommandText = "INSERT INTO whitelist (socialclubid) VALUES (@socialclubid)";
                command3.Parameters.AddWithValue("socialclubid", socialclubid);
                command3.ExecuteNonQuery();
                player.SendChatMessage("~g~Thêm vào whitelist thành công!");
            }
        }

        [Command("invite", "/invite để mời người chơi vào tổ chức của bạn!")]
        public void CMD_invite(Player player, String playertarget)
        {
            Accounts account = player.GetData<Accounts>(Accounts.Account_Key);
            if (account.Fraktion == 0)
            {
                player.SendChatMessage("~r~Bạn không có trong tổ chức nào!");
                return;
            }
            if (account.Rang < 10)
            {
                player.SendChatMessage($"~r~Bạn không phải là {Accounts.FraktionsDaten[account.Fraktion, 10]}");
                return;
            }
            Player target = Utils.GetPlayerByNameOrID(playertarget);
            Accounts accounttarget = target.GetData<Accounts>(Accounts.Account_Key);
            accounttarget.Fraktion = account.Fraktion;
            accounttarget.Rang = 1;
            player.SendChatMessage($"Bạn đã mời {target.Name} vào tổ chức {Accounts.FraktionsDaten[account.Fraktion, 0]} !");
            target.SendChatMessage($"Bạn đã được {player.Name} mời vào tổ chức {Accounts.FraktionsDaten[account.Fraktion, 0]} !");
        }

        [Command("pistole", "/pistole tạo một khẩu súng lục")]
        public void CMD_pistole(Player player)
        {
            Accounts account = player.GetData<Accounts>(Accounts.Account_Key);
            if (account.Fraktion != 1)
            {
                player.SendChatMessage($"~r~Bạn không phải là thành viên của {Accounts.FraktionsDaten[1, 0]}!");
                return;
            }
            NAPI.Player.GivePlayerWeapon(player, NAPI.Util.WeaponNameToModel("pistol"), 500);
            player.SendChatMessage("Tạo súng lục thành công!");
        }

        [Command("carlock", "/carlock để mở/khóa một phương tiện!")]
        public void CMD_carlock(Player player)
        {
            Vehicle getVehicle = Utils.GetClosestVehicle(player);
            if (getVehicle != null)
            {
                Accounts account = player.GetData<Accounts>(Accounts.Account_Key);
                if (getVehicle.GetData<int>("VEHICLE_FRAKTION") == account.Fraktion)
                {
                    if (getVehicle.Locked == true)
                    {
                        getVehicle.Locked = false;
                        Utils.sendNotification(player, "phương tiện đã mở", "fas fa-car");
                    }
                    else
                    {
                        getVehicle.Locked = true;
                        Utils.sendNotification(player, "phương tiện đã khóa", "fas fa-car");
                    }
                }
            }
        }

        [Command("charcreator")]
        public void CMD_charcreator(Player player)
        {
            player.TriggerEvent("showHideMoneyHud");
            player.TriggerEvent("charcreator-show");
        }

        [Command("crosshair")]
        public void CMD_crosshair(Player player, int crosshair)
        {
            if (crosshair < 0 || crosshair > 18)
            {
                Utils.sendNotification(player, "Tâm không hợp lệ", "fas fa-user");
                return;
            }
            player.TriggerEvent("showcrosshair", crosshair);
            Utils.sendNotification(player, "Đặt tâm ngắm thành công!", "fas fa-user");
        }

        [Command("crosshairhide")]
        public void CMD_crosshairhide(Player player)
        {
            player.TriggerEvent("hidecrosshair");
            Utils.sendNotification(player, "vô hiệu hóa tâm ngắm!", "fas fa-user");
        }

        [Command("geld")]
        public void CMD_geld(Player player)
        {
            Accounts account = player.GetData<Accounts>(Accounts.Account_Key);
            account.Geld += 50;
            player.SetOwnSharedData("Account:Geld", account.Geld);
            player.SendChatMessage("~g~Geld gegeben!");
        }

        [Command("docs")]
        public void CMD_docs(Player player)
        {
            Accounts account = player.GetData<Accounts>(Accounts.Account_Key);
            if (account.Fraktion == 1)
            {
                player.TriggerEvent("ShowDocsWindow");
            }
        }
    }
}
