using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApiFox.Models;

namespace WebApiFox.Controllers
{
    [EnableCors(origins: "http://tramites.opamss.org.sv:8083,http://tramites.opamss.org.sv:8084,http://localhost:54490,http://localhost:4200,https://tramites.opamss.org.sv", headers: "*", methods: "*")]
    public class TramitesController : ApiController
    {
        TramitesPostContext cnPost;
        List<TipoTramite> TiposTramite;
        // GET: api/Tramites
        [HttpGet]
        [Route("api/tramites/GetT")]
        public IHttpActionResult Get()
        {
            try
            {
                List<TipoTramite> newList = new List<TipoTramite>();
                cnPost = new TramitesPostContext();
                this.TiposTramite = cnPost.GetData("select id_tipo_tramite, codigo_tramite, nombre from public.tipo_tramite").ConvertDataTable<TipoTramite>();
                //foreach (var item in TiposTramite)
                //{
                //    if (item.codigo_tramite != "ca " && item.codigo_tramite != "de " && item.codigo_tramite != "le")
                //        newList.Add(item);
                //}
                return Ok(TiposTramite.OrderBy(p => p.nombre).ToList());
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpGet]
        public HttpResponseMessage Get(string id)
        {
            var datosArchivo = EncryptClass.Decriptar(id);
            var baseArchivo = ConfigurationManager.AppSettings["fileSource"];
            var arregloARchivo = datosArchivo.Split('_');
            var arregloEstado = arregloARchivo[1].Split('-');
            var ano = arregloEstado[0].Substring(arregloEstado[0].Length - 4, 4);
            var expediente = arregloEstado[0].Substring(0, arregloEstado[0].Length - 4);
            var estado = arregloEstado[1];

            expediente += estado == "D" ? "DEN" : estado == "M" ? "memo2" : "";
        a:
            int i = 1;
            string rutaCompleta = string.Format(@"{0}\{1}\{2}\{3}.pdf", baseArchivo, arregloARchivo[0], ano, expediente);

            if (System.IO.File.Exists(rutaCompleta))
            {
                var bytesArechivo = System.IO.File.ReadAllBytes(rutaCompleta);

                HttpResponseMessage result = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                var memory = new MemoryStream(bytesArechivo);
                result.Content = new StreamContent(memory);
                result.Content.Headers.ContentType =
                    new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                return result;
            }
            else
            {

                if (i == 2)
                    return null;
                else
                    expediente = arregloEstado[0].Substring(0, arregloEstado[0].Length - 4);
                expediente = expediente + "memo1";
                i++;
                goto a;
            }


        }

        [HttpGet]
        [Route("api/Tramites/GetA")]
        public IHttpActionResult GetAnnio()
        {
            List<string> anios = new List<string>();
            for (int i = 2020; i <= DateTime.Now.Year; i++)
            {
                anios.Add(i.ToString());
            }
            return Ok(anios);
        }
    }
}
