using Newtonsoft.Json.Linq;
using System;
using System.Text;

namespace ExampleTools
{
    public static class ExampleHandler
    {
        public static string ExampleToolNoParams(out int exitCode)
        {
            exitCode = 0;

            try
            {
                // TODO: implement tool logic here
                return "example_tool_no_params result";
            }
            catch (Exception ex)
            {
                exitCode = -1;
                return "Error: " + ex.Message;
            }
        }

        public static string ExampleToolWithParams(string requiredParam, string optionalParam, string configString, int configInt, bool configBool, out int exitCode)
        {
            exitCode = 0;

            try
            {
                // TODO: implement tool logic here
                return "example_tool_with_params result";
            }
            catch (Exception ex)
            {
                exitCode = -1;
                return "Error: " + ex.Message;
            }
        }

        public static string ExampleToolWithArrayParam(JArray items, out int exitCode)
        {
            exitCode = 0;

            try
            {
                if (items == null || items.Count == 0)
                {
                    exitCode = 1;
                    return "error: missing or empty 'items' argument.";
                }

                StringBuilder result = new StringBuilder();
                result.Append("Received ").Append(items.Count).Append(" item(s):");
                foreach (JToken token in items)
                {
                    JObject obj = token as JObject;
                    if (obj == null) continue;
                    result.Append("\n- ")
                        .Append((string)obj["label"] ?? "")
                        .Append(": ")
                        .Append((string)obj["value"] ?? "");
                }
                return result.ToString();
            }
            catch (Exception ex)
            {
                exitCode = -1;
                return "Error: " + ex.Message;
            }
        }

        public static string GetContext(string configString, int configInt, bool configBool)
        {
            return "Example Tools context: exampleString=" + configString
                + ", exampleInt=" + configInt
                + ", exampleBool=" + (configBool ? "true" : "false");
        }
    }
}
