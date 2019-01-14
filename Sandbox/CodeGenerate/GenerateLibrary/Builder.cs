namespace GenerateLibrary
{
    using System;

    public class Builder
    {
        public Factory ToFactory(Type type)
        {
            // TODO
            return new Factory(null, new Engine());
        }
    }
}
