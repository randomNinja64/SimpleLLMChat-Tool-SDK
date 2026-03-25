# SimpleLLMChat Tool SDK
This repository provides a starting point for anyone wishing to develop tools for [SimpleLLMChat](https://github.com/randomNinja64/SimpleLLMChat).

# The Tool Format
SimpleLLMChat tools are packaged as an executable and an associated JSON file of the same base name (The specific formatting is explained in "JSON Manifest Format"). Any needed dependencies should be placed in the same folder as the tool. Tools themselves can be written in many languages, but the official example implementation, like the main program, is written in C# targeting .NET 4.0.

# JSON Manifest Format

- `name` - Display name for the tool package, shown as a heading in the SimpleLLMChat options window (e.g. "Example Tools")
- `executable` - Filename of the executable (e.g. "ExampleTools.exe")
- `options` - Array of options to be configured by the user within the SimpleLLMChat UI; each object in the array contains the following fields
  - `name` - Key used to read the option's value in code (e.g. `"exampleOption"`)
  - `label` - Name of the option within the UI (e.g. `"Example Option"`)
  - `type` - `"string"`, `"int"`, or `"bool"`; strings and ints display as textboxes in the GUI; bools display as checkboxes (saved as 0/1 internally)
  - `default` - Default value for a config option; stored as a JSON string, regardless of option type (e.g. `"0"`)
- `tools` - Array of tools made available by the package; each object in the array contains the following fields
  - `name` - Tool name; passed to the executable as an argument when the tool is invoked (e.g. `"example_tool_no_params"`)
  - `description` - Description passed to the LLM so that the AI knows when/how to use the tool (e.g. `"A tool that adds two numbers together"`)
  - `parameters` - Array containing parameter objects (can be empty if no parameters are required), each object in the array contains the following fields
    - `name` - Key passed to the LLM so that it knows what to name the parameter; extracted from arguments JSON by the executable (e.g. `"search_query"`)
    - `type` - JSON Schema type passed to the LLM (e.g. `"string"`, `"number"`) (If omitted, the default is `"string"`)
    - `description` - Passed to the LLM so that it knows what value to provide for this argument (e.g. `"The query to search for"`)
    - `required` - Boolean that tells the LLM whether the parameter is required when calling the tool