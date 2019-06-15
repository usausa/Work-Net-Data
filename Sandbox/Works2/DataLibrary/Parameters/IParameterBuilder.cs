using System.Data.Common;

namespace DataLibrary.Parameters
{
    public interface IParameterBuilder
    {
        void Build(DbParameter parameter);
    }
}
