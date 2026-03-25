using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ExampleTools
{
    internal class Program
    {
        static Dictionary<string, string> Config
            = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        static void LoadManifestDefaults()
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

        static string GetConfigString(string key)
        {
            string value;
            if (Config.TryGetValue(key, out value))
                return value;
            return "";
        }

        static int GetConfigInt(string key, int defaultValue)
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

        static string GetRequiredArg(string arguments, string argName)
        {
            string value = JsonExtractString(arguments, argName)?.Trim() ?? "";
            if (string.IsNullOrEmpty(value))
                throw new ArgumentException("missing '" + argName + "' argument.");
            return value;
        }

        static string JsonExtractString(string json, string key)
        {
            if (string.IsNullOrEmpty(json) || string.IsNullOrEmpty(key))
                return "";

            try
            {
                string trimmedJson = json.Trim();
                if (trimmedJson.Length == 0)
                    return "";

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
                        return token.Type == JTokenType.String ? token.Value<string>() ?? "" : token.ToString();
                }
            }
            catch { }

            return "";
        }

        static int Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            LoadManifestDefaults();

            if (args.Length < 1)
            {
                Console.Write("Usage: ExampleTools.exe <tool_name>\nArguments JSON is read from stdin.");
                return 1;
            }

            string toolName = args[0];

            // Read arguments JSON from stdin
            string stdinData = Console.In.ReadToEnd();
            string argumentsJson = "";

            if (!string.IsNullOrWhiteSpace(stdinData))
            {
                try
                {
                    JObject root = JObject.Parse(stdinData);

                    // Extract config if present
                    JObject configObj = root["config"] as JObject;
                    if (configObj != null)
                    {
                        foreach (JProperty prop in configObj.Properties())
                        {
                            Config[prop.Name] = prop.Value.ToString();
                        }
                    }

                    // Extract arguments if present
                    JToken argsToken = root["arguments"];
                    if (argsToken != null)
                    {
                        argumentsJson = argsToken.ToString();
                    }
                }
                catch
                {
                    // If parsing fails, treat whole stdin as arguments JSON
                    argumentsJson = stdinData;
                }
            }

            int exitCode = 0;
            string output = "";

            try
            {
                switch (toolName)
                {
                    case "example_tool_no_params":
                        output = ExampleHandler.ExampleToolNoParams(out exitCode);
                        break;

                    case "example_tool_with_params":
                        {
                            string requiredParam = GetRequiredArg(argumentsJson, "required_param");
                            string optionalParam = JsonExtractString(argumentsJson, "optional_param") ?? "";
                            string configString  = GetConfigString("exampleString");
                            int    configInt     = GetConfigInt("exampleInt", 42);
                            bool   configBool    = GetConfigInt("exampleBool", 0) == 1;
                            output = ExampleHandler.ExampleToolWithParams(requiredParam, optionalParam, configString, configInt, configBool, out exitCode);
                            break;
                        }

                    default:
                        output = "error: unknown tool '" + toolName + "'.";
                        exitCode = 1;
                        break;
                }
            }
            catch (Exception e)
            {
                // Catches errors that occur before the handler is reached,
                // e.g. GetRequiredArg throwing when a required parameter is missing.
                // Errors from inside handler methods should be caught there instead.
                output = "error: " + e.Message;
                exitCode = 1;
            }

            Console.Write(output);
            return exitCode;
        }
    }
}
