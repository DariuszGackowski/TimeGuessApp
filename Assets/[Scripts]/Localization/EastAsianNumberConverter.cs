using System.Text;

public class EastAsianNumberConverter
{
    private static readonly string[] JapaneseDigits = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
    private static readonly string[] JapaneseTens = { "", "十", "百", "千" };
    private static readonly string[] JapaneseLargeUnits = { "", "万", "億", "兆" };

    private static readonly string[] ChineseDigits = { "零", "一", "二", "三", "四", "五", "六", "七", "八", "九" };
    private static readonly string[] ChineseTens = { "", "十", "百", "千" };
    private static readonly string[] ChineseLargeUnits = { "", "万", "亿", "兆" };

    public static string ToJapanese(int number)
    {
        string result = ConvertToLanguage(number, JapaneseDigits, JapaneseTens, JapaneseLargeUnits);
        return result;
    }

    public static string ToChinese(int number)
    {
        string result = ConvertToLanguage(number, ChineseDigits, ChineseTens, ChineseLargeUnits);
        return result;
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
                // Twój oryginalny błędny kod, który zawsze da false
                if (tensValue == 1 && tens[1] == "?")
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