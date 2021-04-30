using Dicom;
using Serilog;
using System;
using System.Threading.Tasks;

namespace XRoute
{
    class RouteService
    {
        readonly UploadService m_UploadService;

        public RouteService(UploadService uploadService)
        {
            m_UploadService = uploadService;
        }

        public async Task<bool> StoreAsync(DicomFile file, Settings.RouteSettings routeSettings)
        {
            var successCount = 0;
            foreach (var destinationSettings in routeSettings.Destinations)
            {
                try
                {
                    await m_UploadService.StoreAsync(file, destinationSettings);
                    ++successCount;
                }
                catch (Exception ex)
                {
                    Log.Error(ex, "Exception occurred");
                }
            }
            return successCount > 0;
        }
    }
}
