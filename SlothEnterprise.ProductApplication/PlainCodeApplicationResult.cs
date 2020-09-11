using System;
using System.Collections.Generic;
using SlothEnterprise.External;

namespace SlothEnterprise.ProductApplication
{
    public class PlainCodeApplicationResult : IApplicationResult
    {
        private const string ResultIsReadonly = "Result is readonly";

        public PlainCodeApplicationResult(int code)
        {
            Code = code;
        }

        public int Code { get; }

        public int? ApplicationId 
        { 
            get => null;
            set => throw new NotSupportedException(ResultIsReadonly);
        }

        public bool Success 
        {
            get => Code >= 0; 
            set => throw new NotSupportedException(ResultIsReadonly);
        }

        public IList<string> Errors 
        { 
            get => Success
                ? Array.Empty<string>()
                : new[] { $"Underlying service has returned error code: {Code}" };

            set => throw new NotSupportedException(ResultIsReadonly);
        }
    }
}