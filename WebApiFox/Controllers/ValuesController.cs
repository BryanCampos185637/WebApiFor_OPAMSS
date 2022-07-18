using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Data;
using WebApiFox.Models;
using System.Globalization;
using System.Web.Http.Cors;
using WebApiFox.Repositorio;
using System.Threading.Tasks;

namespace WebApiFox.Controllers
{

    [EnableCors(origins: "http://tramites.opamss.org.sv:8083,http://tramites.opamss.org.sv:8084,http://localhost:54490,http://localhost:4200,https://tramites.opamss.org.sv", headers: "*", methods: "*")]
    public class ValuesController : ApiController
    {

        ContextModel cn;
        TramitesPostContext cnPost;
        List<DiasFestivos> diasFestivosList;
        List<TipoTramite> TiposTramite;
        List<TiemposMaximos> tiempos;

        public ValuesController()
        {
            cn = new ContextModel();
            cnPost = new TramitesPostContext();
            this.diasFestivosList = new List<DiasFestivos>();

            CultureInfo _culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            CultureInfo _uiculture = (CultureInfo)CultureInfo.CurrentUICulture.Clone();

            _culture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;
            _uiculture.DateTimeFormat.FirstDayOfWeek = DayOfWeek.Monday;

            System.Threading.Thread.CurrentThread.CurrentCulture = _culture;
            System.Threading.Thread.CurrentThread.CurrentUICulture = _uiculture;

        }
        [HttpGet]
        [Route("api/Values/GetAllVentanillaVirtual")]
        public List<ConsultaModel> GetAllVentanillaVirtual(string anio, string tabla, string expediente)
        {
            var repositorioVerntanilla = new RepositorioExpedienteVentanillaVirtual();
            var lista = repositorioVerntanilla.ObtenerExpedientesDeVentanillaConsultaModel(tabla, Convert.ToInt32(anio), expediente);
            return lista;
        }
        [HttpGet]
        [Route("api/Values/Getall")]
        public async Task<List<ConsultaModel>> Getall(string anio, string tabla, string expediente)
        {
            try
            {
                tabla = ReemplazarCaracteres(tabla);
                List<ConsultaModel> lista = new List<ConsultaModel>();
                switch (tabla)
                {
                    case "mo":
                        lista.AddRange(this.PostMO(new requesDataApi() { anio = anio, codigo_tramite = tabla, complementoWhere = expediente }));
                        break;
                    case "sc":

                        lista.AddRange(this.PostSC(new requesDataApi() { anio = anio, codigo_tramite = tabla, complementoWhere = expediente }));
                        break;
                    case "iv":
                        lista.AddRange(this.PostIV(new requesDataApi() { anio = anio, codigo_tramite = tabla, complementoWhere = expediente }));
                        break;
                    default:
                        lista.AddRange(this.Post(new requesDataApi() { anio = anio, codigo_tramite = tabla, complementoWhere = expediente }));
                        break;
                }
                var listaExpedienteVentanilla = await new RepositorioExpedienteVentanillaVirtual().GetExpedientesByEndPoint(tabla, Convert.ToInt32(anio), expediente);
                if (listaExpedienteVentanilla != null && listaExpedienteVentanilla.Count > 0)
                    lista.AddRange(listaExpedienteVentanilla);
                return lista;
            }
            catch (Exception ex)
            {
                throw new Exception($"Ocurrio un problema con la peticion: ${ex.Message}");
            }
        }

