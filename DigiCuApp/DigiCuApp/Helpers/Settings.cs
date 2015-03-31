// Helpers/Settings.cs

using System;
using Refractored.Xam.Settings;
using Refractored.Xam.Settings.Abstractions;

namespace DigicuApp.Helpers
{
  /// <summary>
  /// This is the Settings static class that can be used in your Core solution or in any
  /// of your client applications. All settings are laid out the same exact way with getters
  /// and setters. 
  /// </summary>
  public static class Settings
  {
    private static ISettings AppSettings
    {
      get
      {
        return CrossSettings.Current;
      }
    }

    #region Setting Constants

    private const string SettingsKey = "settings_key";
    private static readonly string SettingsDefault = string.Empty;
    
    private const string pinKey = "pin_key";
    private static readonly string pinDefault = string.Empty;
    private const string secretKey = "secret_key";
    private static readonly string secretDefault = String.Empty;
    

    #endregion


    public static string GeneralSettings
    {
      get
      {
        return AppSettings.GetValueOrDefault(SettingsKey, SettingsDefault);
      }
      set
      {
        AppSettings.AddOrUpdateValue(SettingsKey, value);
      }
    }

    public static string PinSettings
    {
        get
        {
            return AppSettings.GetValueOrDefault(pinKey, pinDefault);
        }
        set
        {
            AppSettings.AddOrUpdateValue(pinKey, value);
        }
    }

    public static string SecretSettings
    {
        get
        {
            return AppSettings.GetValueOrDefault(secretKey, secretDefault);
        }
        set
        {
            AppSettings.AddOrUpdateValue(secretKey, value);
        }
    }

  }
}