using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alan.Utils.ExtensionMethods
{
    public static class ExObject
    {
        public static T ExIfNull<T>(this object obj, T whenFail = default(T)) where T : class, new()
        {
            return (obj as T) ?? (whenFail ?? new T());
        }

    }
}
