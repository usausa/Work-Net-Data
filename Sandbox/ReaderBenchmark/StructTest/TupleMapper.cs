using System;
using System.Data;

namespace StructTest
{
    public class MasterEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public class SlaveEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public SlaveEntity()
        {
        }

        public SlaveEntity(int id, string name)
        {
            Id = id;
            Name = name;
        }
    }

    public static class TupleMapper
    {
        public static Tuple<MasterEntity, SlaveEntity> Map1(IDataReader reader)
        {
            object value;

            // Master
            var master = new MasterEntity();

            value = reader.GetValue(0);
            if (value is DBNull)
            {
                master.Id = default;
            }
            else
            {
                master.Id = (int)value;
            }

            value = reader.GetValue(1);
            if (value is DBNull)
            {
                master.Name = default;
            }
            else
            {
                master.Name = (string)value;
            }

            // Master
            SlaveEntity slave;

            value = reader.GetValue(2);
            if (value is DBNull)
            {
                slave = null;
                goto all;
            }
            else
            {
                slave = new SlaveEntity();
                slave.Id = (int)value;
            }

            value = reader.GetValue(3);
            if (value is DBNull)
            {
                slave.Name = default;
            }
            else
            {
                slave.Name = (string)value;
            }

            all:
            return new Tuple<MasterEntity, SlaveEntity>(master, slave);
        }

        public static Tuple<MasterEntity, SlaveEntity> Map2(IDataReader reader)
        {
            object value;

            // Master
            var master = new MasterEntity();

            value = reader.GetValue(0);
            if (value is DBNull)
            {
                master.Id = default;
            }
            else
            {
                master.Id = (int)value;
            }

            value = reader.GetValue(1);
            if (value is DBNull)
            {
                master.Name = default;
            }
            else
            {
                master.Name = (string)value;
            }

            // Master
            SlaveEntity slave;

            var arg1 = reader.GetValue(2);
            if (value is DBNull)
            {
                slave = null;
                goto all;
            }

            var arg2 = reader.GetValue(3);
            if (arg2 is DBNull)
            {
                arg2 = default(string);
            }

            slave = new SlaveEntity((int)arg1, (string)arg2);

            all:
            return new Tuple<MasterEntity, SlaveEntity>(master, slave);
        }
    }
}
