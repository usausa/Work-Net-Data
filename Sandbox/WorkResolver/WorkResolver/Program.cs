namespace WorkResolver
{
    using WorkResolver.Usage;

    public static class Program
    {
        public static void Main()
        {
            ServiceCollectionUsage.UseSimple();
            ServiceCollectionUsage.UseMultiple();
            StandardUsage.Use();
        }
    }
}
