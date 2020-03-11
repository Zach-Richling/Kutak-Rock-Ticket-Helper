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
using System.ServiceModel.Web;

namespace Ticketing_WCF_Application {
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Database_Service : IDatabase_Service
    {
        string key = "b14ca5898a4e4133bbce2ea2315a1916";
        MachineInfo IDatabase_Service.getMachineinfo(string machineName)
        {
            try 
            {
                string connectionString = @"Server=tcp:kutak-rock.database.windows.net,1433;Initial Catalog=Kutak Rock Ticketing;Persist Security Info=False;User ID=Kutak_Rock_WCF;Password=7rM-mg!E-7Nh>J8q;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand insert = new SqlCommand(null, conn);
                    insert.CommandText = "exec GetMachineInfoByMachineName @MachineName;";
                    insert.Parameters.Add(new SqlParameter("@MachineName", SqlDbType.VarChar, 20));
                    conn.Open();
                    insert.Prepare();
                    insert.Parameters[0].Value = machineName;
                    using (SqlDataReader reader = insert.ExecuteReader())
                    {
                        MachineInfo output = new MachineInfo();
                        while (reader.Read())
                        {
                            output.id = reader["ComputerId"].ToString();
                            output.macAddress = reader["MACAddress"].ToString();
                            output.manufacturer = reader["Manufacturer"].ToString();
                            output.model = reader["Model"].ToString();
                            output.OSName = reader["OSName"].ToString();
                            output.OSVersion = reader["OSVersion"].ToString();
                            output.ramAmount = reader["RAM"].ToString();
                            output.BIOSNumber = reader["BIOSNumber"].ToString();
                            output.BIOSVersion = reader["BIOSVersion"].ToString();
                            output.userName = reader["Username"].ToString();
                        }
                        conn.Close();
                        return output;
                    }

                }
            } catch (Exception e)
            {
                return new MachineInfo() { id = "-1"};
            }
        }

        string IDatabase_Service.addMachineinfo(string machineName, string machineInfoInput)
        {
            /*
            int checkLength = machineInfoInput.Length % 4;
            if (checkLength > 0)
            {
                machineInfoInput += new string('=', 4 - checkLength);
            }
            string decryptedString = DecryptString(key, machineInfoInput.Replace(" ", "+"));
            */
            try {
                MachineInfo machineInfo = JsonConvert.DeserializeObject<MachineInfo>(machineInfoInput);
                string connectionString = @"Server=tcp:kutak-rock.database.windows.net,1433;Initial Catalog=Kutak Rock Ticketing;Persist Security Info=False;User ID=Kutak_Rock_WCF;Password=7rM-mg!E-7Nh>J8q;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand insert = new SqlCommand(null, conn);
                    insert.CommandText = "exec InsertMachineInfo @ComputerId, @OSName, @OSVersion, @Username, @Manufacturer, @Model, @BIOSVersion, @BIOSNumber, @MACAddress, @RAM;";
                    insert.Parameters.Add(new SqlParameter("@ComputerId", SqlDbType.Int));
                    insert.Parameters.Add(new SqlParameter("@OSName", SqlDbType.VarChar, 20));
                    insert.Parameters.Add(new SqlParameter("@OSVersion", SqlDbType.VarChar, 20));
                    insert.Parameters.Add(new SqlParameter("@Username", SqlDbType.VarChar, 20));
                    insert.Parameters.Add(new SqlParameter("@Manufacturer", SqlDbType.VarChar, 20));
                    insert.Parameters.Add(new SqlParameter("@Model", SqlDbType.VarChar, 20));
                    insert.Parameters.Add(new SqlParameter("@BIOSVersion", SqlDbType.VarChar, 20));
                    insert.Parameters.Add(new SqlParameter("@BIOSNumber", SqlDbType.VarChar, 20));
                    insert.Parameters.Add(new SqlParameter("@MACAddress", SqlDbType.VarChar, 20));
                    insert.Parameters.Add(new SqlParameter("@RAM", SqlDbType.Int));
                    conn.Open();
                    insert.Prepare();
                    insert.Parameters[0].Value = int.Parse(machineInfo.id);
                    insert.Parameters[1].Value = machineInfo.OSName;
                    insert.Parameters[2].Value = machineInfo.OSVersion;
                    insert.Parameters[3].Value = machineInfo.userName;
                    insert.Parameters[4].Value = machineInfo.manufacturer;
                    insert.Parameters[5].Value = machineInfo.model;
                    insert.Parameters[6].Value = machineInfo.BIOSVersion;
                    insert.Parameters[7].Value = machineInfo.BIOSNumber;
                    insert.Parameters[8].Value = machineInfo.macAddress;
                    insert.Parameters[9].Value = int.Parse(machineInfo.ramAmount);
                    using (SqlDataReader reader = insert.ExecuteReader())
                    {
                        string output = "";
                        while (reader.Read())
                        {
                            output = reader["Info"].ToString();
                        }
                        conn.Close();
                        return output;
                    }

                }
            } catch (Exception e)
            {
                return e.Message;
            }
        }

        DatabaseOutput IDatabase_Service.addComputer(string ip, string machineName)
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
                        DatabaseOutput output = new DatabaseOutput();
                        while (reader.Read())
                        {
                            output.id = reader["Id"].ToString();
                            output.info = reader["Info"].ToString();
                        }
                        conn.Close();
                        return output;
                    }
                }
            } catch (Exception e)
            {
                return new DatabaseOutput() { id = "-1", info = e.Message};
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
                SmtpClient smtp = new SmtpClient("mail.smtp2go.com");
                smtp.Port = 2525;
                smtp.Credentials = new NetworkCredential("giked83069@finxmail.com", "PMbMInSthgwI");

                mail.From = new MailAddress("giked83069@finxmail.com");
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

        public List<FAQOutput> getFAQ()
        {
            try
            {
                //checkHeaders();
                string connectionString = @"Server=tcp:kutak-rock.database.windows.net,1433;Initial Catalog=Kutak Rock Ticketing;Persist Security Info=False;User ID=Kutak_Rock_WCF;Password=7rM-mg!E-7Nh>J8q;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand query = new SqlCommand(null, conn);
                    query.CommandText = "exec getFAQ";
                    conn.Open();
                    query.Prepare();
                    using (SqlDataReader reader = query.ExecuteReader())
                    {
                        List<FAQOutput> output = new List<FAQOutput>();
                        while (reader.Read())
                        {
                            string outputQuestion = reader["Question"].ToString();
                            string outputDescription = reader["Description"].ToString();
                            string[] outputAnswers = reader["Answers"].ToString().Split('|');
                            output.Add(new FAQOutput { question = outputQuestion, description = outputDescription, answers = outputAnswers});
                        }
                        conn.Close();
                        return output;
                    }
                }
            }
            catch (Exception e)
            {
                return new List<FAQOutput>();
            }
        }

        private bool checkHeaders()
        {
            IncomingWebRequestContext request = WebOperationContext.Current.IncomingRequest;
            WebHeaderCollection headers = request.Headers;
            MessageBox.Show(request.Method + " " + request.UriTemplateMatch.RequestUri.AbsolutePath);
            foreach (string headerName in headers.AllKeys)
            {
                MessageBox.Show(headerName + ": " + headers[headerName]);
            }

            return false;
        }
        private string EncryptString(string key, string plainInput)
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

        private static string DecryptString(string key, string cipherText)
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
