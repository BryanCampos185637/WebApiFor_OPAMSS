using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Web;

namespace WebApiFox.Models
{
    public class ContextUbicaciones:IDisposable
    {
        OdbcConnection cn;
        public ContextUbicaciones(string dsnName= "dnsUbicacion")
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
        public DataTable GetMunicipioParcelario(string sql)
        {
            OdbcDataAdapter da = new OdbcDataAdapter(sql, cn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            cn.Close();
            return ds.Tables[0];
        }

   

    }
    public class ParcelasMunicipio
    {
        public int id { get; set; }
        public string cod_parcel { get; set; }
        public string cod_mun { get; set; }
        public string geom { get; set; }

    }

    public class LimiteMunicipio
    {

        public int id { get; set; }
        public string geom { get; set; }
        public string nom_mun { get; set; }
        public string id_mun { get; set; }

    }

    public class LimiteDistrito
    {

        public int id { get; set; }
        public string geom { get; set; }
        public string distrito { get; set; }

    }

    public class Geometry
    {
        public string type { get; set; }
        public List<List<List<List<double>>>> coordinates { get; set; }
    }

    public class Feature
    {
        public string type { get; set; } = "Feature";
        public Object properties { get; set; }
        public Geometry geometry { get; set; }
    }
  
    public class JsonClass
    {
        public string type { get; set; } = "FeatureCollection";
        public IEnumerable<Feature> features { get; set; }
    }
}