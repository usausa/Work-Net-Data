using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace WorkTask
{
    public class WorkBuildTask : Task
    {
        [Required]
        public ITaskItem[] SourceFiles { get; set; }

        [Required]
        public ITaskItem OutputFile { get; set; }

        public override bool Execute()
        {
            Log.LogMessage("Work build test.");
            Log.LogMessage("[SourceFiles]");
            foreach (var sourceFile in SourceFiles)
            {
                Log.LogMessage($"  {sourceFile.ItemSpec}");
            }

            return true;
        }
    }
}
