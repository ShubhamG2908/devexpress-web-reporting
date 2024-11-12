using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Extensions;

using System.Reflection;
using System.ServiceModel;

namespace DevExpressReportMVCDemo.Services
{
    public class CustomReportStorage : ReportStorageWebExtension
    {
        //private readonly string _reportFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "Reports");
        //private readonly Dictionary<string, Type> _reportTypes;
        //public CustomReportStorage()
        //{
        //    if (!Directory.Exists(_reportFolderPath))
        //    {
        //        Directory.CreateDirectory(_reportFolderPath);
        //    }
        //    _reportTypes = Assembly.GetExecutingAssembly()
        //    .GetTypes()
        //    .Where(t => t.IsClass && t.Namespace == "DevExpressReportMVCDemo.Reports" && t.IsSubclassOf(typeof(XtraReport)))
        //    .ToDictionary(t => t.Name, t => t);
        //}

        //public override bool CanSetData(string url) => true;

        //public override byte[] GetData(string url)
        //{
        //    // Load the report file data from the Reports folder
        //    //string filePath = Path.Combine(_reportFolderPath, $"{url}.repx");
        //    //if (File.Exists(filePath))
        //    //{
        //    //    return File.ReadAllBytes(filePath);
        //    //}
        //    //throw new FileNotFoundException($"Report '{url}' not found.");
        //    if (_reportTypes.TryGetValue(url, out Type reportType))
        //    {
        //        // Create an instance of the report dynamically
        //        if (Activator.CreateInstance(reportType) is XtraReport report)
        //        {
        //            using (var stream = new System.IO.MemoryStream())
        //            {
        //                report.SaveLayoutToXml(stream);
        //                return stream.ToArray();
        //            }
        //        }
        //    }

        //    throw new KeyNotFoundException($"Report '{url}' not found.");
        //}

        //public override Dictionary<string, string> GetUrls()
        //{
        //    return _reportTypes.ToDictionary(r => r.Key, r => r.Key);
        //    // Return a dictionary of available report names (URLs) and display names
        //    //var reports = new Dictionary<string, string>();
        //    //foreach (var filePath in Directory.GetFiles(_reportFolderPath, "*.repx"))
        //    //{
        //    //    string reportName = Path.GetFileNameWithoutExtension(filePath);
        //    //    reports[reportName] = reportName;
        //    //}
        //    //return reports;
        //}

        //public override bool IsValidUrl(string url) => _reportTypes.ContainsKey(url);

        //public override void SetData(XtraReport report, string url)
        //{
        //    // Save the report to the Reports folder
        //    string filePath = Path.Combine(_reportFolderPath, $"{url}.repx");
        //    //report.SaveLayoutToXml(filePath);
        //    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        //    {
        //        report.SaveLayoutToXml(fileStream);
        //    }
        //}

        //public override string SetNewData(XtraReport report, string defaultUrl)
        //{
        //    // Save a new report with a unique URL if needed
        //    //string uniqueUrl = defaultUrl; // Customize as needed for uniqueness
        //    //SetData(report, uniqueUrl);
        //    //return uniqueUrl;
        //    string reportName = defaultUrl;
        //    string reportPath = Path.Combine(_reportFolderPath, $"{reportName}.repx");

        //    int count = 1;
        //    while (File.Exists(reportPath))
        //    {
        //        // Add a suffix to the report name if a file with the same name already exists
        //        reportName = $"{defaultUrl}_{count}";
        //        reportPath = Path.Combine(_reportFolderPath, $"{reportName}.repx");
        //        count++;
        //    }

        //    // Save the new report to the determined path
        //    using (var fileStream = new FileStream(reportPath, FileMode.Create, FileAccess.Write))
        //    {
        //        report.SaveLayoutToXml(fileStream);
        //    }

