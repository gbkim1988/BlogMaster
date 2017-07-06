using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AsyncTest
{

    class Program
    {
        public class HangulUrlBuilder
        {
            public String mBaseUrl;
            public Dictionary<String, String> mParamDict;
            public HangulUrlBuilder(String baseUrl) {
                this.mBaseUrl = baseUrl;
                this.mParamDict = new Dictionary<String, String>();
            }   

            public String this[string key]
            {
                get { return this.mParamDict[key]; }
                set { this.mParamDict[key] = value; }
            }

            public String BuildQuery() {
                String Query = "";
                List<String> tmpList = new List<String>();
                foreach (var param in this.mParamDict) {
                    tmpList.Add(String.Format("{0}={1}", param.Key, param.Value));
                    /*
                    foreach (var x in param.Value) {
                        Console.WriteLine(String.Format("{0} : {1}", param, x));
                    }*/
                }
                Query = String.Join("&", tmpList.ToArray<String>());
                Console.WriteLine(Query);
                Query = String.Format("{0}?{1}", this.mBaseUrl, Query);
                Console.WriteLine(Query);
                return Query;
            }

            public bool MakeHangul(String key) {
                bool result = false;
                String tmp = "";
                UTF8Encoding utf8 = new UTF8Encoding();
                byte[] utf8Bytes = utf8.GetBytes(this.mParamDict[key]);
                foreach (byte b in utf8Bytes)
                {
                    tmp += String.Format("%{0:X}", b);
                    //Console.Write("%{0:X}", b); // byte를 16진수로 표기합니다.
                }
                this.mParamDict[key] = tmp;
                return result;
            }

        }
        static void Main(string[] args)
        {
            List<String> FavoriteKeywords = new List<String>();
            UTF8Encoding utf8 = new UTF8Encoding();

            HttpClient client = new HttpClient(new HttpClientHandler { UseCookies = false });
            byte[] utf8Bytes = utf8.GetBytes("한글아 놀자");
            foreach (byte b in utf8Bytes)
            {
                Console.Write("{0:X} ", b); // byte를 16진수로 표기합니다.
            }
            //Console.WriteLine(utf8.GetString(utf8Bytes));
            HangulUrlBuilder hanguleBuilder = new HangulUrlBuilder(@"http://ac.search.naver.com/nx/ac");

            //hanguleBuilder["q"] = "대한민국";            
            
            hanguleBuilder["_callback"] = "";
            hanguleBuilder["q"] = "시발";
            hanguleBuilder.MakeHangul("q");
            //query["q"] = utf8.GetString(utf8Bytes);
            hanguleBuilder["q_enc"] = "UTF-8";
            hanguleBuilder["st"] = "100";
            hanguleBuilder["frm"] = "nx";
            hanguleBuilder["r_format"] = "json";
            hanguleBuilder["r_enc"] = "UTF-8";
            hanguleBuilder["r_unicode"] = "0";
            hanguleBuilder["t_koreng"] = "1";
            hanguleBuilder["ans"] = "2";
            hanguleBuilder["run"] = "2";
            hanguleBuilder["rev"] = "4";
            hanguleBuilder["con"] = "1";

            string url = hanguleBuilder.BuildQuery();
            Console.WriteLine(url);
            //hanguleBuilder.MakeHangul("p");
            #region Making URL strings with paramemters
            var builder = new UriBuilder(@"http://ac.search.naver.com/nx/ac");
            builder.Port = -1;
            var query = HttpUtility.ParseQueryString(builder.Query);
            query["_callback"] = "";
            query["q"] = "대한민국";
            //query["q"] = utf8.GetString(utf8Bytes);
            query["q_enc"] = "UTF-8";
            query["st"] = "100";
            query["frm"] = "nx";
            query["r_format"] = "json";
            query["r_enc"] = "UTF-8";
            query["r_unicode"] = "0";
            query["t_koreng"] = "1";
            query["ans"] = "2";
            query["run"] = "2";
            query["rev"] = "4";
            query["con"] = "1";
            //builder.Query = query.ToString();
            //Console.WriteLine(query.ToString());
            //Console.WriteLine(builder.Query);
            
            //string url = builder.ToString();
            //Console.WriteLine(url);
            #endregion

            #region Making Http Request Header
            client.DefaultRequestHeaders.Add("Host", "ac.search.naver.com");
            client.DefaultRequestHeaders.Add("Connection", "close");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "deflate, sdch, br");
            client.DefaultRequestHeaders.Add("Accept-Language", "ko-KR,ko;q=0.8,en-US;q=0.6,en;q=0.4");
            client.DefaultRequestHeaders.Add("Cookie", "npic=6Xng4oNBzrgwRRlNBCDqv/Qg3SztBDDLYmUmM6Q0FGyum7ORqjjjtvP0STzjarziCA==; NNB=A5A5SU6XJQKVS; nid_iplevel=1; page_uid=TSxpsspySo0ssbEVSgVssssssOV-352025; nx_ssl=2; nx_open_so=1");
            #endregion

            client.BaseAddress = new Uri(url);

            
            var response = client.GetAsync(url).Result;
            
            if (response.IsSuccessStatusCode)
            {
                var respCont = response.Content;
                String respStr = respCont.ReadAsStringAsync().Result;
                Console.WriteLine(respStr);
                dynamic dynObj = JsonConvert.DeserializeObject(respStr);


                foreach (var items in dynObj.items)
                {
                    foreach(var item in items)
                    Console.WriteLine("Keyword" + ":" + item[0]);
                }


            }
        }
    }
}
