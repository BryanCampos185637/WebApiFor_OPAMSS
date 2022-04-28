using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApiFox.Models
{
    public class ConsultaModelivsc
    {
      
        public string numeroexpediente { get; set; }
        public string estado { get; set; }
        public string descripcionestado { get; set; }
        public string entidad { get; set; }
        public string n_entidad { get; set; }
        public string acceso1 { get; set; }
        public string n_acceso1 { get; set; }
        public string nom_proy { get; set; }
        public string acceso2 { get; set; }
        public string n_acceso2 { get; set; }
        public string acceso3 { get; set; }
        public string n_acceso3 { get; set; }
        public string municipio { get; set; }
    }
}