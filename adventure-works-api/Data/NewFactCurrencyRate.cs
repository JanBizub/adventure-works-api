using System;
using System.Collections.Generic;

namespace DependencyInjectionApi.Data;

public partial class NewFactCurrencyRate
{
    public float? AverageRate { get; set; }

    public string? CurrencyId { get; set; }

    public DateOnly? CurrencyDate { get; set; }

    public float? EndOfDayRate { get; set; }

    public int? CurrencyKey { get; set; }

    public int? DateKey { get; set; }
}
