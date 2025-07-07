

// La URL base de la API.
const API_URL = 'https://localhost:7248/api/Todo';


document.addEventListener('DOMContentLoaded', () => {
    // -- SELECCION DE ELEMENTOS DEL DOM --
    // Guardar en constantes todas las "vistas" y los botones de navegacion
    const vistas = {
        inicio: document.getElementById('vista-inicio'),
        lista: document.getElementById('vista-lista-tareas'),
        crear: document.getElementById('vista-crear-tarea'),
        detalle: document.getElementById('vista-detalle-tarea'),
    };
    const notificacionEl = document.getElementById('notificacion');
    function mostrarNotificacion(mensaje, tipo = 'info') {
        notificacionEl.textContent = mensaje;   // Asignamos el mensaje a la notificación
        notificacionEl.className = "";  // Limpiamos las clases anteriores
        notificacionEl.classList.add(tipo); // Añadimos la clase "exito" o "error"

        setTimeout(() => {  // Ocultamos la notificación después de 3 segundos
            notificacionEl.classList.add('oculto');
        }, 3000);
    }

    const botones = {
        verTareas: document.getElementById('btn-ver-tareas'),
        irACrear: document.getElementById('btn-ir-a-crear'),
        volver: document.querySelectorAll(".btn-volver"), // querySelectorAll porque hay varios
    };

    let tareas = []; // Variable para almacenar las tareas obtenidas de la API

    // añadimos una referencia a nuesttro contenedor de tareas
    const listaTareasContainer =document.getElementById('lista-tareas');

    // --- LOGICA DE NAVEGACION ---
    // Función central para cambiar de vista. Es reutilizable.
    // Oculta todas las vistas y luego muestra solo la que queremos.
    console.log(vistas);
    function mostrarVista(nombreVista){
        // Ocultamos todas las vistas iterando sobre el objeto "vistas"
        for (let key in vistas){
            vistas[key].classList.add("oculto");
        }
        // Mostramos la vista que queremos
        vistas[nombreVista].classList.remove("oculto");
    }

    // LOGICA DE API Y RENDERIZADO DE TAREAS
    // FUNCION 1: HABLAR CON LA API PARA OBTENER LAS TAREAS(UNICA RESPONSABILIDAD)
    async function obtenerTareasAPI(){
        try{
            const response = await fetch(API_URL);
            if (!response.ok){
                // SI LA API DEVUELVE UN ERROR, LANZAMOS UNA EXCEPCION
                throw new Error(`Error al obtener las tareas: ${response.statusText}`);
            }
            const tareas = await response.json();
            console.log("Tareas obtenidas:", tareas); // <-- Esto te dice si llegaron datos
            return tareas; // DEVUELVE EL ARRAY DE TAREAS   
        } catch(error){
            console.error('Error de red o de API:', error);
            // Podríamos mostrar un mensaje de error en la interfaz de usuario
            listaTareasContainer.innerHTML = "<li>Error al cargar las tareas. inténtalo más tarde.</li>";
            return []; // DEVUELVE UN ARRAY VACIO EN CASO DE ERROR
        }
    }

    
    


        // FUNCION 2: MOSTRAR LOS DATOS EN HTML (UNICA RESPONSABILIDAD)
        function renderizarListaTareas(tareas){

            const listaTareasContainer = document.getElementById('lista-tareas');

            // PRIMERO, LIMPIAMOS CUALQUIER CONTENIDO ANTERIOR
            listaTareasContainer.innerHTML = '';

            if (tareas.length === 0){
                listaTareasContainer.innerHTML = '<li>No hay tareas disponibles. ¡Crea una!</li>';
                return;
            }
            
            
        
        // POR CADA TAREA EN EL ARRAY, CREAMOS UN ELEMENTO <LI>
        tareas.forEach(tarea => {
            const li = document.createElement('li');

            // ASIGNAMOS UNA CLASE SI LA TAREA ESTA COMPLETADA PARA PODER DARLE ESTILOS
            li.className = tarea.isCompleted ? 'completada' : "";
            // ¡ESTO ES CRUCIAL! Guardamos el ID de la tarea en un atributo 'data-'
            // Esto nos permitirá saber a qué tarea se hizo clic más adelante.
            li.dataset.id = tarea.id;
            li.textContent = tarea.title;

            if (tarea.isCompleted){
                li.classList.add('completada');
            }

            listaTareasContainer.appendChild(li);
        });

    }


    // Asignamos eventos a los botones de navegación
    botones.verTareas.addEventListener('click', async () => {// cuando se hace click en "Ver Tareas", mostramos la vista de lista Aqui, mas adelante llamamos a la funcion que carga las tareas de la API
        mostrarVista('lista');

        listaTareasContainer.innerHTML = '<li>Cargando tareas...</li>'; // Mensaje temporal

        // LLAMADO A LA FUNCION QUE OBTIENE LOS DATOS
        const tareas = await obtenerTareasAPI();
        // LLAMADO A LA FUNCION QUE RENDERIZA LOS DATOS
        renderizarListaTareas(tareas);
        
    });





    botones.irACrear.addEventListener('click', () => {
        // cuando se hace click en "Crear Tarea", mostramos la vista de crear
        mostrarVista('crear');
    });

    




    console.log(botones.volver);
    // Como hay varios botones "Volver", iteramos sobre ellos
    botones.volver.forEach(btn => {
        btn.addEventListener('click', () => {
            // cuando se hace click en "Volver", mostramos la vista de inicio
            mostrarVista('inicio');
        });
    });
    // Al cargar la pagina, nos aseguramos de que la vista de inicio es la que se muestra
    mostrarVista('inicio');


    const formCrearTarea = document.getElementById('form-crear-tarea');
    const inputTitulo = document.getElementById('input-titulo');
    const inputDescripcion = document.getElementById('input-descripcion');



// LOGICA DE API PARA CREAR UNA TAREA (RENDERIZADO Y FUNCION POST)
// ENVIAR NUEVA TAREA A LA API

async function crearTareaAPI(nuevaTarea){
    try{
        const response = await fetch(API_URL, {
            method: 'POST', // MENCION AL METODO HTTP (POST EN ESTE CASO)
            headers: { // IMPORTANTE: MENCIONA QUE EL CONTENIDO ES JSON
                'Content-Type': 'application/json'
            }, // COMVERTIR EL OBJETO JS A JSON
            body: JSON.stringify(nuevaTarea)
        })
        if (!response.ok){ // SI LA API DEVUELVE UN ERROR, LANZAMOS UNA EXCEPCION
            throw new Error(`Error al crear la tarea: ${response.statusText}`);
        }

        const tareaCreada = await response.json();
        console.log("Tarea creada:", tareaCreada); 
        return tareaCreada; // DEVUELVE LA TAREA CREADA
    } catch(error){
        console.error('Error al crear la tarea:', error);
        alert('Error al crear la tarea. Inténtalo más tarde.');
    }
}



// --- INTERACCION CON TAREAS ---



// OBTENER DETALLES DE UNA TAREA POR ID
    async function obtenerTareasPorIdAPI(id){
        try{
            const response = await fetch(`${API_URL}/${id}`);
            if (!response.status === 404){
                alert('Tarea no encontrada');
                return null; // DEVUELVE NULL SI NO SE ENCUENTRA LA TAREA
            }
            if (!response.ok){
                throw new Error(`Error al obtener la tarea: ${response.statusText}`);
            }

            const tarea = await response.json(); 
            console.log("detalles de la tarea obtenida:", tarea); // <-- Esto te dice si llegaron datos desde la consola
            return tarea; // DEVUELVE LA TAREA OBTENIDA
        } catch(error){
            console.error('Error al obtener detalles de la tarea:', error);
            alert('Error al obtener los detalles de la tarea. Inténtalo más tarde.');
            return null; // DEVUELVE NULL EN CASO DE ERROR
        }
    }

    function renderizarDetalleTarea(tarea){

        const detalleTitulo = document.getElementById('detalle-titulo');
        const detalleDescripcion = document.getElementById('detalle-descripcion');
        const detalleEstado = document.getElementById('detalle-estado');
        const btnCambiarEstado = document.getElementById('btn-cambiar-estado');
        const vistaDetalle = document.getElementById('vista-detalle-tarea');

        vistaDetalle.dataset.idActual = tarea.id; // ASIGNAMOS EL ID DE LA TAREA ACTUAL PARA USARLO EN LOS BOTONES DESPUES

        // OBTENEMOS LOS DATOS Y CON ELLOS RELLENAMOS LOS CAMPOS DEL HTML
        detalleTitulo.textContent = tarea.title;
        detalleDescripcion.textContent = tarea.description;
        detalleEstado.textContent = tarea.isCompleted ? 'Completada' : 'Pendiente';

        if (tarea.isCompleted) {
            btnCambiarEstado.textContent = 'Marcar como Pendiente';
        }else{
            btnCambiarEstado.textContent = 'Marcar como Completada';
        }
        btnCambiarEstado.disabled = false;

        mostrarVista('detalle'); // MOSTRAMOS LA VISTA DE DETALLE
    }





// CUANDO SE HACE CLICK EN UNA TAREA, MOSTRAMOS SUS DETALLES
listaTareasContainer.addEventListener('click', async (event) => {
    const elementoClicado = event.target; // ESTO ES PARA VERIFICAR QUE SE HIZO CLICK SOBRE UN ELEMENTO LI, Y NO EN EL ESPACIO VACIO
    if(elementoClicado.tagName === 'LI'){
        const idTarea = elementoClicado.dataset.id;
        console.log('!Se hizo click en la tarea con ID: ${idTarea}¡');
        
        // LLAMADO A LA FUNCION QUE OBTIENE LOS DETALLES DE LA TAREA POR ID
        const tareaDetallada = await obtenerTareasPorIdAPI(idTarea);

        // LA API DEVUELVE ALGUN VALOR?
        if (tareaDetallada){
            console.log("Datos para la vista de detalle:", tareaDetallada);
            renderizarDetalleTarea(tareaDetallada); // LLAMADO A LA FUNCION QUE RENDERIZA LOS DETALLES DE LA TAREA
        }
    }
});






// --- ASIGNACION DE EVENTOS AL FORMULARIO ---

formCrearTarea.addEventListener('submit', async (event) => {
    event.preventDefault();  // EVITA QUE SE RECARGUE LA PAGINA AL ENVIAR EL FORMULARIO

    const titulo = inputTitulo.value.trim(); // OBTIENE EL VALOR DEL INPUT Y LO ASIGNA A "TITULO"
    const descripcion = inputDescripcion.value.trim(); // OBTIENE EL VALOR DEL INPUT Y LO ASIGNA A "DESCRIPCION"

    // VALIDACION BASICA
    if (!titulo || !descripcion){
        alert('Por favor, completa todos los campos.');
        return;
    }

    // CREAR UN OBJETO TAREA CON LOS DATOS DEL FORMULARIO
    const nuevaTareaDto = {
        title: titulo,
        description: descripcion,
    };

    // TOMA EL OBJETO/DTO CON LOS DATOS DEL FORMULARIO Y LO ENVIA A LA API
    // RECORDAR QUE "crearTareaAPI" ES UNA FUNCION PARA HABLAR CON LA API
    const tareaCreada = await crearTareaAPI(nuevaTareaDto);

    // SI LA NUEVA TAREA SE CREO CORRECTAMENTE, LIMPIAMOS EL FORMULARIO
    if (tareaCreada) {
        inputTitulo.value = '';
        inputDescripcion.value = '';
        // Y VOLVEMOS A LA VISTA DE LISTA DE TAREAS
        mostrarNotificacion("¡Tarea creada con éxito!", "exito");

        botones.verTareas.click(); // Simula un clic en el botón "Ver Tareas" para recargar la lista
    }

})});
