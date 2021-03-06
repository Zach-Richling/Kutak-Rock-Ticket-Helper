﻿using System;
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
        string connectionString = @"Server=tcp:kutak-rock.database.windows.net,1433;Initial Catalog=Kutak Rock Ticketing;Persist Security Info=False;User ID=Kutak_Rock_WCF;Password=7rM-mg!E-7Nh>J8q;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
        MachineInfo IDatabase_Service.getMachineinfo(string id)
        {
            try
            {
                //string connectionString = @"Server=tcp:kutak-rock.database.windows.net,1433;Initial Catalog=Kutak Rock Ticketing;Persist Security Info=False;User ID=Kutak_Rock_WCF;Password=7rM-mg!E-7Nh>J8q;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand insert = new SqlCommand(null, conn);
                    insert.CommandText = "exec GetMachineInfo @Id;";
                    insert.Parameters.Add(new SqlParameter("@Id", SqlDbType.Int));
                    conn.Open();
                    insert.Prepare();
                    insert.Parameters[0].Value = int.Parse(id);
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
            }
            catch (Exception e)
            {
                return new MachineInfo() { id = "-1" };
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
            try
            {
                MachineInfo machineInfo = JsonConvert.DeserializeObject<MachineInfo>(machineInfoInput);
                //string connectionString = @"Server=tcp:kutak-rock.database.windows.net,1433;Initial Catalog=Kutak Rock Ticketing;Persist Security Info=False;User ID=Kutak_Rock_WCF;Password=7rM-mg!E-7Nh>J8q;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
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
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        DatabaseOutput IDatabase_Service.addComputer(string ip, string machineName)
        {
            try
            {
                //string connectionString = @"Server=tcp:kutak-rock.database.windows.net,1433;Initial Catalog=Kutak Rock Ticketing;Persist Security Info=False;User ID=Kutak_Rock_WCF;Password=7rM-mg!E-7Nh>J8q;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
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
            }
            catch (Exception e)
            {
                return new DatabaseOutput() { id = "-1", info = e.Message };
            }
        }
        string IDatabase_Service.createTicket(string ticketName, string ticketDesc, string ticketSeverity, string computerID)
        {
            try
            {
                //string connectionString = @"Server=tcp:kutak-rock.database.windows.net,1433;Initial Catalog=Kutak Rock Ticketing;Persist Security Info=False;User ID=Kutak_Rock_WCF;Password=7rM-mg!E-7Nh>J8q;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
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
            }
            catch (Exception e)
            {
                return "-1";
            }
        }

        public string sendTicket(string ticketId)
        {
            try
            {
                //string connectionString = @"Server=tcp:kutak-rock.database.windows.net,1433;Initial Catalog=Kutak Rock Ticketing;Persist Security Info=False;User ID=Kutak_Rock_WCF;Password=7rM-mg!E-7Nh>J8q;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand insert = new SqlCommand(null, conn);
                    insert.CommandText = "exec GetTicket @TicketId";
                    insert.Parameters.Add(new SqlParameter("@TicketId", SqlDbType.Int));
                    conn.Open();
                    insert.Prepare();
                    insert.Parameters[0].Value = int.Parse(ticketId);
                    using (SqlDataReader reader = insert.ExecuteReader())
                    {
                        string output = "";
                        while (reader.Read())
                        {
                            output = "Submitted from " + reader["MachineName"].ToString() + " at " + reader["ExternalIP"].ToString() + Environment.NewLine +
                                     "Ticket Name: " + reader["TicketName"].ToString() + Environment.NewLine +
                                     "Ticket Description: " + reader["TicketDescription"].ToString() + Environment.NewLine +
                                     "OS Name: " + reader["OSName"].ToString() + Environment.NewLine +
                                     "OS Version: " + reader["OSVersion"].ToString() + Environment.NewLine +
                                     "Username: " + reader["Username"].ToString() + Environment.NewLine +
                                     "Manufacturer: " + reader["Manufacturer"].ToString() + Environment.NewLine +
                                     "Model: " + reader["Model"].ToString() + Environment.NewLine +
                                     "BIOS Version: " + reader["BIOSVersion"].ToString() + Environment.NewLine +
                                     "MAC Address: " + reader["MACAddress"].ToString() + Environment.NewLine +
                                     "RAM Amount: " + reader["RAM"].ToString() + " GB" + Environment.NewLine;
                        }
                        conn.Close();

                        //THE BELOW STILL NEEDS TO BE CONFIGURED
                        /*
                        MailMessage mail = new MailMessage();
                        SmtpClient smtp = new SmtpClient("SMTP SERVER HERE");
                        smtp.Port = 2525;
                        smtp.Credentials = new NetworkCredential("EMAIL HERE", "PASSWORD HERE");
                        mail.From = new MailAddress("EMAIL HERE");
                        mail.To.Add("WHO YOU WANT TO SEND TO");
                        mail.Subject = "Ticket Submission: No. " + ticketId;
                        mail.Body = output;
						smtp.Send(mail);
                        */
                        return "true";
                    }
                }
            }
            catch (Exception e)
            {
                return "false";
            }
        }

        public List<FAQOutput> getFAQ()
        {
            try
            {
                //string connectionString = @"Server=tcp:kutak-rock.database.windows.net,1433;Initial Catalog=Kutak Rock Ticketing;Persist Security Info=False;User ID=Kutak_Rock_WCF;Password=7rM-mg!E-7Nh>J8q;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
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
                            int outputId = int.Parse(reader["Id"].ToString());
                            int outputCount = int.Parse(reader["QuestionCount"].ToString());
                            string[] outputAnswers = reader["Answers"].ToString().Split('|');
                            output.Add(new FAQOutput { question = outputQuestion, description = outputDescription, answers = outputAnswers, id = outputId });
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

        public int createFAQ(string question, string desc)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand insert = new SqlCommand(null, conn);
                    insert.CommandText = "exec CreateQuestion @Question, @Description";
                    insert.Parameters.Add(new SqlParameter("@Question", SqlDbType.VarChar, 200));
                    insert.Parameters.Add(new SqlParameter("@Description", SqlDbType.VarChar, 200));
                    conn.Open();
                    insert.Prepare();
                    insert.Parameters[0].Value = question;
                    insert.Parameters[1].Value = desc;
                    using (SqlDataReader reader = insert.ExecuteReader())
                    {
                        int output = -1;
                        while (reader.Read())
                        {
                            output = int.Parse(reader["Id"].ToString());
                            break;
                        }
                        conn.Close();
                        return output;
                    }
                }
            }
            catch (Exception e)
            {
                return -1;
            }
        }

        public string createFAQAnswer(string questionId, string answer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    SqlCommand insert = new SqlCommand(null, conn);
                    insert.CommandText = "exec CreateAnswer @QuestionId, @Answer";
                    insert.Parameters.Add(new SqlParameter("@QuestionId", SqlDbType.Int));
                    insert.Parameters.Add(new SqlParameter("@Answer", SqlDbType.VarChar, 200));
                    conn.Open();
                    insert.Prepare();
                    insert.Parameters[0].Value = questionId;
                    insert.Parameters[1].Value = answer;
                    using (SqlDataReader reader = insert.ExecuteReader())
                    {
                        string output = "";
                        while (reader.Read())
                        {
                            output = reader["Output"].ToString();
                            break;
                        }
                        conn.Close();
                        return output;
                    }
                }
            }
            catch (Exception e)
            {
                return "Error";
            }
        }

        public string GetComputer(string ip)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand query = new SqlCommand(null, conn);
                query.CommandText = "exec GetComputerByIP @ExternalIP";
                query.Parameters.Add(new SqlParameter("@ExternalIP", SqlDbType.VarChar, 20));
                conn.Open();
                query.Prepare();
                query.Parameters[0].Value = ip;
                using (SqlDataReader reader = query.ExecuteReader())
                {
                    string output = "1";
                    while (reader.Read())
                    {
                        output = reader["Id"].ToString();
                        break;
                    }
                    conn.Close();
                    return output;
                }
            }
        }
    }
}
