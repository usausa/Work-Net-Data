namespace Smart.Data.Accessor.Generator
{
    using Xunit;

    public class ParameterNamesTest
    {
        [Fact]
        public void GetParameterNames()
        {
            Assert.Equal("p0", ParameterNames.GetParameterName(0));
            Assert.Equal("p255", ParameterNames.GetParameterName(255));
            Assert.Equal("p256", ParameterNames.GetParameterName(256));
        }
    }
}
