const archivosEstaticosOffOnline = [
    'https://cdn.datatables.net/plug-ins/1.10.19/i18n/Spanish.json',
    'https://cdn.jsdelivr.net/gh/Mikhus/canvas-gauges@gh-pages/download/2.1.7/radial/gauge.min.js',
    'https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/5.1.3/css/bootstrap.min.css',
    'https://cdn.datatables.net/1.12.1/css/dataTables.bootstrap5.min.css',
    'https://fonts.googleapis.com/css?family=Roboto:300,400,500,700&display=swap',
    'https://cdnjs.cloudflare.com/ajax/libs/mdb-ui-kit/4.2.0/mdb.min.css',
    'https://cdn.datatables.net/1.12.1/js/jquery.dataTables.min.js',
    'https://cdn.datatables.net/1.12.1/js/dataTables.bootstrap5.min.js',
    'https://cdnjs.cloudflare.com/ajax/libs/mdb-ui-kit/4.2.0/mdb.min.js',
    'https://use.fontawesome.com/30dbec44bb.css',
    //'https://use.fontawesome.com/releases/v4.7.0/css/font-awesome-css.min.css',
    '/Content/site.css',
    '/Scripts/jquery-3.4.1.min.js',
    '/Scripts/peticionesHttp.js',
    '/Content/bootstrap.css',
    '/Scripts/modernizr-2.8.3.js',
    '/Scripts/jquery.consulta.js',
    '/Scripts/jquery.consultaModal.js',
    '/Scripts/jquery-3.4.1.js',
    '/Scripts/bootstrap.js',
    '/Content/img/logo_opamss.png',
    '/'
], nombreCacheEstatico = 'cacheEstatico';

self.addEventListener('install', event => {
    event.waitUntil(
        caches.open(nombreCacheEstatico).then(resp => {
            return resp.addAll(archivosEstaticosOffOnline)
        })
    )
});

self.addEventListener('activate', event => {
    console.log('event activate')
    event.waitUntil(self.clients.claim())
});

self.addEventListener('fetch', event => {
    const respuesta = caches.match(event.request).then(resp => {
        if (resp)
            return resp;
        else {
            return fetch(event.request).then(response => {
                return response;
            })
        }
    }).catch(err => {
        return null;
    })
    event.respondWith(respuesta);
});