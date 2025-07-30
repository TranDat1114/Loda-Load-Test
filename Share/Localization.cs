
using System;
using System.Collections.Generic;
using Loda.Share.Enum;

namespace Loda.Share
{
    public static class Localization
    {
        // Sự kiện đổi ngôn ngữ
        public static event Action OnLanguageChanged;
    
        public static AppLanguage CurrentLanguage { get; set; } = AppLanguage.English;

        private static Dictionary<string, string> _en = new();
        private static Dictionary<string, string> _vi = new();
        private static bool _loaded = false;
        private static readonly string LangDir = "Language";

        public static void Load()
        {
            var langPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, LangDir);
            _en = LoadLangFile(Path.Combine(langPath, "lang.en.json"));
            _vi = LoadLangFile(Path.Combine(langPath, "lang.vi.json"));
            _loaded = true;
        }

        private static Dictionary<string, string> LoadLangFile(string file)
        {
            if (!File.Exists(file)) return new Dictionary<string, string>();
            var json = File.ReadAllText(file);
            return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
        }

        public static string T(string key)
        {
            if (!_loaded) Load();
            var dict = CurrentLanguage == AppLanguage.English ? _en : _vi;
            if (dict.TryGetValue(key, out var val))
                return val;
            return key;
        }

        public static void SetLanguage(AppLanguage lang)
        {
            CurrentLanguage = lang;
            Load(); // Nạp lại file ngôn ngữ ngay khi đổi ngôn ngữ
            SettingHelper.Current.Language = lang;
            SettingHelper.Save();
            OnLanguageChanged?.Invoke();
        }
    }
}
