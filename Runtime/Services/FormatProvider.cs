#region Header
// Date: 10/11/2023
// Created by: Huynh Phong Tran
// File name: FormatProvider.cs
#endregion

using System;
using System.Globalization;
using Base.Core;
using UnityEngine;

namespace Base.Core
{
    public class FormatProviderService : Service
    {
        private enum RoundingStrategy
        {
            Round, Floor, Ceil
        }
        private const string ROMAN_NUMBER_1  = "I";
        private const string ROMAN_NUMBER_2  = "II";
        private const string ROMAN_NUMBER_3  = "III";
        private const string ROMAN_NUMBER_4  = "IV";
        private const string ROMAN_NUMBER_5  = "V";
        private const string ROMAN_NUMBER_6  = "VI";
        private const string ROMAN_NUMBER_7  = "VII";
        private const string ROMAN_NUMBER_8  = "VIII";
        private const string ROMAN_NUMBER_9  = "IX";
        private const string ROMAN_NUMBER_10 = "X";

        private double ApplyRounding(double number, int decimalPlaces, RoundingStrategy roundingStrategy)
        {
            switch (roundingStrategy)
            {
                case RoundingStrategy.Round:
                    return Math.Round(number, decimalPlaces);
                case RoundingStrategy.Floor:
                {
                    double multiplier = Math.Pow(10, Convert.ToDouble(decimalPlaces));
                    return Math.Truncate(number * multiplier) / multiplier;
                }
                case RoundingStrategy.Ceil:
                {    
                    double multiplier = Math.Pow(10, Convert.ToDouble(decimalPlaces));
                    return Math.Ceiling(number * multiplier) / multiplier;
                }
                default:
                    throw new ArgumentOutOfRangeException(nameof(roundingStrategy), roundingStrategy, null);
            }
        }
        
        /// <summary>
        /// Convert money string with format x.xxx.xxx or x,xxx,xxx to Float
        /// </summary>
        /// <param name="moneyStr"></param>
        /// <returns>money in float</returns>
        public uint ConvertMoneyToNumber(string moneyStr)
        {
            var resultStr = moneyStr.Split('.');
            if (resultStr.Length <= 1)
            {
                resultStr = moneyStr.Split(',');
            }

            string joinString = string.Join("", resultStr);
            return uint.Parse(joinString);
        }
        
        /// <summary>
        /// Format a number to string with comma seperated format.
        /// </summary>
        /// <param name="money"></param>
        /// <returns>money in string</returns>
        public string FormatStringCommaSeparated(uint money)
        {
            object o = money;
            return $"{o:#,##0.##}";
        }
        
        public string FormatMoney(int money, int decPlace = 2)
        {
            string result = String.Empty;
            float  place  = Mathf.Pow(10f, decPlace);

            string[] abbrev = {"K", "M", "B", "T"};
            string   str    = (money < 0) ? "-" : "";
            float    size;

            money = Mathf.Abs(money);

            for (int i = abbrev.Length - 1; i >= 0; --i)
            {
                size = Mathf.Pow(10, (i + 1) * 3);
                if (size <= money)
                {
                    money = (int) (Mathf.Floor(money * place / size) / place);
                    if ((money == 1000) && (i < abbrev.Length - 1))
                    {
                        money =  1;
                        i     += 1;
                    }

                    result = money + abbrev[i];
                    break;
                }
            }

            return str + result;
        }

        public string GetFormatStringForDigitsBehindDelimiter(int digits)
        {
            if (digits == 0)
            {
                return "0";
            }

            string formatString = "0.";
            for (int i = 0; i < digits; ++i)
            {
                formatString += "#";
            }

            return formatString;
        }

        public string FormatRomanNumber(int number) => number switch
                                                       {
                                                               0  => "",
                                                               1  => ROMAN_NUMBER_1,
                                                               2  => ROMAN_NUMBER_2,
                                                               3  => ROMAN_NUMBER_3,
                                                               4  => ROMAN_NUMBER_4,
                                                               5  => ROMAN_NUMBER_5,
                                                               6  => ROMAN_NUMBER_6,
                                                               7  => ROMAN_NUMBER_7,
                                                               8  => ROMAN_NUMBER_8,
                                                               9  => ROMAN_NUMBER_9,
                                                               10 => ROMAN_NUMBER_10,
                                                               _  => $"{ROMAN_NUMBER_10}{FormatRomanNumber(number - 10)}"
                                                       };
        
        public string FormatFractional(float number)
        {
            string formatString = GetFormatStringForDigitsBehindDelimiter(1);
        
            return number.ToString(formatString, CultureInfo.InvariantCulture);
        }
    }
}