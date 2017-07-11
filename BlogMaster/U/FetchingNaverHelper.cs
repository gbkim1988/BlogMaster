using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace BlogMaster.U
{
    public class FetchingNaverHelper
    {
        public class NaverInfo
        {
            public int TotalBlogNumber { get; set; }
            public int NonNaverBlogNumber { get; set; }
            public int NumberOfRelate { get; set; }
            public List<String> RelatedKeyword { get; set; }
        }

        public static string[] NaverFavoriteKeyword(String keyword) {
            // Use local instance 
            List<String> FavoriteKeywords = new List<String>();
            FavoriteKeywords.Clear();
            HttpClient client = new HttpClient(new HttpClientHandler { UseCookies = false });

            #region Making URL strings with paramemters
            HangulUrlBuilder builder = new HangulUrlBuilder(@"http://ac.search.naver.com/nx/ac");

            builder["_callback"] = "";
            builder["q"] = keyword;
            builder.MakeHangul("q");
            builder["q_enc"] = "UTF-8";
            builder["st"] = "100";
            builder["frm"] = "nx";
            builder["r_format"] = "json";
            builder["r_enc"] = "UTF-8";
            builder["r_unicode"] = "0";
            builder["t_koreng"] = "1";
            builder["ans"] = "2";
            builder["run"] = "2";
            builder["rev"] = "4";
            builder["con"] = "1";

            string url = builder.BuildQuery();
            /*
            var builder = new UriBuilder(@"https://ac.search.naver.com/nx/ac");
                builder.Port = -1;
                var query = HttpUtility.ParseQueryString(builder.Query);
                query["_callback"] = "";
                query["q"] = keyword;
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
                builder.Query = query.ToString();
                string url = builder.ToString();
            */
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
                dynamic dynObj = JsonConvert.DeserializeObject(respStr);
                foreach (var items in dynObj.items)
                {
                    foreach (var item in items)
                    {
                        try
                        {
                            FavoriteKeywords.Add(item[0].ToString());
                        }
                        catch (Exception e) {
                            // pass
                        }
                        
                    }
                }

            }
            else {
                throw new Exception("While processing Http Response, Unknown Error Ocurred");
            }
            return FavoriteKeywords.ToArray();
        }


        public static NaverInfo GetParsedNaverInfo(String keyword) {
            
            int TotalBlogNumber = -1;
            int NonNaverBlogNumber = -1;
            int NumberOfRelate = 1;
            List<String> RelatedKeyword = new List<String>();

            HangulUrlBuilder builder = new HangulUrlBuilder(@"https://search.naver.com/search.naver");

            builder["where"] = "post";
            builder["sm"] = "tab_jum";
            builder["ie"] = "utf8";
            builder["st"] = "100";
            builder["query"] = keyword;
            builder.MakeHangul("query");

            string url = builder.BuildQuery();
            HttpClient client = new HttpClient(new HttpClientHandler { UseCookies = false });
            client.DefaultRequestHeaders.Add("Host", "search.naver.com");
            client.DefaultRequestHeaders.Add("Connection", "close");
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36");
            client.DefaultRequestHeaders.Add("Accept", "*/*");
            client.DefaultRequestHeaders.Add("Accept-Encoding", "deflate, sdch, br");
            client.DefaultRequestHeaders.Add("Accept-Language", "ko-KR,ko;q=0.8,en-US;q=0.6,en;q=0.4");

            client.BaseAddress = new Uri(url);
            var response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                var respCont = response.Content;
                String respStr = respCont.ReadAsStringAsync().Result;
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(respStr);

                HtmlNode node = htmlDoc.DocumentNode.SelectSingleNode(String.Format("//span[@class=\"title_num\"]"));

                int.TryParse(String.Join("", node.InnerText.Split('/').Last<String>().Trim(' ').Trim('건').Split(',')), out TotalBlogNumber);

                HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes(String.Format("//*[contains(@id,'sp_blog')]"));

                if (nodes != null)
                {
                    NonNaverBlogNumber = 0;
                    foreach (var nde in nodes.Where(x => !x.Id.EndsWith("_base")))
                    {
                        String link = nde.SelectSingleNode("dl/dt/a").Attributes["href"].Value;
                        if (!(link.Contains("naver.com/") || link.Contains("blog.me/")))
                        {
                            NonNaverBlogNumber += 1;
                        }
                    }
                }
                else {
                    NonNaverBlogNumber = 0;
                }

                HtmlNodeCollection nodex = htmlDoc.DocumentNode.SelectNodes(String.Format("//dd[@class=\"lst_relate\"]/ul/li/a"));

                if (nodes != null)
                {
                    NumberOfRelate = nodex.Count;
                    foreach (var nde in nodex)
                    {
                        RelatedKeyword.Add(nde.InnerHtml);
                    }
                }
                else {
                    NumberOfRelate = 0;
                }

            }
            else
            {
                throw new Exception("While processing Http Response, Unknown Error Ocurred");
            }

            return new NaverInfo{ TotalBlogNumber = TotalBlogNumber,
                NonNaverBlogNumber = NonNaverBlogNumber,
                NumberOfRelate = NumberOfRelate,
                RelatedKeyword = RelatedKeyword
            };
        }
        //public static T ParseJsonObject<T>(string json)
    }
}
