var path = "/";
//var path = "/Consulta/"; //Esta es usada en produccion
var fns = {
    PostData: function (ruta, data) {
        var ruta = path + ruta;
        var rt;
        $.ajax({
            url: ruta,
            type: "POST",
            data: data,
            async: false,
            dataType: "json"
        }).done(function (data) {
            rt = data;
        }).fail(function (xhr, textStatus, errorThrown) {
            alert(xhr.responseText);
            alert(textStatus);
        });
        return rt;
    },

    PostDataAsync: function (ruta, data, callBack) {
        var ruta = path + ruta;
        var rt;
        $.blockUI({ message: "Procesando, favor espere." });
        $.ajax({
            url: ruta,
            type: "POST",
            data: data,
            async: true,
            dataType: "json"
        }).done(function (data) {
            rt = data;
            $.unblockUI();
            callBack(data);
        }).fail(function (xhr, textStatus, errorThrown) {
            alert(xhr.responseText);
            alert(textStatus);
            $.unblockUI();
        });
        return rt;
    },
    CallGet: function (ruta, data) {
        var ruta = path + ruta;
        var rt;
        $.ajax({
            url: ruta,
            type: "GET",
            data: data,
            async: false
        }).done(function (data) {
            rt = data;
        }).fail(function (xhr, textStatus, errorThrown) {
            alert(xhr.responseText);
            alert(textStatus);
        });
        return rt;
    },
    CallGetAsync: function (ruta, data, callBack) {
        var ruta = path + ruta;
        var rt;
        $.blockUI({ message: "Procesando, favor espere." });
        $.ajax({
            url: ruta,
            type: "GET",
            data: data,
            async: true
        }).done(function (data) {
            $.unblockUI();
            rt = data;
            callBack(data);
        }).fail(function (xhr, textStatus, errorThrown) {
            alert(xhr.responseText);
            alert(textStatus);
            $.unblockUI();
        });
        return rt;
    }
}