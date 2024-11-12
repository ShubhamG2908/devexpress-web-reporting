using DevExpress.XtraReports.UI;
using DevExpressReportMVCDemo.Reports;

namespace DevExpressReportMVCDemo.Services
{
    public static class ReportsFactory
    {
        public static Dictionary<string, Func<XtraReport>> Reports = new Dictionary<string, Func<XtraReport>>()
        {
            ["TestReport"] = () => new TestReport()
        };
    }
}
