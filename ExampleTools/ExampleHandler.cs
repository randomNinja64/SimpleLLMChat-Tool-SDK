using System;

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
    }
}
