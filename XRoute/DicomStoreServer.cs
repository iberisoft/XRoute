using Dicom.Network;
using Serilog;
using System.Linq;

namespace XRoute
{
    class DicomStoreServer : DicomServer<DicomStoreService>
    {
        readonly RouteService m_RouteService;

        public DicomStoreServer()
        {
            m_RouteService = Program.Resolve<RouteService>();
        }

        Settings.RouteSettings m_RouteSettings;

        public static DicomStoreServer Create(Settings.RouteSettings routeSettings)
        {
            var server = (DicomStoreServer)DicomServer.Create<DicomStoreService, DicomStoreServer>(null, routeSettings.Port);
            server.m_RouteSettings = routeSettings;
            Log.Information("Store service {AeTitle} is listening on port {Port}", routeSettings.AeTitle, routeSettings.Port);
            return server;
        }

        protected override DicomStoreService CreateScp(INetworkStream stream)
        {
            var scp = base.CreateScp(stream);
            scp.AeTitle = m_RouteSettings.AeTitle;
            scp.AuthorizeClient = (aeTitle, host) => !m_RouteSettings.AuthorizeSources ||
                m_RouteSettings.Sources.Any(source => source.AeTitle == aeTitle && (source.Host == null || source.Host == host));
            scp.OnCStoreRequest = OnCStoreRequest;
            return scp;
        }

        private DicomStatus OnCStoreRequest(DicomCStoreRequest request)
        {
            return m_RouteService.StoreAsync(request.File, m_RouteSettings).Result ? DicomStatus.Success : DicomStatus.ProcessingFailure;
        }
    }
}
