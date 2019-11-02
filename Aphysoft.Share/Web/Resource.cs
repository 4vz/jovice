using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.SessionState;

using System.Resources;
using System.Text;

namespace Aphysoft.Share
{
    public sealed class Resource
    {
        #region Resource Provider

        #region Fields

        private static Dictionary<string, Resource> registeredResources = new Dictionary<string, Resource>();
        private static Dictionary<string, string> keyHashes = new Dictionary<string, string>();

        private static int commonResourceScriptIndex = 100;
        private static int commonResourceCSSIndex = 100;

        #endregion

        #region Const        

        public const string CommonResourceScript = "script_common";
        public const string CommonResourceCSS = "css_common";

        #endregion

        #region Internal Methods

        internal static void Init()
        {
            Register("xhr_stream", ResourceTypes.Text, Provider.StreamBeginProcessRequest, Provider.StreamEndProcessRequest)
    .NoBufferOutput().AllowOrigin("http" + (WebSettings.Secure ? "s" : "") + "://" + WebSettings.Domain).AllowCredentials();
            Resource.Register("xhr_provider", ResourceTypes.JSON, Provider.ProviderBeginProcessRequest, Provider.ProviderEndProcessRequest);

            Resource.Register("xhr_content_provider", ResourceTypes.JSON, Content.Begin, Content.End);
        }

        internal static IAsyncResult Begin(HttpContext context, AsyncCallback cb, object extraData)
        {
            return null;
        }

        internal static void End(IAsyncResult result)
        {

        }

        #endregion

        #region Methods

        public static Resource Get(string key)
        {
            Resource con = null;

            if (keyHashes.ContainsKey(key))
            {
                string akey = keyHashes[key];

                if (registeredResources.ContainsKey(akey))
                    con = registeredResources[akey];
            }
            else
            {
                // search by name, key = name
                if (registeredResources.ContainsKey(key))
                {
                    con = registeredResources[key];
                }
            }

            return con;
        }

        public static string GetPath(string key)
        {
            if (registeredResources.ContainsKey(key))
            {
                Resource resource = registeredResources[key];

                string fileHash;

                if (resource.BeginHandler == null)
                {
                    string fileBeenUpdatedSignature = null;
                    resource.ResourceCheck();

                    if (resource.groupSources != null) // group resource
                    {
                        resource.ResourceCheck();

                        string[] igus = new string[resource.groupSources.Count];

                        int i = 0;
                        foreach (KeyValuePair<int, ResourceGroupEntry> kvp in resource.groupSources)
                        {
                            ResourceGroupEntry gentry = kvp.Value;

                            string signature = gentry.Signature;

                            igus[i++] = signature;
                        }

                        fileBeenUpdatedSignature = string.Join("", igus);
                    }
                    else
                    {
                        fileBeenUpdatedSignature = resource.MD5;
                    }

                    fileHash = Hash.Basic(StringHelper.Create(resource.Key, fileBeenUpdatedSignature));
                }
                else fileHash = "res";

                string path = string.Format("/resources/{0}/{1}{2}",
                    resource.RealName ? resource.Key : resource.KeyHash,
                    fileHash,
                    resource.FileExtension);

                return path;
            }
            else
                return string.Empty;
        }

        // C:\a\b\c  = 4
        // ../../afis/lima.txt

        /// <summary>
        /// Convert specific application path to physical path, eg. ~/path/to/file.aspx to C:\app\path\to\file.aspx
        /// </summary>
        /// <returns></returns>
        public static string GetPhysicalPath(string virtualpath)
        {
            if (virtualpath.StartsWith("../"))
            {
                string[] appPaths = HttpContext.Current.Request.PhysicalApplicationPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                string[] vPaths = virtualpath.Split(StringSplitTypes.Slash, StringSplitOptions.RemoveEmptyEntries);

                int appPathCount = appPaths.Length;
                int vPathCount = 0;
                foreach (string vp in vPaths)
                {
                    if (vp == "..")
                    {
                        appPathCount--;
                        vPathCount++;
                    }
                    else break;
                }
                return string.Join("\\", appPaths, 0, appPathCount) + "\\" + string.Join("\\", vPaths, vPathCount, vPaths.Length - vPathCount);
            }
            else
            {
                return HttpContext.Current.Server.MapPath(virtualpath);
            }

            //string path = string.Format("{0}{1}", HttpContext.Current.Request.PhysicalApplicationPath, Absolute(virtualpath).Substring(1).Replace('/', '\\'));

            //return path;
        }

        #endregion

        #region Register Resources

