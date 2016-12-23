using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;


namespace Aphysoft.Share
{
    public class WebUtilities
    {
        private static Microsoft.Ajax.Utilities.Minifier minifier = new Microsoft.Ajax.Utilities.Minifier();

        public static Microsoft.Ajax.Utilities.Minifier Minifier
        {
            get { return WebUtilities.minifier; }
        }
        
        private static JavaScriptSerializer serializer = new JavaScriptSerializer();

        public static JavaScriptSerializer Serializer
        {
            get { return WebUtilities.serializer; }
        }
    }

    public class BindingParameters
    {
        #region Fields

        private string refererUrl;

        public string RefererUrl
        {
            get { return refererUrl; }
            set { refererUrl = value; }
        }

        #endregion

        #region Constructor

        public BindingParameters()
        {
        }

        #endregion
    }

    public class ScriptData
    {
        #region Fields

        private Dictionary<string, object> data = new Dictionary<string, object>();

        private object dataSync = new object();

        #endregion

        #region Constructor

        internal ScriptData()
        {
        }

        #endregion

        #region Methods

        internal void System(string name, string value)
        {
            Add(string.Format("____system____{0}", name), value);
        }

        public void Add(string name, string value)
        {
            AddData(name, value);
        }

        internal void System(string name, int value)
        {
            Add(string.Format("____system____{0}", name), value);
        }

        public void Add(string name, int value)
        {
            AddData(name, value);
        }

        internal void System(string name, DateTime value)
        {
            Add(string.Format("____system____{0}", name), value);
        }

        public void Add(string name, DateTime value)
        {
            AddData(name, value);
        }

        internal void System(string name, bool value)
        {
            Add(string.Format("____system____{0}", name), value);
        }

        public void Add(string name, bool value)
        {
            AddData(name, value);
        }

        internal void System(string name, int[] value)
        {
            Add(string.Format("____system____{0}", name), value);
        }

        public void Add(string name, int[] value)
        {
            AddData(name, value);
        }

        private void AddData(string name, object value)
        {
            if (!data.ContainsKey(name))
            {
                lock (dataSync)
                {
                    if (!data.ContainsKey(name))
                    {
                        data.Add(name, value);
                    }
                }
            }
        }

        internal string[] GetArrayString()
        {
            List<string> listData = new List<string>();

            foreach (KeyValuePair<string, object> pair in data)
            {
                string name = pair.Key;
                object value = pair.Value;

                listData.Add(string.Format("{0}: {1}", name, WebUtilities.Serializer.Serialize(value)));
            }

            return listData.ToArray();
        }

        internal object[] GetArrayObject()
        {
            List<object> listData = new List<object>();

            foreach (KeyValuePair<string, object> pair in data)
            {
                string name = pair.Key;
                object value = pair.Value;

                listData.Add(name);
                listData.Add(value);
            }

            return listData.ToArray();
        }

        #endregion
    }

    public class StyleSheetData
    {
        #region Fields

        private Dictionary<string, StyleSheetDataClass> css = new Dictionary<string, StyleSheetDataClass>();

        internal Dictionary<string, StyleSheetDataClass> Css
        {
            get { return css; }
        }

        private object cssSync = new object();

        #endregion

        #region Constructor

        internal StyleSheetData()
        {
        }

        #endregion

        #region Methods

        public void Add(string className, string line)
        {
            StyleSheetDataClass cssClass;

            if (!css.ContainsKey(className))
            {
                lock (cssSync)
                {
                    cssClass = new StyleSheetDataClass(className);
                    css.Add(className, cssClass);
                }
            }
            else
                cssClass = css[className];

            cssClass.AddLine(line);
        }

        #endregion
    }

    internal class StyleSheetDataClass
    {
        #region Fields

        private string identifier;

        public string Identifier
        {
            get { return identifier; }
        }

        private List<string> lines;

        internal List<string> Lines
        {
            get { return lines; }
        }

        private object linesSync = new object();

        #endregion

        #region Constructor

        public StyleSheetDataClass(string identifier)
        {
            this.identifier = identifier;

            lines = new List<string>();
        }

        #endregion

        #region Methods

        public void AddLine(string line)
        {
            lock (linesSync)
            {
                lines.Add(line);
            }
        }

        #endregion
    }
    
}
