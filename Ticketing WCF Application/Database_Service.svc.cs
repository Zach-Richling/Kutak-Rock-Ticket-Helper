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
using System.Data;
using System.Windows.Forms;
using System.Net.Mail;
using System.Net;

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
                    SqlCommand insert = new SqlCommand(null, conn);
                    insert.CommandText = "exec EnsureComputerExists @MachineName, @ExternalIP";
                    insert.Parameters.Add(new SqlParameter("@MachineName", SqlDbType.VarChar, 20));
                    insert.Parameters.Add(new SqlParameter("@ExternalIP", SqlDbType.VarChar, 20));
                    conn.Open();
                    insert.Prepare();
                    insert.Parameters[0].Value = machineName;
                    insert.Parameters[1].Value = ip;
                    using (SqlDataReader reader = insert.ExecuteReader())
                    {
                        string output = "";
                        while (reader.Read())
                        {
                            output += "Id: " + reader["Id"].ToString() + " ";
                            output += "Info: " + reader["INFO"].ToString();
                        }
                        conn.Close();
                        return output;
                    }
                }
            } catch (Exception e)
            {
                return e.ToString();
            }
        }
        string IDatabase_Service.createTicket(string ticketName, string ticketDesc, string ticketSeverity, string computerID)
        {
            try
            {
                string connectionString = @"Server=tcp:kutak-rock.database.windows.net,1433;Initial Catalog=Kutak Rock Ticketing;Persist Security Info=False;User ID=Kutak_Rock_WCF;Password=7rM-mg!E-7Nh>J8q;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand insert = new SqlCommand(null, conn);
                    insert.CommandText = "exec CreateTicket @ComputerId, @TicketName, @TicketDescription, @TicketSeverity";
                    insert.Parameters.Add(new SqlParameter("@ComputerId", SqlDbType.Int));
                    insert.Parameters.Add(new SqlParameter("@TicketName", SqlDbType.VarChar, 50));
                    insert.Parameters.Add(new SqlParameter("@TicketDescription", SqlDbType.VarChar, 200));
                    insert.Parameters.Add(new SqlParameter("@TicketSeverity", SqlDbType.Int));
                    conn.Open();
                    insert.Prepare();
                    insert.Parameters[0].Value = computerID;
                    insert.Parameters[1].Value = ticketName;
                    insert.Parameters[2].Value = ticketDesc;
                    insert.Parameters[3].Value = ticketSeverity;
                    using (SqlDataReader reader = insert.ExecuteReader())
                    {
                        string output = "";
                        while (reader.Read())
                        {
                            output += reader["Id"].ToString();
                        }
                        conn.Close();
                        return output;
                    }
                }
            } catch (Exception e)
            {
                return e.ToString();
            }
        }

        public string sendTicket(string ticketId)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.Port = 465;
                smtp.Credentials = new NetworkCredential("kutakrockticketing@gmail.com", "7rM-mg!E-7Nh>J8q");
                smtp.EnableSsl = true;

                mail.From = new MailAddress("kutakrockticketing@gmail.com");
                mail.To.Add("zwrdude@gmail.com");
                mail.Subject = "Test Subject";
                mail.Body = "Testing Body";

                smtp.Send(mail);

                return "Sent";

            } catch (Exception e)
            {
                return e.ToString();
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
