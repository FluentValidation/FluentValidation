using System;

namespace FluentValidation.Validators
{
    public class Comparer
    {
        public Comparer(Func<object, double, int> compareFunc)
        {
            CompareFunc = compareFunc;
            CanCompare = o => true;
        }

        public Comparer(Func<object, double, int> compareFunc, Func<object, bool> canCompare)
        {
            CompareFunc = compareFunc;
            CanCompare = canCompare;
        }

        public Func<object, double, int> CompareFunc { get; private set; }
        public Func<object, bool> CanCompare { get; private set; }

        public static bool TryCompare(IComparable value, IComparable valueToCompare, out int result)
        {
            try
            {
                // ensure both are comparable
                var valComp = BuildComparer(value.GetType());
                BuildComparer(valueToCompare.GetType());

                // get it to a comparable type
                var convertible = (IConvertible)valueToCompare;
                var format = System.Globalization.CultureInfo.InvariantCulture.NumberFormat;
                double doubleValue = convertible.ToDouble(format);

                result = valComp.CompareFunc(value, doubleValue);

                return true;
            }
            catch
            {
            }

            result = 0;
            return false;
        }

        public static Comparer BuildComparer(Type type)
        {
            if (typeof(double) == type)
                return new Comparer((o, d) => ((IComparable)o).CompareTo(d));
            if (typeof(double?) == type)
                return new Comparer((o, d) => ((IComparable)((double?)o).Value).CompareTo(d), o => ((double?)o).HasValue);

            if (typeof(decimal) == type)
                return new Comparer((o, d) => ((IComparable)o).CompareTo((decimal)d));
            if (typeof(decimal?) == type)
                return new Comparer((o, d) => ((IComparable)((decimal?)o).Value).CompareTo((decimal)d), o => ((decimal?)o).HasValue);

            if (typeof(int) == type)
                return new Comparer((o, d) => ((IComparable)o).CompareTo((int)d));
            if (typeof(int?) == type)
                return new Comparer((o, d) => ((IComparable)((int?)o).Value).CompareTo((int)d), o => ((int?)o).HasValue);

            if (typeof(long) == type)
                return new Comparer((o, d) => ((IComparable)o).CompareTo((long)d));
            if (typeof(long?) == type)
                return new Comparer((o, d) => ((IComparable)((long?)o).Value).CompareTo((long)d), o => ((long?)o).HasValue);

            if (typeof(float) == type)
                return new Comparer((o, d) => ((IComparable)o).CompareTo((float)d));
            if (typeof(float?) == type)
                return new Comparer((o, d) => ((IComparable)((float?)o).Value).CompareTo((float)d), o => ((float?)o).HasValue);

            if (typeof(short) == type)
                return new Comparer((o, d) => ((IComparable)o).CompareTo((short)d));
            if (typeof(short?) == type)
                return new Comparer((o, d) => ((IComparable)((short?)o).Value).CompareTo((short)d), o => ((short?)o).HasValue);

            if (typeof(char) == type)
                return new Comparer((o, d) => ((IComparable)o).CompareTo((char)d));
            if (typeof(char?) == type)
                return new Comparer((o, d) => ((IComparable)((char?)o).Value).CompareTo((char)d), o => ((char?)o).HasValue);

            if (typeof(byte) == type)
                return new Comparer((o, d) => ((IComparable)o).CompareTo((byte)d));
            if (typeof(byte?) == type)
                return new Comparer((o, d) => ((IComparable)((byte?)o).Value).CompareTo((byte)d), o => ((byte?)o).HasValue);

            if (typeof(uint) == type)
                return new Comparer((o, d) => ((IComparable)o).CompareTo((uint)d));
            if (typeof(uint?) == type)
                return new Comparer((o, d) => ((IComparable)((uint?)o).Value).CompareTo((uint)d), o => ((uint?)o).HasValue);

            if (typeof(ulong) == type)
                return new Comparer((o, d) => ((IComparable)o).CompareTo((ulong)d));
            if (typeof(ulong?) == type)
                return new Comparer((o, d) => ((IComparable)((ulong?)o).Value).CompareTo((ulong)d), o => ((ulong?)o).HasValue);

            if (typeof(ushort) == type)
                return new Comparer((o, d) => ((IComparable)o).CompareTo((ushort)d));
            if (typeof(ushort?) == type)
                return new Comparer((o, d) => ((IComparable)((ushort?)o).Value).CompareTo((ushort)d), o => ((ushort?)o).HasValue);

            if (typeof(sbyte) == type)
                return new Comparer((o, d) => ((IComparable)o).CompareTo((sbyte)d));
            if (typeof(sbyte?) == type)
                return new Comparer((o, d) => ((IComparable)((sbyte?)o).Value).CompareTo((sbyte)d), o => ((sbyte?)o).HasValue);

            throw new NotSupportedException(string.Format("Comparison between [type:{0}] and [type{1}] is not supported for numeric validators", typeof(double), type));
        }
    }
}