namespace Smart
{
    using System.IO;
    using System.Linq;

    using Microsoft.Build.Framework;
    using Microsoft.Build.Utilities;

    public sealed class AccessorGenerateTask : Task
    {
        [Required]
        public string BaseDirectory { get; set; }

        [Required]
        public string OutputFile { get; set; }

        [Required]
        public ITaskItem[] SourceFiles { get; set; }

        public override bool Execute()
        {
            var baseDir = new DirectoryInfo(BaseDirectory);
            var outputFile = new DirectoryInfo(OutputFile);
            var sourceFiles = SourceFiles
                .Select(x => new FileInfo(x.ItemSpec))
                .Where(x => x.FullName.StartsWith(baseDir.FullName + "\\"));

            Log.LogMessage("[baseDir]");
            Log.LogMessage(baseDir.FullName);
            Log.LogMessage("[outputFile]");
            Log.LogMessage(outputFile.FullName);
            Log.LogMessage("[sourceFiles]");
            foreach (var sourceFile in sourceFiles)
            {
                Log.LogMessage(sourceFile.FullName);
            }

            return true;
        }
    }
}
