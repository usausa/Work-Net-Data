namespace WorkGenerator
{
    using System.IO;

    using WorkGenerator.Generators.Misc;

    public static class Program
    {
        public static void Main(string[] args)
        {
            var source = File.ReadAllText("ISampleDao.cs");
            var generator = new MiscCodeGenerator(source);

            generator.Test();
        }
    }
}
