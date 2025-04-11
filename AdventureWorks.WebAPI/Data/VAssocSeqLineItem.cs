using System;
using System.Collections.Generic;

namespace DependencyInjectionApi.Data;

public partial class VAssocSeqLineItem
{
    public string OrderNumber { get; set; } = null!;

    public byte LineNumber { get; set; }

    public string? Model { get; set; }
}
