using BlogMaster.U;
using BlogMaster.VM;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using static BlogMaster.U.FetchingNaverHelper;

namespace BlogMaster.M
{
    public class LowestSlave : Notifier
    {
        public enum Status {
            Pending = 1,
            Processing = 2,
            Finish = 3,
            Error= -1,
        }
        public Stopwatch mElapsed;
        public Status mStat;
        public String mKeyword;
        public String mWorkerName;
        public int mStemFrom;
        public ISqlLiteManager mLiteManager;
        public LowestSlave(String keyword, ISqlLiteManager db, String Worker="Worker") {
            Stat = Status.Pending;
            this.mElapsed = new Stopwatch();
            Keyword = keyword;
            this.mLiteManager = db;
            WorkerName = SyncHelperUtil.RandomNameGenerator(Worker);
        }
        public int StemFrom {
            get {
                return this.mStemFrom;
            }
            private set {
                this.mStemFrom = value;
                RaisePropertyChanged("StemFrom");
            }
        }

        public string Keyword
        {
            get
            {
                return this.mKeyword;
            }

            private set
            {
                this.mKeyword = value;
                RaisePropertyChanged("Keyword");
            }
        }

        public Status Stat {
            get {
                return this.mStat;
            }

            private set {
                this.mStat = value;
                RaisePropertyChanged("Stat");
            }
        }

        public string WorkerName
        {
            get
            {
                return this.mWorkerName;
            }

            private set
            {
                this.mWorkerName = value;
                RaisePropertyChanged("WorkerName");
            }
        }
        public async Task<bool> Collect()
        {
            /*
                테스트의 목적은 Pending 상태에서, 
                프로세스 상태, 그리고 Finish 상태로 페이즈 전환이 
                정확히 View 에 반영되는가에 달려있다. 
             */

            Stat = Status.Processing;
            //await U.AsyncHelperUtil.Delay(5000);
            NaverAdAPI naverApi = new NaverAdAPI(KeywordMasterViewModel.NaverApiURL, 
                    KeywordMasterViewModel.ACCESS,
                    KeywordMasterViewModel.Secret,
                    KeywordMasterViewModel.CsID
                );
            var request = new RestRequest("/keywordstool", Method.GET);
            request.AddQueryParameter("hintKeywords", this.mKeyword);

            try
            {
                List<KeywordListResponse> Statistics = naverApi.Execute<List<KeywordListResponse>>(request, long.Parse(KeywordMasterViewModel.CsID));
                
                try
                {
                    foreach (var item in Statistics)
                    {
                        foreach (var props in item.KeywordList)
                        {
                            int monthlyPcQcCnt;
                            int monthlyMobileQcCnt;
                            String relKeyword;
                            String compIdx;
                            try { monthlyPcQcCnt = int.Parse(props.monthlyPcQcCnt);}catch(Exception e){ monthlyPcQcCnt = -2;}
                            try { monthlyMobileQcCnt = int.Parse(props.monthlyMobileQcCnt); } catch (Exception e) { monthlyMobileQcCnt = -2; }
                            relKeyword = props.relKeyword;
                            compIdx = props.compIdx;
                            StemFrom = monthlyPcQcCnt;
                            NaverInfo naverInfo = GetParsedNaverInfo(relKeyword);
                            foreach (var relate in naverInfo.RelatedKeyword) {
                                this.mLiteManager.AddPendingKeyword(relate, 0);
                            }
                            this.mLiteManager.AddStatisticsKeyword(relKeyword, 
                                monthlyPcQcCnt, 
                                monthlyMobileQcCnt,
                                compIdx, 
                                naverInfo.TotalBlogNumber, 
                                naverInfo.NonNaverBlogNumber, 
                                naverInfo.NumberOfRelate);

                        }
                    }
                    //this.mLiteManager.AddCollectedKeyword(this.Keyword, this.mStemFrom, 0);
                }
                catch (Exception e)
                {
                    Stat = Status.Error;
                }

            }
            catch (Exception e)
            {
                Stat = Status.Error;
            }
            finally
            {
                Stat = Status.Finish;
                this.mElapsed.Start();
            }
            return true;
        }

        public async Task<bool> Work() {
            /*
                테스트의 목적은 Pending 상태에서, 
                프로세스 상태, 그리고 Finish 상태로 페이즈 전환이 
                정확히 View 에 반영되는가에 달려있다. 
             */
            
            Stat = Status.Processing;
            //await U.AsyncHelperUtil.Delay(5000);
            var result = FetchingNaverHelper.NaverFavoriteKeyword(this.mKeyword);
            StemFrom = result.Length;
            try {
                try
                {
                    this.mLiteManager.AddCollectedKeyword(this.Keyword, this.mStemFrom, 0);
                }
                catch (Exception e) {
                }
                
                foreach (var elem in result) {
                    try
                    {
                        this.mLiteManager.AddPendingKeyword(elem, 0);
                    }
                    catch (Exception e) {
                        Console.WriteLine(e.Message);
                    }
                    
                }
            }
            catch (Exception e) {
                Stat = Status.Error;
            }
            finally {
                Stat = Status.Finish;
                this.mElapsed.Start();
            }
            return true;
        }
    }
}
