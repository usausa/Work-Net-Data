using System;
using System.Collections.Generic;
using System.Text;

namespace StructTest
{
    public struct StructMasterEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public struct StructSlaveEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }

    public static class StructTupleMapper
    {
        public static Tuple<StructMasterEntity, StructSlaveEntity> Map()
        {
            var master = new StructMasterEntity();
            master.Id = 0;
            var slave = new StructSlaveEntity();
            slave.Id = 0;
            return new Tuple<StructMasterEntity, StructSlaveEntity>(master, slave);
        }

        public static ValueTuple<StructMasterEntity, StructSlaveEntity> MapValue()
        {
            var master = new StructMasterEntity();
            master.Id = 0;
            var slave = new StructSlaveEntity();
            slave.Id = 0;
            return new ValueTuple<StructMasterEntity, StructSlaveEntity>(master, slave);
        }

        public static Tuple<StructMasterEntity?, StructSlaveEntity?> MapNullable()
        {
            var master = new StructMasterEntity();
            master.Id = 0;
            var slave = new StructSlaveEntity();
            slave.Id = 0;
            return new Tuple<StructMasterEntity?, StructSlaveEntity?>(master, slave);
        }

        public static ValueTuple<StructMasterEntity?, StructSlaveEntity?> MapValueNullable()
        {
            var master = new StructMasterEntity();
            master.Id = 0;
            var slave = new StructSlaveEntity();
            slave.Id = 0;
            return new ValueTuple<StructMasterEntity, StructSlaveEntity>(master, slave);
        }
    }
}
