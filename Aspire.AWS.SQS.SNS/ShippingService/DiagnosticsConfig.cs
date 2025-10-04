namespace ShippingProcessor;

using System.Diagnostics;

public static class DiagnosticsConfig
{
    public static readonly ActivitySource ActivitySource = new("Orders.ShippingProcessor");
}