        /// <summary>
        /// Adds JavaScript or CSS resource as common in page resource.
        /// </summary>
        public static void Common(Resource resource)
        {
            if (resource != null && resource.BeginHandler == null)
            {
                if (resource.ResourceType == ResourceTypes.JavaScript)
                    Group(CommonResourceScript, resource.Key, commonResourceScriptIndex++);
                else if (resource.ResourceType == ResourceTypes.CSS)
                    Group(CommonResourceCSS, resource.Key, commonResourceCSSIndex++);
            }
        }

        public static Resource Group(string groupKey, string key, int position)
        {
            Resource resource = Resource.Get(key);

            if (resource != null && resource.BeginHandler == null)
            {
                Resource groupResource = Resource.Register(groupKey, resource.ResourceType);

                if (groupResource.groupSources == null)
                {
                    groupResource.groupSources = new SortedList<int, ResourceGroupEntry>();
                }

                if (!groupResource.groupSources.ContainsKey(position))
                {
                    ResourceGroupEntry entry = new ResourceGroupEntry(resource, "-");

                    groupResource.groupSources.Add(position, entry);

                    resource.groupResource = groupResource;

                    return groupResource;
                }

                return null;
            }
            else
                return null;
        }

        private static Resource Register(string key, string keyHash, ResourceTypes resourceType)
        {
            Resource resource;

            if (!registeredResources.ContainsKey(key))
            {
                resource = new Resource(key, keyHash, resourceType);

                lock (registeredResources)
                {
                    if (!registeredResources.ContainsKey(key))
                    {
                        keyHashes.Add(resource.KeyHash, key);
                        registeredResources.Add(key, resource);
                    }
                }
            }
            else
                resource = registeredResources[key];
#if DEBUG
            resource.NoMinify();
#endif
            return resource;
        }

        private static Resource Register(string key, ResourceTypes resourceType)
        {
            return Resource.Register(key, null, resourceType);
        }

        public static Resource Register(string key, string keyHash, ResourceTypes resourceType, ResourceBeginProcessRequest beginHandler, ResourceEndProcessRequest endHandler)
        {
            Resource resource = Resource.Register(key, keyHash, resourceType);

            resource.SetHandler(beginHandler, endHandler);

            return resource;
        }

        internal static Resource Register(string key, ResourceTypes resourceType, ResourceBeginProcessRequest beginHandler, ResourceEndProcessRequest endHandler)
        {
            return Register(key, null, resourceType, beginHandler, endHandler);
        }

        public static Resource Register(string key, string keyHash, ResourceTypes resourceType, string rootRelativePath)
        {
            if (rootRelativePath == null) return null;
            string physicalPath = GetPhysicalPath(rootRelativePath);

            FileInfo pathInfo = new FileInfo(physicalPath);
            if (!pathInfo.Exists) return null;

            Resource resource;

            resource = Register(key, keyHash, resourceType);
            resource.OriginalFilePath = physicalPath;
            resource.LastModified = pathInfo.LastWriteTime;
            resource.SetData(File.ReadAllBytes(physicalPath));

            return resource;
        }

        public static Resource Register(string key, ResourceTypes resourceType, string rootRelativePath)
        {
            return Resource.Register(key, null, resourceType, rootRelativePath);
        }

        public static Resource Register(string key, string keyHash, ResourceTypes resourceType, ResourceManager resourceManager, string objectName)
        {
            Resource resource = Resource.Register(key, keyHash, resourceType);

            object obj = resourceManager.GetObject(objectName);
            if (obj.GetType() == typeof(string)) resource.SetData((string)obj);
            else if (obj.GetType() == typeof(Bitmap))
            {
                Bitmap b = (Bitmap)obj;

                using (MemoryStream ms = new MemoryStream())
                {
                    if (resourceType == ResourceTypes.PNG)
                        b.Save(ms, ImageFormat.Png);
                    else if (resourceType == ResourceTypes.JPEG)
                        b.Save(ms, ImageFormat.Jpeg);
                    resource.SetData(ms.ToArray());
                }

            }
            else resource.SetData((Byte[])obj);

            return resource;
        }

        public static Resource Register(string key, ResourceTypes resourceType, ResourceManager resourceManager, string objectName)
        {
            return Resource.Register(key, null, resourceType, resourceManager, objectName);
        }

        public static Resource Register(string key, ResourceTypes resourceType, ResourceManager resourceManager, string objectName, string rootRelativePath)
        {
            return Register(key, null, resourceType, resourceManager, objectName, rootRelativePath);
        }

