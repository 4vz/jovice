using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Threading;

namespace Aphysoft.Share
{
    public class ResourceResult : IAsyncResult
    {
        private HttpContext context;
        private AsyncCallback callback;
        private object asyncState;
        private bool isCompleted = false;
        private object responseObject;
        private ResourceOutput resourceOutput;
        private Resource resource;

        public ResourceOutput ResourceOutput
        {
            get { return resourceOutput; }
            set { resourceOutput = value; }
        }

        public Resource Resource
        {
            get { return resource; }
            set { resource = value; }
        }

        public ResourceResult(HttpContext context, AsyncCallback callback, object asyncState)
        {
            this.context = context;
            this.callback = callback;
            this.asyncState = asyncState;
        }

        #region IAsyncResult Members

        public object AsyncState
        {
            get { return asyncState; }
        }

        public WaitHandle AsyncWaitHandle
        {
            get { throw new InvalidOperationException("ASP.NET Should never use this property"); }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        public bool IsCompleted
        {
            get { return isCompleted; }
        }

        #endregion

        public HttpContext Context
        {
            get { return context; }
        }

        public HttpResponse Response
        {
            get { return context.Response; }
        }

        public HttpRequest Request
        {
            get { return context.Request; }
        }

        public object ResponseObject
        {
            get { return responseObject; }
            set { responseObject = value; }
        }

        public void SetCompleted()
        {
            isCompleted = true;

            if (callback != null)               
                callback(this);
        }

        private int tag;

        public int Tag
        {
            get { return tag; }
            set { tag = value; }
        }
    }
}
