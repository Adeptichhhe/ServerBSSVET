using System;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Threading;
using System.Text;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using Qiwi;
using Qiwi.BillPayments;
using Qiwi.BillPayments.Client;
using Qiwi.BillPayments.Model;
using Qiwi.BillPayments.Model.In;
using Qiwi.BillPayments.Model.Out;
using Qiwi.BillPayments.Web;
using System.Security.Cryptography;
using System.Collections;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

namespace ServerRussianMining
{
    public class SHA
    {
        public static List<Room> rooms = new List<Room>();
        //public static string[] posROT = new string[14];
        //метод шифрования строки
        public static string Shifrovka(string ishText, string pass,
               string sol = "doberman", string cryptographicAlgorithm = "SHA1",
               int passIter = 2, string initVec = "a8doSuDitOz1hZe#",
               int keySize = 256)
        {
            try
            {
                if (string.IsNullOrEmpty(ishText))
                    return "";

                byte[] initVecB = Encoding.ASCII.GetBytes(initVec);
                byte[] solB = Encoding.ASCII.GetBytes(sol);
                byte[] ishTextB = Encoding.UTF8.GetBytes(ishText);

                PasswordDeriveBytes derivPass = new PasswordDeriveBytes(pass, solB, cryptographicAlgorithm, passIter);
                byte[] keyBytes = derivPass.GetBytes(keySize / 8);
                RijndaelManaged symmK = new RijndaelManaged();
                symmK.Mode = CipherMode.CBC;

                byte[] cipherTextBytes = null;

                using (ICryptoTransform encryptor = symmK.CreateEncryptor(keyBytes, initVecB))
                {
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memStream, encryptor, CryptoStreamMode.Write))
                        {
                            cryptoStream.Write(ishTextB, 0, ishTextB.Length);
                            cryptoStream.FlushFinalBlock();
                            cipherTextBytes = memStream.ToArray();
                            memStream.Close();
                            cryptoStream.Close();
                        }
                    }
                }

