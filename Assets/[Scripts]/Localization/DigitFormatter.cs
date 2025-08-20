using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.Core.Extensions;


namespace UnityEngine.Localization.SmartFormat.Extensions
{
    [DisplayName("Digit Formatter")]
    public class DigitFormatter : FormatterBase
    {
        public override string[] DefaultNames => new[] { "dg" };

        public override bool TryEvaluateFormat(IFormattingInfo formattingInfo)
        {
            object currentArgument = formattingInfo.FormatDetails.OriginalArgs[0];
            string argStr = currentArgument.ToString(); // "[number, 2]"

            int number;
            var parts = argStr.Trim('[', ']').Split(',');
            if (parts.Length > 1)
            {
                if (int.TryParse(parts[1].Trim(), out number))
                {
                    Debug.Log(number);
                }
                else
                {
                    return false;
                }
            }
            else
                return false;

            string result;
            var localeCode = LocalizationSettings.SelectedLocale.Identifier.Code;
            switch (localeCode)
            {
                case "ja":
                    result = EastAsianNumberConverter.ToJapanese(number);
                    break;
                case "zh":
                case "zh-Hans":
                    result = EastAsianNumberConverter.ToChinese(number);
                    break;
                default:
                    result = number.ToString();
                    break;
            }

            formattingInfo.Write(result);
            return true;
        }
    }
}