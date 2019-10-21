using System;
using System.Diagnostics;
using System.IO;
using Codex.Analysis.Managed;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.CodeAnalysis;

namespace CompilerFileAccessLogger
{
    public class CompilerFileAccessLogger : Logger
    {
        public override void Initialize(IEventSource eventSource)
        {
            eventSource.MessageRaised += EventSourceOnMessageRaised;
        }

        private void EventSourceOnMessageRaised(object sender, BuildMessageEventArgs e)
        {
            if (e is TaskCommandLineEventArgs commandLine && commandLine.CommandLine.IndexOf("csc.exe", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                //Debugger.Launch();
                var cscPosition = commandLine.CommandLine.IndexOf("csc.exe");
                var cscArgString = commandLine.CommandLine.Substring(cscPosition + 8);

                var ci = new CompilerInvocation()
                {
                    CommandLine = cscArgString,
                    Language = LanguageNames.CSharp,
                    ProjectFile = commandLine.ProjectFile
                };

                var args = ci.GetParsedCommandLineArguments();
            }
        }

        private void VisitFiles(CommandLineArguments args)
        {
            foreach (var file in args.SourceFiles)
            {
                VisitInput(file.Path);
            }

            VisitInput(args.AppConfigPath);
            VisitInput(args.DocumentationPath);
            VisitOutput(args.OutputFileName);
            
            // TODO: Add the rest
        }

        private void VisitOutput(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                using (File.OpenWrite(filePath)) { }
            }
        }

        private void VisitInput(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                using (File.OpenRead(filePath)) { }
            }
            
        }
    }
}
