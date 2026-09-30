using System.Runtime.CompilerServices;

namespace BattleForDune.FastLookup
{
    public struct EnumSet<TEnum> where TEnum : Enum
    {
        private int _mask;

        public EnumSet(params ReadOnlySpan<TEnum> values)
        {
            foreach (var value in values)
            {
                Add(value);
            }
        }

        public void Add(TEnum value)
        {
            _mask |= 1 << SafeEnumToInt(value);
        }

        public readonly bool Contains(TEnum value)
        {
            return (_mask & (1 << SafeEnumToInt(value))) != 0;
        }

        public void Remove(TEnum value)
        {
            _mask &= ~(1 << SafeEnumToInt(value));
        }

        private static int SafeEnumToInt(TEnum enumValue)
        {
            if (Unsafe.SizeOf<TEnum>() == 1)
            {
                return Unsafe.As<TEnum, byte>(ref enumValue);
            }
            if (Unsafe.SizeOf<TEnum>() == 2)
            {
                return Unsafe.As<TEnum, short>(ref enumValue);
            }
            if (Unsafe.SizeOf<TEnum>() == 4)
            {
                return Unsafe.As<TEnum, int>(ref enumValue);
            }

            return (int)Unsafe.As<TEnum, long>(ref enumValue);
        }
    }
}
