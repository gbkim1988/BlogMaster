using BlogMaster.U;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using static BlogMaster.U.FetchingNaverHelper;

namespace NaverParsingTest
{
    class Program
    {
        static void Main(string[] args) {
            int TotalBlogNumber = -1;
            int NonNaverBlogNumber = -1;
            int NumberOfRelate = 1;
            List<String> RelatedKeyword = new List<String>();

            HangulUrlBuilder builder = new HangulUrlBuilder(@"https://search.naver.com/search.naver");

            builder["where"] = "post";
            builder["sm"] = "tab_jum";
            builder["ie"] = "utf8";
            builder["st"] = "100";
            builder["query"] = "키워드";
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
                else
                {
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
                else
                {
                    NumberOfRelate = 0;
                }

            }
            else
            {
                throw new Exception("While processing Http Response, Unknown Error Ocurred");
            }

            NaverInfo naverInfo = new NaverInfo
            {
                TotalBlogNumber = TotalBlogNumber,
                NonNaverBlogNumber = NonNaverBlogNumber,
                NumberOfRelate = NumberOfRelate,
                RelatedKeyword = RelatedKeyword
            };

            Console.WriteLine(naverInfo.NonNaverBlogNumber);
            Console.WriteLine(naverInfo.NumberOfRelate);
            Console.WriteLine(String.Join("\n", naverInfo.RelatedKeyword.ToArray<String>()));
            Console.WriteLine(naverInfo.TotalBlogNumber);

            Program.Main2(new string[] { "", "" });
        }
        static void Main2(string[] args)
        {
            // //*[@id="main_pack"]/div[2]/div/span
            // //span[@class="title_num"]
            //https://search.naver.com/search.naver?where=post&sm=tab_jum&ie=utf8&query=%EC%A4%8C%EC%98%AC%EC%BD%94%EB%93%9C
            HangulUrlBuilder builder = new HangulUrlBuilder(@"https://search.naver.com/search.naver");

            builder["where"] = "post";
            builder["sm"] = "tab_jum";
            builder["ie"] = "utf8";
            builder["st"] = "100";
            builder["query"] = "키워드";
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

                // 총 검색 수 도출 완료
                int TotalNumber;
                Console.WriteLine(node.InnerText.Split('/').Last<String>().Trim(' ').Trim('건'));
                int.TryParse(String.Join("", node.InnerText.Split('/').Last<String>().Trim(' ').Trim('건').Split(',')), out TotalNumber);
                Console.WriteLine(TotalNumber);

                ////*[@id="sp_blog_1"]/dl/dd[3]/span/a[3]
                HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes(String.Format("//*[contains(@id,'sp_blog')]"));
                foreach (var nde in nodes.Where(x => !x.Id.EndsWith("_base"))) {

                        String link = nde.SelectSingleNode("dl/dt/a").Attributes["href"].Value;
                        if ( !(link.Contains("naver.com/") || link.Contains("blog.me/")) )
                            Console.WriteLine(link);

                }

                HtmlNodeCollection nodex = htmlDoc.DocumentNode.SelectNodes(String.Format("//dd[@class=\"lst_relate\"]/ul/li/a"));

                if (nodex != null)
                {
                    Console.WriteLine(nodex.Count);
                    foreach (var nde in nodex)
                    {
                        Console.WriteLine(nde.InnerHtml);
                    }
                }

            }
            else
            {
                throw new Exception("While processing Http Response, Unknown Error Ocurred");
            }

            /*
            HangulUrlBuilder builder2 = new HangulUrlBuilder(@"https://search.naver.com/search.naver");

            builder2["where"] = "nexearch";
            builder2["ie"] = "utf8";
            builder2["query"] = "추천검색어";
            builder2.MakeHangul("query");

            string url2 = builder2.BuildQuery();
            HttpClient client2 = new HttpClient(new HttpClientHandler { UseCookies = false });
            client2.DefaultRequestHeaders.Add("Host", "search.naver.com");
            client2.DefaultRequestHeaders.Add("Connection", "close");
            client2.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36");
            */

            //client2.DefaultRequestHeaders.Add("Accept", "*/*");
            /*
            client2.DefaultRequestHeaders.Add("Accept-Encoding", "deflate, sdch, br");
            client2.DefaultRequestHeaders.Add("Accept-Language", "ko-KR,ko;q=0.8,en-US;q=0.6,en;q=0.4");

            client2.BaseAddress = new Uri(url2);
            var response2 = client2.GetAsync(url2).Result;

            if (response2.IsSuccessStatusCode)
            {
                var respCont = response2.Content;
                String respStr = respCont.ReadAsStringAsync().Result;
                HtmlDocument htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(respStr);

                HtmlNodeCollection nodes = htmlDoc.DocumentNode.SelectNodes(String.Format("//dd[@class=\"lst_relate\"]/ul/li/a"));

                if(nodes != null)
                {
                    Console.WriteLine(nodes.Count);
                    foreach (var node in nodes) {
                        Console.WriteLine(node.InnerHtml);
                    }
                }
                // 총 검색 수 도출 완료
                

                ////*[@id="sp_blog_1"]/dl/dd[3]/span/a[3]
                

            }
            else
            {
                throw new Exception("While processing Http Response, Unknown Error Ocurred");
            }
            */

        }
    }
}
