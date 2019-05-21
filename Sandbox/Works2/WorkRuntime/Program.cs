namespace WorkRuntime
{
    using System;
    using System.Diagnostics;

    using WorkRuntimeLibrary;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var generator = new Generator();
            var executor = generator.Create<IExecutor>();
            var ret = executor.Add(1, 2);
            Console.WriteLine(ret);

            var executor2 = generator.Create<IExecutor>();
            Debug.Assert(executor == executor2);

            var subExecutor = generator.Create<Sub.IExecutor>();
            var ret2 = subExecutor.Minus(1, 2);
            Console.WriteLine(ret2);
        }
    }
}