        private string ReemplazarCaracteres(string tabla)
        {
            return tabla.Replace(@"'", "").Replace(@"/", "").Replace(@"?", "").Replace(@"%", "").Replace(@"+", "").Replace(@"=", "")
                .Replace(@"or", "").Replace(@"-", "").Replace(@".", "");
        }
        public List<ConsultaModel> PostMO(requesDataApi requesdata)
        {
            this.diasFestivosList = cnPost.GetData("Select * from public.dia_festivo order by fecha desc").ConvertDataTable<DiasFestivos>();
            this.TiposTramite = cnPost.GetData("select id_tipo_tramite, codigo_tramite, nombre from public.tipo_tramite").ConvertDataTable<TipoTramite>();
            this.tiempos = cn.GetQuery("select * from tiempomaximo").ConvertDataTable<TiemposMaximos>();
            var rtQuery = cn.GetTramites(requesdata.anio, requesdata.codigo_tramite, requesdata.complementoWhere);

            var tramites = rtQuery.ConvertDataTable<ConsultaFoxModel>();
            List<ConsultaModel> resultado = new List<ConsultaModel>();

            foreach (var registro in tramites)
            {
                var esteTramite = TiposTramite.Where(p => p.codigo_tramite == requesdata.codigo_tramite).FirstOrDefault();
                string subicacion = "{0} {1} {2} {3} {4} {5} {6} {7} {8} ";
                subicacion = string.Format(subicacion,
                    registro.entidad.Trim(),
                    registro.n_entidad.Trim(),
                    registro.acceso1.Trim(),
                    registro.n_acceso1.Trim(),
                    registro.acceso2.Trim(),
                    registro.n_acceso2.Trim(),
                    registro.acceso3.Trim(),
                    registro.n_acceso3.Trim(),
                    registro.poligono.Trim());

                var model = new ConsultaModel()
                {
                    numeroExpediente = registro.numeroexpediente.Trim(),
                    nombreTipoTramite = esteTramite?.nombre.Trim(),
                    tipoTramite = requesdata.codigo_tramite.Trim(),
                    ubicacion = subicacion,
                    municipio = registro.municipio,
                    uso_esp = registro.uso_esp,
                    codigo_uso = registro.codigo_uso
                };
                resultado.Add(model);
            }
            return resultado.OrderByDescending(p => Convert.ToUInt64(p.numeroExpediente)).ToList();
        }

