using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Aphysoft.Share
{
    public delegate APIPacket APIRequest(APIAsyncResult result, string[] paths, string apiAccessID);

    public static class API
    {
        #region Fields

        private static Dictionary<string, string> api = new Dictionary<string, string>();
        private static Dictionary<string, APIRegister> apiRegisters = new Dictionary<string, APIRegister>();

        #endregion

        #region Methods

        internal static void Init()
        {
            Database share = Share.Database;

            Dictionary<string, Row> rapi = share.QueryDictionary("select * from Api", "AP_Name");

            foreach (KeyValuePair<string, Row> pair in rapi)
            {
                api.Add(pair.Key, pair.Value["AP_ID"].ToString());
                apiRegisters.Add(pair.Key, null);
            }
        }

        public static void Register(string api, APIRequest handler)
        {
            if (apiRegisters.ContainsKey(api))
            {
                apiRegisters[api] = new APIRegister(handler);
            }
        }

        internal static APIRegister Get(string api)
        {
            if (apiRegisters.ContainsKey(api))
            {
                return apiRegisters[api];
            }
            else return null;
        }

        #endregion
    }

    internal class APIRegister
    {
        private APIRequest apiRequestHandler;

        public APIRequest APIRequest
        {
            get { return apiRequestHandler; }
        }

        public APIRegister(APIRequest handler)
        {
            apiRequestHandler = handler;
        }
    }

    public class APIAsyncResult : AsyncResult
    {
        #region Fields

        private ResourceOutput resourceOutput;

        public ResourceOutput ResourceOutput
        {
            get { return resourceOutput; }
            set { resourceOutput = value; }
        }

        private Resource resource;

        public Resource Resource
        {
            get { return resource; }
            set { resource = value; }
        }

        #endregion

        #region Constructors

        public APIAsyncResult(HttpContext context, AsyncCallback callback, object asyncState) : base(context, callback, asyncState)
        {
        }

        #endregion
    }

    [DataContract]
    public class APIPacket
    {
        public APIPacket()
        {
        }

        public static APIPacket Null()
        {
            return new APIPacket();
        }
    }

    [DataContract]
    public class ErrorAPIPacket : APIPacket
    {
        #region Fields

        private string error = null;

        [DataMember(Name = "error")]
        public string Error
        {
            get { return error; }
            set { error = value; }
        }

        private int httpStatusCode = 404;

        public int HttpStatusCode
        {
            get { return httpStatusCode; }
        }

        #endregion

        #region Constructors

        public ErrorAPIPacket(string error, int statusCode)
        {
            this.error = error;
            this.httpStatusCode = statusCode;
        }

        public ErrorAPIPacket(APIErrors error)
        {
            switch (error)
            {
                case APIErrors.BadRequestFormat:
                    this.error = "Bad Request Format";
                    this.httpStatusCode = 400;
                    break;
            }
        }

        #endregion
    }

    public enum APIErrors
    {
        BadRequestFormat
    }
    
    [DataContract]
    public abstract class ResultAPIPacket : APIPacket
    {
        #region Fields

        private int resultCount = 0;

        [DataMember(Name = "resultcount")]
        public int ResultCount
        {
            get { return resultCount; }
            set { resultCount = value; }
        }

        private int resultOffset = 0;

        [DataMember(Name = "resultoffset")]
        public int ResultOffset
        {
            get { return resultOffset; }
            set { resultOffset = value; }
        }

        private int resultLimit = 0;

        [DataMember(Name = "resultlimit")]
        public int ResultLimit
        {
            get { return resultLimit; }
            set { resultLimit = value; }
        }

        #endregion
    }

    [DataContract]
    public abstract class ResultItemAPIPacket
    {
        #region Fields

        private long index = 0;

        [DataMember(Name = "index")]
        public long Index
        {
            get { return index; }
            set { index = value; }
        }

        #endregion
    }

}
