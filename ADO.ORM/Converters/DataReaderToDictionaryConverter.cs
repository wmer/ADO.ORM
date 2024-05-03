using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;

namespace ADO.ORM.Converters; 
public class DataReaderToDictionaryConverter {

    public Dictionary<int, Dictionary<string, object>> Converte(DbDataReader dataReader) {
        var dict = new Dictionary<int, Dictionary<string, object>>();
        var indice = 0;

        if (dataReader.HasRows) {
            while (dataReader.Read()) {
                Dictionary<string, object>? dic = [];
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
