using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace ADO.ORM.Converters {
    public class DataReaderToDictionaryConverter {
        private object lock1 = new object();

        public ConcurrentDictionary<int, ConcurrentDictionary<String, object>> Converte(DbDataReader dataReader) {
            lock (lock1) {
                var dict = new ConcurrentDictionary<int, ConcurrentDictionary<String, object>>();
                var dic = new ConcurrentDictionary<String, object>();
                var indice = 0;

                if (dataReader.HasRows) {
                    while (dataReader.Read()) {
                        dic = new ConcurrentDictionary<String, object>();
                        dict[indice] = dic;
                        for (int i = 0; i < dataReader.FieldCount; i++) {
                            dic[dataReader.GetName(i)] = dataReader.GetValue(i);
                        }
                        indice++;
                    }
                }
               
                return dict;
            }
        }
    }
}
