using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Build.Utilities;

namespace WorkTask
{
    public class WorkBuildTask : Task
    {
        public override bool Execute()
        {
            Log.LogMessage("Work build test.");
            return true;
        }
    }
}
