using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.IO;

namespace Ticketing_WCF_Application {
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Database_Service : IDatabase_Service
    {
        string key = "b14ca5898a4e4133bbce2ea2315a1916";
        MachineInfo IDatabase_Service.getMachineinfo(string input)
        {
            return new MachineInfo();
        }

        string IDatabase_Service.addMachineinfo(string machineName, string machineInfoInput)
        {
            int checkLength = machineInfoInput.Length % 4;
            if (checkLength > 0)
            {
                machineInfoInput += new string('=', 4 - checkLength);
            }
            string decryptedString = DecryptString(key, machineInfoInput.Replace(" ", "+"));
            MachineInfo machineInfo = JsonConvert.DeserializeObject<MachineInfo>(decryptedString);

            string connectionString = @"Server=tcp:kutak-rock.database.windows.net,1433;Initial Catalog=Kutak Rock Ticketing;Persist Security Info=False;User ID=Kutak_Rock_WCF;Password=7rM-mg!E-7Nh>J8q;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand($"INSERT INTO Framing.MachineInfo ", conn))
                {
                    conn.Open();
                    cmd.ExecuteNonQuery();
                    return "Added Computer";
                }
            }
        }

        string IDatabase_Service.addComputer(string ip, string machineName)
        {
            try
            {
                string connectionString = @"Server=tcp:kutak-rock.database.windows.net,1433;Initial Catalog=Kutak Rock Ticketing;Persist Security Info=False;User ID=Kutak_Rock_WCF;Password=7rM-mg!E-7Nh>J8q;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand($"INSERT INTO Framing.Computers(ExternalIp, MachineName) VALUES ('{ip}', '{machineName}')", conn))
                    {
                        conn.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            
                        }
                            return "Added Computer";
                    }
                }
            } catch (SqlException e)
            {
                return e.Message;
            }
        }

        public string EncryptString(string key, string plainInput)
        {
            byte[] iv = new byte[16];
            byte[] array;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainInput);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public static string DecryptString(string key, string cipherText)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }
        }
    }
}
