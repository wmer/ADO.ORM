using System;
using System.Collections.Generic;
using System.Text;

namespace ADO.ORM.Core.Enumerators {
    public enum Conflict {
        ROLLBACK,
        ABORT,
        FAIL,
        IGNORE,
        REPLACE
    }
}
