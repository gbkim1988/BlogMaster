using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogMaster.U
{
    public class SyncHelperUtil
    {
        public static Random RandomAssociates = new Random();
        public static String RandomNameGenerator(String baseName) {
            String RndName = "";
            RndName = String.Format("{0}-{1}", baseName ,(RandomAssociates.Next() % 10000).ToString());
            return RndName;
        }
    }
}
