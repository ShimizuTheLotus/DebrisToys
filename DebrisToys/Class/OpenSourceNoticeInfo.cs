using System;
using System.Collections.Generic;
using System.Text;

namespace DebrisToys.Class
{
    public class OpenSourceNoticeInfo
    {
        public string CodeSourceName { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public string License { get; set; } = "Unknown Lisence";
        public string? Link { get; set; } = null;
        public string? LicenseText { get; set; } = null;
    }
}
