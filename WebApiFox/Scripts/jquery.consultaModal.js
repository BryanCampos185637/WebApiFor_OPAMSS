
$().ready(function ($) {

    $(function () {
        $('[data-toggle="tooltip"]').tooltip();
    });

});


function VerRegistro(NumeroExpediente) {
    Swal.fire({
        title: "<div></div>",
        html: HtmlCuerpoModal(NumeroExpediente),
        showCloseButton: true,
        showConfirmButton: false,
        allowOutsideClick: false,
        focusConfirm: false,
        width: '70%'
    });

    GetDeailData(NumeroExpediente);
}

function HtmlCuerpoModal() {

    return HtmlCardFlip() + HtmlDetalle();
}

function HtmlCardFlip() {

    var cadrAnalisis = `
<div style="z-index:1"> 
<div class="col-md-4">
       <div class="flip-card">
         <div class="flip-card-inner">
           <div class="flip-card-front" id="frontAsignacion">
             <font color="white">Asignación</font>
             <hr />
             <i class="fa fa-folder-open"  style="font-size:70px;color:#fff" aria-hidden="true"></i>
           </div>
           <div class="flip-card-back" id="backAsignacion">
           <font color="white" id="textAsignacion"  size="3" ></font>
           </div>
         </div>
       </div>
   </div>

<div class="col-md-4">
    <div class="flip-card">
         <div class="flip-card-inner">
           <div class="flip-card-front" id="frontAnalisis">
             <font color="white">Análisis</font>
             <hr />
             <i class="fa fa-search" style="font-size:70px;color:#fff" aria-hidden="true"></i>
           </div>
           <div class="flip-card-back" id="backAnalisis">
            <font color="white" id="textAnalisis"  size="3"></font>
           </div>
         </div>
       </div>
     </div>

<div class="col-md-4">
       <div class="flip-card">
         <div class="flip-card-inner">
           <div class="flip-card-front" id="frontRespuesta">
             <font color="white">Respuesta en Ventanilla</font>
             <hr />
             <i class="fa fa-file-pdf-o"  style="font-size:70px;color:#fff" aria-hidden="true"></i>
           </div>
           <div class="flip-card-back" id="backRespuesta">
             <font color="white" id="textRespuesta" size="3" ></font>
           </div>
         </div>
         </div>
  </div>

  </div>

`;
    return cadrAnalisis;

}

function HtmlDetalle() {
  
    var html = `
<div class="container" style="width:100%;z-index:-1;">

    <dl style="float:left;width:40%;margin-top:5%">
                        <dt>
                            <label>Numero de Expediente:</label>
                        </dt>
                        <dd>
                            <span id="numExp"></span>
                        </dd>

                        <dt>
                            <label>Nombre del Proyecto:</label>
                        </dt>
                        <dd>
                            <span id="nomProy"></span>

                        </dd>

                        <dt>
                            <label>Fecha de Ingreso:</label>
                        </dt>
                        <dd>
                            <span id="dateIngre"></span>

                        </dd>

                        <dt>
                            Tipo de resolución:
                        </dt>
                        <dd>
                            <span id="estado"></span>

                        </dd>

                        <dt id="catdt">

                        </dt>
                        <dd>
                            <span id="cat"></span>

                        </dd>

                        <dt>
                            <label>Numero de Ingreso:</label>
                        </dt>
                        <dd>
                            <span id="numIngr"></span>

                        </dd>

                        <dt>
                            <label>Dias Transcurridos:</label>
                        </dt>
                        <dd>
                            <span id="DiaTran"></span>

                        </dd>
    </dl>

    <div style="width:60%;float:rigth;margin-top:5%" class="col-md-4">
        <div>
            <canvas  id="gaugeTranscurrido" style="float:left"></canvas >
        </div>
        <div>
            <canvas  id="gaugeRestante" style="float:rigth" ></canvas >
        </div>
    </div>
</div>
<div style="float:rigth" id="btnDownloadPDF"></div>
`;
    return html;
}

