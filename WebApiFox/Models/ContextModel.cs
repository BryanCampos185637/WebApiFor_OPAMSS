using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Data.Odbc;
using System.Linq;
using System.Web;
using System.Reflection;
using System.Configuration;
namespace WebApiFox.Models
{
    public class ContextModel:IDisposable   
    {
        OdbcConnection cn;
        public ContextModel(string dsnName= "dnsTramites")
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

        public DataTable GetQuery(string sql) {
            OdbcDataAdapter da = new OdbcDataAdapter(sql, cn);
            DataSet ds = new DataSet();
            da.Fill(ds);
            cn.Close();
            return ds.Tables[0];
            }

        public DataTable GetTramites(string anio, string tabla, string requesdata = "") {
            anio = anio.Replace(@"'", "");
            anio = anio.Replace(@"/", "");
            anio = anio.Replace(@"?", "");
            anio = anio.Replace(@"%", "");
            anio = anio.Replace(@"+", "");
            anio = anio.Replace(@"=", "");
            anio = anio.Replace(@"or", "");
            anio = anio.Replace(@"-", "");
            anio = anio.Replace(@".", "");   
            
            tabla = tabla.Replace(@"'", "");
            tabla = tabla.Replace(@"/", "");
            tabla = tabla.Replace(@"?", "");
            tabla = tabla.Replace(@"%", "");
            tabla = tabla.Replace(@"+", "");
            tabla = tabla.Replace(@"=", "");
            tabla = tabla.Replace(@"or", "");
            tabla = tabla.Replace(@"-", "");
            tabla = tabla.Replace(@".", "");

            var complementoWhere = "";
           
            var sql = "";
            switch (tabla)
            {
                case "mo":

                    if (!string.IsNullOrEmpty(requesdata))
                    {
                        complementoWhere = string.Format(" and expediente='{0}'", requesdata);
                    }

                    sql = "SELECT codigo_uso, expediente as numeroExpediente, entidad, n_entidad, acceso1, n_acceso1, acceso2, n_acceso2, acceso3, n_acceso3, poligono,municipio FROM {1}" +
                        " WHERE right(expediente, 4) = '{0}' {2}";

                    break;
                case "sc":

                    if (!string.IsNullOrEmpty(requesdata))
                    {
                        complementoWhere = string.Format(" and expediente='{0}'", requesdata);
                    }

                    sql = "SELECT nom_proy, expediente as numeroExpediente, resolucion as estado , entidad ,n_entidad ,acceso1 , n_acceso1, acceso2 , n_acceso2 ,acceso3 , n_acceso3 ,Municipio FROM {1}" + " WHERE right(expediente, 4) = '{0}'  {2}";

                    break;
                case "iv":

                    if (!string.IsNullOrEmpty(requesdata))
                    {
                        complementoWhere = string.Format(" and expediente='{0}'", requesdata);
                    }

                    sql = "SELECT nom_proy, expediente as numeroExpediente, entidad ,n_entidad ,acceso1 , n_acceso1, acceso2 , n_acceso2 ,acceso3 , n_acceso3  FROM {1}" + " WHERE right(expediente, 4) = '{0}'  {2}";

                    break;
                default:

                    if (!string.IsNullOrEmpty(requesdata))
                    {
                        complementoWhere = string.Format(" and t.expediente='{0}'", requesdata);
                    }
                    string licReson = "";
                    if (tabla == "pp" || tabla == "pps")
                    {
                        licReson = "t.urbano as licrespon,";
                    }
                    else
                    {
                        licReson = "t.lic_respon as licrespon,";
                    }
                    sql = "SELECT " + licReson + " t.codigo_uso, t.expediente as numeroExpediente, t.resolucion as estado,t2.nombre descripcionEstado, t.entidad, t.n_entidad, t.acceso1, t.n_acceso1, t.nom_proy,"
                         + " t.acceso2, t.n_acceso2, t.acceso3, t.n_acceso3, t.poligono, t.f_ing_rec, t.f_rno_rec, t.f_emi_memo, t.f_r1_sal,"
                         + " t.f_r1_ing, t.f_r2_ing,t.categoria, t.retiromemo, t.municipio, t.f_rno_sal1, t.f_rno_sal2,t.f_asig_tec,t.f_inspec,t.f_sal_rec,t.f_resoluc {3} FROM {1} t join resolucion t2 on t.resolucion=t2.resolucion"
                         + " WHERE right(t.expediente, 4) = '{0}' AND t.resolucion <> '' {2} order by f_r1_ing";

                    break;
            }      

            complementoWhere = complementoWhere.Replace(@"/", "");
            complementoWhere = complementoWhere.Replace(@"?", "");
            complementoWhere = complementoWhere.Replace(@"%", "");
            complementoWhere = complementoWhere.Replace(@"+", "");

            complementoWhere = complementoWhere.Replace(@"or", "");


            var fromculplete = "";

            if (tabla.Trim().ToLower() == "cl" || tabla.Trim().ToLower() == "pc")
            {
                fromculplete = ", t.uso_esp";
            }
            else {
                fromculplete = ", 'N/A' uso_esp";
            }

            sql = string.Format(sql, anio, tabla,complementoWhere, fromculplete);
            var dtable= this.GetQuery(sql);
           /* dtable.Columns.Add(new DataColumn("descripcionEstado", "".GetType()));

            dtable.AsEnumerable().Where(p => p["estado"].ToString() == "M").ToList().ForEach(p =>
            {
                p["descripcionEstado"] = "Memo";
            });
            dtable.AsEnumerable().Where(p => p["estado"].ToString() == "F").ToList().ForEach(p =>
            {
                p["descripcionEstado"] = "Favorable";
            });
            dtable.AsEnumerable().Where(p => p["estado"].ToString() == "C").ToList().ForEach(p =>
            {
                p["descripcionEstado"] = "Condicionado";
            });
            dtable.AsEnumerable().Where(p => p["estado"].ToString() == "D").ToList().ForEach(p =>
            {
                p["descripcionEstado"] = "Denegado";
            });
            dtable.AsEnumerable().Where(p => p["estado"].ToString() == "T").ToList().ForEach(p =>
            {
                p["descripcionEstado"] = "Tramite";
            });*/
            
            return dtable;
        }

       
    }

    public static class TableExtencion {
        public static IEnumerable<object> toIenumerable(this DataTable  tabla) {
            List<Dictionary<string, object>> jsonList = new List<Dictionary<string, object>>();
            foreach (DataRow row in tabla.Rows)
            {
                var unarow = new Dictionary<string, object>();
                foreach (DataColumn unacol in tabla.Columns)
                {
                    unarow.Add(unacol.ColumnName, row[unacol.ColumnName]);
                }
                jsonList.Add(unarow);
            }
            return jsonList;
        }


        public static List<T> ConvertDataTable<T>(this DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
    }
}