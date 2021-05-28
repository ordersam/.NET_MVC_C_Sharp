using MySql.Data.MySqlClient;
using MySQLDemo.ViewModel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
// 讀取連線字串
using System.Web.Configuration;

namespace MySQLDemo.Models
{
    public class CproductFactory
    {
        #region 共用模組
        // 讀取連線字串
        public string SqlConnectionString = WebConfigurationManager.ConnectionStrings["connStr"].ToString();

        // ================ 新增、修改、刪除 共用 ================
        public bool sqlAction(string sqlstring, List<MySqlParameter> paras)
        {
            bool result = false;
            try
            {
                // 1.接水管(Connection)
                MySqlConnection con = new MySqlConnection();
                con.ConnectionString = SqlConnectionString;
                con.Open();

                // 2.開水龍頭(Command)
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = con;
                cmd.CommandText = sqlstring;
                /*
                    ================ 防SQL-Injection 參數化 ================
                    加參數法1
                        cmd.Parameters.AddWithValue("要取代的@變數", SQL指令 例如 "%"+欄位+"%");
                    加參數法2
                        直接用loop 加入 List<SqlParameter> 每個元素
                            foreach (SqlParameter p in paras)
                                cmd.Parameters.Add(p);
                */
                if (paras != null)
                {
                    foreach (MySqlParameter p in paras)
                    {
                        cmd.Parameters.Add(p);
                    }
                }

                // 3.放水桶
                cmd.ExecuteNonQuery();
                con.Close();
                result = true;
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                Console.WriteLine(msg);
            }
            return result;
        }

        // ========================= 查詢1 連線型 =========================
        private List<Cproduct> getBySql(string sql, List<MySqlParameter> paras)
        {
            List<Cproduct> list = new List<Cproduct>();
            try
            {
                // 1.接水管(Connection)
                MySqlConnection con = new MySqlConnection(SqlConnectionString);
                con.Open();
                // 2.開水龍頭(Command)
                MySqlCommand cmd = new MySqlCommand(sql, con);
                if (paras != null)
                {
                    foreach (MySqlParameter p in paras)
                    {
                        cmd.Parameters.Add(p);
                    }
                }

                // 3.放水桶
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Cproduct x = new Cproduct();
                    x.fId = (int)reader["fId"];
                    x.fName = (string)reader["fName"];
                    list.Add(x);
                }
                con.Close();
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                Console.WriteLine(msg);
            }
            return list;
        }

        // ========================= 查詢2 離線型 =========================
        // ParasName格式 "@參數", ParasValue格式 "%" + 字串數值 + "%"
        public DataTable getBySql2(string sqlstring/*, List<string> ParasName, List<string> ParasValue*/, List<MySqlParameter> paras)
        {
            DataTable dt = new DataTable();
            try
            {
                // 1.接水管(Connection)
                MySqlConnection con = new MySqlConnection();
                con.ConnectionString = SqlConnectionString;
                con.Open();

                // 2.開水龍頭(Command)
                MySqlDataAdapter adapter = new MySqlDataAdapter(sqlstring, con);
                if (paras != null)
                {
                    foreach (MySqlParameter p in paras)
                    {
                        adapter.SelectCommand.Parameters.Add(p);
                    }
                }
                //for (int i = 0; i < ParasName.Count; i++)
                //{

                //    adapter.SelectCommand.Parameters.AddWithValue(ParasName[i], ParasValue[i]);
                //}
                // 3.放水桶
                DataSet ds = new DataSet();
                adapter.Fill(ds);
                con.Close();

                dt = ds.Tables[0];
            }
            catch (Exception ex)
            {
                string msg = ex.ToString();
                Console.WriteLine(msg);
            }
            return dt;
        }
        #endregion

        #region 查詢
        public List<Cproduct> QueryAll()
        {
            string sqlstring = "SELECT * FROM elevator.product";
            // 沒有參數給null
            List<Cproduct> list = getBySql(sqlstring, null);
            if (list.Count == 0)
                return null;
            else
                return list;
        }

        public Cproduct QueryByfId(int fId)
        {
            string sqlstring = "SELECT * FROM elevator.product where fId = @fId";

            List<MySqlParameter> paras = new List<MySqlParameter>();
            paras.Add(new MySqlParameter("fId", (object)fId));

            List<Cproduct> list = getBySql(sqlstring, paras);
            if (list.Count == 0)
                return null;
            else
                return list[0];
        }
        public Cproduct QueryByfId2(int fId)
        {
            string sqlstring = "SELECT * FROM elevator.product where fId = @fId";

            List<MySqlParameter> paras = new List<MySqlParameter>();
            paras.Add(new MySqlParameter("fId", (object)fId));

            DataTable dt = getBySql2(sqlstring, paras);
            List<Cproduct> list = dt.DataTableToList<Cproduct>();
            if ((list != null) && (list.Count > 0))
            {
                return list[0];
            }
            return null;
        }
        #endregion

        #region 修改
        public bool Update(int fId, string fName)
        {
            bool result = false;
            string sqlstring = "UPDATE elevator.product SET fName = @fName WHERE fId = @fId";

            List<MySqlParameter> paras = new List<MySqlParameter>();
            paras.Add(new MySqlParameter("fId", (object)fId));
            paras.Add(new MySqlParameter("fName", (object)fName));

            result = sqlAction(sqlstring, paras);
            return result;
        }
        #endregion

        #region 刪除
        public bool Delete(int fId)
        {
            bool result = false;
            string sqlstring = "DELETE FROM elevator.product WHERE fId = @fId";

            List<MySqlParameter> paras = new List<MySqlParameter>();
            paras.Add(new MySqlParameter("fId", (object)fId));

            result = sqlAction(sqlstring, paras);
            return result;
        }
        #endregion

        #region 新增
        public bool Add(string fName)
        {
            bool result = false;
            string sqlstring = "INSERT INTO elevator.product (fName) VALUES (@fName);";

            List<MySqlParameter> paras = new List<MySqlParameter>();
            paras.Add(new MySqlParameter("fName", (object)fName));

            result = sqlAction(sqlstring, paras);
            return result;
        }
        #endregion
    }
}