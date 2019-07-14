namespace ReaderBenchmark
{
    using System.Data;

    using ReaderBenchmark.Mock;

    public sealed class Int10Mapper
    {
        public IntEntity10 Map(IDataRecord reader)
        {
            var entity = new IntEntity10();
            entity.Id1 = Helper.GetValue<int>(reader, 0);
            entity.Id2 = Helper.GetValue<int>(reader, 1);
            entity.Id3 = Helper.GetValue<int>(reader, 2);
            entity.Id4 = Helper.GetValue<int>(reader, 3);
            entity.Id5 = Helper.GetValue<int>(reader, 4);
            entity.Id6 = Helper.GetValue<int>(reader, 5);
            entity.Id7 = Helper.GetValue<int>(reader, 6);
            entity.Id8 = Helper.GetValue<int>(reader, 7);
            entity.Id9 = Helper.GetValue<int>(reader, 8);
            entity.Id10 = Helper.GetValue<int>(reader, 9);
            return entity;
        }
    }

    public sealed class Int20Mapper
    {
        public IntEntity20 Map(IDataRecord reader)
        {
            var entity = new IntEntity20();
            entity.Id1 = Helper.GetValue<int>(reader, 0);
            entity.Id2 = Helper.GetValue<int>(reader, 1);
            entity.Id3 = Helper.GetValue<int>(reader, 2);
            entity.Id4 = Helper.GetValue<int>(reader, 3);
            entity.Id5 = Helper.GetValue<int>(reader, 4);
            entity.Id6 = Helper.GetValue<int>(reader, 5);
            entity.Id7 = Helper.GetValue<int>(reader, 6);
            entity.Id8 = Helper.GetValue<int>(reader, 7);
            entity.Id9 = Helper.GetValue<int>(reader, 8);
            entity.Id10 = Helper.GetValue<int>(reader, 9);
            entity.Id11 = Helper.GetValue<int>(reader, 10);
            entity.Id12 = Helper.GetValue<int>(reader, 11);
            entity.Id13 = Helper.GetValue<int>(reader, 12);
            entity.Id14 = Helper.GetValue<int>(reader, 13);
            entity.Id15 = Helper.GetValue<int>(reader, 14);
            entity.Id16 = Helper.GetValue<int>(reader, 15);
            entity.Id17 = Helper.GetValue<int>(reader, 16);
            entity.Id18 = Helper.GetValue<int>(reader, 17);
            entity.Id19 = Helper.GetValue<int>(reader, 18);
            entity.Id20 = Helper.GetValue<int>(reader, 19);
            return entity;
        }
    }

    public sealed class String10Mapper
    {
        public StringEntity10 Map(IDataRecord reader)
        {
            var entity = new StringEntity10();
            entity.Id1 = Helper.GetValue<string>(reader, 0);
            entity.Id2 = Helper.GetValue<string>(reader, 1);
            entity.Id3 = Helper.GetValue<string>(reader, 2);
            entity.Id4 = Helper.GetValue<string>(reader, 3);
            entity.Id5 = Helper.GetValue<string>(reader, 4);
            entity.Id6 = Helper.GetValue<string>(reader, 5);
            entity.Id7 = Helper.GetValue<string>(reader, 6);
            entity.Id8 = Helper.GetValue<string>(reader, 7);
            entity.Id9 = Helper.GetValue<string>(reader, 8);
            entity.Id10 = Helper.GetValue<string>(reader, 9);
            return entity;
        }
    }

    public sealed class String20Mapper
    {
        public StringEntity20 Map(IDataRecord reader)
        {
            var entity = new StringEntity20();
            entity.Id1 = Helper.GetValue<string>(reader, 0);
            entity.Id2 = Helper.GetValue<string>(reader, 1);
            entity.Id3 = Helper.GetValue<string>(reader, 2);
            entity.Id4 = Helper.GetValue<string>(reader, 3);
            entity.Id5 = Helper.GetValue<string>(reader, 4);
            entity.Id6 = Helper.GetValue<string>(reader, 5);
            entity.Id7 = Helper.GetValue<string>(reader, 6);
            entity.Id8 = Helper.GetValue<string>(reader, 7);
            entity.Id9 = Helper.GetValue<string>(reader, 8);
            entity.Id10 = Helper.GetValue<string>(reader, 9);
            entity.Id11 = Helper.GetValue<string>(reader, 10);
            entity.Id12 = Helper.GetValue<string>(reader, 11);
            entity.Id13 = Helper.GetValue<string>(reader, 12);
            entity.Id14 = Helper.GetValue<string>(reader, 13);
            entity.Id15 = Helper.GetValue<string>(reader, 14);
            entity.Id16 = Helper.GetValue<string>(reader, 15);
            entity.Id17 = Helper.GetValue<string>(reader, 16);
            entity.Id18 = Helper.GetValue<string>(reader, 17);
            entity.Id19 = Helper.GetValue<string>(reader, 18);
            entity.Id20 = Helper.GetValue<string>(reader, 19);
            return entity;
        }
    }
}