        public static Resource Register(string key, string keyHash, ResourceTypes resourceType, ResourceManager resourceManager, string objectName, string rootRelativePath)
        {
            Resource resource;
#if DEBUG
            if (rootRelativePath == null) return null;
            string physicalPath = GetPhysicalPath(rootRelativePath);
            FileInfo pathInfo = new FileInfo(physicalPath);
            if (!pathInfo.Exists) return null;
            resource = Register(key, keyHash, resourceType);
            resource.OriginalFilePath = physicalPath;
            resource.LastModified = pathInfo.LastWriteTime;
            resource.SetData(File.ReadAllBytes(physicalPath));

            resource.NoMinify().NoCache();
#else
            resource = Register(key, keyHash, resourceType, resourceManager, objectName);
#endif

            return resource;
        }

        public string Key { get; }

        public string KeyHash { get; }

        public ResourceTypes ResourceType { get; set; }

        private string fileExtension;

        public string FileExtension
        {
            get
            {
                if (fileExtension == null)
                {
                    switch (ResourceType)
                    {
                        case ResourceTypes.CSS: fileExtension = ".css"; break;
                        case ResourceTypes.HTML: fileExtension = ".html"; break;
                        case ResourceTypes.JavaScript: fileExtension = ".js"; break;
                        case ResourceTypes.JPEG: fileExtension = ".jpg"; break;
                        case ResourceTypes.JSON: fileExtension = ".json"; break;
                        case ResourceTypes.Text: fileExtension = ""; break;
                        case ResourceTypes.PNG: fileExtension = ".png"; break;
                        case ResourceTypes.TTF: fileExtension = ".ttf"; break;
                        case ResourceTypes.WOFF: fileExtension = ".woff"; break;
                        default: fileExtension = ".bin"; break;
                    }
                }

                return fileExtension;
            }
            set
            {
                string ival = value;
                if (ival.Length > 1 && !ival.StartsWith("."))
                    fileExtension = "." + ival;
                else if (ival == "." || ival == null)
                    fileExtension = null;
                else
                    fileExtension = ival;
            }
        }

        public string MimeType
        {
            get
            {
                string mimeType;

                switch (ResourceType)
                {
                    case ResourceTypes.CSS: mimeType = "text/css"; break;
                    case ResourceTypes.HTML: mimeType = "text/html"; break;
                    case ResourceTypes.JavaScript: mimeType = "application/javascript"; break;
                    case ResourceTypes.JPEG: mimeType = "image/jpeg"; break;
                    case ResourceTypes.JSON: mimeType = "application/json"; break;
                    case ResourceTypes.Text: mimeType = "text/plain"; break;
                    case ResourceTypes.PNG: mimeType = "image/png"; break;
                    case ResourceTypes.TTF: mimeType = "application/x-font-ttf"; break;
                    case ResourceTypes.WOFF: mimeType = "application/x-font-woff"; break;
                    default: mimeType = "application/octet-stream"; break;
                }

                return mimeType;
            }
        }

        internal byte[] data;

        public Byte[] Data
        {
            get
            {
                if (BeginHandler != null)
                    return null;

                if (groupSources != null)
                {
                    ResourceCheck();

                    if (data == null)
                    {
                        int sdatal = 0;
                        List<byte[]> sdatas = new List<byte[]>();


                        foreach (KeyValuePair<int, ResourceGroupEntry> kvp in groupSources)
                        {
                            ResourceGroupEntry gentry = kvp.Value;

                            Resource source = gentry.Resource;

                            byte[] sdata = source.Data;
                            int sdatacl = sdata.Length;
                            sdatal += sdatacl;
                            sdatas.Add(sdata);

                            // Resource type = javascript fix
                            if (ResourceType == ResourceTypes.JavaScript)
                            {
                                // Prevent "(intermediate value)() is not function"
                                if (sdatacl > 0)
                                {
                                    if (Buffer.GetByte(sdata, sdatacl - 1) == (byte)41) // if last byte is ')'
                                    {
                                        sdatal += 1;
                                        sdatas.Add(new byte[] { 59 }); // add ';'
                                    }
                                }

                                sdatal += 2;
                                sdatas.Add(new byte[] { 13, 10 });
                            }
                        }

                        if (sdatal > 0)
                        {
                            byte[] cdata = new byte[sdatal];
                            int offset = 0;
                            foreach (byte[] sdatai in sdatas)
                            {
                                Buffer.BlockCopy(sdatai, 0, cdata, offset, sdatai.Length);
                                offset += sdatai.Length;
                            }
                            data = cdata;
                        }
                    }

                    return data;
                }
                else
                {
                    if (!string.IsNullOrEmpty(this.OriginalFilePath))
                    {
                        FileInfo info = new FileInfo(this.OriginalFilePath);

                        if (info.LastWriteTime > this.LastModified)
                        {
                            // newer file
                            this.LastModified = info.LastWriteTime;
                            this.SetData(File.ReadAllBytes(this.OriginalFilePath));
                        }
                    }

                    if (data == null)
                    {
                        if (Minify)
                        {
                            if (ResourceType == ResourceTypes.JavaScript)
                            {
                                data = Encoding.UTF8.GetBytes(WebUtilities.Minifier.MinifyJavaScript(Encoding.UTF8.GetString(OriginalData)));
                            }
                            else if (ResourceType == ResourceTypes.CSS)
                            {
                                data = Encoding.UTF8.GetBytes(WebUtilities.Minifier.MinifyStyleSheet(Encoding.UTF8.GetString(OriginalData)));
                            }
                            else
                                data = OriginalData;
                        }
                        else data = OriginalData;

                        if (groupResource != null)
                        {

                            foreach (KeyValuePair<int, ResourceGroupEntry> kvp in groupResource.groupSources)
                            {
                                ResourceGroupEntry gentry = kvp.Value;
                                Resource r = gentry.Resource;

                                if (r == this)
                                {
                                    gentry.Signature = this.MD5;
                                    break;
                                }
                            }

                            groupResource.data = null;
                        }
                    }
                    return data;
                }
            }
        }

