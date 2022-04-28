using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Mvc;
using System.Configuration;
using WebApiFox.Models;
using System.IO;

namespace WebApiFox.Controllers
{
    public class SendDataApi
    {
        public string anio { get; set; }
        public string codigo_tramite { get; set; }
        public string complementoWhere { get; set; }
    }
    public class ConsultaController : Controller
    {
        //Conexion al Client para la api
        public HttpClient Client()
        {

            var Url = new HttpClient();
            var urlbase = ConfigurationManager.AppSettings["baseapi"].ToString();
            Url.BaseAddress = new Uri(urlbase);
            return Url;
        }
        //Llamo la Lista de tramites de la Api
        public List<TipoTramite> tipoTramitesList()
        {
            var BaseAddress = Client();
            var Request = BaseAddress.GetAsync("Tramites/GetT");
            Request.Wait();
            if (Request.Result.IsSuccessStatusCode)
            {
                var Read = Request.Result.Content.ReadAsAsync<List<TipoTramite>>();
                Read.Wait();
                return Read.Result;
            }
            else
                return new List<TipoTramite>();
        }
        // GET: Consulta
        public ActionResult Index()
        {
            var Lista = tipoTramitesList().OrderBy(x => x.nombre).Where(x => x.codigo_tramite != "ca" && x.codigo_tramite != "de" && x.codigo_tramite != "le").ToList();
            ViewBag.codigo_tramite = new SelectList(Lista, "codigo_tramite", "nombre");
            return View(new List<Models.ConsultaModel>());
        }

        public HttpResponseMessage GetPDf(string id)
        {
            var datosArchivo = EncryptClass.Decriptar(id);
            var baseArchivo = ConfigurationManager.AppSettings["fileSource"];
            var arregloARchivo = datosArchivo.Split('_');
            var arregloEstado = arregloARchivo[1].Split('-');
            var ano = arregloEstado[0].Substring(arregloEstado[0].Length - 4, 4);
            var expediente = arregloEstado[0].Substring(0, arregloEstado[0].Length - 4);
            var estado = arregloEstado[1];
            expediente += estado == "D" ? "DEN" : "";
            string rutaCompleta = string.Format(@"{0}\{1}\{2}\{3}.pdf", baseArchivo, arregloARchivo[0], ano, expediente);

            if (System.IO.File.Exists(rutaCompleta))
            {
                var bytesArechivo = System.IO.File.ReadAllBytes(rutaCompleta);
                var contenTtypeArchivo = new System.Net.Mime.ContentDisposition();

                HttpResponseMessage result = new HttpResponseMessage(System.Net.HttpStatusCode.OK);
                var memory = new MemoryStream(bytesArechivo);
                result.Content = new StreamContent(memory);
                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf");
                return result;
            }
            else
                return null;
        }
    }
}
