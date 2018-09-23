using System;
using System.Collections.Generic;
using System.Text;

namespace ADO.ORM.Core.Events {
    public class DatabseEventHub {
        public static event DatabaseChangedEventHandler DatabaseChanged;
        public static event DatabaseOperationFailedEventHandler DatabaseOperationFailed;

        public static void OnDatabaseChanged(object sender, DatabaseChangedEventArgs e) {
            DatabaseChanged?.Invoke(sender, e);
        }

        public static void OnDatabaseOperationFailed(object sender, DatabaseOperationFailedEventArgs e) {
            DatabaseOperationFailed?.Invoke(sender, e);
        }
    }
}
