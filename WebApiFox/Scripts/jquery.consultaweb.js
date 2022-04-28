
function GetData() {

    //Modelo para request de data
    var datos = {
        anio: $("#lsAno").val(),
        tabla: $("#codigo_tramite").val(),
        expediente: $('#lsExpe').val()
    };
    //Funcion de ajax para mapear los tramites
    fns.CallGetAsync("api/values/Getall", datos, function (requestData) {

        //LLENADO DE DATATABLE
        $("#consultaTable").DataTable().clear();
        $("#consultaTable").DataTable().rows.add(requestData).draw();


    });

}

//Funcion que crea la DataTable y al final renderiza la Data
function DatatableConsultaWeb() {
    $('#consultaTable').DataTable({
        destroy:true,
        dom: '<"datatables_toolbar">lfrtip',
        data: [],     
        columns: [
            { data: "numeroExpediente" },
            { data: "descripcionEstado" },
            { data: "nomProy" },
            { data: "municipio" },
            { data: "tiempoRespuesta" }],
        columnDefs: [
            {
                "targets": 5,
                "data": "numeroExpediente",
                "render": function (data, type, full, meta) {
                    return '<button onclick="VerRegistro(\'' + data + '\')" class="btn btn-success" id="boton">Ver</button>';
                }
            }
        ],
           "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.19/i18n/Spanish.json"
        }
    });

    GetData();
}

//Instancia de DOCUMENT READY
$().ready(function ($) {

    //Primera Instancia de Datatable para la lista de Tipos de Tramite
    $('#consultaTable').DataTable({
        destroy: true,
        "language": {
            "url": "//cdn.datatables.net/plug-ins/1.10.19/i18n/Spanish.json"
        }
    });
    //Click de boton buscar
    $("#btBuscar").click(function () {
        DatatableConsultaWeb();
    });

    //SELECT desde Jquery con los Tipos de Tramite
    SelectTipoTramite();
    //SELECT desde Jquery con los anios
    DropdownAnios();

    //TEXTBOX configuraciones NoExpediente
    NumeroExpTextBox();

});

//Funcion para modificiar el Dropdonw list de los tipos de Tramite
function SelectTipoTramite() {

    //FNS para los tipos de Tramites
    fns.CallGetAsync("api/Tramites/GetT", "", function (data) {
        var dataRequest = data;
        let select = `<select class="form-control" id="codigo_tramite">`;
        dataRequest.forEach(x => {
            let Opcion = '<option value="' + x.codigo_tramite.trim() + '">' + x.nombre + '</option>';
            select = select + Opcion;
        });
        select = select + "</select>";

        //Con HTML se inserta el objeto select que creamos arriba
        $("#selectTipoTramite").html(select);
    });

}

//Funcion para configurar DropdownList de Años 
function DropdownAnios() {
    var fecha = new Date();
    var anio = fecha.getFullYear();

    let Dropdown = `<select class="form-control" id="lsAno">`;
    for (var i = 0; i < 3; i++) {
        let aniosPasados = parseInt(anio) - i;
        let Opcion = '<option value="' + aniosPasados + '">' + aniosPasados + '</option>';
        Dropdown = Dropdown + Opcion;
    }
    Dropdown = Dropdown + "</select>";

    //Con HTML se inserta el objeto select que creamos arriba
    $("#DropAnios").html(Dropdown);
}


//Funcion para configurar el TEXTBOX de Numero de Expediente
function NumeroExpTextBox() {

    //Input mask para Numero de Expediente
    $("#lsExpe").mask("99999999", { placeholder: "" });

    $('#lsExpe').on('keyup keypress', function (e) {
        var keyCode = e.keyCode || e.which;
        if (keyCode === 13) {
            DatatableConsultaWeb();
        }
    });
}
