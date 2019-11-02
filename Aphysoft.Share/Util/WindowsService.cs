using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public static class WindowsService
    {
        public static ServiceController GetService(string name)
        {
            if (name == null) return null;

            ServiceController serviceController = null;

            foreach (ServiceController svc in ServiceController.GetServices())
            {
                if (svc.ServiceName == name)
                {
                    serviceController = svc;
                    break;
                }
            }

            return serviceController;
        }

        public static ManagementObject GetServiceManagementObject(ServiceController service)
        {
            if (service == null) return null;

            ManagementObject wmiService = new ManagementObject("Win32_Service.Name='" + service.ServiceName + "'");
            wmiService.Get();

            return wmiService;
        }

        public static string GetServiceDisplayName(ManagementObject serviceMO)
        {
            if (serviceMO == null) return null;

            return serviceMO["DisplayName"].ToString();
        }
    }
}
