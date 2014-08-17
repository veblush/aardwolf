using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Aardwolf
{
    public sealed class FileContentResponse : StatusResponse
    {
        private readonly string _contentType;
        private readonly Stream _content;

        public FileContentResponse(int statusCode, string statusDescription, string contentType, Stream content)
            : base(statusCode, statusDescription)
        {
            _contentType = contentType;
            _content = content;
        }

        public override async Task Execute(IHttpRequestResponseContext context)
        {
            SetStatus(context);
            context.Response.ContentLength64 = _content.Length;
            context.Response.SendChunked = false;
            context.Response.ContentType = _contentType;

            using (_content)
            using (context.Response.OutputStream)
            {
                await _content.CopyToAsync(context.Response.OutputStream);
            }
        }
    }
}
