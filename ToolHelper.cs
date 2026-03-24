using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

/// <summary>
/// Shared helper utilities for tool executable projects.
/// </summary>
public static class ToolHelper
{
    // Loads option defaults from the manifest file sitting next to the executable.
    // Must be called before stdin config is parsed so that stdin values take precedence.
    public static void LoadManifestDefaults()
    {
        try
        {
            string manifestPath = Path.ChangeExtension(
                Assembly.GetEntryAssembly().Location, ".json");
            if (!File.Exists(manifestPath)) return;
            JObject manifest = JObject.Parse(File.ReadAllText(manifestPath));
            foreach (JToken opt in manifest["options"] ?? new JArray())
            {
                string name = opt["name"]?.ToString();
                string def  = opt["default"]?.ToString();
                if (!string.IsNullOrEmpty(name) && def != null)
                    Config[name] = def;
            }
        }
        catch { }
    }

    // Config values passed via the "config" key in stdin JSON
    public static Dictionary<string, string> Config
        = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    public static string GetConfigString(string key)
    {
        string value;
        if (Config.TryGetValue(key, out value))
            return value;
        return "";
    }

    public static int GetConfigInt(string key, int defaultValue)
    {
        string value;
        if (Config.TryGetValue(key, out value))
        {
            int result;
            if (int.TryParse(value, out result))
                return result;
        }
        return defaultValue;
    }

    public static string GetRequiredArg(string arguments, string argName)
    {
        string value = JsonExtractString(arguments, argName)?.Trim() ?? "";
        if (string.IsNullOrEmpty(value))
            throw new ArgumentException("missing '" + argName + "' argument.");
        return value;
    }

    public static string JsonExtractString(string json, string key)
    {
        if (string.IsNullOrEmpty(json) || string.IsNullOrEmpty(key))
        {
            return "";
        }

        try
        {
            string trimmedJson = json.Trim();
            if (trimmedJson.Length == 0)
            {
                return "";
            }

            JToken root = JToken.Parse(trimmedJson);
            if (root.Type == JTokenType.Object)
            {
                JObject obj = (JObject)root;
                JToken token;
                if (!obj.TryGetValue(key, out token))
                {
                    foreach (JProperty property in obj.Properties())
                    {
                        if (string.Equals(property.Name, key, StringComparison.OrdinalIgnoreCase))
                        {
                            token = property.Value;
                            break;
                        }
                    }
                }

                if (token != null && token.Type != JTokenType.Null)
                {
                    return token.Type == JTokenType.String ? token.Value<string>() ?? "" : token.ToString();
                }
            }
        }
        catch
        {
        }

        return "";
    }
}
