using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.U
{
    public class HangulUrlBuilder
    {
        public String mBaseUrl;
        public Dictionary<String, String> mParamDict;
        public HangulUrlBuilder(String baseUrl)
        {
            this.mBaseUrl = baseUrl;
            this.mParamDict = new Dictionary<String, String>();
        }

        public String this[string key]
        {
            get { return this.mParamDict[key]; }
            set { this.mParamDict[key] = value; }
        }

        public String BuildQuery()
        {
            String Query = "";
            List<String> tmpList = new List<String>();
            foreach (var param in this.mParamDict)
            {
                tmpList.Add(String.Format("{0}={1}", param.Key, param.Value));
            }
            Query = String.Join("&", tmpList.ToArray<String>());
            Query = String.Format("{0}?{1}", this.mBaseUrl, Query);
            return Query;
        }

        public bool MakeHangul(String key)
        {
            bool result = false;
            String tmp = "";
            UTF8Encoding utf8 = new UTF8Encoding();
            byte[] utf8Bytes = utf8.GetBytes(this.mParamDict[key]);
            foreach (byte b in utf8Bytes)
            {
                tmp += String.Format("%{0:X}", b);
            }
            this.mParamDict[key] = tmp;
            return result;
        }
    }
}
