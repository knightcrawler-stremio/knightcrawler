// <auto-generated/>
#pragma warning disable
using Microsoft.Extensions.Logging;
using System.Net.Http;

namespace Internal.Generated.WolverineHandlers
{
    // START: GetImdbDataRequestHandler1674247617
    public class GetImdbDataRequestHandler1674247617 : Wolverine.Runtime.Handlers.MessageHandler
    {
        private readonly System.Net.Http.IHttpClientFactory _httpClientFactory;
        private readonly Microsoft.Extensions.Logging.ILogger<Metadata.Features.DownloadImdbData.GetImdbDataRequestHandler> _logger;

        public GetImdbDataRequestHandler1674247617(System.Net.Http.IHttpClientFactory httpClientFactory, Microsoft.Extensions.Logging.ILogger<Metadata.Features.DownloadImdbData.GetImdbDataRequestHandler> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }



        public override async System.Threading.Tasks.Task HandleAsync(Wolverine.Runtime.MessageContext context, System.Threading.CancellationToken cancellation)
        {
            var getImdbDataRequestHandler = new Metadata.Features.DownloadImdbData.GetImdbDataRequestHandler(_httpClientFactory, _logger);
            // The actual message body
            var getImdbDataRequest = (Metadata.Features.DownloadImdbData.GetImdbDataRequest)context.Envelope.Message;

            
            // The actual message execution
            var outgoing1 = await getImdbDataRequestHandler.Handle(getImdbDataRequest, cancellation).ConfigureAwait(false);

            
            // Outgoing, cascaded message
            await context.EnqueueCascadingAsync(outgoing1).ConfigureAwait(false);

        }

    }

    // END: GetImdbDataRequestHandler1674247617
    
    
}
