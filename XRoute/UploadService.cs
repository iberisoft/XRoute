using Dicom;
using Dicom.Network;
using System.Threading.Tasks;
using DicomClient = Dicom.Network.Client.DicomClient;

namespace XRoute
{
    class UploadService
    {
        public async Task StoreAsync(DicomFile file, Settings.DestinationSettings destinationSettings)
        {
            var client = new DicomClient(destinationSettings.Host, destinationSettings.Port, false, "XRoute", destinationSettings.AeTitle);
            client.NegotiateAsyncOps();
            var request = new DicomCStoreRequest(file);
            await client.AddRequestAsync(request);
            await client.SendAsync();
        }
    }
}