function GetDeailData(NoExp) {
  
    //Modelo para request de data
    var datos = {
        anio: $("#lsAno").val(),
        tabla: $("#codigo_tramite").val(),
        expediente: NoExp
    };
    fns.CallGetAsync("api/values/Getall", datos, function (request) {

        //*********************TABLA DE DETALLE***********************//
        //Se obtiene el indice 1 del request ya que se envia en forma de lista aunque siempre trae 1
        var detailReques = request[0];

        validacion = detailReques.uso_esp.substring(0, 3);
        validacion2 = detailReques.codigo_uso.substring(0, 3);

        //LLENADO DE INFORMACION DEL DEATLLE
        $("#numExp").text(datos.tabla.toUpperCase()+"-"+detailReques.numeroExpediente);
        $("#nomProy").text(detailReques.nomProy);
        $("#dateIngre").text(detailReques.fechaIngreso);
        $("#estado").text(detailReques.descripcionEstado);
        $("#numIngr").text(detailReques.numeroIngreso);
        $('#DiaTran').text(detailReques.tiempoRespuesta);

        //if que mestra un icono y categoria si la tabla es permiso Construccion o Calificacion de lugar
        if (datos.tabla == "pc" || datos.tabla == "cl") {
            $('#catdt').html(' <i data-toggle="tooltip" title="" id="catInfo" class="fa fa-info-circle" style="color:#0095E8;font-size:20px" aria-hidden="true"></i> <label>Categoria:</label> ');
            $('#catInfo').attr("title", detailReques.descripcionCategoria);
            $('#cat').text(detailReques.categoria);
        }

        if (validacion != "HAB" & validacion2 != "HAB") {

            ButtonPDF.Add(detailReques.EncryptFile);
        }
        else {
            ButtonPDF.Remove();
        }
        //*********************TABLA DE DETALLE***********************//

        //*********************CARDS DE ESTADO***********************//

        //Ids de los 3 cards 
        let idAsignacion = "Asignacion";
        let idAnalisis = "Analisis";
        let idRespuesta = "Respuesta";

        //instancia de mensajes que tendran dentro los cards
        var mensajeAsig = 'Expediente en proceso de ser asignado al personal del departamento correpondiente';
        var mensajeresp = 'No iniciado';
        var mensajeAnali = 'No iniciado';


        if (detailReques.fechaAsignacion != null) {
            AddClasesCard(idAsignacion, "Verde");
          
            mensajeAsig = 'El expediente ya ha sido asignado al departamento correspondiente';

            if (detailReques.fechaRetornoRecep != null) {
                AddClasesCard(idAnalisis,"Verde");
                AddClasesCard(idRespuesta,"Verde");
               
                if (detailReques.fechaSalRec != null) {

                    mensajeresp = "La resolución ha sido retirada";
                } else {

                    mensajeresp = "La resolución se encuentra en ventanilla en espera de su retiro";

                }
                mensajeAnali = 'Los procesos de análisis ya han sido completados';
            } else {
                AddClasesCard(idAnalisis, "Azul");

                AddClasesCard(idRespuesta, "Gris");              

                mensajeAnali = 'El expediente se encuentra en los procesos de análisis correspondientes';
            }
        } else {

            AddClasesCard(idAsignacion, "Gris");
            AddClasesCard(idAnalisis, "Gris");   
            AddClasesCard(idRespuesta, "Gris");   
         
        }

        //SI EL ESTADO ES FAVORABLE O DENEGADO
        if (detailReques.estado == "F" || detailReques.estado == "D") {
   
        
            AddClasesCard(idAsignacion, "Verde");   

            AddClasesCard(idAnalisis, "Verde");   

            AddClasesCard(idRespuesta, "Verde");         
        
            mensajeresp = "La resolución ha sido retirada";
            mensajeAnali = 'Los procesos de análisis ya han sido completados';
            mensajeAsig = 'El expediente ya ha sido asignado al departamento correspondiente';


        }

        //SI EL ESTADO ES MEMO
        if (detailReques.estado == "M") {
            //$('#restante').attr('data-value', 0);

            AddClasesCard(idAsignacion, "Verde");   

            AddClasesCard(idAnalisis, "Verde");   

            AddClasesCard(idRespuesta, "Azul");  
            
            mensajeresp = "La resolución ha sido retirada";
            mensajeAnali = 'Los procesos de análisis ya han sido completados';

            if (detailReques.fechaSalRec != null) {

                mensajeresp = "La resolución ha sido retirada";
            } else {

                mensajeresp = "La resolución se encuentra en ventanilla en espera de su retiro";

            }

            //Saber la fecha de HOY
            var d = new Date();
            var month = d.getMonth() + 1;
            var day = d.getDate();
            var year = d.getFullYear();
            var FechaHoy = d.getFullYear() + '/' +
                (('' + month).length < 2 ? '0' : '') + month + '/' +
                (('' + day).length < 2 ? '0' : '') + day;


            //Separar la FechaFOO para saber el año
            let str = detailReques.FechaFoo.split("/");
            let st = str[2].split(" ");
            //Saber si tiene fecha de MEMO 2
            if (detailReques.fechaRnoSal2 == null) {

              
                if ((detailReques.FechaFoo == FechaHoy || st[0] < year ) & validacion != "HAB" & validacion2 != "HAB") {


                    ButtonPDF.Add(detailReques.EncryptFile);

                    AddClasesCard(idRespuesta, "Verde");  

                } else {
                    AddClasesCard(idRespuesta, "Verde");
                    ButtonPDF.Remove();

                }
            } else {
                
                if ((detailReques.FechaFoo == FechaHoy || st[0] < year) & validacion != "HAB" & validacion2 != "HAB") {

                    ButtonPDF.Add(detailReques.EncryptFile);
                    AddClasesCard(idRespuesta, "Verde");  
                }
                else {
                    ButtonPDF.Remove();
                    AddClasesCard(idRespuesta, "Verde");  
                }
               
            }

        }
        //Se insertan los mensajes en los cards
        $("#textAsignacion").text(mensajeAsig);
        $("#textAnalisis").text(mensajeAnali);
        $("#textRespuesta").text(mensajeresp);

        //*********************CARDS DE ESTADO***********************//


        //*********************RELOJES GAUGES***********************//
        //Hecho con JavaScript
        GenerarGaugeCanvas("gaugeTranscurrido", detailReques.tiempoMinimo, detailReques.tiempoMaximoDemora, detailReques.tiempoMaximo, detailReques.tiempoRespuesta, "Transcurridos");      
        var setValueGauge = 0;
        var valor = (detailReques.tiempoMaximoDemora - detailReques.tiempoRespuesta);
        if (detailReques.estado != "T" && detailReques.estado != "M") {
            setValueGauge = 0;

        } else {
            setValueGauge = valor;
        }
        GenerarGaugeCanvas("gaugeRestante", detailReques.tiempoMinimo, detailReques.tiempoMaximoDemora, detailReques.tiempoMaximo,  setValueGauge, "Restantes");      

        //*********************RELOJES GAUGES***********************//
     


    });

}


