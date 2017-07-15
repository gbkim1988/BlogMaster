using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace BlogMaster.M
{
    public class SqlLiteManager : ISqlLiteManager
    {
        /// <summary>
        /// 참조 : https://www.janaks.com.np/using-sqlite-in-wpf-application/
        /// </summary>
        private static String TABLE_COLLECTED_KEYWORD = @"
        CREATE TABLE IF NOT EXISTS `KEYWORDS` (
	        `no`	INTEGER PRIMARY KEY AUTOINCREMENT,
	        `keyword`	TEXT UNIQUE,
	        `DerivedCnt`	INTEGER,
	        `Processed`	NUMERIC
        )
        ";

        private static String TABLE_NAVER_API_KEYS = @"
        CREATE TABLE IF NOT EXISTS `SecreteKeys` (
	        `no`	INTEGER PRIMARY KEY AUTOINCREMENT,
	        `CustomerID`	TEXT UNIQUE,
	        `ACCESS`	TEXT,
	        `Secret`	TEXT,
            `active`    INTEGER
        )
        ";

        private static String TABLE_PENDING_KEYWORD = @"
        CREATE TABLE IF NOT EXISTS `PENDING` (
	        `no`	INTEGER PRIMARY KEY AUTOINCREMENT,
	        `keyword`	TEXT UNIQUE,
            `Processed`	NUMERIC
        )
        ";
        private static String TABLE_COLLECTED_STATISTICS = @"
        CREATE TABLE IF NOT EXISTS `STATISTICS` (
	        `no`	INTEGER PRIMARY KEY AUTOINCREMENT,
	        `keyword`	TEXT UNIQUE,
	        `monthlyPcCnt`	INTEGER,
	        `monthlyMobCnt`	NUMERIC,
            `competive`	TEXT,
	        `blogCount`	INTEGER,
	        `noNaverBlogCount`	INTEGER,
	        `associateKeywordCount`	INTEGER
        )
        ";
        private static String INSERT_STATISTICS_QUERY = @"
        INSERT INTO `STATISTICS` (`keyword`, `monthlyPcCnt`, `monthlyMobCnt`, `competive`, `blogCount`, `noNaverBlogCount`, `associateKeywordCount`) 
        values ('{0}', {1}, {2}, '{3}', {4}, {5}, {6})
        ";

        private static String INSERT_KEYWORD_QUERY = @"
        INSERT INTO `KEYWORDS` (`keyword`, `DerivedCnt`, `Processed`) 
        values ('{0}', {1}, {2})
        ";

        private static String INSERT_PENDING_QUERY = @"
        INSERT INTO `PENDING` (`keyword`, `Processed`) 
        values ('{0}', {1})
        ";

        private static String SELECT_PENDING = @"
        SELECT keyword FROM PENDING where Processed=0 LIMIT 10;
        ";

        private static String SELECT_KEYWORD = @"
        SELECT keyword FROM KEYWORDS where Processed=0 LIMIT 10;
        ";

        private static String SELECT_PROCESSED_KEYWORD = @"
        SELECT keyword FROM KEYWORDS where Processed=1;
        ";
        /*
         	        `no`	INTEGER PRIMARY KEY AUTOINCREMENT,
	        `keyword`	TEXT UNIQUE,
	        `monthlyPcCnt`	INTEGER,
	        `monthlyMobCnt`	NUMERIC,
            `competive`	TEXT,
	        `blogCount`	INTEGER,
	        `noNaverBlogCount`	INTEGER,
	        `associateKeywordCount`	INTEGER
             
             */
        private static String SELECT_STATISTICS = @"
        SELECT no, keyword, monthlyPcCnt, monthlyMobCnt, competive, blogCount, noNaverBlogCount, associateKeywordCount  
        FROM STATISTICS WHERE monthlyPcCnt > 0 and monthlyMobCnt > 0 ORDER By monthlyPcCnt, monthlyMobCnt DESC;
        ";

        private static String DELETE_PENDING = @"
        DELETE FROM PENDING WHERE keyword='{0} COLLATE NOCASE'
        ";
        private static String DELETE_FULL_PENDING = @"
        DELETE FROM PENDING WHERE Processed = 0
        ";

        private static String INIT_ALL_PENDING = @"
        DELETE FROM PENDING 
        ";
        private static String INIT_ALL_KEYWORDS = @"
        DELETE FROM KEYWORDS 
        ";
        private static String INIT_ALL_STATISTICS = @"
        DELETE FROM STATISTICS
        ";
        private static String DELETE_FULL_KEYWORDS = @"
        DELETE FROM KEYWORDS WHERE Processed = 0
        ";
        private static String UPDATE_PENDING = @"
        UPDATE PENDING
            SET Processed=1
        WHERE keyword='{0}' COLLATE NOCASE;
        ";

        private static String UPDATE_KEYWORD = @"
        UPDATE KEYWORDS
            SET Processed=1
        WHERE keyword='{0}' COLLATE NOCASE;
        ";
        private String mSqliteName;
        private SQLiteConnection m_dbConnection;
        public SqlLiteManager(String sqlite) {
            this.mSqliteName = sqlite;
            if (!File.Exists(sqlite))
            {
                try
                {
                    SQLiteConnection.CreateFile(sqlite);
                    this.m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3", sqlite));
                    this.m_dbConnection.Open();
                    (new SQLiteCommand(SqlLiteManager.TABLE_COLLECTED_KEYWORD, this.m_dbConnection)).ExecuteNonQuery();
                    (new SQLiteCommand(SqlLiteManager.TABLE_COLLECTED_STATISTICS, this.m_dbConnection)).ExecuteNonQuery();
                    (new SQLiteCommand(SqlLiteManager.TABLE_PENDING_KEYWORD, this.m_dbConnection)).ExecuteNonQuery();
                    (new SQLiteCommand(SqlLiteManager.TABLE_NAVER_API_KEYS, this.m_dbConnection)).ExecuteNonQuery();

                }
                catch (Exception e)
                {
                    MessageBox.Show(String.Format("Create SQLite File Error {0}", e.Message));
                }

            }
            else {
                this.m_dbConnection = new SQLiteConnection(String.Format("Data Source={0};Version=3", sqlite));
                this.m_dbConnection.Open();
                (new SQLiteCommand(SqlLiteManager.TABLE_COLLECTED_KEYWORD, this.m_dbConnection)).ExecuteNonQuery();
                (new SQLiteCommand(SqlLiteManager.TABLE_COLLECTED_STATISTICS, this.m_dbConnection)).ExecuteNonQuery();
                (new SQLiteCommand(SqlLiteManager.TABLE_PENDING_KEYWORD, this.m_dbConnection)).ExecuteNonQuery();
                (new SQLiteCommand(SqlLiteManager.TABLE_NAVER_API_KEYS, this.m_dbConnection)).ExecuteNonQuery();

            }
        }

        public bool Initialize()
        {
            // If SqlLite DB File Exists

            return true;
        }

        public string info() {
            return this.mSqliteName;
        }
        public void DeleteUnPorcessedRecords()
        {
            DeleteSQL(SqlLiteManager.DELETE_FULL_KEYWORDS);
            DeleteSQL(SqlLiteManager.DELETE_FULL_PENDING);
        }
        public void AddCollectedKeyword(String keyword, int Count, int Processed) {
            String InsertQuery = String.Format(SqlLiteManager.INSERT_KEYWORD_QUERY, keyword, Count, Processed);
            try
            {
                RunInsertSQL(InsertQuery);
                //UpdateSQL(String.Format(SqlLiteManager.UPDATE_PENDING, keyword));
            }
            catch (Exception e) {
                Console.WriteLine("AddCollectedKeyword : " + e.Message);
            }
            
        }

        public void AddPendingKeyword(String keyword, int Processed)
        {
            String InsertQuery = String.Format(SqlLiteManager.INSERT_PENDING_QUERY, keyword, Processed);
            try
            {
                RunInsertSQL(InsertQuery);
            }
            catch (Exception e) {
                Console.WriteLine("AddPendingKeyword : " + e.Message);
            }
            
        }
        //`blogCount`, `noNaverBlogCount`, `associateKeywordCount`
        public void AddStatisticsKeyword(String keyword, int monthlyPcCnt, int monthlyMobCnt, String comp, int blogCount, int noNaverBlogCount, int associateKeywordCount) {
            String InsertQuery = String.Format(SqlLiteManager.INSERT_STATISTICS_QUERY, keyword, monthlyPcCnt, monthlyMobCnt, comp, blogCount, noNaverBlogCount, associateKeywordCount);
            try
            {
                RunInsertSQL(InsertQuery);
                //UpdateSQL(String.Format(SqlLiteManager.UPDATE_KEYWORD, keyword));

            }
            catch (System.Data.SQLite.SQLiteException e) {
                //e.Con
                //UpdateKeywordTable(new List<String> { keyword });
                //Console.WriteLine("IMPORTANT : " + e.Message);
            }
            finally{
                
            }
            
        }

        public void DeleteAllTables()
        {

            try
            {
                DeleteSQL(SqlLiteManager.INIT_ALL_PENDING);
                DeleteSQL(SqlLiteManager.INIT_ALL_KEYWORDS);
                DeleteSQL(SqlLiteManager.INIT_ALL_STATISTICS);
            }
            catch
            {

            }
        }

        public void UpdateKeywordTable(IList<String> keywords) {
            foreach (var keyword in keywords) {
                try
                {
                    UpdateSQL(String.Format(SqlLiteManager.UPDATE_KEYWORD, keyword));
                }
                catch (Exception e) {
                }
            }
        }

        public void UpdatePendingTable(IList<String> keywords)
        {
            foreach (var keyword in keywords)
            {
                try
                {
                    UpdateSQL(String.Format(SqlLiteManager.UPDATE_PENDING, keyword));
                }
                catch (Exception e)
                {

                }
            }
        }
        public IList<String> RetrieveKewordList()
        {
            IList<String> keyList = new List<String>();

            SQLiteDataReader reader = RunSelectSQL(SqlLiteManager.SELECT_KEYWORD);
            while (reader.Read())
            {
                keyList.Add((String)reader["keyword"]);
                UpdateSQL(String.Format(SqlLiteManager.UPDATE_KEYWORD, (String)reader["keyword"]));
            }

            return keyList;
        }

        public IList<String> RetrieveProcessedKewordList()
        {
            IList<String> keyList = new List<String>();

            SQLiteDataReader reader = RunSelectSQL(SqlLiteManager.SELECT_PROCESSED_KEYWORD);
            while (reader.Read())
            {
                keyList.Add((String)reader["keyword"]);
                UpdateSQL(String.Format(SqlLiteManager.UPDATE_KEYWORD, (String)reader["keyword"]));
            }

            return keyList;
        }

        public IList<String> RetrievePendingList() {
            IList<String> keyList = new List<String>();

            SQLiteDataReader reader = RunSelectSQL(SqlLiteManager.SELECT_PENDING);
            while (reader.Read()) {
                keyList.Add((String)reader["keyword"]);
                UpdateSQL(String.Format(SqlLiteManager.UPDATE_PENDING, (String)reader["keyword"]));
            }

            return keyList;
        }

        public IList<NaverStatistics> RetrieveStatisticsList()
        {
            IList<NaverStatistics> statsList = new List<NaverStatistics>();
            SQLiteDataReader reader = RunSelectSQL(SqlLiteManager.SELECT_STATISTICS);
            while (reader.Read())
            {
                int no = reader.GetInt32(0);
                String keyword = reader.GetString(1);// (String)reader["keyword"];
                int monthlyPcCnt = reader.GetInt32(2); //(int)reader["monthlyPcCnt"];
                int monthlyMobCnt = reader.GetInt32(3); //(int)reader["monthlyMobCnt"];
                String CompIndex = reader.GetString(4); //(String)reader["competive"];
                int blogCount;
                int noNaverBlogCount;
                int associateKeywordCount;
                try { blogCount = reader.GetInt32(5); }catch (Exception e) { blogCount = -1; }
                try { noNaverBlogCount = reader.GetInt32(6); } catch (Exception e) { noNaverBlogCount = -1;  }
                try { associateKeywordCount = reader.GetInt32(7); } catch (Exception e) { associateKeywordCount = -1; }

                statsList.Add(new NaverStatistics {
                    no = no,
                    keyword = keyword,
                    monthlyPcCnt = monthlyPcCnt,
                    monthlyMobCnt = monthlyMobCnt,
                    CompIndex = CompIndex,
                    blogCount = blogCount,
                    noNaverBlogCount = noNaverBlogCount,
                    associateKeywordCount = associateKeywordCount
                });
            }

            return statsList;
        }
        /// <summary>
        /// Below Code From https://stackoverflow.com/questions/20001129/multithreading-in-c-sharp-sqlite
        /// </summary>
        private ReaderWriterLockSlim _readerWriterLock = new ReaderWriterLockSlim();

        public SQLiteDataReader RunSelectSQL(string Sql)
        {
            SQLiteDataReader selectDataTable = null;
            
            try
            {
                _readerWriterLock.EnterReadLock();
                //Function to acess your database and return the selected results
                selectDataTable = (new SQLiteCommand(Sql, this.m_dbConnection)).ExecuteReader();
            }
            finally
            {
                _readerWriterLock.ExitReadLock();
            }
            return selectDataTable;
        }
        public void DeleteSQL(string Sql)
        {
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
                    (new SQLiteCommand(Sql, this.m_dbConnection)).ExecuteNonQuery();
                }
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }

            if (isbreaked)
            {
                Thread.Sleep(10);
                UpdateSQL(Sql);
            }
        }
        public void UpdateSQL(string Sql) {
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
                    (new SQLiteCommand(Sql, this.m_dbConnection)).ExecuteNonQuery();
                }
            }
            finally
            {
                _readerWriterLock.ExitWriteLock();
            }
            
            if (isbreaked)
            {
                Thread.Sleep(10);
                UpdateSQL(Sql);
            }
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
                    (new SQLiteCommand(Sql, this.m_dbConnection)).ExecuteNonQuery();
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