        //    // Return the unique name of the saved report
        //    return reportName;
        //}
        readonly string reportDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Reports");
        const string FileExtension = ".repx";
        private readonly Dictionary<string, Type> _reportTypes;
        public CustomReportStorage()
        {
            if (!Directory.Exists(reportDirectory))
            {
                Directory.CreateDirectory(reportDirectory);
            }
            _reportTypes = Assembly.GetExecutingAssembly()
            .GetTypes()
            .Where(t => t.IsClass && t.Namespace == "DevExpressReportMVCDemo.Reports" && t.IsSubclassOf(typeof(XtraReport)))
            .ToDictionary(t => t.Name, t => t);
        }

        private bool IsWithinReportsFolder(string url, string folder)
        {
            var rootDirectory = new DirectoryInfo(folder);
            var fileInfo = new FileInfo(Path.Combine(folder, url));
            return fileInfo.Directory.FullName.ToLower().StartsWith(rootDirectory.FullName.ToLower());
        }

        public override bool CanSetData(string url)
        {
            // Determines whether a report with the specified URL can be saved.
            // Add custom logic that returns **false** for reports that should be read-only.
            // Return **true** if no valdation is required.
            // This method is called only for valid URLs (if the **IsValidUrl** method returns **true**).

            return true;
        }

        public override bool IsValidUrl(string url)
        {
            // Determines whether the URL passed to the current report storage is valid.
            // Implement your own logic to prohibit URLs that contain spaces or other specific characters.
            // Return **true** if no validation is required.

            return Path.GetFileName(url) == url;
        }

        public override byte[] GetData(string url)
        {
            // Uses a specified URL to return report layout data stored within a report storage medium.
            // This method is called if the **IsValidUrl** method returns **true**.
            // You can use the **GetData** method to process report parameters sent from the client
            // if the parameters are included in the report URL's query string.
            try
            {
                if (Directory.EnumerateFiles(reportDirectory).Select(Path.GetFileNameWithoutExtension).Contains(url))
                {
                    if (url == "TestReport")
                    {
                        if (_reportTypes.TryGetValue(url, out Type reportType))
                        {
                            // Create an instance of the report dynamically
                            if (Activator.CreateInstance(reportType) is XtraReport report)
                            {
                                using (var stream = new System.IO.MemoryStream())
                                {
                                    report.SaveLayoutToXml(stream);
                                    return stream.ToArray();
                                }
                            }
                        }
                    }
                    return File.ReadAllBytes(Path.Combine(reportDirectory, url + FileExtension));
                }
                if (ReportsFactory.Reports.ContainsKey(url))
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        ReportsFactory.Reports[url]().SaveLayoutToXml(ms);
                        return ms.ToArray();
                    }
                }
            }
            catch (Exception)
            {
                throw new FaultException(new FaultReason("Could not get report data."), new FaultCode("Server"), "GetData");
            }
            throw new FaultException(new FaultReason(string.Format("Could not find report '{0}'.", url)), new FaultCode("Server"), "GetData");
        }

        public override Dictionary<string, string> GetUrls()
        {
            // Returns a dictionary that contains the report names (URLs) and display names. 
            // The Report Designer uses this method to populate the Open Report and Save Report dialogs.

            return Directory.GetFiles(reportDirectory, "*" + FileExtension)
                                     .Select(Path.GetFileNameWithoutExtension)
                                     .Union(ReportsFactory.Reports.Select(x => x.Key))!
                                     .ToDictionary<string, string>(x => x);
        }

        public override void SetData(XtraReport report, string url)
        {
            // Saves the specified report to the report storage with the specified name
            // (saves existing reports only). 
            if (!IsWithinReportsFolder(url, reportDirectory))
                throw new FaultException(new FaultReason("Invalid report name."), new FaultCode("Server"), "GetData");
            report.SaveLayoutToXml(Path.Combine(reportDirectory, url + FileExtension));
        }

        public override string SetNewData(XtraReport report, string defaultUrl)
        {
            // Allows you to validate and correct the specified name (URL).
            // This method also allows you to return the resulting name (URL),
            // and to save your report to a storage. The method is called only for new reports.
            SetData(report, defaultUrl);
            return defaultUrl;
        }
    }
}
