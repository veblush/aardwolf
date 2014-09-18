using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Aardwolf
{
    public class JsonHttpHandler
    {
        public class RequestContext
        {
            public IHttpRequestContext Request;
            public String HttpMethod;
            public Uri Url;
            public UriTemplateMatch UrlMatch;
            public JObject Data;
        }

        public class ResponseContext
        {
            public int Status;
            public JObject Data;
        }

        public static HandlerDispatcher.HandlerDelegate
            Handler(Func<RequestContext, ResponseContext, Task> func)
        {
            return async delegate(IHttpRequestContext request, UriTemplateMatch uriMatch)
            {
                JObject requestJson = null;

                if (request.Request.ContentType == "application/json")
                {
                    Encoding encoding = new UTF8Encoding();
                    using (var bodyStream = request.Request.InputStream)
                    using (var streamReader = new StreamReader(bodyStream, encoding))
                    {
                        var str = await streamReader.ReadToEndAsync();
                        try
                        {
                            requestJson = JObject.Parse(str);
                        }
                        catch (JsonException e)
                        {
                            return new ContentResponse(400, "Error in parsing json", e.ToString());
                        }
                    }
                }

                var q = new RequestContext
                {
                    Request = request,
                    HttpMethod = request.Request.HttpMethod,
                    Url = request.Request.Url,
                    UrlMatch = uriMatch,
                    Data = requestJson
                };
                var r = new ResponseContext
                {
                    Status = 200,
                    Data = null,
                };
                await func(q, r);
                return new JsonResponse(r.Status, "", r.Data);
            };
        }
    }
}
