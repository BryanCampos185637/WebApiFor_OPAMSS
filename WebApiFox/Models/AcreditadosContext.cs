using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Web;

namespace WebApiFox.Models
{
    public class AcreditadosContext: IDisposable
    {
        OdbcConnection cn;
        public AcreditadosContext(string dsnName = "dnsTramites")
        {
            var dns = ConfigurationManager.AppSettings[dsnName];
            cn = new OdbcConnection();
            cn.ConnectionString = "Dsn=" + dns + ";Read Only=true";
            cn.Open();
        }
        public void Dispose()
        {
            this.cn.Close();
        }

        public DataTable GetQuery(string sql)
        {
            OdbcDataAdapter da = new OdbcDataAdapter(sql, cn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            cn.Close();
            return ds.Tables[0];
        }
        public DataTable GetAcreditados(string NoAcre)
        {
            string ifwhere = "";
            if (NoAcre != "")
            {
                ifwhere = string.Format("WHERE licencia == '{0}'", NoAcre);
            }
            var sql = string.Format("SELECT Licencia, Nombre,Profesion FROM prof {0}",ifwhere);
            var dtable = this.GetQuery(sql);

            return dtable;

        }

    }
}