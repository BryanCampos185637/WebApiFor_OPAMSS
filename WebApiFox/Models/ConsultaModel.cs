using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApiFox.Models
{
    public class ConsultaModel
    {
        public ConsultaModel()
        {
        }
        [Display(Name = "Expediente")]
        public string numeroExpediente { get; set; }
        [Display(Name = "Estado del trámite")]
        public string estado { get; set; }
        public string descripcionEstado { get; set; }
        public string ubicacion { get; set; }
        [Display(Name = "Tiempo Respuesta")]
        public int tiempoRespuesta { get; set; }
        public string fechaIngreso { get; set; }
        public DateTime? fechaFirma { get; set; }
        public DateTime? fechaAsignacion { get; set; }
        public DateTime? fechaInspeccion { get; set; }
        public DateTime? fechaResoluc { get; set; }
        public DateTime? fechaSalRec { get; set; }
        public DateTime? fechaRetornoRecep { get; set; }
        public DateTime? fechaEmiMemo { get; set; }
        public DateTime? fechaReingreso1 { get; set; }
        public DateTime? fechaReingreso2 { get; set; }
        public DateTime? fechaRetiroMemo { get; set; }
        public string licRespon { get; set; }
        public string tipoTramite { get; set; }
        public string nombreTipoTramite { get; set; }
        [Display(Name = "Proyecto")]
        public string nomProy { get; set; }
        [Display(Name = "Municipio")]
        public string municipio { get; set; }
        public string categoria { get; set; }
        public string fechaRnoSal1 { get; set; } = "";
        public string fechaRnoSal { get; set; } = "";
        public DateTime? fechaRnoSal2 { get; set; }
       
        public int tiempoRestante { get; set; }
        public bool retrasado { get; set; }
        public int tiempoDemora { get; set; }
        public int tiempoMaximo { get; set; }
        public int tiempoMinimo { get; set; }
        public int tiempoMaximoDemora { get; set; }
        public int diasIngreso1 { get; set; }
        public int diasIngreso2 { get; set; }
        public int diasIngreso3 { get; set; }
        public int diasUsuario { get; set; }
        public int numeroIngreso { get; set; }
        public string descripcionCategoria { get; set; }
        public string uso_esp { get; set; }

        public string codigo_uso { get; set; }

        public string EncryptFile { get {
                return HttpUtility.UrlEncode(EncryptClass.EncripTar(string.Format("{0}_{1}-{2}", this.tipoTramite, numeroExpediente,estado)));    
            } 
        }
        public string FechaFoo { get; set; }
        public string OrigenData { get; set; } = "DbFox";
    }
}