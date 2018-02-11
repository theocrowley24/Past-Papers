using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using PastPapers.Helpers;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using MimeKit;
using MailKit.Net.Smtp;

namespace PastPapers.Models
{
    public class LoginModel
    {
        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string Password { get; set; }

        public bool LoginSuccess { get; set; }
        public string LoginErrorMessage { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3)]
        public string NewUsername { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [Required]
        [StringLength(60, MinimumLength = 3)]
        [DataType(DataType.EmailAddress)]
        public string NewEmail { get; set; }

        public bool RegisterSuccess { get; set; }
        public string RegisterErrorMessage { get; set; }

        public HttpContext HttpContext { get; set; }

        public LoginModel()
        {
            LoginSuccess = false;
            RegisterSuccess = false;
        }

        /// <summary>
        /// Removes all < and > from attributes of LoginModel to prevent XSS
        /// </summary>
        private void SanitizeAttributes()
        {
            if (Username !=null) Username = Regex.Replace(Username, @"<[^>]*>", String.Empty);
            if (Password != null) Password = Regex.Replace(Password, @"<[^>]*>", String.Empty);
            if (NewUsername != null) NewUsername = Regex.Replace(NewUsername, @"<[^>]*>", String.Empty);
            if (NewEmail != null) NewEmail = Regex.Replace(NewEmail, @"<[^>]*>", String.Empty);
            if (NewPassword != null) NewPassword = Regex.Replace(NewPassword, @"<[^>]*>", String.Empty);
        }

        /// <summary>
        /// Attempts to create a user account, to check it will update the registerSuccess attribute if it was successful
        /// if not the registerErrorMessage will contain what the problem was.
        /// </summary>
        public void AttemptRegister()
        {
            SanitizeAttributes();

            try
            {
                DatabaseContext context = HttpContext.RequestServices.GetService(typeof(PastPapers.Models.DatabaseContext)) as DatabaseContext;
                MySqlConnection connection = context.GetConnection();
                connection.Open();
                string sql = "INSERT INTO users (id, username, password, email) VALUES (DEFAULT, @username, @password, @email)";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@username", NewUsername);
                cmd.Parameters.AddWithValue("@password", SecurePasswordHasher.Hash(NewPassword));
                cmd.Parameters.AddWithValue("@email", NewEmail);

                if (cmd.ExecuteNonQuery() == 0)
                {
                    System.Diagnostics.Debug.WriteLine("Account failed to create!");
                    RegisterSuccess = false;
                } else
                {
                    /*
                    var message = new MimeMessage();
                    message.From.Add(new MailboxAddress("Past Papers", "noreply@pastpapers.com"));
                    message.To.Add(new MailboxAddress(NewUsername, NewEmail));
                    message.Subject = "Past Papers account verification";

                    message.Body = new TextPart("plain")
                    {
                        Text = @"Hey " + NewUsername + ", To activate your new account please click the link below"
                    };

                    using (var client = new SmtpClient())
                    {
                        client.ServerCertificateValidationCallback = (s, c, h, e) => true;

                        client.Connect("smpt", 587, false);

                        client.Send(message);
                        client.Disconnect(true);
                    }
                    */

                    RegisterSuccess = true;
                }

                connection.Close();

            } catch (Exception ex)
            {
                RegisterErrorMessage = "Error connecting to server";
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Attempts to log in to a user account, it will update the loginSuccess boolean.
        /// If login fails the loginErrorMessage attribute contains why.
        /// </summary>
        public void AttemptLogin()
        {
            SanitizeAttributes();

            try
            {
                DatabaseContext context = HttpContext.RequestServices.GetService(typeof(PastPapers.Models.DatabaseContext)) as DatabaseContext;
                MySqlConnection connection = context.GetConnection();
                connection.Open();
                string sql = "SELECT password FROM users WHERE username=@username";
                MySqlCommand cmd = new MySqlCommand(sql, connection);
                cmd.Prepare();
                cmd.Parameters.AddWithValue("@username", Username);
                MySqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    string dbPassword = reader.GetString(0);

                    if (SecurePasswordHasher.Verify(Password, dbPassword))
                    {
                        LoginSuccess = true;
                        HttpContext.Session.SetString("username", Username);
                        
                    } else
                    {
                        LoginSuccess = false;
                        LoginErrorMessage = "Incorrect password";
                    }
                } else
                {
                    LoginErrorMessage = "Unknown username";
                }

                reader.Close();
                connection.Close();

            } catch (Exception ex)
            {
                LoginErrorMessage = "Error connecting to server";
                System.Diagnostics.Debug.WriteLine(ex.Message);
            }
            
        }
    }
}