        // GET api/values
        public List<ConsultaModel> Post(requesDataApi requesdata)
        {
            this.diasFestivosList = cnPost.GetData("Select * from public.dia_festivo order by fecha desc").ConvertDataTable<DiasFestivos>();
            this.TiposTramite = cnPost.GetData("select id_tipo_tramite, codigo_tramite, nombre from public.tipo_tramite").ConvertDataTable<TipoTramite>();
            this.tiempos = cn.GetQuery("select * from tiempomaximo").ConvertDataTable<TiemposMaximos>();


            var rtQuery = cn.GetTramites(requesdata.anio, requesdata.codigo_tramite, requesdata.complementoWhere);

            var tramites = rtQuery.ConvertDataTable<ConsultaFoxModel>();
            List<ConsultaModel> resultado = new List<ConsultaModel>();




            foreach (var registro in tramites)
            {
                registro.estado = registro.estado.Trim();
                var esteTramite = TiposTramite.Where(p => p.codigo_tramite == requesdata.codigo_tramite).FirstOrDefault();
                var tiempo = tiempos
                    .Where(p => p.tipo.ToUpper().Trim() == requesdata.codigo_tramite.ToUpper().Trim() && (p.categoria.ToUpper().Trim() == registro.categoria.ToLower() || p.categoria.ToUpper() == "N/A")).FirstOrDefault();

                int ingresos = registro.NumeroIngresos;
                int diasingreso1 = 0, diasingreso2 = 0, diasingreso3 = 0;
                int sumdias = 1;
                diasingreso1 = registro.DiasIngreso1(diasFestivosList);
                diasingreso2 = registro.DiasIngreso2(diasFestivosList);
                diasingreso3 = registro.DiasIngreso3(diasFestivosList);

                var tiempoRespuesta = 0;
                if (ingresos == 1) tiempoRespuesta = diasingreso1;
                if (ingresos == 2) tiempoRespuesta = diasingreso2;
                if (ingresos == 3) tiempoRespuesta = diasingreso3;

                tiempoRespuesta = tiempoRespuesta == 0 ? 0 : tiempoRespuesta + sumdias;

                string subicacion = "{0} {1} {2} {3} {4} {5} {6} {7} {8} ";
                subicacion = string.Format(subicacion,
                    registro.entidad.Trim(),
                    registro.n_entidad.Trim(),
                    registro.acceso1.Trim(),
                    registro.n_acceso1.Trim(),
                    registro.acceso2.Trim(),
                    registro.n_acceso2.Trim(),
                    registro.acceso3.Trim(),
                    registro.n_acceso3.Trim(),
                    registro.poligono.Trim());

                var model = new ConsultaModel()
                {
                    numeroExpediente = registro.numeroexpediente.Trim(),
                    estado = registro.estado.Trim(),
                    descripcionEstado = registro.descripcionestado.Trim(),
                    nomProy = registro.nom_proy.Trim(),
                    nombreTipoTramite = esteTramite?.nombre.Trim(),
                    tipoTramite = requesdata.codigo_tramite.Trim(),
                    municipio = registro.municipio,
                    categoria = registro.categoria.Trim(),
                    fechaIngreso = registro.FechaIngreso.Value.ToString("dd/MM/yyyy"),
                    tiempoMaximo = tiempo != null ? Convert.ToInt32(tiempo.t_max) : 0,
                    numeroIngreso = ingresos,
                    diasIngreso1 = diasingreso1 <= 0 ? 0 : diasingreso1 + sumdias,
                    diasIngreso2 = diasingreso2 <= 0 ? 0 : diasingreso2 + sumdias,
                    diasIngreso3 = diasingreso3 <= 0 ? 0 : diasingreso3 + sumdias,
                    fechaEmiMemo = registro.f_emi_memo,
                    fechaAsignacion = registro.f_asig_tec,
                    fechaInspeccion = registro.f_inspec,
                    fechaReingreso1 = registro.f_r1_ing,
                    fechaResoluc = registro.f_resoluc,
                    fechaReingreso2 = registro.f_r2_ing,
                    fechaRetiroMemo = registro.retiromemo,
                    fechaSalRec = registro.f_sal_rec,
                    fechaRetornoRecep = registro.f_rno_rec,
                    fechaRnoSal1 = registro.f_rno_sal1?.ToString("dd/MM/yyyy"),
                    fechaRnoSal2 = registro.f_rno_sal2,
                    ubicacion = subicacion,
                    tiempoRespuesta = tiempoRespuesta <= 0 ? 0 : tiempoRespuesta,
                    tiempoMaximoDemora = tiempo == null ? 0 : Convert.ToInt32(tiempo.d_max),
                    descripcionCategoria = string.IsNullOrEmpty(requesdata.complementoWhere) == true ? "" : tiempo.descrip,
                    uso_esp = registro.uso_esp,
                    codigo_uso = registro.codigo_uso,
                    licRespon = registro.licrespon



                };
                if (model.fechaAsignacion <= registro.FechaIngreso) model.fechaAsignacion = null;
                if (model.fechaRetornoRecep <= registro.FechaIngreso) model.fechaRetornoRecep = null;
                if (model.fechaSalRec <= registro.FechaIngreso) model.fechaSalRec = null;

                if (registro.f_rno_sal2 < registro.f_rno_sal1)
                {
                    registro.f_rno_sal2 = null;
                    model.fechaRnoSal2 = registro.f_rno_sal2;
                    model.FechaFoo = registro.f_rno_sal1.Value.AddMonths(6).ToString();

                }
                else
                {
                    model.FechaFoo = registro.f_rno_sal2.Value.AddMonths(6).ToString();
                }



                /*
                 calculo tiempo restante
                 */
                if (model.tiempoRespuesta < model.tiempoMaximo)
                {
                    if (model.estado == "F" || model.estado == "C" || model.estado == "M" || model.estado == "D")
                    {
                        model.tiempoRestante = 0;
                    }
                    else
                    {
                        model.tiempoRestante = model.tiempoMaximo - model.tiempoRespuesta; ;
                    }
                    model.tiempoDemora = 0;
                    model.retrasado = false;
                }
                else
                {
                    if (model.estado == "C" || model.estado == "F" || model.estado == "M")
                    {
                        model.tiempoRestante = 0;
                    }
                    else
                    {
                        model.tiempoRestante = model.tiempoMaximoDemora - model.tiempoRespuesta;
                    }
                    model.tiempoDemora = model.tiempoMaximoDemora - model.tiempoRespuesta;
                    model.retrasado = true;
                }
                if (model.estado == "M")
                {
                    DateTime? fechaRetiromemo = registro.retiromemo;
                    if (registro.retiromemo < registro.f_r1_sal)
                    {
                        fechaRetiromemo = registro.f_r1_sal;
                        model.fechaRetiroMemo = registro.f_r1_sal;
                    }

                    if (registro.f_rno_sal1 > registro.f_ing_rec && registro.f_r1_ing < registro.f_rno_sal1)
                    {
                        model.diasUsuario = DateTime.Now.Subtract(registro.f_rno_sal1.Value).Days;
                        model.diasUsuario = model.diasUsuario - DiasAsumar(registro.f_rno_sal1.Value, DateTime.Now);
                    }
                    if (registro.f_rno_sal2 > registro.f_ing_rec && registro.f_r2_ing < registro.f_rno_sal2)
                    {
                        //  model.diasUsuario = 1;
                    }
                    if (model.diasUsuario <= 0 && fechaRetiromemo > registro.f_ing_rec)
                    {
                        model.diasUsuario = DateTime.Now.Subtract(fechaRetiromemo.Value).Days;
                        model.diasUsuario = model.diasUsuario - DiasAsumar(fechaRetiromemo.Value, DateTime.Now);
                    }

                }

                resultado.Add(model);
            }
            return resultado.OrderByDescending(p => Convert.ToUInt64(p.numeroExpediente)).ToList();
        }

