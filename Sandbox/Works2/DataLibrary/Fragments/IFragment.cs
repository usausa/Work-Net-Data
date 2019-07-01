namespace DataLibrary.Fragments
{
    public interface IFragment
    {
        bool IsDynamic(IFragmentContext context);

        void Build(IFragmentCodeBuilder builder);
    }
}
