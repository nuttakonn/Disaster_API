using System;

namespace Disaster_API.Models.Common;

 public class IndexModel
    {
        public string? IP { get; set; }
        public string? AssemblyName { get; set; }
        public string? AssemblyVersion { get; set; }
        public string? AspNetCoreEnvironment { get; set; }
        public string? DateModified { get; set; }
    }