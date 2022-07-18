using Dapper;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using WebApiFox.Models;

namespace WebApiFox.Repositorio
{
    public class RepositorioExpedienteVentanillaVirtual
    {
        private readonly string connectionString = string.Empty;
        private readonly string urlBase = string.Empty;

        public RepositorioExpedienteVentanillaVirtual()
        {
            connectionString =  @"data source=172.16.0.47;user id=SysDev; password=D3sarr0ll0; initial catalog=Tramites;integrated security=false;";
            urlBase = "https://tramites.opamss.org.sv:8089";
        }
        //ConsultaModel
        public List<ConsultaModel> ObtenerExpedientesDeVentanillaConsultaModel(string tipoTramite = "", int @annio = 0, string codigo = "")
        {
            using (var cn = new SqlConnection(connectionString))
            {
                var lista = cn.Query<ConsultaModel>($"EXEC upsGetAllExpedientesVentanillaVirtuaL '{tipoTramite}',{annio},'{codigo}'");
                return lista.ToList();
            }
        }

        public async Task<List<ConsultaModel>> GetExpedientesByEndPoint(string tipoTramite = "", int annio = 0, string codigo = "")
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                string complementoUrl = string.IsNullOrEmpty(codigo) ? $"api/ConsultaWebApi/GetConsultasVentanillaVirtual?tipoTramite={tipoTramite}&annio={annio}" :
                    $"api/ConsultaWebApi/GetConsultasVentanillaVirtual?tipoTramite={tipoTramite}&annio={annio}&codigo={codigo}";
                var response = await httpClient.GetAsync($"{urlBase}/{complementoUrl}");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var listaExpedientes = JsonConvert.DeserializeObject<List<ConsultaModel>>(await response.Content.ReadAsStringAsync());
                    return listaExpedientes;
                }
                return new List<ConsultaModel>();
            }
            catch (System.Exception)
            {
                return new List<ConsultaModel>();
            }
        }


    }
}