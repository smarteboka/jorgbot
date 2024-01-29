using System.Text;

namespace Smartbot.Web.App.Utils;

public static class TimeSpanExtensions
{
    public static string ToPrettyFormat(this TimeSpan span)
    {
        if (span == TimeSpan.Zero) return "0 minutter";

        var sb = new StringBuilder();
        if (span.Days > 0)
            sb.AppendFormat("{0} dag{1} ", span.Days, span.Days > 1 ? "er" : string.Empty);
        if (span.Hours > 0)
            sb.AppendFormat("{0} time{1} ", span.Hours, span.Hours > 1 ? "r" : string.Empty);
        if (span.Minutes > 0)
            sb.AppendFormat("{0} minutt{1} ", span.Minutes, span.Minutes > 1 ? "er" : string.Empty);
        return sb.ToString().TrimEnd();
    }
        

}