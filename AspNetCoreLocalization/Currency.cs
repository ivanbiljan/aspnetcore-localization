namespace AspNetCoreLocalization;

public sealed class Currency
{
    public static IList<Currency> DefaultCurrencies { get; } = new List<Currency>
    {
        new("is-IS", "ISK", 0, ",", "."),
        new("hr-HR", "kn", 2, ".", ","),
        new("en-US", "USD", 2, ".", ",")
    };
    
    public Currency(string locale, string displaySign, int decimalPlaces, string decimalSeparator, string groupSeparator)
    {
        Locale = locale;
        DisplaySign = displaySign;
        DecimalPlaces = decimalPlaces;
        GroupSeparator = groupSeparator;
        DecimalSeparator = decimalSeparator;
    }

    public string Locale { get; set; }
    
    public string DisplaySign { get; set; }
    
    public int DecimalPlaces { get; set; }
    
    public string DecimalSeparator { get; set; }
    
    public string GroupSeparator { get; set; }
}