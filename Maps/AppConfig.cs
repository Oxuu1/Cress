using System.IO;
using System;

public static class AppConfig
{
    private static readonly string appDataPath;
    private static readonly string configFilePath;

    static AppConfig()
    {
        appDataPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Cress");
        configFilePath = Path.Combine(appDataPath, "config.txt");

        // Create the AppData folder if it doesn't exist
        if (!Directory.Exists(appDataPath))
        {
            Directory.CreateDirectory(appDataPath);
        }

        // Create the config file if it doesn't exist
        if (!File.Exists(configFilePath))
        {
            SaveDefaultConfig();
        }
    }

    public static string GetUserFilePath()
    {
        // Load the user file path from the config
        return File.ReadAllText(configFilePath).Trim();
    }

    public static void SaveUserFilePath(string userFilePath)
    {
        // Save the user file path to the config
        File.WriteAllText(configFilePath, userFilePath);
    }

    private static void SaveDefaultConfig()
    {
        // Save default config values
        SaveUserFilePath(Path.Combine(appDataPath, "users.json"));
    }
}