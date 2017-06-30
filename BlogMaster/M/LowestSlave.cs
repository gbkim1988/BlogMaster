using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BlogMaster.M
{
    public class LowestSlave
    {
        public enum Status {
            Pending = 1,
            Processing = 2,
            Finish = 3,
            Error= -1,
        }
        private Status mStat;
        private String mKeyword;
        public LowestSlave(String keyword, SqlLiteManager db) {
            this.mStat = Status.Pending;
            this.mKeyword = keyword;
        }

        public async Task<bool> Work() {
            // Slave Status Change
            this.mStat = Status.Processing;
            try {

            }
            catch (Exception e) {
                
            }
            finally {

            }
            return true;
        }
    }
}
