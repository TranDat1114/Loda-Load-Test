using System;
using System.IO;
using System.Text.Json;

namespace Loda.Share;

public class AppSetting
{
    public int Timeout { get; set; } = 10000;
    public int ThreadCount { get; set; } = 10;
    public Enum.LogLevel LogLevel { get; set; } = Enum.LogLevel.Info;
    public int Retry { get; set; } = 1;
    public Enum.LoadStrategy LoadStrategy { get; set; } = Enum.LoadStrategy.Concurrent;
    public AppLanguage Language { get; set; } = AppLanguage.English;
}

public static class SettingHelper
{
    private static readonly string SettingFile = "appsettings.json";
    public static AppSetting Current { get; private set; }

    static SettingHelper()
    {
        Load();
    }

    public static void Load()
    {
        if (File.Exists(SettingFile))
        {
            var json = File.ReadAllText(SettingFile);
            var options = new JsonSerializerOptions();
            options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase, false));
            // Đọc thủ công để ép kiểu int cho enum
            var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var setting = new AppSetting();
            setting.Timeout = root.GetProperty("Timeout").GetInt32();
            setting.ThreadCount = root.GetProperty("ThreadCount").GetInt32();
            setting.LogLevel = (Enum.LogLevel)root.GetProperty("LogLevel").GetInt32();
            setting.Retry = root.GetProperty("Retry").GetInt32();
            setting.LoadStrategy = (Enum.LoadStrategy)root.GetProperty("LoadStrategy").GetInt32();
            setting.Language = (AppLanguage)root.GetProperty("Language").GetInt32();
            Current = setting;
        }
        else
        {
            Current = new AppSetting();
            Save(); // Tạo file appsettings.json mặc định
        }
        // Đồng bộ ngôn ngữ với Localization
        Localization.CurrentLanguage = Current.Language;
    }

    public static void Save()
    {
        // Serialize enum as int
        var obj = new
        {
            Timeout = Current.Timeout,
            ThreadCount = Current.ThreadCount,
            LogLevel = (int)Current.LogLevel,
            Retry = Current.Retry,
            LoadStrategy = (int)Current.LoadStrategy,
            Language = (int)Current.Language
        };
        var json = JsonSerializer.Serialize(obj, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(SettingFile, json);
    }
}

