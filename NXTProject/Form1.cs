using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oracle.ManagedDataAccess.Client; //加入引用
using Oracle.ManagedDataAccess.Types; //加入引用


namespace NXTProject
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public void ConnOracle()
        {
            try
            {
                string connStr =
                    "User Id=fujiuser;Password=fujiuser;Data Source=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=172.16.2.249)(PORT=1521)))(CONNECT_DATA=(SERVICE_NAME=fujidb)))";
                using (var conn = new OracleConnection(connStr))
                {
                    conn.Open();
                    MessageBox.Show("连接成功");
                    DataSet ds = new DataSet();
                    string sql = "select * from T_DID";
                    OracleDataAdapter oda = new OracleDataAdapter(sql, conn);
                    oda.Fill(ds);
                    DataTable dt = ds.Tables[0];
                    MessageBox.Show("读取成功");
                    dataGridView1.DataSource = dt;
                }
            }
            catch (OracleException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        string as400ConnectionString = "Dsn=EBSFLIB;uid=DENSIO;password=ODBC";

        #region  获取 LOGNO

        public string GetMiLotNo(string lotNo)
        {
            string mi_lotNo = string.Empty;
            if (!string.IsNullOrWhiteSpace(lotNo))
            {
                if (lotNo.Length == 11)
                {
                    string apszno = lotNo.Substring(0, 8);
                    string apltno = lotNo.Substring(8, 3);
                    using (OdbcConnection connection = new OdbcConnection(as400ConnectionString))
                    {

                        string sql =
                            " select APCODE from EBSFLIB.MPAPBSP  where   APSZNO='{0}'  and APLTNO='{1}'  FETCH FIRST 1 ROWS ONLY ";

                        OdbcCommand command = new OdbcCommand(string.Format(sql, apszno, apltno), connection);
                        command.CommandTimeout = 6000;
                        try
                        {
                            connection.Open();
                            OdbcDataReader reader = command.ExecuteReader();
                            while (reader.Read())
                            {
                                mi_lotNo = reader[0].ToString();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("AS400 访问失败：" + ex.Message);
                        }
                        finally
                        {
                            if (connection.State != ConnectionState.Closed)
                                connection.Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("LotNo 长度不正确");
                }
            }
            return mi_lotNo;
        }

        #endregion



        private void button1_Click_1(object sender, EventArgs e)
        {
            ConnOracle();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            using (OdbcConnection connection = new OdbcConnection(as400ConnectionString))
            {

                if (connection.State == ConnectionState.Closed)
                {
                    try
                    {
                        connection.Open();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("AS400 访问失败：" + ex.Message);
                    }
                    finally
                    {
                        if (connection.State != ConnectionState.Closed)
                            connection.Close();
                    }
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

            string host = "192.9.2.42";
            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipEndPoint = new IPEndPoint(ip, 30040);

            Socket mySocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Console.Write("riben");
            try
            {
                mySocket.Connect(ipEndPoint);
            }
            catch (Exception exception)
            {
                throw;
            }

            mySocket.Listen(5);
            Console.Write("监听已打开");
        }

    }   
}

