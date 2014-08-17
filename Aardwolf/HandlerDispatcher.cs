using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Aardwolf
{
    public class HandlerDispatcher : IHttpAsyncHandler
    {
        public delegate Task<IHttpResponseAction> HandlerDelegate(
            IHttpRequestContext request,
            UriTemplateMatch uriMatch);

        class UriHandler
        {
            public UriTemplate UriTemplate;
            public HandlerDelegate Handler;
        }

        private readonly List<UriHandler> _uriHandlers = new List<UriHandler>();

        public void AddHandler(UriTemplate uriTemplate, HandlerDelegate handler)
        {
            _uriHandlers.Add(new UriHandler()
            {
                UriTemplate = uriTemplate,
                Handler = handler
            });
        }

        public async Task<IHttpResponseAction> Execute(IHttpRequestContext state)
        {
            var baseUri = new Uri(state.Request.Url.Scheme + "://" + state.Request.Url.Authority);

            // Find a handler matched by URI

            HandlerDelegate handler = null;
            UriTemplateMatch uriMatch = null;
            foreach (var i in _uriHandlers)
            {
                var match = i.UriTemplate.Match(baseUri, state.Request.Url);
                if (match != null)
                {
                    handler = i.Handler;
                    uriMatch = match;
                    break;
                }
            }

            if (handler == null)
            {
                var r = new ContentResponse(400, "Cannot find handler", "");
                return r;
            }

            // Handle request

            try
            {
                var r = await handler(state, uriMatch);
                return r;
            }
            catch (Exception e)
            {
                var r = new ContentResponse(500, "", "Error!\n" + e);
                return r;
            }
        }
    }
}
