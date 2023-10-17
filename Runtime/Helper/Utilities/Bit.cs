/// Class: Bit
/// Author: GearInc
/// Description: Extensions used for bit operators
using System;

public static class Bit
{
    //=================
    public static int SwapBit(int value)
    {
        return ((value & 0xff00) >> 8) | ((value & 0x00ff) << 8) | (value & (~0xffff));
    }
    public static long SwapBit(long value)
    {
        return ((value & 0xff00) >> 8) | ((value & 0x00ff) << 8) | (value & (~0xffff));
    }
    
    //=================
    public static int BitValue(int bit)
    {
        return (1 << bit);
    }
    public static int BitValue(Enum enumValue)
    {
        int bit = (int) (object) (enumValue);
        return (1 << bit);
    }
    
    public static T BitValue<T>(int bit)
    {
        object value = ((typeof(T) == typeof(long) ? 1L : 1) << bit);
        return (T)value;
    }
    public static T BitValue<T>(Enum enumValue)
    {
        int bit = (int) (object) (enumValue);
        object value = ((typeof(T) == typeof(long) ? 1L : 1) << bit);
        return (T)value;
    }

    //=================
    public static void AddBit(ref int value, int bit)
    {
        value |= (1 << bit);
    }
    public static void AddBit(ref int value, Enum enumValue)
    {
        int bit = (int) (object) (enumValue);
        value |= (1 << bit);
    }
    public static void AddBit(ref long value, int bit)
    {
        value |= (1L << bit);
    }
    public static void AddBit(ref long value, Enum enumValue)
    {
        int bit = (int) (object) (enumValue);
        value |= (1L << bit);
    }
    
    //=================
    public static void ToggleBit(ref int value, int bit)
    {
        value ^= (1 << bit);
    }
    public static void ToggleBit(ref int value, Enum enumValue)
    {
        int bit = (int) (object) (enumValue);
        value ^= (1 << bit);
    }
    
    public static void ToggleBit(ref long value, int bit)
    {
        value ^= (1L << bit);
    }
    public static void ToggleBit(ref long value, Enum enumValue)
    {
        int bit = (int) (object) (enumValue);
        value ^= (1L << bit);
    }
    
    //=================
    public static void ClearBit(ref int value, int bit)
    {
        value &= ~(1 << bit);
    }
    public static void ClearBit(ref int value, Enum enumValue)
    {
        int bit = (int) (object) (enumValue);
        value &= ~(1 << bit);
    }
    public static void ClearBit(ref long value, int bit)
    {
        value &= ~(1L << bit);
    }
    public static void ClearBit(ref long value, Enum enumValue)
    {
        int bit = (int) (object) (enumValue);
        value &= ~(1L << bit);
    }
    //=================
    public static void ClearAllBit(ref int value)
    {
        value = 0;
    }
    public static void ClearAllBit(ref long value)
    {
        value = 0L;
    }
    
    //=================
    public static bool HasBit(int value, int bit)
    {
        return (value & (1 << bit)) == (1 << bit);
    }
    public static bool HasBit(int value, Enum enumValue)
    {
        int bit = (int) (object) (enumValue);
        return (value & (1 << bit)) == (1 << bit);
    }
    public static bool HasBit(long value, int bit)
    {
        return (value & (1L << bit)) == (1L << bit);
    }
    public static bool HasBit(long value, Enum enumValue)
    {
        int bit = (int) (object) (enumValue);
        return (value & (1L << bit)) == (1L << bit);
    }

    //==================
    public static bool HasValue(int value, int value2)
    {
        return (value & value2) == value2;
    }
    public static bool HasValue(long value, long value2)
    {
        return (value & value2) == value2;
    }
    
    //=================
    public static void SumValue(ref int value, int value2)
    {
        value |= value2;
    }
    public static void SumValue(ref long value, long value2)
    {
        value |= value2;
    }

    //=================
    public static int SumValue(int value1, int value2)
    {
        return value1 | value2;
    }
    public static long SumValue(long value1, long value2)
    {
        return value1 | value2;
    }

    //=================
    public static int MinusValue(int value1, int value2)
    {
        int flip  = ~value2;
        int value = value1 & flip;
        return value;
    }
    public static long MinusValue(long value1, long value2)
    {
        long flip  = ~value2;
        long value = value1 & flip;
        return value;
    }
    
    //=================
    public static int GetAnd(int value1, int value2)
    {
        return value1 & value2;
    }
    public static long GetAnd(long value1, long value2)
    {
        return value1 & value2;
    }
}

public static class BitHelper
{
    //=================
    public static T BitValue<T>(this int bit)
    {
        return Bit.BitValue<T>(bit);
    }
    public static T BitValue<T>(this Enum enumValue)
    {
        return Bit.BitValue<T>(enumValue);
    }
    
    //=================
    public static bool HasBit(this int value, int bit)
    {
        return Bit.HasBit(value, bit);
    }
    public static bool HasBit(this int value, Enum enumValue)
    {
        return Bit.HasBit(value, enumValue);
    }
    public static bool HasBit(this long value, int bit)
    {
        return Bit.HasBit(value, bit);
    }
    public static bool HasBit(this long value, Enum enumValue)
    {
        return Bit.HasBit(value, enumValue);
    }
    
    //=================
    public static bool HasValue(this int value, int value2)
    {
        return Bit.HasValue(value, value2);
    }
    public static bool HasValue(this long value, int value2)
    {
        return Bit.HasValue(value, value2);
    }
    
    //=================
    public static int SwapBit(this int value)
    {
        return Bit.SwapBit(value);
    }
    public static long SwapBit(this long value)
    {
        return Bit.SwapBit(value);
    }
}
