using RestSharp;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Naver.SearchAd
{
    class SampleProgram
    {
        static void Main(string[] args)
        {
            var baseUrl = ConfigurationManager.AppSettings["BASE_URL"];
            var apiKey = ConfigurationManager.AppSettings["API_KEY"];
            var secretKey = ConfigurationManager.AppSettings["SECRET_KEY"];
            var managerCustomerId = long.Parse(ConfigurationManager.AppSettings["CUSTOMER_ID"]);

            var rest = new SearchAdApi(baseUrl, apiKey, secretKey);
                        
            for (int i = 0; i < 10; i++)
            {
                var request = new RestRequest("/keywordstool", Method.GET);
                request.AddQueryParameter("hintKeywords", "한글");
                List<KeywordListResponse> customerLinks = rest.Execute<List<KeywordListResponse>>(request, managerCustomerId);
            }
            
            var request2 = new RestRequest("/keywordstool", Method.GET);
            request2.AddQueryParameter("hintKeywords", "window,browsers,browsers1,browsers2,browsers3");
            List<KeywordListResponse> tooDi = rest.Execute<List<KeywordListResponse>>(request2, managerCustomerId);
            /*
            Console.WriteLine(customerLinks.Count);
            foreach (var item in customerLinks) {
                foreach (var item2 in item.KeywordList)
                {
                    Console.WriteLine(item2.monthlyAvePcClkCnt);
                    Console.WriteLine(item2.relKeyword);
                }
            }
            //foreach(var item in customerLinks)
            //long customerId = customerLinks[0].ClientCustomerId;

            request = new RestRequest("/ncc/campaigns", Method.GET);
            List<Campaign> campaigns = rest.Execute<List<Campaign>>(request, managerCustomerId);
            */
        }
    }
}
