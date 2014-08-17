using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Aardwolf;

namespace Aardwolf
{
    public class FileHttpHandler
    {
        public static HandlerDispatcher.HandlerDelegate
            Handler(string basePath, string contentType, string fileExt)
        {
            return delegate(IHttpRequestContext request, UriTemplateMatch uriMatch)
            {
                var requestPath = uriMatch.BoundVariables["path"];
                var path = Path.Combine(basePath, requestPath) + fileExt;
                if (path.StartsWith(basePath) == false)
                {
                    var r = new ContentResponse(400, "No File", "");
                    return Task.FromResult<IHttpResponseAction>(r);
                }

                try
                {
                    var file = File.OpenRead(path);
                    var r = new FileContentResponse(200, "Description", contentType, file);
                    return Task.FromResult<IHttpResponseAction>(r);
                }
                catch (Exception)
                {
                    var r = new ContentResponse(400, "No File", "");
                    return Task.FromResult<IHttpResponseAction>(r);
                }
            };
        }
    }
}
