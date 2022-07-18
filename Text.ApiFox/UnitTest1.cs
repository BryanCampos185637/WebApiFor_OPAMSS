using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WebApiFox.Repositorio;

namespace Text.ApiFox
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ObtenerTramiteEspecifico()
        {
            RepositorioExpedienteVentanillaVirtual dal = new RepositorioExpedienteVentanillaVirtual();

            const string codigo = "0001", tipoTramite = "pc";
            const int annio = 2022;
            var lista = dal.ObtenerExpedientesDeVentanillaConsultaModel(tipoTramite,annio,codigo);
            var resultado = lista[0];
           Assert.IsNotNull(resultado);
        }

        [TestMethod]
        public void ObtenerTodosLosTramite()
        {
            RepositorioExpedienteVentanillaVirtual dal = new RepositorioExpedienteVentanillaVirtual();

            const string codigo = "", tipoTramite = "pc";
            const int annio = 2022;
            var resultado = dal.GetExpedientesByEndPoint(tipoTramite, annio, codigo).Result;

            Assert.IsTrue(resultado.Count > 0);
        }
    }
}
