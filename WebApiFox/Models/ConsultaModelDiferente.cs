using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApiFox.Models
{
    public class ConsultaModelDiferente
    {
        [Display(Name = "Expediente")]
        public string numeroExpediente { get; set; }       
        public string ubicacion { get; set; }
        public string licRespon { get; set; }
        public string tipoTramite { get; set; }
        public string nombreTipoTramite { get; set; }
    
        [Display(Name = "Municipio")]
        public string municipio { get; set; }
        public int tiempoRestante { get; set; }
        public int numeroIngreso { get; set; }
        public string uso_esp { get; set; }

        public string codigo_uso { get; set; }

    }
}