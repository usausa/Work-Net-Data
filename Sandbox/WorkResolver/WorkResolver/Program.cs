namespace WorkResolver
{
    public static class Program
    {
        public static void Main()
        {
            //WorkResolver.Usage.SmartResolverUsage.Usage.UseSimple();
            WorkResolver.Usage.ServiceCollectionUsage.Usage.UseSimple();
            WorkResolver.Usage.ServiceCollectionUsage.Usage.UseMultiple();
            WorkResolver.Usage.StandardUsage.Usage.Use();
        }
    }
}
