using System.Diagnostics;

namespace BillingProcessor;

public static class DiagnosticsConfig
{
    public static readonly ActivitySource ActivitySource = new("Orders.BillingProcessor");
}
