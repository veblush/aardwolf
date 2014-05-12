using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#pragma warning disable 1998

namespace Aardwolf
{
    public sealed class JsonResponse : StatusResponse, IHttpResponseAction
    {
        readonly object _value;

        public JsonResponse(int statusCode, string statusDescription, object value)
            : base(statusCode, statusDescription)
        {
            _value = value;
        }

        public override async Task Execute(IHttpRequestResponseContext context)
        {
            SetStatus(context);
            context.Response.ContentType = "application/json; charset=utf-8";
            //context.Response.ContentEncoding = UTF8.WithoutBOM;

            var sb = new StringBuilder(0x400);
            using (var textWriter = new StringWriter(sb))
                Json.Serializer.Serialize(textWriter, _value);

            using (context.Response.OutputStream)
            using (var tw = new StreamWriter(context.Response.OutputStream, UTF8.WithoutBOM, 65536, true))
                await tw.WriteAsync(sb.ToString());
        }
    }
}
