using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NuGet.Configuration;
using NuGet.Packaging;
using NuGet.Packaging.Signing;
using NuGet.ProjectManagement;

namespace EventBuilder.NuGet
{
    /// <summary>
    /// Context for handling logging inside the nuget project system.
    /// </summary>
    internal class NuGetProjectContext : INuGetProjectContext
    {
        public NuGetProjectContext(ISettings settings)
        {
            var nuGetLogger = new NuGetLogger();
            PackageExtractionContext = new PackageExtractionContext(
                PackageSaveMode.Files,
                XmlDocFileSaveMode.None,
                ClientPolicyContext.GetClientPolicy(settings, nuGetLogger),
                nuGetLogger);
        }

        /// <inheritdoc />
        public PackageExtractionContext PackageExtractionContext { get; set; }

        /// <inheritdoc />
        public XDocument OriginalPackagesConfig { get; set; }

        /// <inheritdoc />
        public NuGetActionType ActionType { get; set; }

        /// <inheritdoc />
        public Guid OperationId { get; set; }

        /// <inheritdoc />
        public ISourceControlManagerProvider SourceControlManagerProvider => null;

        /// <inheritdoc />
        public ExecutionContext ExecutionContext => null;

        /// <inheritdoc />
        public void Log(MessageLevel level, string message, params object[] args)
        {
            switch (level)
            {
                case MessageLevel.Warning:
                    Serilog.Log.Warning(message, args);
                    break;
                case MessageLevel.Error:
                    Serilog.Log.Error(message, args);
                    break;
                case MessageLevel.Info:
                    Serilog.Log.Information(message, args);
                    break;
                case MessageLevel.Debug:
                    Serilog.Log.Debug(message, args);
                    break;
                default:
                    Serilog.Log.Verbose(message, args);
                    break;
            }
        }

        /// <inheritdoc />
        public FileConflictAction ResolveFileConflict(string message) => FileConflictAction.Ignore;

        /// <inheritdoc />
        public void ReportError(string message)
        {
            Serilog.Log.Error(message);
        }
    }
}
