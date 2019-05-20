namespace WorkScript
{
    using System.Diagnostics;

    using WorkLibrary;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var generator = new Generator();
            var executor = generator.Create<IExecutor>();
            var ret = executor.Add(1, 2);
            Debug.WriteLine(ret);

            var executor2 = generator.Create<IExecutor>();
            Debug.Assert(executor == executor2);

            var subExecutor = generator.Create<Sub.IExecutor>();
            var ret2 = subExecutor.Minus(1, 2);
            Debug.WriteLine(ret2);

            // Different assembly
            Debug.WriteLine(executor.GetType().FullName);
            Debug.WriteLine(executor.GetType().Assembly.FullName);
            Debug.WriteLine(subExecutor.GetType().FullName);
            Debug.WriteLine(subExecutor.GetType().Assembly.FullName);
        }
    }
}