        public byte[] OriginalData { get; private set; }

        public string MD5 { get; private set; }

        internal ResourceBeginProcessRequest BeginHandler { get; private set; } = null;

        internal ResourceEndProcessRequest EndHandler { get; private set; } = null;

        private bool compressed = true;

        public bool Compressed
        {
            get { return compressed; }
            set
            {
                compressed = value;
                if (compressed == true) bufferOutput = true;
            }
        }

        public bool Cache { get; set; } = true;

        private bool minify = true;

        public bool Minify
        {
            get { return minify; }
            set
            {
                minify = value;
                data = null;
                if (minify == true) bufferOutput = true;
            }
        }

        private bool bufferOutput = true;

        public bool BufferOutput
        {
            get { return bufferOutput; }
            set
            {
                bufferOutput = value;
                if (bufferOutput == false)
                {
                    compressed = false;
                    minify = false;
                }
            }
        }

        public bool RealName { get; set; } = false;

        public string AccessControlAllowOrigin { get; set; } = null;

        public bool AccessControlAllowCredentials { get; set; } = false;

        internal string OriginalFilePath { get; set; }

        internal DateTime LastModified { get; set; } = DateTime.MinValue;

        internal SortedList<int, ResourceGroupEntry> groupSources = null;

        //internal List<Resource> groupSources = null;

        //internal Dictionary<int, Resource> groupSourcesIndexer = null;

        internal Resource groupResource = null;

        //internal List<string> groupUpdateSignature = null;

        #endregion

        #region Constructors

        public Resource(string key, string keyHash, ResourceTypes resourceType)
        {
            this.Key = key;
            this.ResourceType = resourceType;

            if (keyHash == null)
                this.KeyHash = Hash.Basic(key).Substring(0, 5);
            else
                this.KeyHash = keyHash;
        }

        #endregion

        #region Method

        private readonly byte[] utf16BigEndian = new byte[] { 254, 255 };
        private readonly byte[] utf16LittleEndian = new byte[] { 255, 254 };
        private readonly byte[] utf8 = new byte[] { 239, 187, 191 };

        public unsafe void SetData(byte[] data)
        {
            //byte[] beginDebug = System.Text.Encoding.UTF8.GetBytes("//DEBUG");
            //byte[] endDebug = Encoding.UTF8.Get
            byte[] endData = null;

            if (data.Length >= 3)
            {
                if (data[0] == utf8[0] && data[1] == utf8[1] && data[2] == utf8[2])
                {
                    endData = new byte[data.Length - 3];
                    Buffer.BlockCopy(data, 3, endData, 0, data.Length - 3);
                }
            }
            if (data.Length >= 2)
            {
                if ((data[0] == utf16BigEndian[0] && data[1] == utf16BigEndian[1]) ||
                    (data[0] == utf16LittleEndian[0] && data[1] == utf16LittleEndian[1]))
                {
                    endData = new byte[data.Length - 2];
                    Buffer.BlockCopy(data, 2, endData, 0, data.Length - 2);
                }
            }
            if (endData == null)
                endData = data;

            this.OriginalData = endData;
            this.MD5 = Hash.MD5(endData);
            this.data = null;
        }

