using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Odbc;
using System.Reflection;

namespace WebApiFox.Models
{
    public class ContextModel : IDisposable
    {
        OdbcConnection connectionDbFox;
        public ContextModel(string dsnName = "dnsTramites")
        {
            var dns = ConfigurationManager.AppSettings[dsnName];
            connectionDbFox = new OdbcConnection();
            connectionDbFox.ConnectionString = $"Dsn={dns};Read Only=true";
            connectionDbFox.Open();
        }

        public void Dispose()
        {
            this.connectionDbFox.Close();
        }

        public DataTable GetQuery(string querySql)
        {
            OdbcDataAdapter odbcDataAdapter = new OdbcDataAdapter(querySql, connectionDbFox);
            DataSet dataSet = new DataSet();
            odbcDataAdapter.Fill(dataSet);
            connectionDbFox.Close();
            return dataSet.Tables[0];
        }

        private string RemoverCaracteresEspeciales(string valorString)
        {
            valorString = valorString.Replace(@"'", "");
            valorString = valorString.Replace(@"/", "");
            valorString = valorString.Replace(@"?", "");
            valorString = valorString.Replace(@"%", "");
            valorString = valorString.Replace(@"+", "");
            valorString = valorString.Replace(@"=", "");
            valorString = valorString.Replace(@"or", "");
            valorString = valorString.Replace(@"-", "");
            valorString = valorString.Replace(@".", "");
            return valorString;
        }

        #region Original [Quitar palabra Async]
        public DataTable GetTramitesDbFox(string anio, string tabla, string codigoExpediente = "")
        {
            anio = RemoverCaracteresEspeciales(anio);

            tabla = RemoverCaracteresEspeciales(tabla);

            var filtroPorCodigoExpediente = "";

            var sql = "";
            switch (tabla)
            {
                case "mo":

                    if (!string.IsNullOrEmpty(codigoExpediente))
                    {
                        filtroPorCodigoExpediente = string.Format(" and expediente='{0}'", codigoExpediente);
                    }

                    sql = "SELECT codigo_uso, expediente as numeroExpediente, entidad, n_entidad, acceso1, n_acceso1, acceso2, n_acceso2, acceso3, n_acceso3, poligono,municipio FROM {1}" +
                        " WHERE right(expediente, 4) = '{0}' {2}";

                    break;
                case "sc":

                    if (!string.IsNullOrEmpty(codigoExpediente))
                    {
                        filtroPorCodigoExpediente = string.Format(" and expediente='{0}'", codigoExpediente);
                    }

                    sql = "SELECT nom_proy, expediente as numeroExpediente, resolucion as estado , entidad ,n_entidad ,acceso1 , n_acceso1, acceso2 , n_acceso2 ,acceso3 , n_acceso3 ,Municipio FROM {1}" + " WHERE right(expediente, 4) = '{0}'  {2}";

                    break;
                case "iv":

                    if (!string.IsNullOrEmpty(codigoExpediente))
                    {
                        filtroPorCodigoExpediente = string.Format(" and expediente='{0}'", codigoExpediente);
                    }

                    sql = "SELECT nom_proy, expediente as numeroExpediente, entidad ,n_entidad ,acceso1 , n_acceso1, acceso2 , n_acceso2 ,acceso3 , n_acceso3  FROM {1}" + " WHERE right(expediente, 4) = '{0}'  {2}";

                    break;
                default:

                    if (!string.IsNullOrEmpty(codigoExpediente))
                    {
                        filtroPorCodigoExpediente = string.Format(" and t.expediente='{0}'", codigoExpediente);
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
                    #region Consulta original
                    sql = "SELECT " + licReson + " t.codigo_uso, t.expediente as numeroExpediente, t.resolucion as estado,t2.nombre as descripcionEstado, t.entidad, t.n_entidad, t.acceso1, t.n_acceso1, t.nom_proy,"
                         + " t.acceso2, t.n_acceso2, t.acceso3, t.n_acceso3, t.poligono, t.f_ing_rec, t.f_rno_rec, t.f_emi_memo, t.f_r1_sal,"
                         + " t.f_r1_ing, t.f_r2_ing,t.categoria, t.retiromemo, t.municipio, t.f_rno_sal1, t.f_rno_sal2,t.f_asig_tec,t.f_inspec,t.f_sal_rec,t.f_resoluc {3} FROM {1} t join resolucion t2 on t.resolucion=t2.resolucion"
                         + " WHERE right(t.expediente, 4) = '{0}' AND (t.resolucion != '' AND t.resolucion IS NOT NULL) {2} ORDER BY t.expediente";
                    #endregion

                    break;
            }

            filtroPorCodigoExpediente = filtroPorCodigoExpediente.Replace(@"/", "").Replace(@"?", "").Replace(@"%", "").Replace(@"+", "").Replace(@"or", "");


            string fromculplete = (tabla.Trim().ToLower() == "cl" || tabla.Trim().ToLower() == "pc") ? ", t.uso_esp" : ", 'N/A' uso_esp";

            #region basura
            //if (tabla.Trim().ToLower() == "cl" || tabla.Trim().ToLower() == "pc")
            //{
            //    fromculplete = ", t.uso_esp";
            //}
            //else
            //{
            //    fromculplete = ", 'N/A' uso_esp";
            //}
            #endregion

            sql = string.Format(sql, anio, tabla, filtroPorCodigoExpediente, fromculplete);
            var resultSqlQuery = this.GetQuery(sql);

            #region Codigo basura
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
            #endregion
            return resultSqlQuery;
        }
        #endregion

        public DataTable GetTramiteByCodigoDbFox(string anio, string tabla, string codigoExpediente = "")
        {
            anio = RemoverCaracteresEspeciales(anio);

            tabla = RemoverCaracteresEspeciales(tabla);
           
            string licReson = tabla == "pp" || tabla == "pps"? "t.urbano as licrespon,": "t.lic_respon as licrespon,";

            #region Consulta original
            var sql = "SELECT " + licReson + " t.codigo_uso, t.expediente as numeroExpediente, t.resolucion as estado,t2.nombre as descripcionEstado, t.entidad, t.n_entidad, t.acceso1, t.n_acceso1, t.nom_proy,"
                 + " t.acceso2, t.n_acceso2, t.acceso3, t.n_acceso3, t.poligono, t.f_ing_rec, t.f_rno_rec, t.f_emi_memo, t.f_r1_sal,"
                 + " t.f_r1_ing, t.f_r2_ing,t.categoria, t.retiromemo, t.municipio, t.f_rno_sal1, t.f_rno_sal2,t.f_asig_tec,t.f_inspec,t.f_sal_rec,t.f_resoluc {2} FROM {1} t join resolucion t2 on t.resolucion=t2.resolucion"
                 + " WHERE t.expediente='{0}'";
            #endregion


            var filtroPorCodigoExpediente  = codigoExpediente.Replace(@"/", "").Replace(@"?", "").Replace(@"%", "").Replace(@"+", "").Replace(@"or", "");

            string fromculplete = (tabla.Trim().ToLower() == "cl" || tabla.Trim().ToLower() == "pc") ? ", t.uso_esp" : ", 'N/A' uso_esp";
            sql = string.Format(sql, filtroPorCodigoExpediente, tabla, fromculplete);
            var resultSqlQuery = this.GetQuery(sql);
            return resultSqlQuery;
        }


    }

    public static class TableExtencion
    {
        public static IEnumerable<object> toIenumerable(this DataTable tabla)
        {
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