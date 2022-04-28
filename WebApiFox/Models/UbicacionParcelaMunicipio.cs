using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiFox.Models
{
    public class UbicacionParcela
    {
        public int id { get; set; }
        public string cod_parcel { get; set; }
        public string cod_mun { get; set; }
        public string geomParcelas { get; set; }
 
    }
    public class UbicacionMunicipio
    {
        public string nom_mun { get; set; }
        public string geomMunicipio { get; set; }

    }
}