        // GET api/values
        public List<ConsultaModel> PostSC(requesDataApi requesdata)
        {
            this.diasFestivosList = cnPost.GetData("Select * from public.dia_festivo order by fecha desc").ConvertDataTable<DiasFestivos>();
            this.TiposTramite = cnPost.GetData("select id_tipo_tramite, codigo_tramite, nombre from public.tipo_tramite").ConvertDataTable<TipoTramite>();
            this.tiempos = cn.GetQuery("select * from tiempomaximo").ConvertDataTable<TiemposMaximos>();


            var rtQuery = cn.GetTramites(requesdata.anio, requesdata.codigo_tramite, requesdata.complementoWhere);

            var tramites = rtQuery.ConvertDataTable<ConsultaModelivsc>();
            List<ConsultaModel> resultado = new List<ConsultaModel>();

            foreach (var registro in tramites)
            {
                registro.estado = registro.estado.Trim();
                var esteTramite = TiposTramite.Where(p => p.codigo_tramite == requesdata.codigo_tramite).FirstOrDefault();


                string subicacion = "{0} {1} {2} {3} {4} {5} {6} {7}";
                subicacion = string.Format(subicacion,
                    registro.entidad.Trim(),
                    registro.n_entidad.Trim(),
                    registro.acceso1.Trim(),
                    registro.n_acceso1.Trim(),
                    registro.acceso2.Trim(),
                    registro.n_acceso2.Trim(),
                    registro.acceso3.Trim(),
                    registro.n_acceso3.Trim());

                var model = new ConsultaModel()
                {
                    nomProy = registro.nom_proy.Trim(),
                    numeroExpediente = registro.numeroexpediente.Trim(),
                    estado = registro.estado.Trim(),
                    nombreTipoTramite = esteTramite?.nombre.Trim(),
                    tipoTramite = requesdata.codigo_tramite.Trim(),
                    municipio = registro.municipio,
                    ubicacion = subicacion,
                };
                /*
                 calculo tiempo restante
                 */
                if (model.tiempoRespuesta < model.tiempoMaximo)
                {
                    if (model.estado == "F" || model.estado == "C" || model.estado == "M" || model.estado == "D")
                    {
                        model.tiempoRestante = 0;
                    }
                    else
                    {
                        model.tiempoRestante = model.tiempoMaximo - model.tiempoRespuesta; ;
                    }
                    model.tiempoDemora = 0;
                    model.retrasado = false;
                }
                else
                {
                    if (model.estado == "C" || model.estado == "F" || model.estado == "M")
                    {
                        model.tiempoRestante = 0;
                    }
                    else
                    {
                        model.tiempoRestante = model.tiempoMaximoDemora - model.tiempoRespuesta;
                    }
                    model.tiempoDemora = model.tiempoMaximoDemora - model.tiempoRespuesta;
                    model.retrasado = true;
                }
                resultado.Add(model);
            }
            return resultado.OrderByDescending(p => Convert.ToUInt64(p.numeroExpediente)).ToList();
        }
        public List<ConsultaModel> PostIV(requesDataApi requesdata)
        {
            this.diasFestivosList = cnPost.GetData("Select * from public.dia_festivo order by fecha desc").ConvertDataTable<DiasFestivos>();
            this.TiposTramite = cnPost.GetData("select id_tipo_tramite, codigo_tramite, nombre from public.tipo_tramite").ConvertDataTable<TipoTramite>();
            this.tiempos = cn.GetQuery("select * from tiempomaximo").ConvertDataTable<TiemposMaximos>();


            var rtQuery = cn.GetTramites(requesdata.anio, requesdata.codigo_tramite, requesdata.complementoWhere);

            var tramites = rtQuery.ConvertDataTable<ConsultaModelivsc>();
            List<ConsultaModel> resultado = new List<ConsultaModel>();

            foreach (var registro in tramites)
            {

                var esteTramite = TiposTramite.Where(p => p.codigo_tramite == requesdata.codigo_tramite).FirstOrDefault();


                string subicacion = "{0} {1} {2} {3} {4} {5} {6} {7}";
                subicacion = string.Format(subicacion,
                    registro.entidad.Trim(),
                    registro.n_entidad.Trim(),
                    registro.acceso1.Trim(),
                    registro.n_acceso1.Trim(),
                    registro.acceso2.Trim(),
                    registro.n_acceso2.Trim(),
                    registro.acceso3.Trim(),
                    registro.n_acceso3.Trim());

                var model = new ConsultaModel()
                {
                    nomProy = registro.nom_proy.Trim(),
                    numeroExpediente = registro.numeroexpediente.Trim(),
                    nombreTipoTramite = esteTramite?.nombre.Trim(),
                    tipoTramite = requesdata.codigo_tramite.Trim(),
                    ubicacion = subicacion,
                };
                /*
                 calculo tiempo restante
                 */
                if (model.tiempoRespuesta < model.tiempoMaximo)
                {
                    if (model.estado == "F" || model.estado == "C" || model.estado == "M" || model.estado == "D")
                    {
                        model.tiempoRestante = 0;
                    }
                    else
                    {
                        model.tiempoRestante = model.tiempoMaximo - model.tiempoRespuesta; ;
                    }
                    model.tiempoDemora = 0;
                    model.retrasado = false;
                }
                else
                {
                    if (model.estado == "C" || model.estado == "F" || model.estado == "M")
                    {
                        model.tiempoRestante = 0;
                    }
                    else
                    {
                        model.tiempoRestante = model.tiempoMaximoDemora - model.tiempoRespuesta;
                    }
                    model.tiempoDemora = model.tiempoMaximoDemora - model.tiempoRespuesta;
                    model.retrasado = true;
                }
                resultado.Add(model);
            }
            return resultado.OrderByDescending(p => Convert.ToUInt64(p.numeroExpediente)).ToList();
        }

