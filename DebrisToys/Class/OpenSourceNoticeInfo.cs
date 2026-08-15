using System;
using System.Collections.Generic;
using System.Text;

namespace DebrisToys.Class
{
    public class OpenSourceNoticeInfo
    {
        public string PackageName { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string License { get; set; } = "Unknown";
        public string? LicenseLink { get; set; } = null;
        public string? LicenseText { get; set; } = null;
    }
}
