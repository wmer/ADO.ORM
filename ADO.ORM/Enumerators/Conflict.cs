﻿using System;
using System.Collections.Generic;
using System.Text;

namespace ADO.ORM.Enumerators {
    public enum Conflict {
        ROLLBACK,
        ABORT,
        FAIL,
        IGNORE,
        REPLACE
    }
}
