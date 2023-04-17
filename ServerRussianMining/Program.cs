using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using Qiwi.BillPayments.Client;
using Qiwi.BillPayments.Model;
using Qiwi.BillPayments.Model.In;
using Qiwi.BillPayments.Model.Out;
using Newtonsoft.Json;
using MySqlConnector;
using System.Data;

namespace ServerRussianMining
{
    public static class SQL_Connect 
    {
        public static MySqlConnection connection = new MySqlConnection($"server=bssvety7.beget.tech;port=3306;username=bssvety7_droneat;password=Ex679956blstbe;database=bssvety7_droneat;minimumpoolsize=0;maximumpoolsize=1000;pooling=true;");
        public static void Connect() 
        {
            connection.Open();
            Console.WriteLine("-SQL_CONNECTOR: " + connection.State.ToString());
        }
    }
    public class DataSQL 
    {
        public int id;
        public int clas;
        public int colwar;
        public string data;
        public string emale;
        public int gameover;
        public string gildiya;
        public int medal;
        public int min1;
        public int min2;
        public int min3;
        public int min4;
        public int min5;
        public int min6;
        public string name;
        public string online;
        public string password;
        public int platina;
        public int raiting;
        public int rubicoins;
        public int wins;
        public string zvanie;
    }
    public class DataSQLTable
    {
        public string Admin;
        public int Id;
        public string Rota;
        public string Zam1;
        public string Zam2;
        public string Zam3;
        public int LVL;
        public int Pulemet;
        public int Laser;
        public int Rocket;
        public int Kamiladze;
        public int PVO;
        public int Mining1;
        public int Mining2;
        public int Mining3;
        public int Mining4;
        public int Mining5;
        public int Mining6;
    }
    public static class DB 
    {
        public static List<DataSQL> Receve(string text)
        {
            List<DataSQL> sqlL = new List<DataSQL>();
            try
            {
                if (SQL_Connect.connection.State == ConnectionState.Closed)
                {
                    SQL_Connect.Connect();
                }
                Console.WriteLine("-SQL: |STATUS=" + SQL_Connect.connection.State + "|  " + text);
                MySqlCommand command = new MySqlCommand(text, SQL_Connect.connection);
                MySqlDataReader reader = command.ExecuteReader();
                DataSQL sql = new DataSQL();
                for (int a = 0; reader.Read(); a++)
                {
                    sql.id = reader.GetInt32("Id");
                    sql.clas = reader.GetInt32("Class");
                    sql.colwar = reader.GetInt32("ColWar");
                    sql.data = reader.GetString("Data");
                    sql.emale = reader.GetString("Emale");
                    sql.gameover = reader.GetInt32("GameOver");
                    sql.gildiya = reader.GetString("Gildiya");
                    sql.medal = reader.GetInt32("Medal");
                    sql.min1 = reader.GetInt32("mining1");
                    sql.min2 = reader.GetInt32("mining2");
                    sql.min3 = reader.GetInt32("mining3");
                    sql.min4 = reader.GetInt32("mining4");
                    sql.min5 = reader.GetInt32("mining5");
                    sql.min6 = reader.GetInt32("mining6");
                    sql.name = reader.GetString("Name");
                    sql.online = reader.GetString("Online");
                    sql.password = reader.GetString("Password");
                    sql.platina = reader.GetInt32("Platina");
                    sql.raiting = reader.GetInt32("Raiting");
                    sql.rubicoins = reader.GetInt32("Rubicoins");
                    sql.wins = reader.GetInt32("Wins");
                    sql.zvanie = reader.GetString("Zvanie");
                    sqlL.Add(sql);
                }
                reader.Dispose();
            }
            catch
            {
                try
                {
                    if (SQL_Connect.connection.State == ConnectionState.Closed)
                    {
                        SQL_Connect.Connect();
                    }
                    MySqlCommand command = new MySqlCommand(text, SQL_Connect.connection);
                    command.ExecuteNonQuery();
                }
                catch(Exception ex) { Console.WriteLine(ex.ToString()); }

            }
            return sqlL;
        }
        public static List<DataSQLTable> ReceveTable(string text)
        {
            List<DataSQLTable> sqlL = new List<DataSQLTable>();
            try
            {
                if (SQL_Connect.connection.State == ConnectionState.Closed)
                {
                    SQL_Connect.Connect();
                }
                Console.WriteLine("-SQL: |STATUS="+ SQL_Connect.connection.State + "|  " + text);
                MySqlCommand command = new MySqlCommand(text, SQL_Connect.connection);
                MySqlDataReader reader = command.ExecuteReader();
                DataSQLTable sql = new DataSQLTable();
                for (int a = 0; reader.Read(); a++)
                {
                    sql.Rota = reader.GetString("Rota");
                    sql.Id = reader.GetInt32("Id");
                    sql.Admin = reader.GetString("Admin");
                    sql.LVL = reader.GetInt32("LVL");
                    sql.Pulemet = reader.GetInt32("Pulemet");
                    sql.Laser = reader.GetInt32("Laser");
                    sql.Rocket = reader.GetInt32("Rocket");
                    sql.Kamiladze = reader.GetInt32("Kamikadze");
                    sql.PVO = reader.GetInt32("PVO");
                    sql.Mining1 = reader.GetInt32("Mining1");
                    sql.Mining2 = reader.GetInt32("Mining2");
                    sql.Mining3 = reader.GetInt32("Mining3");
                    sql.Mining4 = reader.GetInt32("Mining4");
                    sql.Mining5 = reader.GetInt32("Mining5");
                    sql.Mining6 = reader.GetInt32("Mining6");
                    sql.Zam1 = reader.GetString("Zam1");
                    sql.Zam2 = reader.GetString("Zam2");
                    sql.Zam3 = reader.GetString("Zam3");
                    sqlL.Add(sql);
                }
                reader.Dispose();
            }
            catch 
            {
                if (SQL_Connect.connection.State == ConnectionState.Closed)
                {
                    SQL_Connect.Connect();
                }
                try
                {
                    MySqlCommand command = new MySqlCommand(text, SQL_Connect.connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            }
            return sqlL;
        }
    }
    public class SHA
    {
        public static List<Room> rooms = new List<Room>();
    }
    public class Program
    {
        public static MySqlConnection connection;
        const int port = 8888;
        static TcpListener listener;
        public static Chat[] Chats = new Chat[100];
        static void Main(string[] args)
        {
            SQL_Connect.Connect();
            Room room = new Room();
            SHA.rooms.Add(room);
            try
            {
                Console.Write("-IP: ");string ip = Console.ReadLine();
                listener = new TcpListener(IPAddress.Parse(ip), port);
                listener.Start();
                Console.WriteLine("-Waiting for connection...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(client);

                    // создаем новый поток для обслуживания нового клиента
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                    Console.WriteLine("-User Connect");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (listener != null)
                    listener.Stop();
            }
        }
    }
    class ClientObject
    {
        BillPaymentsClient client_Bill = BillPaymentsClientFactory.Create(secretKey: "eyJ2ZXJzaW9uIjoiUDJQIiwiZGF0YSI6eyJwYXlpbl9tZXJjaGFudF9zaXRlX3VpZCI6ImJnYWRqaC0wMCIsInVzZXJfaWQiOiI3OTE4OTk0MjQ5MSIsInNlY3JldCI6IjU5Njk5ZGZlYjZmMzYxMTVlMzY5NDlmMDVhM2UzMzgzNDc4NDI0N2Q5MzkwOWJhNzJjYmI0ZjAzMDM5NDgyMTIifX0=");
        BillResponse bill = new BillResponse();
        public TcpClient client;
        public ClientObject(TcpClient tcpClient)
        {
            client = tcpClient;
        }
        public void Process()
        {
            
            NetworkStream stream = null;
            stream = client.GetStream();
            byte[] data = new byte[64]; // буфер для получаемых данных
            while (true)
            {
                try
                {
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        try
                        {
                            bytes = stream.Read(data, 0, data.Length);
                            builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                        }
                        catch { }
                    }
                    while (stream.DataAvailable);
                    string message = builder.ToString();
                    var Responce = JsonConvert.DeserializeObject<BaceResponce>(message);
                    BaceResponce Return = new BaceResponce();
                    if (Responce != null)
                    {

                        switch (Responce.Type)
                        {
                            case TypeData.Game:
                                Return.Type = TypeData.Game;
                                Return.Data = GameData(JsonConvert.DeserializeObject<Game>(Responce.Data.ToString()));
                                break;
                            case TypeData.PocketGildiya:
                                Console.Write("-Pocket_Gildiya ");
                                Return.Type = TypeData.PocketGildiya;
                                Return.Data = GreatPocket(JsonConvert.DeserializeObject<GildiyaZ>(Responce.Data.ToString()));
                                break;
                            case TypeData.StartGame:
                                Console.Write("-Start_Game ");
                                Return.Type = TypeData.StartGame;
                                Return.Data = StartG(JsonConvert.DeserializeObject<StartRoom>(Responce.Data.ToString()));
                                break;
                            case TypeData.Delete_Add_User:
                                Console.Write("-Add_User ");
                                Return.Type = TypeData.Delete_Add_User;
                                Return.Data = UserF(JsonConvert.DeserializeObject<User>(Responce.Data.ToString()));
                                break;
                            case TypeData.SQL_Req:
                                Console.Write("-SQL_Req ");
                                SQL_Reqesting(JsonConvert.DeserializeObject<SQLconnect>(Responce.Data.ToString()).SQL_Reqest);
                                break;
                            case TypeData.User_Info:
                                Console.Write("-User_Info ");
                                Return.Type = TypeData.User_Info;
                                Return.Data = User_Info_Check(JsonConvert.DeserializeObject<User_Info>(Responce.Data.ToString()));
                                break;
                            case TypeData.Gildiya_List:
                                Console.Write("-Gildiya ");
                                Return.Type = TypeData.Gildiya_List;
                                Return.Data = Search_Gildiya(JsonConvert.DeserializeObject<Gildiya_List>(Responce.Data.ToString()));
                                break;
                            case TypeData.Login_Register:
                                Console.Write("-Log,Reg ");
                                Return.Type = TypeData.Login_Register;
                                Return.Data = LoginChecker_And_Regicstrator(JsonConvert.DeserializeObject<Login_Register>(Responce.Data.ToString()));
                                break;
                            case TypeData.Chat:
                                Return.Type = TypeData.Chat;
                                Return.Data = Chat_Updater(JsonConvert.DeserializeObject<Chat>(Responce.Data.ToString()));
                                break;
                            case TypeData.Money:
                                Console.Write("-Money ");
                                Return.Type = TypeData.Money;
                                Return.Data = Money_Control(JsonConvert.DeserializeObject<Money_Farm_Other>(Responce.Data.ToString()));
                                break;
                            case TypeData.Buy:
                                Console.Write("-Buy ");
                                Return.Type = TypeData.Buy;
                                Return.Data = Buy_Market_Start(JsonConvert.DeserializeObject<Buy_Market>(Responce.Data.ToString()));
                                break;
                            case TypeData.Vetka:
                                Console.Write("-Vetka ");
                                Return.Type = TypeData.Vetka;
                                Return.Data = Data_Checker(JsonConvert.DeserializeObject<Vetka_Dronov>(Responce.Data.ToString()));
                                break;
                            default:
                                Console.Write("-Close ");
                                client.Close();
                                break;
                        }
                    }
                    data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(Return));
                    stream.Write(data, 0, data.Length);
                }
                catch
                {
                    Console.WriteLine("-User Disconnect ");
                    client.Close();
                    break;
                }
            }
        }
        public Vetka_Dronov Data_Checker(Vetka_Dronov data1) 
        {
            string text_Data = "";
            if (data1.start == true)
            {
                var rec = DB.Receve($"SELECT * FROM Users WHERE Name = '{data1.Name}'");
                string[] words = rec[0].data.Split('/');
                data1.save = words;
            }
            else {
                for (int a = 0; a < data1.save.Length; a++)
                {
                    text_Data += data1.save[a] + "/";
                }
                SQL_($"UPDATE Users SET Data = '{text_Data}' WHERE Name = '{data1.Name}'");
            }
            return data1;
        }
        public Buy_Market Buy_Market_Start(Buy_Market data1) 
        {
            string BillId_Info = data1.BillID;
            //string BillId_Info = Guid.NewGuid().ToString();
            //--------------------------------------------------------------------------
            if (data1.status == "BUY")
            {
                bill = new BillResponse();
                bill = client_Bill.CreateBill(info: new CreateBillInfo
                {
                    BillId = BillId_Info,
                    Amount = new MoneyAmount
                    {
                        ValueDecimal = Decimal.Parse(data1.Rub.ToString()),
                        CurrencyEnum = CurrencyEnum.Rub
                    },
                    Comment = "ytups2@gmail.com",
                    ExpirationDateTime = DateTime.Now.AddHours(1),
                    Customer = new Customer
                    {
                        Email = "bss-vet@yandex.ru",
                        Account = Guid.NewGuid().ToString(),
                        Phone = "89189942491"
                    },
                    SuccessUrl = new Uri("https://transonlain.ru/wp-content/uploads/2021/07/%D0%BF%D0%BB%D0%B0%D1%82%D0%B5%D0%B6-%D0%BF%D1%80%D0%BE%D1%88%D0%B5%D0%BB-1536x1086.png")
                });
                data1.BuyUri = bill.PayUrl.ToString();
                data1.status = bill.Status.ValueString;
            }
            else 
            {
                BillResponse bill1 = client_Bill.GetBillInfo(bill.BillId);
                switch (bill1.Status.ValueString) 
                {
                    case "WAITING":
                        data1.status = "WAITING";
                        break;
                    case "REJECTED":
                        data1.status = "REJECTED";
                        break;
                    case "PAID":
                        SQL_($"UPDATE Users SET Class = '{data1.Add_Class}', Medal = Medal + {data1.Add_Medal},Platina = Platina + {data1.Add_Platinum},Rubicoins = Rubicoins + {data1.Add_Rubicoin} WHERE Name = '{data1.User_Name}'");
                        data1.status = "PAID";
                        bill = new BillResponse();
                        break;
                    default:
                        data1.status = "ERROR";
                        break;
                }
            }
            //--------------------------------------------------------------------------
            return data1;
        }
        public Money_Farm_Other Money_Control(Money_Farm_Other data1) 
        {
            if (data1.Add == true) 
            {
                SQL_($"UPDATE Users SET Medal = Medal + {data1.medal},Platina = Platina + {data1.platinum},Rubicoins = Rubicoins + {data1.rubicoin},mining1 = mining1 + {data1.mining[0]},mining2 = mining2 + {data1.mining[1]},mining3 = mining3 + {data1.mining[2]},mining4 = mining4 + {data1.mining[3]},mining5 = mining5 + {data1.mining[4]},mining6 = mining6 + {data1.mining[5]} WHERE Name = '{data1.Name}'");
            } 
            else 
            {
                SQL_($"UPDATE Users SET Medal = Medal - {data1.medal},Platina = Platina - {data1.platinum},Rubicoins = Rubicoins - {data1.rubicoin},mining1 = mining1 - {data1.mining[0]},mining2 = mining2 - {data1.mining[1]},mining3 = mining3 - {data1.mining[2]},mining4 = mining4 - {data1.mining[3]},mining5 = mining5 - {data1.mining[4]},mining6 = mining6 - {data1.mining[5]} WHERE Name = '{data1.Name}'");
            }
            var rec = DB.Receve($"SELECT * FROM Users WHERE Name = '{data1.Name}'");
            data1.medal = rec[0].medal;
            data1.platinum = rec[0].platina;
            data1.rubicoin = rec[0].rubicoins;
            data1.mining[0] = rec[0].min1;
            data1.mining[1] = rec[0].min2;
            data1.mining[2] = rec[0].min3;
            data1.mining[3] = rec[0].min4;
            data1.mining[4] = rec[0].min5;
            data1.mining[5] = rec[0].min6;
            return data1;
        }
        public static Chat[] addPos(Chat num)
        {
            int index = 0;
            Chat[] result = new Chat[Program.Chats.Length];
            Array.Copy(Program.Chats, 0, result, 0, index);
            Array.Copy(Program.Chats, index, result, index + 1, Program.Chats.Length - index - 1);
            result[index] = num;
            return result;
        }
        public Chat Chat_Updater(Chat data1) 
        {
            if (data1.Text_Chat != null)
            {
                Program.Chats = addPos(data1);
                if (Program.Chats[99] != null) { Program.Chats[99] = null; }return data1;
            }
            else 
            {
                for (int a = 0;a<Program.Chats.Length;a++) 
                {
                    if (Program.Chats[a] != null)
                    {
                        if (Program.Chats[a].Chat_Name == data1.Chat_Name)
                        {
                            data1.Text_Chat = Program.Chats[a].Text_Chat;
                            break;
                        }
                    }
                }
                return data1;
            }
        }
        public Login_Register LoginChecker_And_Regicstrator(Login_Register data1) 
        {
            if (data1.Log_Reg == true)//Login
            {
                var list = SQL($"SELECT * FROM Users WHERE Emale = '{data1.E_Male}' and Password = '{data1.Password}'");
                if (list.Length > 0) 
                {
                    data1.Result = list[0];
                }
            }
            else //Register
            {
                SQL_($"INSERT INTO Users (Name,Password,Zvanie,Online,Medal,Platina,Rubicoins,Class,ColWar,Wins,GameOver,Emale,mining1,mining2,mining3,mining4,mining5,mining6,Raiting) VALUES ('{data1.Login}','{data1.Password}','new','{DateTime.Now}',0,0,0,0,0,0,0,'{data1.E_Male}',0,0,0,0,0,0,0)");
                var list = SQL($"SELECT * FROM Users WHERE Emale = '{data1.E_Male}' and Password = '{data1.Password}'");
                data1.Result = list[0];
            }
            return data1;
        }
        public User_Info User_Info_Check(User_Info data1)
        {
            var rec = DB.Receve($"SELECT * FROM Users WHERE Name = '{data1.Name}'");
            data1.Name = rec[0].name;
            data1.Medal = rec[0].medal;
            data1.Platina = rec[0].platina;
            data1.Rubicoin = rec[0].rubicoins;
            data1.Time_Online = rec[0].online;
            data1.Zvanie = rec[0].zvanie;
            data1.Class = rec[0].clas;
            data1.Gildiya = rec[0].gildiya;
            data1.Col_War = rec[0].colwar;
            data1.Wins = rec[0].wins;
            data1.Game_Over = rec[0].gameover;
            return data1;
        }
        public Gildiya_List Search_Gildiya(Gildiya_List data1) 
        {
            string command = "";
            if (data1.Search_Name != "")
            {
                command = $"SELECT * FROM Table WHERE Rota = '{data1.Search_Name}'";
            }
            else 
            {
                command = $"SELECT TOP 50 * FROM Table";
            }
            var rec = DB.Receve(command);
            for (int a = 0;a< rec.Count;a++) 
            {
                data1.Data_Table[a] = rec[a].name + "/" + rec[a].medal + "/" + rec[a].password + "/" + SQL($"SELECT * FROM Users WHERE Gildiya = '{rec[a].name}'").Length.ToString();
            }
            return data1;
        }
        public void SQL_Reqesting(string arg) 
        {
            DB.Receve(arg);
        }
        public User UserF(User data1) 
        {
            if (data1.Add_Delete == true && data1.Gildiya == true)
            {
                SQL_Z(data1.Name,data1.GildiyaName);
            }
            if (data1.Add_Delete == false && data1.Gildiya == true)
            {
                SQL_Z(data1.Name, "");
            }
            if (data1.Add_Delete == true && data1.Gildiya == false)
            {
                //User Add BD
            }
            if (data1.Add_Delete == false && data1.Gildiya == false)
            {
                //User Delete BD
            }
            return data1;
        }
        public Games GameData(Game data1)
        {
            var a = SHA.rooms[0];
            for (int b = 0;b<3;b++) 
            {
                a.posTRA[data1.MyNUM, b] = data1.MyTra[b];
                a.posROT[data1.MyNUM, b] = data1.MyRot[b];
                a.posROTBash[data1.MyNUM, b] = data1.MyRotBash[b];
                a.End_Poz[data1.MyNUM, b] = data1.End_Poz[b];
            }
            for (int b =0;b<4;b++) 
            {
                a.moduls[data1.MyNUM,b] = data1.MyModuls[b];
            }
            a.attack[data1.MyNUM] = data1.Attack;
            SHA.rooms[0].HP[data1.minusHP[0]] -= data1.minusHP[1];
            Games games = new Games();
            games.Flag = a.Flag;
            games.Names = a.names;
            games.OtherAttack = a.attack;
            games.OtherModuls = a.moduls;
            games.OtherHp = a.HP;
            games.OtherTra = a.posTRA;
            games.OtherRot = a.posROT;
            games.OtherRotBash = a.posROTBash;
            games.Chat = a.Chat;
            games.OtherEnd_Poz = a.End_Poz;
            return games;
        }
        public void SQL_Z(string Name,string Z)
        {
            DB.Receve($"UPDATE Users SET Gildiya = '{Z}' WHERE Name = '{Name}'");
        }
        public void SQL_(string SQL_Z)
        {
            DB.Receve(SQL_Z);
        }
        public StartRoom StartG(StartRoom data1) 
        {
            DB.Receve($"UPDATE Users SET Online = '{DateTime.Now}' WHERE Name = '{data1.Name}'");
            StartRoom rom = new StartRoom();
            for (int a =0;a< SHA.rooms[0].names.Length;a++) 
            {
                if (data1.Name == SHA.rooms[0].names[a]) 
                {
                    rom.myNum = a;
                    SHA.rooms[0].HP[a] = data1.HP;
                    break;
                }
                if (SHA.rooms[0].names[a] == null)
                {
                    rom.myNum = a;
                    SHA.rooms[0].names[a] = data1.Name;
                    SHA.rooms[0].HP[a] = data1.HP;
                    Console.WriteLine("- "+ rom.myNum +")Connect " + data1.Name);
                    break;
                }
            }
            return rom;
        }
        public string[] SQL(string sql)
        {
            var rec = DB.Receve(sql);
            string[] str = new string[rec.Count];
            for (int a =0;a< rec.Count;a++) 
            {
                str[a] = rec[a].name + "/" + rec[a].online + "/" + rec[a].zvanie + "/" + rec[a].rubicoins + "/" + rec[a].clas + "/" + rec[a].medal;
            }
            return str;
        }
        public PocketGildiya GreatPocket(GildiyaZ data1)
        {
            var pocket = new PocketGildiya();

            try
            {
                pocket.Soldats = SQL($"SELECT * FROM Users WHERE Gildiya = '{data1.Name}'");
                pocket.Zayavli = SQL($"SELECT * FROM Users WHERE Gildiya = '{data1.Name + "@@@Z"}'");
                var rec = DB.ReceveTable($"SELECT * FROM Table WHERE Rota = '{data1.Name}'");
                pocket.Name = rec[0].Rota;
                pocket.LVL = rec[0].LVL;
                pocket.Pulemet = rec[0].Pulemet;
                pocket.Laser = rec[0].Laser;
                pocket.Rocket = rec[0].Rocket;
                pocket.Kamiladze = rec[0].Kamiladze;
                pocket.PVO = rec[0].PVO;
                pocket.Mining1 = rec[0].Mining1;
                pocket.Mining2 = rec[0].Mining2;
                pocket.Mining3 = rec[0].Mining3;
                pocket.Mining4 = rec[0].Mining4;
                pocket.Mining5 = rec[0].Mining5;
                pocket.Mining6 = rec[0].Mining6;
            }
            catch { }
            return pocket;
        }
    }
    public class Vetka_Dronov
    {
        public bool start;
        public string Name;
        public string[] save;
    }
    public class Buy_Market 
    {
        public int Rub;
        public string E_mail;
        public string Number_Phone;
        public string BillID;
        public string BuyUri;
        public string User_Name;
        public int Add_Class;
        public int Add_Medal;
        public int Add_Rubicoin;
        public int Add_Platinum;
        public string status;
    }
    public class Money_Farm_Other
    {
        public string Name;
        public bool Add;
        public int medal;
        public int rubicoin;
        public int platinum;
        public int[] mining;
    }
    public class Chat 
    {
        public string Chat_Name;
        public string Text_Chat;
    }
    public class Login_Register 
    {
        public bool Log_Reg;
        public string Login;
        public string Password;
        public string E_Male;
        public string Result;
    }
    public class Gildiya_List
    {
        public string Search_Name;
        public string[] Data_Table;
    }
    public class User_Info 
    {
        public string Name;
        public int Medal;
        public int Platina;
        public int Rubicoin;
        public string Time_Online;
        public string Zvanie;
        public int Class;
        public string Gildiya;
        public int Col_War;
        public int Wins;
        public int Game_Over;
    }
    public class Room
    {
        public float[,] End_Poz = new float[14,3];
        public int Flag = 0;
        public string[] names = new string[14];
        public int[] HP = new int[14];
        public float[,] posTRA = new float[14, 3];
        public float[,] posROT = new float[14, 3];
        public float[,] posROTBash = new float[14, 3];
        public bool[] attack = new bool[14];
        public bool[,] moduls = new bool[13, 4];
        public string Chat = "null";
    }
    public class User
    {
        public string GildiyaName;
        public bool Add_Delete;
        public bool Gildiya;
        public string Name;
        public string Password;
        public string E_mile;
    }
    public class SQLconnect
    {
        public string SQL_Reqest;
    }
    public class StartRoom 
    {
        public string Name = "";
        public int myNum = 0;
        public int HP = 0;
        public int rezhim = 0;
    }
    public enum TypeData 
    {
       Game,
       PocketGildiya,
       StartGame,
       Delete_Add_User,
       SQL_Req,
       User_Info,
       Gildiya_List,
       Login_Register,
       Chat,
       Money,
       Buy,
       Vetka
    }
    public class BaceResponce
    {
        public TypeData Type { get; set; }
        public object Data { get; set; }
    }
    public class PocketGildiya 
    {
        public string Name;
        public int LVL;
        public string[] Soldats;//[Name/Zvaniye/Klass/Online/];
        public int Pulemet;
        public int Laser;
        public int Rocket;
        public int Kamiladze;
        public int PVO;
        public int Mining1;
        public int Mining2;
        public int Mining3;
        public int Mining4;
        public int Mining5;
        public int Mining6;
        public string[] Zayavli;//[Name/Zvanie]
    }
    public class Game
    {
        public float[] End_Poz = new float[3]; 
        public float[] MyTra = new float[3];
        public float[] MyRot = new float[3];
        public float[] MyRotBash = new float[3];
        public int[] minusHP;
        public bool[] MyModuls;
        public bool Attack;
        public int MyNUM;
    }
    public class Games
    {
        public float[,] OtherEnd_Poz;
        public int Flag;
        public string[] Names;
        public float[,] OtherTra;
        public float[,] OtherRot;
        public float[,] OtherRotBash;
        public int[] OtherHp;
        public bool[] OtherAttack;
        public bool[,] OtherModuls = new bool[14,4];
        public string Chat;
    }
    public class GildiyaZ
    {
        public string Name;
    }
}
