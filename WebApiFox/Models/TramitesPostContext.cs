using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Web;
using System.Configuration;
namespace WebApiFox.Models
{
    public class TramitesPostContext: IDisposable
    {
        OdbcConnection cn;
        public TramitesPostContext(string dsnName= "dnsPost")
        {
            var dns = ConfigurationManager.AppSettings[dsnName];
            cn = new OdbcConnection();
            cn.ConnectionString = "Dsn=" + dns;
            cn.Open();
        }

        public void Dispose()
        {
            this.cn.Close();
        }

        public DataTable GetData(string sql) {
            OdbcDataAdapter da = new OdbcDataAdapter(sql, this.cn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return ds.Tables[0];
        }

    }
    public class DiasFestivos {
        public int id_dia_festivo { get; set; }
        public DateTime fecha { get; set; }
        public string descripcion { get; set; }
    }
    public class TipoTramite {
        public decimal id_tipo_tramite { get; set; }
        public string codigo_tramite { get; set; }
        public  string nombre { get; set; }
    }
}