using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public static class TypeHelpers
    {
        public static bool IsNumericType(this object o)
        {
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Byte:
                case TypeCode.SByte:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Single:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsPrimitiveType(this object o)
        {
            if (IsNumericType (o)) { return true; }
            switch (Type.GetTypeCode(o.GetType()))
            {
                case TypeCode.Boolean:
                    return true;
                case TypeCode.Char:
                    return true;
                default:
                    break;
            }

            return false;
        }

        public static bool IsString(this object o)
        {
            return Type.GetTypeCode(o.GetType()) == TypeCode.String;
        }
    }
}
