using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;
using WebApiFox.Models;
namespace WebApiFox.Controllers
{
    [EnableCors(origins: "http://tramites.opamss.org.sv:8083,http://tramites.opamss.org.sv:8084,http://localhost:54490,http://localhost:4200,https://tramites.opamss.org.sv", headers: "*", methods: "*")]
    public class UbicacionController : ApiController
    {
        ContextUbicaciones cnContext;
        List<LimiteMunicipio> Limites;
        List<LimiteDistrito> distritos;
        List<ParcelasMunicipio> Parcelas;
        //GET
        [HttpGet]
        [Route("api/parcelario/get")]
        public IHttpActionResult ParcelaGet(string IdMunicipio, string distridoCod = "")
        {
            string Tble = "";
            cnContext = new ContextUbicaciones();
            //this.Limites = cnContext.GetMunicipioParcelario("SELECT id, geom, nom_mun, id_mun, cod_mun, area, perimetro, id_dpto FROM public." + '"' + "0103_LIMITES_MUNICIPALES_AMSS" +'"'+ " Where id_mun =" + IdMunicipio).ConvertDataTable<LimiteMunicipio>();

            switch (IdMunicipio)
            {
                case "01":

                    Tble = "ANTIGUO";
                    break;
                case "02":
                    Tble = "APOPA";
                    break;
                case "03":
                    Tble = "AYUTUXTEPEQUE";
                    break;
                case "04":
                    Tble = "CUSCATANCINGO";
                    break;
                case "07":
                    Tble = "ILOPANGO";
                    break;
                case "08":
                    Tble = "MEJICANOS";
                    break;
                case "09":
                    Tble = "NEJAPA";
                    break;
                case "11":
                    Tble = "TECLA";
                    break;
                case "12":
                    Tble = "SAN_MARCOS";
                    break;
                case "13":
                    Tble = "SAN_MARTIN";
                    break;
                case "14":
                    Tble = "SAN_SALVADOR";
                    break;
                case "17":
                    Tble = "SOYAPANGO";
                    break;
                case "18":
                    Tble = "TONACATEPEQUE";
                    break;
                case "19":
                    Tble = "DELGADO";
                    break;
                default:
                    break;
            }
            if (IdMunicipio == "14")
            {
                this.Parcelas = cnContext.GetMunicipioParcelario("SELECT id, ST_AsGeoJSON(ST_Transform(geom,4326))::json As geom, cod_mun, cod_parcel FROM public." + '"' + "0108_" + Tble + '"' + "Where distrito = " + "'" + distridoCod + "'").ConvertDataTable<ParcelasMunicipio>();

            }
            else
            {
                this.Parcelas = cnContext.GetMunicipioParcelario("SELECT id, ST_AsGeoJSON(ST_Transform(geom,4326))::json As geom, cod_mun, cod_parcel FROM public." + '"' + "0108_" + Tble + '"').ConvertDataTable<ParcelasMunicipio>();

            }

            //LimiteMunicipio municipios = Limites.FirstOrDefault();

            IEnumerable<Feature> FeatureJson = new List<Feature>();

            FeatureJson = (from item in this.Parcelas
                           select new Feature()
                           {
                               properties = new ParcelasMunicipio() { id = item.id, cod_mun = item.cod_mun, cod_parcel = item.cod_parcel },
                               geometry = JsonConvert.DeserializeObject<Geometry>(item.geom)
                           }).ToList();

            JsonClass ResponseJson = new JsonClass();
            ResponseJson.features = FeatureJson;
            return Ok(ResponseJson);
        }
        //GET
        [HttpGet]
        [Route("api/municipios/get")]
        public IHttpActionResult MunicipioGet(string IdMunicipio)
        {
            cnContext = new ContextUbicaciones();
            string tablaBase = "0103_LIMITES_MUNICIPALES_AMSS";
            this.Limites = cnContext.GetMunicipioParcelario("SELECT id, ST_AsGeoJSON(ST_Transform(geom,4326))::json As geom, nom_mun, id_mun, cod_mun FROM public." + '"' + tablaBase + '"' + " Where id_mun =" + "'" + IdMunicipio + "'").ConvertDataTable<LimiteMunicipio>();


            LimiteMunicipio municipios = Limites.FirstOrDefault();


            IEnumerable<Feature> FeatureJson = new List<Feature>();

            FeatureJson = (from item in this.Limites
                           select new Feature()
                           {
                               properties = new LimiteMunicipio() { id = item.id, id_mun = item.id_mun, nom_mun = item.nom_mun },
                               geometry = JsonConvert.DeserializeObject<Geometry>(item.geom)
                           }).ToList();

            JsonClass ResponseJson = new JsonClass();
            ResponseJson.features = FeatureJson;
            return Ok(ResponseJson);
        }
        //GET
        [HttpGet]
        [Route("api/distritos/get")]
        public IHttpActionResult Distritos(string distridoCod = "")
        {
            cnContext = new ContextUbicaciones();
            string tablaBase = '"' + "0113_DISTRITOS_SS" + '"';
            string where = "";
            if (distridoCod != "")
            {
                where = string.Format("WHERE distrito <> '{0}'", distridoCod);
            }
            string script = "SELECT id, ST_AsGeoJSON(ST_Transform(geom,4326))::json As geom, distrito FROM public.{0} {1}";
            string sql = string.Format(script, tablaBase, where);
            this.distritos = cnContext.GetMunicipioParcelario(sql).ConvertDataTable<LimiteDistrito>();


            IEnumerable<Feature> FeatureJson = new List<Feature>();

            FeatureJson = (from item in this.distritos
                           select new Feature()
                           {
                               properties = new LimiteDistrito() { id = item.id, distrito = item.distrito },
                               geometry = JsonConvert.DeserializeObject<Geometry>(item.geom)
                           }).ToList();

            JsonClass ResponseJson = new JsonClass();
            ResponseJson.features = FeatureJson;
            return Ok(ResponseJson);
        }
        //GET
        [HttpGet]
        [Route("api/ubicacion/getexp")]
        public IHttpActionResult UbicacionExpediente(string id)
        {
            return Ok();
        }
        //GET
        [HttpGet]
        [Route("api/parcelario/parcelas")]
        public IHttpActionResult DistritoPorParcela(string parcela)
        {
            cnContext = new ContextUbicaciones();
            string where = string.Format("WHERE cod_parcel = '{0}'", parcela);
            string tablaBase = '"' + "0108_SAN_SALVADOR" + '"';
            string script = "SELECT distrito FROM public.{0} {1}";
            string sql = string.Format(script, tablaBase, where);
            List<dis> distrito = cnContext.GetMunicipioParcelario(sql).ConvertDataTable<dis>();
            string distritoString = distrito[0].distrito;
            return Ok(distrito);
        }
        public class dis
        {
            public string distrito { get; set; }
        }
    }
}