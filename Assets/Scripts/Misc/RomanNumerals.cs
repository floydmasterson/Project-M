public class RomanNumerals
{
    public static string RomanNumeralGenerator(int number)
    {
        string roman;
        switch (number)
        {
            case 1:
                roman = "I";
                break;
            case 2:
                roman = "II";
                break;
            case 3:
                roman = "III";
                break;
            case 4:
                roman = "IV";
                break;
            case 5:
                roman = "V";
                break;
            case 6:
                roman = "VI";
                break;
            case 7:
                roman = "VII";
                break;
            case 8:
                roman = "VIII";
                break;
            case 9:
                roman = "IX";
                break;
            case 10:
                roman = "X";
                break;
            default:
                roman = number.ToString();
                break;

        }
        return roman;
     }
}