                symmK.Clear();
                return Convert.ToBase64String(cipherTextBytes);
            }
            catch { return null; }
        }

        //метод дешифрования строки
        public static string DeShifrovka(string ciphText, string pass,
               string sol = "doberman", string cryptographicAlgorithm = "SHA1",
               int passIter = 2, string initVec = "a8doSuDitOz1hZe#",
               int keySize = 256)
        {
            try
            {
                if (string.IsNullOrEmpty(ciphText))
                    return "";

                byte[] initVecB = Encoding.ASCII.GetBytes(initVec);
                byte[] solB = Encoding.ASCII.GetBytes(sol);
                byte[] cipherTextBytes = Convert.FromBase64String(ciphText);

                PasswordDeriveBytes derivPass = new PasswordDeriveBytes(pass, solB, cryptographicAlgorithm, passIter);
                byte[] keyBytes = derivPass.GetBytes(keySize / 8);

                RijndaelManaged symmK = new RijndaelManaged();
                symmK.Mode = CipherMode.CBC;

                byte[] plainTextBytes = new byte[cipherTextBytes.Length];
                int byteCount = 0;

                using (ICryptoTransform decryptor = symmK.CreateDecryptor(keyBytes, initVecB))
                {
                    using (MemoryStream mSt = new MemoryStream(cipherTextBytes))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(mSt, decryptor, CryptoStreamMode.Read))
                        {
                            byteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                            mSt.Close();
                            cryptoStream.Close();
                        }
                    }
                }

                symmK.Clear();
                return Encoding.UTF8.GetString(plainTextBytes, 0, byteCount);
            }
            catch { return null; }
        }
    }
    class Program
    {
        const int port = 8888;
        static TcpListener listener;
        public static Chat[] Chats = new Chat[100];
        static void Main(string[] args)
        {
            Room room = new Room();
            SHA.rooms.Add(room);
            try
            {
                Console.Write("IP: ");string ip = Console.ReadLine();
                listener = new TcpListener(IPAddress.Parse(ip), port);
                listener.Start();
                Console.WriteLine();
                Console.Write("- Waiting for connection...");

                while (true)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(client);

                    // создаем новый поток для обслуживания нового клиента
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
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
            SqlConnection sqlConnection = null;
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString);
            sqlConnection.Open();
            if (sqlConnection.State == ConnectionState.Open)
            {
                Console.WriteLine();
                Console.WriteLine("- SQL connection succes");
            }
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
                                Return.Type = TypeData.PocketGildiya;
                                Return.Data = GreatPocket(JsonConvert.DeserializeObject<GildiyaZ>(Responce.Data.ToString()));
                                break;
                            case TypeData.StartGame:
                                Return.Type = TypeData.StartGame;
                                Return.Data = StartG(JsonConvert.DeserializeObject<StartRoom>(Responce.Data.ToString()));
                                break;
                            case TypeData.Delete_Add_User:
                                Return.Type = TypeData.Delete_Add_User;
                                Return.Data = UserF(JsonConvert.DeserializeObject<User>(Responce.Data.ToString()));
                                break;
                            case TypeData.SQL_Req:
                                SQL_Reqesting(JsonConvert.DeserializeObject<SQLconnect>(Responce.Data.ToString()).SQL_Reqest);
                                break;
                            case TypeData.User_Info:
                                Return.Type = TypeData.User_Info;
                                Return.Data = User_Info_Check(JsonConvert.DeserializeObject<User_Info>(Responce.Data.ToString()));
                                break;
                            case TypeData.Gildiya_List:
                                Return.Type = TypeData.Gildiya_List;
                                Return.Data = Search_Gildiya(JsonConvert.DeserializeObject<Gildiya_List>(Responce.Data.ToString()));
                                break;
                            case TypeData.Login_Register:
                                Return.Type = TypeData.Login_Register;
                                Return.Data = LoginChecker_And_Regicstrator(JsonConvert.DeserializeObject<Login_Register>(Responce.Data.ToString()));
                                break;
                            case TypeData.Chat:
                                Return.Type = TypeData.Chat;
                                Return.Data = Chat_Updater(JsonConvert.DeserializeObject<Chat>(Responce.Data.ToString()));
                                break;
                            case TypeData.Money:
                                Return.Type = TypeData.Money;
                                Return.Data = Money_Control(JsonConvert.DeserializeObject<Money_Farm_Other>(Responce.Data.ToString()));
                                break;
                            case TypeData.Buy:
                                Return.Type = TypeData.Buy;
                                Return.Data = Buy_Market_Start(JsonConvert.DeserializeObject<Buy_Market>(Responce.Data.ToString()));
                                break;
                            case TypeData.Vetka:
                                Return.Type = TypeData.Vetka;
                                Return.Data = Data_Checker(JsonConvert.DeserializeObject<Vetka_Dronov>(Responce.Data.ToString()));
                                break;
                            default:
                                client.Close();
                                break;
                        }
                    }
                    data = Encoding.Unicode.GetBytes(JsonConvert.SerializeObject(Return));
                    stream.Write(data, 0, data.Length);
                }
                catch 
                {
                    client.Close();
                    Console.WriteLine("- User Disconnect");
                    break;
                }
            }
        }
        public Vetka_Dronov Data_Checker(Vetka_Dronov data1) 
        {
            string text_Data = "";
            if (data1.start == true)
            {
                DataTable table = new DataTable();
                DataSet set = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter();
                SqlConnection sqlConnection = null;
                sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString);
                sqlConnection.Open();
                SqlCommand command = new SqlCommand();
                command = new SqlCommand($"SELECT * FROM [Users] WHERE Name = N'{data1.Name}'", sqlConnection);
                command.ExecuteNonQuery();
                adapter.SelectCommand = command;
                adapter.Fill(table);
                adapter.Fill(set);
                foreach (DataTable dt in set.Tables)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        var cells = row.ItemArray;
                        text_Data = cells[20].ToString();
                    }
                }
                string[] words = text_Data.Split('/');
                data1.save = words;
            }
            else {
                for (int a = 0; a < data1.save.Length; a++)
                {
                    text_Data += data1.save[a] + "/";
                }
                SQL_($"UPDATE [Users] SET Data = N'{text_Data}' WHERE Name = N'{data1.Name}'");
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
                        SQL_($"UPDATE [Users] SET Class = N'{data1.Add_Class}', Medal += N'{data1.Add_Medal}',Platina += N'{data1.Add_Platinum}',Rubicoins += N'{data1.Add_Rubicoin}' WHERE Name = N'{data1.User_Name}'");
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
                SQL_($"UPDATE [Users] SET Medal += N'{data1.medal}',Platina += N'{data1.platinum}',Rubicoins += N'{data1.rubicoin}',mining1 += N'{data1.mining[0]}',mining2 += N'{data1.mining[1]}',mining3 += N'{data1.mining[2]}',mining4 += N'{data1.mining[3]}',mining5 += N'{data1.mining[4]}',mining6 += N'{data1.mining[5]}' WHERE Name = N'{data1.Name}'");
            } 
            else 
            {
                SQL_($"UPDATE [Users] SET Medal -= N'{data1.medal}',Platina -= N'{data1.platinum}',Rubicoins -= N'{data1.rubicoin}',mining1 -= N'{data1.mining[0]}',mining2 -= N'{data1.mining[1]}',mining3 -= N'{data1.mining[2]}',mining4 -= N'{data1.mining[3]}',mining5 -= N'{data1.mining[4]}',mining6 -= N'{data1.mining[5]}' WHERE Name = N'{data1.Name}'");
            }
            DataTable table = new DataTable();
            DataSet set = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlConnection sqlConnection = null;
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString);
            sqlConnection.Open();
            SqlCommand command = new SqlCommand();
            command = new SqlCommand($"SELECT * FROM [Users] WHERE Name = N'{data1.Name}'", sqlConnection);
            command.ExecuteNonQuery();
            adapter.SelectCommand = command;
            adapter.Fill(table);
            adapter.Fill(set);
            foreach (DataTable dt in set.Tables)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var cells = row.ItemArray;
                    data1.medal = (int)cells[3];
                    data1.platinum = (int)cells[4];
                    data1.rubicoin = (int)cells[5];
                    data1.mining[0] = (int)cells[14];
                    data1.mining[1] = (int)cells[15];
                    data1.mining[2] = (int)cells[16];
                    data1.mining[3] = (int)cells[17];
                    data1.mining[4] = (int)cells[18];
                    data1.mining[5] = (int)cells[19];
                }
            }
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
                var list = SQL($"SELECT * FROM [Users] WHERE Emale = N'{data1.E_Male}' and Password = N'{data1.Password}'");
                if (list.Length > 0) 
                {
                    data1.Result = list[0];
                }
            }
            else //Register
            {
                SQL_($"INSERT INTO [Users] (Name,Password,Zvanie,Online,Medal,Platina,Rubicoins,Class,ColWar,Wins,GameOver,Emale,mining1,mining2,mining3,mining4,mining5,mining6,Raiting) VALUES (N'{data1.Login}',N'{data1.Password}','new',N'{DateTime.Now}',0,0,0,0,0,0,0,N'{data1.E_Male},0,0,0,0,0,0,0')");
            }
            return data1;
        }
        public User_Info User_Info_Check(User_Info data1)
        {
            DataTable table = new DataTable();
            DataSet set = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlConnection sqlConnection = null;
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString);
            sqlConnection.Open();
            SqlCommand command = new SqlCommand();
            command = new SqlCommand($"SELECT * FROM [Users] WHERE Name = N'{data1.Name}'", sqlConnection);
            command.ExecuteNonQuery();
            adapter.SelectCommand = command;
            adapter.Fill(table);
            adapter.Fill(set);
            foreach (DataTable dt in set.Tables)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var cells = row.ItemArray;
                    data1.Name = cells[1].ToString();
                    data1.Medal = (int)cells[3];
                    data1.Platina = (int)cells[4];
                    data1.Rubicoin = (int)cells[5];
                    data1.Time_Online = cells[6].ToString();
                    data1.Zvanie = cells[7].ToString();
                    data1.Class = (int)cells[8];
                    data1.Gildiya = cells[9].ToString();
                    data1.Col_War = (int)cells[10];
                    data1.Wins = (int)cells[11];
                    data1.Game_Over = (int)cells[12];
                }
            }
            return data1;
        }
        public Gildiya_List Search_Gildiya(Gildiya_List data1) 
        {
            DataTable table = new DataTable();
            DataSet set = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlConnection sqlConnection = null;
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString);
            sqlConnection.Open();
            SqlCommand command = new SqlCommand();
            if (data1.Search_Name != "")
            {
                command = new SqlCommand($"SELECT * FROM [Table] WHERE Rota = N'{data1.Search_Name}'", sqlConnection);
            }
            else 
            {
                command = new SqlCommand($"SELECT TOP 50 * FROM [Table]", sqlConnection);
            }
            command.ExecuteNonQuery();
            adapter.SelectCommand = command;
            adapter.Fill(table);
            adapter.Fill(set);
            foreach (DataTable dt in set.Tables)
            {
                int a = 0;
                data1.Data_Table = new string[dt.Rows.Count];
                foreach (DataRow row in dt.Rows)
                {
                    var cells = row.ItemArray;
                    foreach (object cell in cells)
                    {
                        data1.Data_Table[a] = cells[1].ToString() + "/" + cells[3].ToString() + "/" + cells[2].ToString() + "/" + SQL($"SELECT * FROM [Users] WHERE Gildiya = N'{cells[1].ToString()}'").Length.ToString();
                        
                    }a += 1;
                }
            }
            return data1;
        }
        public void SQL_Reqesting(string arg) 
        {
            SqlConnection sqlConnection = null;
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString);
            sqlConnection.Open();
            SqlCommand command = new SqlCommand();
            command = new SqlCommand(arg, sqlConnection);
            command.ExecuteNonQuery();
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
            SqlConnection sqlConnection = null;
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString);
            sqlConnection.Open();
            SqlCommand command = new SqlCommand();
            command = new SqlCommand($"UPDATE [Users] SET Gildiya = N'{Z}' WHERE Name = N'{Name}'", sqlConnection);
            command.ExecuteNonQuery();
        }
        public void SQL_(string SQL_Z)
        {
            SqlConnection sqlConnection = null;
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString);
            sqlConnection.Open();
            SqlCommand command = new SqlCommand();
            command = new SqlCommand(SQL_Z, sqlConnection);
            command.ExecuteNonQuery();
        }
        public StartRoom StartG(StartRoom data1) 
        {
            SqlConnection sqlConnection = null;
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString);
            sqlConnection.Open();
            SqlCommand command = new SqlCommand();
            command = new SqlCommand($"UPDATE [Users] SET Online = N'{DateTime.Now}' WHERE Name = N'{data1.Name}'", sqlConnection);
            command.ExecuteNonQuery();
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
            DataTable table = new DataTable();
            DataSet set = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlConnection sqlConnection = null;
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString);
            sqlConnection.Open();
            SqlCommand command = new SqlCommand();
            command = new SqlCommand(sql, sqlConnection);
            command.ExecuteNonQuery();
            adapter.SelectCommand = command;
            adapter.Fill(table);
            adapter.Fill(set);
            int a = 0;
            string[] str = null;
            foreach (DataTable dt in set.Tables)
            {
                str = new string[dt.Rows.Count];
                foreach (DataRow row in dt.Rows)
                {
                    var cells = row.ItemArray;
                    foreach (object cell in cells)
                        str[a] = cells[1].ToString() + "/" + cells[6].ToString() + "/" + cells[7].ToString() + "/" + cells[5].ToString() + "/" + cells[8].ToString() + "/" + cells[3].ToString();
                    a += 1;
                }
            }
            return str;
        }
        public PocketGildiya GreatPocket(GildiyaZ data1)
        {
            var pocket = new PocketGildiya();
            pocket.Soldats = SQL($"SELECT * FROM [Users] WHERE Gildiya = N'{data1.Name}'");
            pocket.Zayavli = SQL($"SELECT * FROM [Users] WHERE Gildiya = N'{data1.Name + "@@@Z"}'");
            SqlDataAdapter adapter = new SqlDataAdapter();
            SqlConnection sqlConnection = null;
            sqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings["TestDB"].ConnectionString);
            sqlConnection.Open();
            SqlCommand command = new SqlCommand();
            DataTable table1 = new DataTable();
            DataSet set1 = new DataSet();
            command = new SqlCommand($"SELECT * FROM [Table] WHERE Rota = N'{data1.Name}'", sqlConnection);
            command.ExecuteNonQuery();
            adapter.SelectCommand = command;
            adapter.Fill(table1);
            adapter.Fill(set1);
            foreach (DataTable dt in set1.Tables)
            {
                foreach (DataRow row in dt.Rows)
                {
                    var cells = row.ItemArray;
                    foreach (object cell in cells)
                        pocket.Name = (string)cells[1];
                    pocket.LVL = (int)cells[2];
                    pocket.Pulemet = (int)cells[7];
                    pocket.Laser = (int)cells[8];
                    pocket.Rocket = (int)cells[9];
                    pocket.Kamiladze = (int)cells[10];
                    pocket.PVO = (int)cells[11];
                    pocket.Mining1 = (int)cells[12];
                    pocket.Mining2 = (int)cells[13];
                    pocket.Mining3 = (int)cells[14];
                    pocket.Mining4 = (int)cells[15];
                    pocket.Mining5 = (int)cells[16];
                    pocket.Mining6 = (int)cells[17];
                }
            }
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
