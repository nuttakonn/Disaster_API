using Disaster_API.Models.Common;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;

namespace Disaster_API.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var assemblyAll = Assembly.GetEntryAssembly()?.GetName();
            IndexModel model = new IndexModel
            {
                IP = GetIPAddress(),
                AssemblyName = assemblyAll?.Name,
                AssemblyVersion = assemblyAll?.Version?.ToString(),
                AspNetCoreEnvironment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"),
                DateModified = GetDateModified()
            };

            return View(model);
        }

        private string GetIPAddress()
        {
            try
            {
                var host = Dns.GetHostEntry(Dns.GetHostName());
                foreach (var ip in host.AddressList)
                {
                    if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ip.ToString();
                    }
                }
                return "IP not found";
            }
            catch
            {
                return "Error getting IP";
            }
        }

        private string GetDateModified()
        {
            var assemblyExe = Assembly.GetExecutingAssembly();
            FileInfo file = new FileInfo(assemblyExe.Location);
            return file.LastWriteTime.ToString("dd/MM/yyyy - HH:mm:ss", new CultureInfo("en-EN", false));
        }
    }

    
}
