using ADO.ORM.Core.SqLite;
using ADO.ORM.Test.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ADO.ORM.Test.Data;
internal class TesteRepository : SqLiteContext {
    public TesteRepository(string path, string dbName) : base(path, dbName) {
    }

    public Repository<Model1> Models { get; set; }
}
