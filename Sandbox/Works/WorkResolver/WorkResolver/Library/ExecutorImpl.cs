namespace WorkResolver.Library
{
    using System;
    using System.Data;

    public class ExecutorImpl : IExecutor
    {
        public void Execute(IDbConnection con)
        {
            Console.WriteLine("Execute");
        }
    }
}
