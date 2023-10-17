using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Base.Helper
{
    public static class StringHelper
    {
        public static NumberFormatInfo NumberFormat = new NumberFormatInfo {NumberGroupSeparator = ","};

        public const string FORMAT_CURRENCY                 = "#,0";
        public const string FORMAT_CURRENCY_GROUP_SEPARATOR = "#,#";
        public const string FORMAT_CURRENCY_2_DECIMALS      = "#.##";
        public const string FORMAT_CURRENCY_1_DECIMALS      = "#.#";

        public static string FormatCurrency(this string amount)
        {
            return string.Format(CultureInfo.CreateSpecificCulture("en-US"), "{0:N0}", amount);
        }

        public static string GetCurrencySymbol(string source)
        {
            return Regex.Replace(source, "[ ,.0123456789]+", "");
        }

        /// <summary>
        /// https://learn.microsoft.com/en-us/previous-versions/dotnet/netframework-4.0/0c899ak8(v=vs.100)?redirectedfrom=MSDN#SpecifierTh
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertNumberHasSpace(this int value)
        {
            return value.ToString(FORMAT_CURRENCY_GROUP_SEPARATOR, NumberFormat);
        }

        /// <summary>
        /// Convert to currency number
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ConvertToCurrencyNumber(this int value)
        {
            return value.ToString(FORMAT_CURRENCY, NumberFormat);
        }

        /// <summary>
        /// Convert price
        /// </summary>
        /// <param name="value"></param>
        /// <returns>returns</returns>
        public static string ConvertPrice(this decimal value)
        {
            return value.ToString("N");
        }

        /// <summary>
        /// Convert to word MB
        /// </summary>
        /// <param name="value"></param>
        /// <param name="hasSpace"></param>
        /// <returns>string</returns>
        public static string ConvertToWordMB(this int value, bool hasSpace = false)
        {
            return ToMB(value, hasSpace);
        }


        /// <summary>
        /// Convert to word KMB
        /// </summary>
        /// <param name="value"></param>
        /// <param name="hasSpace"></param>
        /// <param name="hasComma"></param>
        /// <returns></returns>
        public static string ConvertToWordKMB(this int value, bool hasSpace = false, bool hasComma = false)
        {
            return ToKMB(value, hasSpace, hasComma);
        }

        /// <summary>
        /// To MB
        /// </summary>
        /// <param name="num"></param>
        /// <param name="hasSpace"></param>
        /// <returns>string</returns>
        public static string ToMB(this decimal num, bool hasSpace)
        {
            var strSpace = hasSpace ? " " : "";
            var dividend = 1000;

            if (num < dividend) return num.ToString(FORMAT_CURRENCY, NumberFormat);

            var realNum = (double)num;

            //is thousand
            realNum /= dividend;

            if (realNum < dividend) return num.ToString(FORMAT_CURRENCY, NumberFormat);

            //is million
            realNum /= dividend;

            if (realNum < dividend) return realNum.ToString(FORMAT_CURRENCY_1_DECIMALS + strSpace + "M", NumberFormat);

            //is billion
            realNum /= dividend;

            return realNum.ToString(FORMAT_CURRENCY_1_DECIMALS + strSpace + "B", NumberFormat);
        }

        /// <summary>
        /// To KMB
        /// </summary>
        /// <param name="num"></param>
        /// <param name="hasSpace"></param>
        /// <returns>string</returns>
        public static string ToKMB(this decimal num, bool hasSpace, bool isUseComma = false)
        {
            var strSpace = hasSpace ? " " : "";
            //less than 10k doesn't convert
            var dividend   = 1000;
            var minimumDiv = 10000;

            if (num < minimumDiv) return num.ToString(FORMAT_CURRENCY, NumberFormat);

            var realNum = (double)num;

            //is 10 thousand
            realNum /= dividend;

            if (realNum < minimumDiv) return realNum.ToString(FORMAT_CURRENCY_1_DECIMALS + strSpace + "K", NumberFormat);

            //is million
            realNum /= dividend;

            if (realNum < minimumDiv) return realNum.ToString(FORMAT_CURRENCY_1_DECIMALS + strSpace + "M", NumberFormat);

            //is billion
            realNum /= dividend;

            return realNum.ToString(FORMAT_CURRENCY_1_DECIMALS + strSpace + "B", NumberFormat);
        }

        public static string ToRoman(this int num)
        {
            var result = string.Empty;
            var romanDic = new Dictionary<int, string>
                           {
                                   {50, "L"}, {40, "XL"}, {10, "X"}, {9, "IX"}, {5, "V"}, {4, "IV"}, {1, "I"}
                           };

            foreach (var item in romanDic)
            {
                if (num <= 0) break;

                while (num >= item.Key)
                {
                    result += item.Value;
                    num    -= item.Key;
                }
            }

            return result;
        }
    }
}