        public void SetData(string data)
        {
            SetData(System.Text.Encoding.UTF8.GetBytes(data));
        }

        public void SetHandler(ResourceBeginProcessRequest beginHandler, ResourceEndProcessRequest endHandler)
        {
            this.BeginHandler = beginHandler;
            this.EndHandler = endHandler;

            this.OriginalData = null;
            this.data = null;
            this.MD5 = null;
        }

        internal void ResourceCheck()
        {
            if (groupSources != null)
            {
                // check all sources for modification
                foreach (KeyValuePair<int, ResourceGroupEntry> kvp in groupSources)
                {
                    ResourceGroupEntry gentry = kvp.Value;
                    Resource csource = gentry.Resource;
                    byte[] csdata = csource.Data; // this will trigger data modification
                }
            }
            else
            {
                byte[] idata = Data;
            }
        }

        public Resource NoCompressed()
        {
            this.Compressed = false;
            return this;
        }
        public Resource NoCache()
        {
            this.Cache = false;
            return this;
        }
        public Resource NoMinify()
        {
            this.Minify = false;
            return this;
        }

        public Resource NoBufferOutput()
        {
            this.BufferOutput = false;
            return this;
        }

        public Resource AllowOrigin(string corsString)
        {
            this.AccessControlAllowOrigin = corsString;
            return this;
        }

        public Resource AllowCredentials()
        {
            this.AccessControlAllowCredentials = true;
            return this;
        }

        public Resource SetFileExtension(string fileExtension)
        {
            this.FileExtension = fileExtension;
            return this;
        }

        public string GetString()
        {
            if (BeginHandler == null)
            {
                return System.Text.Encoding.UTF8.GetString(Data);
            }
            else return null;
        }

        #endregion

        #endregion
    }

    internal class ResourceGroupEntry
    {
        #region Fields

        private Resource resource;

        public Resource Resource
        {
            get { return resource; }
            set { resource = value; }
        }

        private string signature;

        public string Signature
        {
            get { return signature; }
            set { signature = value; }
        }

        #endregion

        #region Constructors

        public ResourceGroupEntry(Resource resource, string signature)
        {
            this.resource = resource;
            this.signature = signature;
        }

        #endregion
    }

    public class ResourceOutput
    {
        #region Fields

        private Byte[] data;

        internal Byte[] Data
        {
            get { return data; }
        }

        private string md5;

        public string MD5
        {
            get { return md5; }
        }

        #endregion

        #region Constructor

        public ResourceOutput()
        {
        }

        #endregion

        #region Methods

        public Byte[] GetData(Resource resource)
        {
            byte[] outputData;

            if (resource.Minify)
            {
                if (resource.ResourceType == ResourceTypes.JavaScript)
                {
                    var minifier = new Microsoft.Ajax.Utilities.Minifier();
                    outputData = Encoding.UTF8.GetBytes(minifier.MinifyJavaScript(Encoding.UTF8.GetString(data)));
                }
                else if (resource.ResourceType == ResourceTypes.CSS)
                {
                    var minifier = new Microsoft.Ajax.Utilities.Minifier();
                    outputData = Encoding.UTF8.GetBytes(minifier.MinifyStyleSheet(Encoding.UTF8.GetString(data)));
                }
                else
                    outputData = data;
            }
            else
                outputData = data;

            //ShareService.Service.Event("output data : " + outputData.Length);

            return outputData;
        }

        public void Clear()
        {
            this.data = null;
            this.md5 = null;
        }

        public void Write(string data)
        {
            Write(System.Text.Encoding.UTF8.GetBytes(data));
        }

        public void Write(Byte[] input)
        {
            if (this.data == null)
            {
                this.data = input;
            }
            else
            {
                Byte[] combined = new Byte[this.data.Length + input.Length];

                System.Buffer.BlockCopy(this.data, 0, combined, 0, this.data.Length);
                System.Buffer.BlockCopy(input, 0, combined, this.data.Length, input.Length);

                this.data = combined;
            }
            this.md5 = Hash.MD5(this.data);
        }

        #endregion
    }

    public delegate void ResourceBeginProcessRequest(ResourceAsyncResult result);

    public delegate void ResourceEndProcessRequest(ResourceAsyncResult result);

    public enum ResourceTypes
    {
        JavaScript,
        JSON,
        Text,
        CSS,
        HTML,
        JPEG,
        PNG,
        TTF,
        WOFF
    }

    public class ResourceAsyncResult : AsyncResult
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

        public ResourceAsyncResult(HttpContext context, AsyncCallback callback, object asyncState) : base(context, callback, asyncState)
        {
        }

        #endregion
    }
}