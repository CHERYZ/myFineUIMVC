using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using FineUIMvc;
using System.Text;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace FineUIMvc.EmptyProject
{
    public class SqlHelper
    {
        #region 对查询函数加上当前版本的限制
        static public DataTable QuerySQL(string tableName, string attri) // 查询语句
        {
            string[] versionTable = { "tableValues", "tableDefine", "estimateTable", "tableBelong", "tUploadFiles", "tUploadFilesFunDefine" };// 包含version字段的表
            int hasVersion = 0;
            string sSQL;

            if (tableName.IndexOf("dbo.") == -1)// 对不规范的表的写法纠正（加上"dbo."）
                tableName = "dbo." + tableName;
            string pureTableName;
            if (tableName.IndexOf(' ') != -1)
                pureTableName = tableName.Substring(0, tableName.IndexOf(' '));
            else
                pureTableName = tableName;
            // 判断查询的表是否包含version字段
            foreach (string name in versionTable)
            {
                string rule = @"^\[{0,1}dbo\]{0,1}\.\[{0,1}" + name + @"\]{0,1}";
                if (Regex.IsMatch(tableName, rule))
                {
                    hasVersion = 1;
                    break;
                }
            }

            if (hasVersion == 0)
            {
                sSQL = string.Format("SELECT {0} FROM {1}", new object[] {
                            attri, tableName
                });
            }
            else if (tableName.Contains("WHERE"))// 处理条件查询语句（条件一般加在tableName后面）
            {
                string t1 = tableName.Substring(0, tableName.IndexOf("WHERE"));
                string t2 = tableName.Substring(t1.Length + 5);
                sSQL = string.Format("SELECT {0} FROM {1} WHERE {3}.versionID = (SELECT id FROM dbo.version WHERE active = 1) AND {2}", new object[] {
                            attri, t1, t2, pureTableName
                });
            }
            else if (tableName.Contains("ORDER BY"))// 处理排序语句
            {
                string t1 = tableName.Substring(0, tableName.IndexOf("ORDER BY"));
                string t2 = tableName.Substring(t1.Length);
                sSQL = string.Format("SELECT {0} FROM {1} WHERE versionID = (SELECT id FROM dbo.version WHERE active = 1) {2}", new object[] {
                            attri, t1, t2
                });
            }
            else
            {
                sSQL = string.Format("SELECT {0} FROM {1} WHERE versionID = (SELECT id FROM dbo.version WHERE active = 1)", new object[] {
                            attri, tableName
                });
            }
            return getDataSource(sSQL).ToTable();
        }
        static public DataTable QuerySQL(string tableName, string attri, string condition)// 带条件的查询语句
        {
            string firstStr = condition.Substring(0, condition.IndexOf(' '));
            string[] notWhereStr = { "group", "order" };
            if (((IList)notWhereStr).Contains(firstStr.ToLower()) == false)
            {
                condition = " WHERE " + condition;
            }
            tableName +=  condition;
            return QuerySQL(tableName, attri);
        }
        static public DataTable SimpleQuery(string tableName, string attri)// 简单查询
        {
            string sSQL = string.Format("SELECT {1} FROM {0} ", new object[] {
                tableName, attri
            });
            return getDataSource(sSQL).ToTable();
        }
        static public DataTable SimpleQuery(string tableName, string attri, string condition)// 简单查询
        {
            // 为带有group, order等的condition语句提供兼容
            if (condition.Contains(" "))
            {
                string firstStr = condition.Substring(0, condition.IndexOf(' '));
                string[] notWhereStr = { "group", "order" };
                if (((IList)notWhereStr).Contains(firstStr.ToLower()) == false)
                {
                    condition = "WHERE " + condition;
                }
            }
            else
                condition = "WHERE " + condition;

            string sSQL = string.Format("SELECT {0} FROM {1} {2}", new object[] {
                attri, tableName, condition
            });
            return getDataSource(sSQL).ToTable();
        }
        #endregion

        #region 查
        /*数据查询方法*/
        static public DataView getDataSource(string sSQLSelect)
        {
            string sConn = ConfigurationManager.ConnectionStrings["GraduateConnectionString"].ToString();
            SqlConnection sqlConn = new SqlConnection(sConn);
            DataView dView = new DataView();
            if (sqlConn.State == ConnectionState.Open) sqlConn.Close();
            try
            {
                sqlConn.Open();
                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = new SqlCommand(sSQLSelect, sqlConn);
                sda.SelectCommand.CommandTimeout = 0;
                DataSet ds = new DataSet();
                sda.Fill(ds, "table");
                dView.Table = ds.Tables["table"];
            }
            catch (Exception e1)
            {
                FineUIMvc.Alert.ShowInParent("数据查询失败，请检查数据！<br>出错信息：" + e1.Message, FineUIMvc.MessageBoxIcon.Error);
            }
            finally
            {
                sqlConn.Close();
            }
            return dView;
        }
        #endregion
        #region 插
        /*-----------------------数据插入方法-----------------*/
        /*有返回值得数据插入*/
        /*static public int insertDataSource(string sSQLInsert, ParameterCollection sInsertPara)
        {
            string sConn = ConfigurationManager.ConnectionStrings["GraduateConnectionString"].ToString();
            SqlConnection sqlConn = new SqlConnection(sConn);
            int rtnResult;
            if (sqlConn.State == ConnectionState.Open) sqlConn.Close();
            SqlDataSource sqlInsert = new SqlDataSource();
            sqlInsert.ConnectionString = sqlConn.ConnectionString;
            sqlInsert.InsertCommand = sSQLInsert;
            try
            {
                foreach (Parameter pTemp in sInsertPara) sqlInsert.InsertParameters.Add(pTemp);
                rtnResult = sqlInsert.Insert();
            }
            catch (Exception e)
            {
                FineUIMvc.Alert.ShowInParent("数据增加失败，请检查数据！<br>出错信息：" + e.Message, FineUIMvc.MessageBoxIcon.Error);
                rtnResult = 0;
            }
            //if (rtnResult > 0) FineUIMvc.Alert.ShowInParent("数据提交成功！",FineUIMvc.MessageBoxIcon.Information);
            return rtnResult;
        }*/
        /*无返回值得数据插入*/
        static public int insertDataSource(string sSQLInsert)
        {
            string sConn = ConfigurationManager.ConnectionStrings["GraduateConnectionString"].ToString();
            SqlConnection sqlConn = new SqlConnection(sConn);
            int rtnResult;
            if (sqlConn.State == ConnectionState.Open) sqlConn.Close();
            SqlCommand sqlCmd = new SqlCommand(sSQLInsert, sqlConn);

            sqlConn.Open();
            try
            {
                rtnResult = Convert.ToInt32(sqlCmd.ExecuteScalar());
            }
            catch (Exception e)
            {
                FineUIMvc.Alert.ShowInParent("数据增加失败，请检查数据！<br>出错信息：" + e.Message, FineUIMvc.MessageBoxIcon.Error);
                rtnResult = -1;
            }
            finally
            {
                sqlConn.Close();
            }
            //if (rtnResult > 0) FineUIMvc.Alert.ShowInParent("数据提交成功！", FineUIMvc.MessageBoxIcon.Information);
            return rtnResult;
        }
        /*插入以自增列为关键字的数据，并返回新增的自增值*/
        static public int insertIdentityDataSource(string sSQLInsert)
        {
            int rtnResult;
            string sConn = ConfigurationManager.ConnectionStrings["GraduateConnectionString"].ToString();
            SqlConnection sqlConn = new SqlConnection(sConn);
            if (sqlConn.State == ConnectionState.Open) sqlConn.Close();
            sSQLInsert += " select scope_identity()";
            SqlCommand sqlCmd = new SqlCommand(sSQLInsert, sqlConn);

            sqlConn.Open();
            try
            {
                //  newsModel.hit = reader[6] == DBNull.Value ? 0 : Convert.ToInt64(reader[6]);
                rtnResult = Convert.ToInt32(sqlCmd.ExecuteScalar());
            }
            catch (Exception e)
            {
                FineUIMvc.Alert.ShowInParent("数据增加失败，请检查数据！<br>出错信息：" + e.Message, FineUIMvc.MessageBoxIcon.Error);
                rtnResult = 0;
            }
            finally
            {
                sqlConn.Close();
            }
            //if (rtnResult > 0) FineUIMvc.Alert.ShowInParent("数据提交成功！", FineUIMvc.MessageBoxIcon.Information);
            return rtnResult;
        }
        #endregion
        #region 删
        /*----------------数据删除---------------------*/
        /*有返回值的删除*/
       /* static public int deleteDataSource(string sSqlDelete, ParameterCollection sDeletePara)
        {
            int rtnResult;
            string sConn = ConfigurationManager.ConnectionStrings["GraduateConnectionString"].ToString();
            SqlConnection sqlConn = new SqlConnection(sConn);
            SqlDataSource sqlDelete = new SqlDataSource();
            sqlDelete.ConnectionString = sqlConn.ConnectionString;
            sqlDelete.DeleteCommand = sSqlDelete;
            try
            {
                foreach (Parameter pTemp in sDeletePara) sqlDelete.DeleteParameters.Add(pTemp);
                rtnResult = sqlDelete.Delete();
                //FineUIMvc.Alert.ShowInParent("数据删除成功！", MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                rtnResult = 0;
                FineUIMvc.Alert.ShowInParent("数据删除失败！<br><br>失败信息为：" + e.Message, MessageBoxIcon.Error);
            }
            return rtnResult;
        }*/
        /*无返回值的数据删除*/
        static public int deleteDataSource(string sSqlDelete)
        {
            int rtnResult;
            string sConn = ConfigurationManager.ConnectionStrings["GraduateConnectionString"].ToString();
            SqlConnection sqlConn = new SqlConnection(sConn);
            if (sqlConn.State == ConnectionState.Open) sqlConn.Close();
            SqlCommand sqlCmd = new SqlCommand(sSqlDelete, sqlConn);
            try
            {
                sqlConn.Open();
                rtnResult = sqlCmd.ExecuteNonQuery();
                //FineUIMvc.Alert.ShowInParent("数据删除成功！", MessageBoxIcon.Information);
            }
            catch (Exception e)
            {
                rtnResult = -1;
                FineUIMvc.Alert.ShowInParent("数据删除失败！<br><br>失败信息为：" + e.Message, MessageBoxIcon.Error);
            }
            finally { sqlConn.Close(); }
            return rtnResult;
        }
        #endregion
        #region 改
        /*有返回值的修改*/
        //static public int updateDataSource(string sSQLUpdate, ParameterCollection sUpdatePara)
        //{
        //    string sConn = ConfigurationManager.ConnectionStrings["GraduateConnectionString"].ToString();
        //    SqlConnection sqlConn = new SqlConnection(sConn);
        //    SqlDataSource sqlUpdate = new SqlDataSource();
        //    sqlUpdate.ConnectionString = sqlConn.ConnectionString;
        //    sqlUpdate.UpdateCommand = sSQLUpdate;
        //    foreach (Parameter pTemp in sUpdatePara) sqlUpdate.UpdateParameters.Add(pTemp);
        //    if (sqlUpdate.Update() > 0)
        //    {
        //        //FineUIMvc.Alert.ShowInParent("数据修改成功！");
        //        return 1;
        //    }
        //    else
        //    { //FineUIMvc.Alert.ShowInParent("数据修改失败，请检查数据！", FineUIMvc.MessageBoxIcon.Error); 
        //        return 0;
        //    }
        //}
        /*无返回值的修改*/
        static public int updateDataSource(string sSQLUpdate)
        {
            string sConn = ConfigurationManager.ConnectionStrings["GraduateConnectionString"].ToString();
            SqlConnection sqlConn = new SqlConnection(sConn);
            int rtnResult;
            if (sqlConn.State == ConnectionState.Open) sqlConn.Close();
            SqlCommand sqlCmd = new SqlCommand(sSQLUpdate, sqlConn);
            sqlConn.Open();
            try
            {
                rtnResult = Convert.ToInt32(sqlCmd.ExecuteScalar());
            }
            catch (Exception e)
            {
                FineUIMvc.Alert.ShowInParent("数据修改失败，请检查数据！<br>出错信息：" + e.Message, FineUIMvc.MessageBoxIcon.Error);
                rtnResult = 0;
            }
            finally
            {
                sqlConn.Close();
            }
            //if (rtnResult > 0) FineUIMvc.Alert.ShowInParent("数据提交成功！", FineUIMvc.MessageBoxIcon.Information);
            return rtnResult;
        }
        #endregion
        #region 上传文件附件
        //注意： FineUIMvc 中使用 fileUpload 必须使用 asp.net 原带控件 而且PageManage 中 EnableAjax 设置为 false
        static public string fileUpload(System.Web.UI.WebControls.FileUpload fUpload, string sPath)
        {

            string sFileName = "";
            if (fUpload.HasFile)
            {
                //文件名：精确到毫秒
                sFileName = DateTime.Now.ToString("yyyymmddhhmmssfff") + System.IO.Path.GetExtension(fUpload.FileName);
                //上传文件
                try
                {
                    fUpload.SaveAs(System.Web.HttpContext.Current.Server.MapPath(sPath + sFileName));
                }
                catch (Exception e)
                {
                    FineUIMvc.Alert.ShowInParent("文件上传失败，请检查数据！<br>出错信息：" + e.Message, FineUIMvc.MessageBoxIcon.Error);

                }

            }
            return sFileName;
        }
        #endregion

        #region 处理附件名称
        static public void buildAttachmentName(string sFileFullName, string sPath, ref FineUIMvc.HyperLink Link1, ref FineUIMvc.HyperLink Link2, ref FineUIMvc.HyperLink Link3) //处理存储的附件名称：将名称字符串绑定到 hyperLink
        {
            Link1.NavigateUrl = ""; Link1.NavigateUrl = ""; Link1.NavigateUrl = "";
            Link1.Text = ""; Link1.Text = ""; Link1.Text = "";
            string[] sFile = sFileFullName.Split(';');

            if (sFile[0] != "")  //当一个文件都没有时，也有一个空元素
            {
                switch (sFile.Length)
                {
                    case 1:
                        Link1.Text = "附件1&nbsp;&nbsp;"; Link1.NavigateUrl = sPath + sFile[0]; break;
                    case 2:
                        Link1.Text = "附件1&nbsp;&nbsp;|&nbsp;&nbsp;"; Link1.NavigateUrl = sPath + sFile[0];
                        Link2.Text = "附件2&nbsp;&nbsp;"; Link2.NavigateUrl = sPath + sFile[1]; break;
                    case 3:
                        Link1.Text = "附件1&nbsp;&nbsp;|&nbsp;&nbsp;"; Link1.NavigateUrl = sPath + sFile[0];
                        Link2.Text = "附件2&nbsp;&nbsp;|&nbsp;&nbsp;"; Link2.NavigateUrl = sPath + sFile[1];
                        Link3.Text = "附件3&nbsp;&nbsp;"; Link3.NavigateUrl = sPath + sFile[2]; break;
                }
            }
        }
        #endregion
        public class fileOper
        {
            //--------- 以下载方式打开 ----------------
            public static void ToDownload(string serverfilpath, string filename)
            {
                FileStream fileStream = new FileStream(serverfilpath, FileMode.Open);
                long fileSize = fileStream.Length;
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + UTF_FileName(filename) + "\";");
                ////attachment --- 作为附件下载
                ////inline --- 在线打开
                HttpContext.Current.Response.AddHeader("Content-Length", fileSize.ToString());
                byte[] fileBuffer = new byte[fileSize];
                fileStream.Read(fileBuffer, 0, (int)fileSize);
                HttpContext.Current.Response.BinaryWrite(fileBuffer);
                fileStream.Close();
                HttpContext.Current.Response.End();
            }

            //--------- 直接在浏览器打开 ----------------
            public static void ToOpen(string serverfilpath, string filename)
            {
                FileStream fileStream = new FileStream(serverfilpath, FileMode.Open);
                long fileSize = fileStream.Length;
                HttpContext.Current.Response.ContentType = "application/octet-stream";
                HttpContext.Current.Response.AddHeader("Content-Disposition", "inline; filename=\"" + UTF_FileName(filename) + "\";");
                HttpContext.Current.Response.AddHeader("Content-Length", fileSize.ToString());
                byte[] fileBuffer = new byte[fileSize];
                fileStream.Read(fileBuffer, 0, (int)fileSize);
                HttpContext.Current.Response.BinaryWrite(fileBuffer);
                fileStream.Close();
                HttpContext.Current.Response.End();
            }

            private static string UTF_FileName(string filename)
            {
                return HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8);
            }


        }

        static public void gridSort(string sSelectSql, ref FineUIMvc.Grid gList, string sSortField, string sSortDirection)
        {

            DataView dView = gridBindDataSource(sSelectSql, ref gList);
            dView.Sort = String.Format("{0} {1}", sSortField, sSortDirection);
            gList.DataSource = dView;
            gList.DataBind();

        }
        static public DataView gridBindDataSource(string sSelectSql, ref FineUIMvc.Grid gList)
        {
            DataView dView = getDataSource(sSelectSql);
            gList.DataSource = dView;
            gList.DataBind();
            return dView;
        }
        static public DataView gridBindDataSourceNews(string sSelectSql, ref FineUIMvc.Grid gList)
        {
            DataView dView = getDataSourceRegister(sSelectSql);
            if (dView == null)
            { }
            else
            {
                gList.DataSource = dView;
                gList.DataBind();
            }

            return dView;
        }
        static public DataView getDataSourceRegister(string sSQLSelect)
        {
            string sConn = ConfigurationManager.ConnectionStrings["GraduateConnectionString"].ToString();
            SqlConnection sqlConn = new SqlConnection(sConn);
            DataView dView = new DataView();
            if (sqlConn.State == ConnectionState.Open) sqlConn.Close();
            try
            {
                sqlConn.Open();
                SqlDataAdapter sda = new SqlDataAdapter();
                sda.SelectCommand = new SqlCommand(sSQLSelect, sqlConn);
                sda.SelectCommand.CommandTimeout = 0;
                DataSet ds = new DataSet();
                sda.Fill(ds, "table");
                dView.Table = ds.Tables["table"];

            }
            catch
            {
                dView = null;
            }
            finally
            {

                sqlConn.Close();

            }
            return dView;
        }
        static public bool DownLoadFileNewName(string FileID, string fOriginName)
        {
            string FileUlr = HttpContext.Current.Server.MapPath(FileID); ;//获取现有文件所在位置
            string OutFileName = fOriginName;
            if (File.Exists(FileUlr))//判断该文件是否存在
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }
}
