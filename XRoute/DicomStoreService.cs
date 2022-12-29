using Dicom;
using Dicom.Log;
using Dicom.Network;
using System;
using System.Text;
using System.Threading.Tasks;

namespace XRoute
{
    class DicomStoreService : DicomService, IDicomServiceProvider, IDicomCEchoProvider, IDicomCStoreProvider
    {
        public DicomStoreService(INetworkStream stream, Encoding fallbackEncoding, Logger log)
            : base(stream, fallbackEncoding, log)
        {
        }

        static readonly DicomTransferSyntax[] m_TransferSyntaxes =
        {
            DicomTransferSyntax.ExplicitVRLittleEndian,
            DicomTransferSyntax.ExplicitVRBigEndian,
            DicomTransferSyntax.ImplicitVRLittleEndian
        };

        static readonly DicomTransferSyntax[] m_ImageTransferSyntaxes =
        {
               DicomTransferSyntax.JPEGLSLossless,
               DicomTransferSyntax.JPEG2000Lossless,
               DicomTransferSyntax.JPEGProcess14SV1,
               DicomTransferSyntax.JPEGProcess14,
               DicomTransferSyntax.RLELossless,
               DicomTransferSyntax.JPEGLSNearLossless,
               DicomTransferSyntax.JPEG2000Lossy,
               DicomTransferSyntax.JPEGProcess1,
               DicomTransferSyntax.JPEGProcess2_4,
               DicomTransferSyntax.ExplicitVRLittleEndian,
               DicomTransferSyntax.ExplicitVRBigEndian,
               DicomTransferSyntax.ImplicitVRLittleEndian
        };

        Task IDicomServiceProvider.OnReceiveAssociationRequestAsync(DicomAssociation association)
        {
            if (association.CalledAE != AeTitle)
            {
                return SendAssociationRejectAsync(DicomRejectResult.Permanent, DicomRejectSource.ServiceUser, DicomRejectReason.CalledAENotRecognized);
            }
            if (!AuthorizeClient(association.CallingAE, association.RemoteHost))
            {
                return SendAssociationRejectAsync(DicomRejectResult.Permanent, DicomRejectSource.ServiceUser, DicomRejectReason.CallingAENotRecognized);
            }

            foreach (var context in association.PresentationContexts)
            {
                if (context.AbstractSyntax == DicomUID.Verification)
                {
                    context.AcceptTransferSyntaxes(m_TransferSyntaxes);
                }
                else
                {
                    if (context.AbstractSyntax.StorageCategory != DicomStorageCategory.None)
                    {
                        context.AcceptTransferSyntaxes(m_ImageTransferSyntaxes);
                    }
                    else
                    {
                        context.SetResult(DicomPresentationContextResult.RejectAbstractSyntaxNotSupported);
                    }
                }
            }

            return SendAssociationAcceptAsync(association);
        }

        Task IDicomServiceProvider.OnReceiveAssociationReleaseRequestAsync()
        {
            return SendAssociationReleaseResponseAsync();
        }

        void IDicomService.OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason)
        {
        }

        void IDicomService.OnConnectionClosed(Exception exception)
        {
            OnConnectionClosed?.Invoke();
        }

        DicomCEchoResponse IDicomCEchoProvider.OnCEchoRequest(DicomCEchoRequest request)
        {
            return new DicomCEchoResponse(request, DicomStatus.Success);
        }

        DicomCStoreResponse IDicomCStoreProvider.OnCStoreRequest(DicomCStoreRequest request)
        {
            var status = OnCStoreRequest?.Invoke(request);
            return new DicomCStoreResponse(request, status);
        }

        void IDicomCStoreProvider.OnCStoreRequestException(string tempFileName, Exception e)
        {
        }

        public Func<DicomCStoreRequest, DicomStatus> OnCStoreRequest { get; set; }
    }
}
