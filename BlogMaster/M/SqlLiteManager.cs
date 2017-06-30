using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlogMaster.M
{
    public class SqlLiteManager : ISqlLiteManager
    {
        /// <summary>
        /// 참조 : https://www.janaks.com.np/using-sqlite-in-wpf-application/
        /// </summary>
        private String mDbPath;
        public SqlLiteManager(String DbPath) {
            this.mDbPath = DbPath;
        }

        public bool Initialize()
        {

            return true;
        }

        /// <summary>
        /// Below Code From https://stackoverflow.com/questions/20001129/multithreading-in-c-sharp-sqlite
        /// </summary>
        private ReaderWriterLockSlim _readerWriterLock = new ReaderWriterLockSlim();

        public DataTable RunSelectSQL(string Sql)
        {
            DataTable selectDataTable = null;
            try
            {
                _readerWriterLock.EnterReadLock();
                //Function to acess your database and return the selected results
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
            return selectDataTable;
        }

        public DataTable RunInsertSQL(string Sql)
        {
            DataTable selectDataTable = null;
            bool isbreaked = false;
            try
            {
                _readerWriterLock.EnterWriteLock();
                if (_readerWriterLock.WaitingReadCount > 0)
                {
                    isbreaked = true;
                }
                else
                {
                    //Function to insert data in your database
                }
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }

            if (isbreaked)
            {
                Thread.Sleep(10);
                return RunInsertSQL(Sql);
            }
            return selectDataTable;
        }

    }
}
