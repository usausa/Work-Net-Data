namespace WorkResolver.Usage.SmartResolverUsage
{
    using Smart.Resolver;

    using WorkResolver.Accessor;

    public class Usage
    {
        public static void UseSimple()
        {
            var config = new ResolverConfig();


            config.Bind<Service>().ToSelf();

            var resolver = config.ToResolver();

            var service = resolver.Get<Service>();
            service.Action();
        }

        // TODO missing resolver

        public class Service
        {
            private readonly IHogeDao hogeDao;

            public Service(IHogeDao hogeDao)
            {
                this.hogeDao = hogeDao;
            }

            public void Action()
            {
                hogeDao.Execute();
            }
        }
    }
}
