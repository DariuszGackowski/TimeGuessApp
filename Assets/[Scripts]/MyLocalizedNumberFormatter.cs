using System;
using System.Text;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Core.Extensions;

[Serializable]
[DisplayName("My Localized Number Formatter")]
public class MyLocalizedNumberFormatter : FormatterBase
{
    public override string[] DefaultNames => new[] { "localizedNumber" };

    public override bool TryEvaluateFormat(IFormattingInfo formattingInfo)
    {
        if (formattingInfo.CurrentValue is int number)
        {
            var localeCode = LocalizationSettings.SelectedLocale.Identifier.Code;

            switch (localeCode)
            {
                case "ja": // Japanese
                    formattingInfo.Write(NumberLocalization.ToJapanese(number));
                    return true;
                case "zh": // Chinese Simplified
                case "zh-Hans":
                    formattingInfo.Write(NumberLocalization.ToChinese(number));
                    return true;
                default:
                    formattingInfo.Write(number.ToString());
                    return true;
            }
        }
        return false;
    }
}

public static class NumberLocalization
{
    private static readonly string[] JapaneseDigits = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
    private static readonly string[] JapaneseTens = { "", "十", "百", "千" };
    private static readonly string[] JapaneseLargeUnits = { "", "万", "億" };

    private static readonly string[] ChineseDigits = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
    private static readonly string[] ChineseTens = { "", "十", "百", "千" };
    private static readonly string[] ChineseLargeUnits = { "", "万", "亿" };

    public static string ToJapanese(int number)
    {
        if (number == 0) return JapaneseDigits[0];
        return ConvertToLanguage(number, JapaneseDigits, JapaneseTens, JapaneseLargeUnits);
    }

    public static string ToChinese(int number)
    {
        if (number == 0) return ChineseDigits[0];
        return ConvertToLanguage(number, ChineseDigits, ChineseTens, ChineseLargeUnits);
    }

    private static string ConvertToLanguage(int number, string[] digits, string[] tens, string[] largeUnits)
    {
        if (number == 0) return digits[0];

        StringBuilder result = new StringBuilder();
        int unitIndex = 0;

        while (number > 0)
        {
            int block = number % 10000;
            if (block > 0)
            {
                string blockString = ConvertBlock(block, digits, tens);
                result.Insert(0, blockString + largeUnits[unitIndex]);
            }

            number /= 10000;
            unitIndex++;
        }

        return result.ToString();
    }

    private static string ConvertBlock(int number, string[] digits, string[] tens)
    {
        StringBuilder blockResult = new StringBuilder();
        bool isZero = true;

        if (number >= 1000)
        {
            int thousands = number / 1000;
            if (thousands > 0)
            {
                blockResult.Append(digits[thousands]).Append(tens[3]);
                isZero = false;
            }
            number %= 1000;
        }

        if (number >= 100)
        {
            int hundreds = number / 100;
            if (hundreds > 0)
            {
                blockResult.Append(digits[hundreds]).Append(tens[2]);
                isZero = false;
            }
            else if (!isZero) blockResult.Append(digits[0]);
            number %= 100;
        }

        if (number >= 10)
        {
            int tensValue = number / 10;
            if (tensValue > 0)
            {
                if (tensValue == 1 && tens[1] == "十")
                {
                    blockResult.Append(tens[1]);
                }
                else
                {
                    blockResult.Append(digits[tensValue]).Append(tens[1]);
                }
                isZero = false;
            }
            else if (!isZero) blockResult.Append(digits[0]);
            number %= 10;
        }

        if (number > 0)
        {
            blockResult.Append(digits[number]);
        }
        else if (isZero && blockResult.Length > 0)
        {
            if (blockResult[blockResult.Length - 1] == digits[0][0])
            {
                blockResult.Remove(blockResult.Length - 1, 1);
            }
        }

        return blockResult.ToString();
    }
}