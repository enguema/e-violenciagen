document.addEventListener("DOMContentLoaded", () => {

    const form =
        document.getElementById("formCaso");

    const btnGuardar =
        document.getElementById("btnGuardar");

    const alertBox =
        document.getElementById("formAlert");


    const provinciaSelect =
        document.getElementById("provinciaSelect");

    const distritoSelect =
        document.getElementById("distritoSelect");

    const barrioSelect =
        document.getElementById("barrioSelect");


    const incluirAgresor =
        document.getElementById("incluirAgresor");

    const agresorFields =
        document.getElementById("agresorFields");


    // =========================================================
    // PRESUNTO AGRESOR
    // =========================================================

    function actualizarVisibilidadAgresor() {

        if (incluirAgresor.checked) {
            agresorFields.classList.remove("d-none");
        } else {
            agresorFields.classList.add("d-none");
        }
    }

    incluirAgresor.addEventListener(
        "change",
        actualizarVisibilidadAgresor);

    actualizarVisibilidadAgresor();


    // =========================================================
    // PROVINCIA → DISTRITO
    // =========================================================

    provinciaSelect.addEventListener(
        "change",
        async () => {

            const provinciaId =
                provinciaSelect.value;


            distritoSelect.innerHTML =
                `<option value="">Cargando...</option>`;

            distritoSelect.disabled = true;

            barrioSelect.innerHTML =
                `<option value="">
                    Seleccione distrito primero
                 </option>`;

            barrioSelect.disabled = true;


            if (!provinciaId) {
                distritoSelect.innerHTML =
                    `<option value="">
                        Seleccione provincia primero
                     </option>`;

                return;
            }


            const response =
                await fetch(
                    `/Caso/Distritos?provinciaId=${provinciaId}`);


            if (!response.ok) {
                mostrarError(
                    "No se pudieron cargar los distritos.");
                return;
            }


            const distritos =
                await response.json();


            distritoSelect.innerHTML =
                `<option value="">Seleccione...</option>`;


            for (const distrito of distritos) {

                const option =
                    document.createElement("option");

                option.value =
                    distrito.id;

                option.textContent =
                    distrito.nombre;

                distritoSelect.appendChild(option);
            }


            distritoSelect.disabled = false;
        });


    // =========================================================
    // DISTRITO → BARRIO
    // =========================================================

    distritoSelect.addEventListener(
        "change",
        async () => {

            const distritoId =
                distritoSelect.value;


            barrioSelect.innerHTML =
                `<option value="">Cargando...</option>`;

            barrioSelect.disabled = true;


            if (!distritoId) {

                barrioSelect.innerHTML =
                    `<option value="">
                        Seleccione distrito primero
                     </option>`;

                return;
            }


            const response =
                await fetch(
                    `/Caso/Barrios?distritoId=${distritoId}`);


            if (!response.ok) {
                mostrarError(
                    "No se pudieron cargar los barrios.");
                return;
            }


            const barrios =
                await response.json();


            barrioSelect.innerHTML =
                `<option value="">Seleccione...</option>`;


            for (const barrio of barrios) {

                const option =
                    document.createElement("option");

                option.value =
                    barrio.id;

                option.textContent =
                    barrio.nombre;

                barrioSelect.appendChild(option);
            }


            barrioSelect.disabled = false;
        });


    // =========================================================
    // GUARDAR EXPEDIENTE
    // =========================================================

    form.addEventListener(
        "submit",
        async event => {

            event.preventDefault();

            ocultarAlerta();

            btnGuardar.disabled = true;

            const textoOriginal =
                btnGuardar.textContent;

            btnGuardar.textContent =
                "Registrando...";


            try {

                /*
                 * FormData incluye automáticamente:
                 *
                 * - campos del formulario;
                 * - checkboxes seleccionados;
                 * - token antiforgery oculto.
                 */
                const formData =
                    new FormData(form);


                const response =
                    await fetch(
                        form.action,
                        {
                            method: "POST",
                            body: formData
                        });


                const data =
                    await response.json();


                if (!response.ok) {

                    mostrarError(
                        data.message ??
                        "No se pudo registrar el expediente.");

                    return;
                }


                mostrarExito(data.message);


                /*
                 * El servidor devuelve la URL exacta.
                 * Evitamos construir manualmente rutas.
                 */
                if (data.redirectUrl) {
                    window.location.href =
                        data.redirectUrl;
                }

            }
            catch (error) {

                console.error(error);

                mostrarError(
                    "Se produjo un error inesperado al registrar el expediente.");
            }
            finally {

                btnGuardar.disabled = false;

                btnGuardar.textContent =
                    textoOriginal;
            }
        });


    // =========================================================
    // ALERTAS
    // =========================================================

    function ocultarAlerta() {

        alertBox.classList.add("d-none");

        alertBox.classList.remove(
            "alert-danger",
            "alert-success");

        alertBox.textContent = "";
    }


    function mostrarError(message) {

        alertBox.textContent = message;

        alertBox.classList.remove(
            "d-none",
            "alert-success");

        alertBox.classList.add(
            "alert-danger");

        window.scrollTo({
            top: 0,
            behavior: "smooth"
        });
    }


    function mostrarExito(message) {

        alertBox.textContent = message;

        alertBox.classList.remove(
            "d-none",
            "alert-danger");

        alertBox.classList.add(
            "alert-success");
    }
});