function AddClasesCard(id, claseNueva) {
    switch (claseNueva) {
        case "Gris"://Gris    
            
            //remover clases
            $("#front" + id).removeClass("Azul");
            $("#back"+id).removeClass("Azul");

            $("#front"+id).removeClass("Verde");
            $("#back"+id).removeClass("Verde");

            //Poner clase
            $("#front" + id).addClass("Gris");
            $("#back" + id).addClass("Gris");
            break;
        case "Azul"://Azul

            //remover clases
            $("#front" + id).removeClass("Gris");
            $("#back"+id).removeClass("Gris");

            $("#front" + id).removeClass("Verde");
            $("#back"+id).removeClass("Verde");

            //Poner clase
            $("#front" + id).addClass("Azul");
            $("#back"+id).addClass("Azul");
            break;
        case "Verde"://Verde
       
            //remover clases
            $("#front" + id).removeClass("Gris");
            $("#back"+id).removeClass("Gris");

            $("#front" + id).removeClass("Azul");
            $("#back"+id).removeClass("Azul");

            //Poner clase
            $("#front" + id).addClass("Verde");
            $("#back"+id).addClass("Verde");
            break;
        default:
            break;
    }
}

var ButtonPDF = {
    Add: function (claveDescarga) {
        var path = "/";
        //var path = "/consulta/";
        let HtmlbtnPDF = `<a href="` + "//" + getAbsolutePath() + path +"
Get?id=" + claveDescarga + `" class="btn btn-primary" style="width:10%" data-toggle="tooltip"  title="Obtener su PDF"> <i class="fa fa-cloud-download" style="color:#fff" aria-hidden="true"></i></a>`;
        $("#btnDownloadPDF").html(HtmlbtnPDF);
    },
    Remove: function () {
        $("#btnDownloadPDF").html("");

    }

};
function getAbsolutePath() {
    var loc = window.location.host;       
    return loc;
}

function GenerarGaugeCanvas(idcanva,minVal, maxVal, halfVal, setVal,MessageText) {

    
    var gauge = new RadialGauge({
        renderTo: idcanva,
        width: 200,
        height: 200,
        units: MessageText,
        valueDec: 0,
        valueInt: 2,
        minValue: minVal,
        startAngle: 90,
        ticksAngle: 180,
        valueBox: true,
        maxValue: maxVal,
        majorTicks: [
            minVal,
            halfVal,
            maxVal
        ],
        minorTicks:4,
        strokeTicks: true,
        highlights: [
            { "from": halfVal, "to": maxVal, "color": "#007bff" }
        ],
        colorPlate: "#fff",
        borderShadowWidth: 0,
        borders: false,
        needleType: "arrow",
        needleWidth: 2,
        needleCircleSize: 7,
        needleCircleOuter: true,
        needleCircleInner: false,
        animationDuration: 1500,
        animationRule: "linear"
    }).draw();

    gauge.value = setVal;
}