using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApiFox.Models;

namespace WebApiFox.Controllers
{
    public class ConsultaGraficaController : Controller
    {

        //Llamo la Lista de tramites de la Api
     

        // GET: ConsultaGrafica
        public ActionResult Index()
        {
         
            return View(new List<Models.ConsultaModel>());
        }
    }
}