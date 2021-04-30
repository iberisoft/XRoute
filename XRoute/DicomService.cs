using Dicom.Log;
using Dicom.Network;
using System;
using System.Text;

namespace XRoute
{
    class DicomService : Dicom.Network.DicomService
    {
        public DicomService(INetworkStream stream, Encoding fallbackEncoding, Logger log)
            : base(stream, fallbackEncoding, log) { }

        public string AeTitle { get; set; }

        public Func<string, string, bool> AuthorizeClient { get; set; }

        public Action OnConnectionClosed { get; set; }
    }
}