        public int DiasAsumar(DateTime fechaI, DateTime fechaF)
        {
            int dias = 0;

            for (DateTime fecha = fechaI; fecha <= fechaF; fecha = fecha.AddDays(1))
            {
                var arreglodias = this.diasFestivosList.Where(p => p.fecha == fecha).FirstOrDefault();
                if (arreglodias != null)
                {
                    dias++;
                }
                else if (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday)
                {
                    dias++;
                }
            }

            return dias;
        }
        [Route("api/Values/GetUnTramite")]
        public object GetUnTramite(string tabla, string expediente = "")
        {
            expediente = expediente.Trim();
            var anio = expediente.Substring(expediente.Length - 4, 4);
            var dataRecibe = new requesDataApi();
            dataRecibe.complementoWhere = expediente;
            dataRecibe.anio = anio;
            dataRecibe.codigo_tramite = tabla;
            return this.Post(dataRecibe).FirstOrDefault();
        }
        [Route("api/Values/GetTablaFox")]
        public IEnumerable<object> GetTablaFox(string tabla)
        {
            return cn.GetQuery("Select * from  " + tabla).toIenumerable();
        }
        [Route("api/Values/GetDiasFestivos")]
        public IEnumerable<object> GetDiasFestivos()
        {

            return cnPost.GetData("Select * from public.dia_festivo order by fecha desc ").ConvertDataTable<DiasFestivos>();
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
    public class requesDataApi
    {
        public string anio { get; set; }
        public string codigo_tramite { get; set; }
        public string complementoWhere { get; set; }
    }
}
