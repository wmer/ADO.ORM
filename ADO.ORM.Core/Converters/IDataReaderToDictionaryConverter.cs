using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace ADO.ORM.Core.Converters {
    public interface IDataReaderToDictionaryConverter {
        ConcurrentDictionary<int, ConcurrentDictionary<String, object>> Converte(DbDataReader dataReader);
    }
}
