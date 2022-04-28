using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using WebApiFox.Models;

namespace WebApiFox.Controllers
{
    public class AcreditadosFoxAPIController : ApiController
    {
        AcreditadosContext cnAcre;
        List<AcreditadosFox> lstAcreditaciones;
        public AcreditadosFoxAPIController()
        {
            cnAcre = new AcreditadosContext();
            lstAcreditaciones = new List<AcreditadosFox>();
        }
        [HttpGet]
        [Route("api/Acreditados/verifi")]
        public Object PGet(string  NoAcreditacion = "")
        {
            this.lstAcreditaciones = this.cnAcre.GetAcreditados(NoAcreditacion).ConvertDataTable<AcreditadosFox>();
            if (NoAcreditacion != "")
                return this.lstAcreditaciones.FirstOrDefault() != null ? this.lstAcreditaciones.FirstOrDefault() : new AcreditadosFox();
            else
                return lstAcreditaciones;
        }
       

    }
}
