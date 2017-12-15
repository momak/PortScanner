using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Fclp;

namespace PortScanner
{
    class CommandLineOptions
    {
        public string StartIpAddress { get; set; }

        public string EndIpAddress { get; set; }

        public string OutputLocation { get; set; }

        public int MaxThreadCount { get; set; }
        
        public static FluentCommandLineParser<CommandLineOptions> Setup()
        {
            var p = new FluentCommandLineParser<CommandLineOptions>();

            p.Setup(arg => arg.StartIpAddress)
                .As('s', "StartIP")
                .Required()
                .WithDescription("Starting IP address");

            p.Setup(arg => arg.EndIpAddress)
                .As('e', "EndIp")
                .Required()
                .WithDescription("Ending IP address");

            p.Setup(arg => arg.OutputLocation)
                .As('o', "Output")
                .WithDescription("Output location")
                .SetDefault(Directory.GetCurrentDirectory() + "\\Output");

            p.Setup(arg => arg.MaxThreadCount)
                .As('t', "MaxThreadCount")
                .WithDescription("Maximal number of threads")
                .Required()
                .SetDefault(50);

            p.SetupHelp("h", "help", "?")
                .Callback(text =>
                {
                    Console.WriteLine();
                    Console.Write("Command line options:");

                    Console.WriteLine(text);
                });

            return p;
        }

        public static bool HandleStandardResult(ICommandLineParserResult result)
        {
            if (result.HelpCalled)
                return true;

            if (result.HasErrors)
            {
                Console.Error.WriteLine(result.ErrorText);
                Console.Error.WriteLine($"Type \"{Assembly.GetExecutingAssembly().GetName().Name} --help\" to show the command line syntax.");

                return true;
            }

            foreach (var option in result.AdditionalOptionsFound)
            {
                Console.Error.WriteLine(string.IsNullOrEmpty(option.Value)
                    ? $"Ignoring option {(option.Key.Length > 1 ? "--" : "-")}{option.Key}"
                    : $"Ignoring option {(option.Key.Length > 1 ? "--" : "-")}{option.Key}='{option.Value}'");
            }

            return false;
        }
    }
}
