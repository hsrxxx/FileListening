using System;
using System.Data;
using System.Data.SqlClient;

namespace FileListening
{
    public class DBHelper
    {
        private string _DBAddress = "";
        private string _DBName = "";
        private string _DBUser = "";
        private string _DBPassword = "";
        private string _ConnectionString = "";
        //构造函数 
        public DBHelper()
        {

        }
        public DBHelper(string connectionString)
        {
            try
            {
                SqlConnectionStringBuilder sqlcnn = new SqlConnectionStringBuilder(connectionString);
                _DBAddress = sqlcnn.DataSource;
                _DBName = sqlcnn.InitialCatalog;
                _DBUser = sqlcnn.UserID;
                _DBPassword = sqlcnn.Password;
                _ConnectionString = "server=" + _DBAddress + ";database=" + _DBName + ";uid=" + _DBUser + ";pwd=" + _DBPassword + "";

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public DBHelper(string svrIPAddress, string svrDataBaseName, string svrUser, string svrPassword)
        {
            _DBAddress = svrIPAddress;
            _DBName = svrDataBaseName;
            _DBUser = svrUser;
            _DBPassword = svrPassword;
            _ConnectionString = "server=" + svrIPAddress + ";database=" + svrDataBaseName + ";uid=" + svrUser + ";pwd=" + svrPassword + "";
        }

        public string USEDB
        {
            get { return _DBName; }
            set
            {
                _DBName = value;
                _ConnectionString = "server=" + _DBAddress + ";database=" + _DBName + ";uid=" + _DBUser + ";pwd=" + _DBPassword + "";
            }
        }
        public string DBAddres
        {
            //如果服务器地址变更。则必需创建一个新实例
            get { return _DBAddress; }
        }

        public string ConnectionString
        {
            get { return _ConnectionString; }
            set
            {
                _ConnectionString = value;
                try
                {
                    SqlConnectionStringBuilder sqlcnn = new SqlConnectionStringBuilder(_ConnectionString);
                    _DBAddress = sqlcnn.DataSource;
                    _DBName = sqlcnn.InitialCatalog;
                    _DBUser = sqlcnn.UserID;
                    _DBPassword = sqlcnn.Password;

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public string SelectDataValue(string sql, string sqlconnectstring = "")
        {
            string strcnn = "";
            if (sqlconnectstring == "")
            {
                if (_ConnectionString == "")
                {
                    //return ""; //如果连接字符串为空时是否立即结束程序执行
                }
                strcnn = _ConnectionString;

            }
            else
            {
                strcnn = sqlconnectstring;
            }
            using (SqlConnection sqlcnn = new SqlConnection(strcnn))
            {

                using (SqlCommand sqlcmd = new SqlCommand(sql, sqlcnn))
                {
                    using (SqlDataAdapter sqldap = new SqlDataAdapter(sqlcmd))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            try
                            {
                                sqlcnn.Open();
                                sqldap.Fill(ds);
                                return ds.Tables[0].Rows[0][0].ToString();
                            }
                            catch (Exception ex)
                            {
                                //进行日志记录,并记录sql
                                return "";
                            }
                        }
                    }
                }
            }
        }
        public DataTable SelectDataTable(string sql, string sqlconnectstring = "")
        {
            string strcnn = "";
            if (sqlconnectstring == "")
            {
                if (_ConnectionString == "")
                {
                    //return null; //如果连接字符串为空时是否立即结束程序执行
                }
                strcnn = _ConnectionString;
            }
            else
            {
                strcnn = sqlconnectstring;
            }
            using (SqlConnection sqlcnn = new SqlConnection(strcnn))
            {
                using (SqlCommand sqlcmd = new SqlCommand(sql, sqlcnn))
                {
                    using (SqlDataAdapter sqldap = new SqlDataAdapter(sqlcmd))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            try
                            {
                                sqlcnn.Open();
                                sqldap.Fill(ds);
                                return ds.Tables[0];
                            }
                            catch (Exception ex)
                            {
                                //进行日志记录,并记录sql
                                return null;
                            }
                        }
                    }
                }
            }
        }

        public DataSet SelectDataSet(string sql, out string ErrorMsg, string sqlconnectstring = "")
        {
            string strcnn = "";
            if (sqlconnectstring == "")
            {
                if (_ConnectionString == "")
                {
                    //return null; //如果连接字符串为空时是否立即结束程序执行
                }
                strcnn = _ConnectionString;
            }
            else
            {
                strcnn = sqlconnectstring;
            }
            using (SqlConnection sqlcnn = new SqlConnection(strcnn))
            {
                using (SqlCommand sqlcmd = new SqlCommand(sql, sqlcnn))
                {
                    using (SqlDataAdapter sqldap = new SqlDataAdapter(sqlcmd))
                    {
                        using (DataSet ds = new DataSet())
                        {
                            try
                            {
                                sqlcnn.Open();
                                sqldap.Fill(ds);
                                sqlcmd.Dispose();
                                sqlcnn.Close();
                                sqlcnn.Dispose();
                                ErrorMsg = "";
                                return ds;
                            }
                            catch (Exception ex)
                            {
                                sqlcmd.Dispose();
                                sqlcnn.Close();
                                sqlcnn.Dispose();
                                //进行日志记录,并记录sql
                                ErrorMsg = ex.Message;
                                return null;
                            }
                            finally
                            {
                                if (sqlcmd != null)
                                {
                                    sqlcmd.Dispose();
                                }
                                if (sqlcnn != null)
                                {
                                    sqlcnn.Close();
                                    sqlcnn.Dispose();
                                }
                            }
                        }
                    }
                }
            }
        }
        public Int64 SaveDataBySqlCommand(string sql, string sqlConnectString = "", int ReturnID = 0, bool AtTrans = false)
        {
            string strcnn = "";
            if (sqlConnectString == "")
            {
                if (_ConnectionString == "")
                {
                    //return -10000; //如果连接字符串为空时是否立即结束程序执行
                }
                strcnn = _ConnectionString;
            }
            else
            {
                strcnn = sqlConnectString;
            }
            if (ReturnID == 1)
            {
                sql = sql + ";SELECT SCOPE_IDENTITY() AS ID";
            }
            if (AtTrans)
            {
                sql = " set nocount on " +
                      ";declare @tran_error int=0 " +
                      ";begin tran tran_handle " +
                      " begin try " + sql + "; commit tran end try " +
                      " begin catch set @tran_error = @tran_error + 1 ;rollback tran;throw end catch" +
                      ";set nocount off ";
            }
            using (SqlConnection sqlcnn = new SqlConnection(strcnn))
            {
                using (SqlCommand sqlcmd = new SqlCommand(sql, sqlcnn))
                {
                    try
                    {
                        sqlcnn.Open();
                        if (ReturnID == 0)
                        {
                            return sqlcmd.ExecuteNonQuery();
                        }
                        else
                        {
                            Int64 r = Convert.ToInt64(sqlcmd.ExecuteScalar());

                            return r;
                        }

                    }
                    catch (Exception ex)
                    {
                        //进行日志记录,并记录sql
                        throw ex;
                    }
                }
            }

        }
        #region 高效能，大数据量存储方案SqlBulkCopy
        public void SaveDataByBulkCopy(DataTable UseDataTable, string TableName, string sqlConnectString = "")
        {
            string strcnn = "";
            if (sqlConnectString == "")
            {
                strcnn = _ConnectionString;
            }
            else
            {
                strcnn = sqlConnectString;
            }
            DataTable tmpdt = SelectDataTable("select * from " + TableName + " where 1<>1");
            if (tmpdt != null)
            {
                foreach (DataColumn tmpcol in tmpdt.Columns)
                {
                    if (!UseDataTable.Columns.Contains(tmpcol.ColumnName))
                    {
                        UseDataTable.Columns.Add(tmpcol.ColumnName, tmpcol.DataType);
                    }
                }
            }

            using (SqlConnection sqlcnn = new SqlConnection(strcnn))
            {

                using (SqlBulkCopy sqlBCLaborReport = new SqlBulkCopy(sqlcnn))
                {
                    try
                    {
                        sqlcnn.Open();
                        sqlBCLaborReport.BatchSize = UseDataTable.Rows.Count;
                        sqlBCLaborReport.BulkCopyTimeout = 60;
                        sqlBCLaborReport.DestinationTableName = TableName;
                        DataTable mappingDataTable = getMappingRelatin(UseDataTable, TableName, strcnn);
                        for (int i = 0; i < mappingDataTable.Rows.Count; i++)
                        {
                            sqlBCLaborReport.ColumnMappings.Add(mappingDataTable.Rows[i][0].ToString(), mappingDataTable.Rows[i][1].ToString());
                        }
                        sqlBCLaborReport.WriteToServer(UseDataTable);

                    }
                    catch (Exception ex)
                    {

                        throw ex;

                    }
                    finally
                    {
                        if (sqlcnn.State != ConnectionState.Closed) sqlcnn.Close();
                    }

                }
            }

        }
        // SqlBulkCopy映射关系处理
        private DataTable getMappingRelatin(DataTable dtsource, string TableName = "", string sqlCnnString = "", string MethodName = "")
        {
            string strcnn = "";
            if (sqlCnnString == "")
            {
                if (_ConnectionString == "")
                {
                    //return ""; //如果连接字符串为空时是否立即结束程序执行
                }
                strcnn = _ConnectionString;

            }
            else
            {
                strcnn = sqlCnnString;
            }

            string strCol = "";
            for (int iCol = 0; iCol < dtsource.Columns.Count; iCol++)
            {
                if (strCol == "")
                {
                    strCol = "'" + dtsource.Columns[iCol].ColumnName + "'";

                }
                else
                {
                    strCol += ",'" + dtsource.Columns[iCol].ColumnName + "'";
                }
            }
            if (strCol == "") return null;
            DataTable TableColName = SelectDataTable("select b.name from sysobjects a inner join syscolumns b on b.id=a.id where a.id=OBJECT_ID ('" + TableName + "') and b.name in (" + strCol + ")", strcnn);
            if (TableColName == null) return null;

            try
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("source", typeof(string));
                dt.Columns.Add("trage", typeof(string));

                if (MethodName == "")
                {
                    //默认方法名为空，表字段与datatable字段名称一致
                    for (int iRow = 0; iRow < TableColName.Rows.Count; iRow++)
                    {
                        DataRow dr = dt.NewRow();

                        dr["source"] = dtsource.Columns[TableColName.Rows[iRow][0].ToString()].ColumnName;
                        dr["trage"] = TableColName.Rows[iRow][0].ToString();
                        dt.Rows.Add(dr);
                    }

                }
                else
                {
                    //特殊方法特殊处理 请使作case 处理
                    //dr["source"] = "ID"; dr["trage"] = "ID"; dt.Rows.Add(dr);

                    //默认方法名为空，表字段与datatable字段名称一致
                    for (int iRow = 0; iRow < TableColName.Rows.Count; iRow++)
                    {
                        DataRow dr = dt.NewRow();

                        dr["source"] = dtsource.Columns[TableColName.Rows[iRow][0].ToString()].ColumnName;
                        dr["trage"] = TableColName.Rows[iRow][0].ToString();
                        dt.Rows.Add(dr);
                    }
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        #endregion
        //**********************************************
    }
}
