/**
 * JavaScript específico del expediente electrónico.
 *
 * No colocamos esta lógica directamente dentro de Details.cshtml
 * para mantener separada la vista de su comportamiento.
 */

document.addEventListener("DOMContentLoaded", () => {

    const casoId = window.sigevigCasoId;

    if (!casoId) {
        console.error("No se encontró el identificador del caso.");
        return;
    }


    /**
     * Vuelve a solicitar al servidor solamente
     * la sección de actuaciones.
     */
    async function recargarActuaciones() {

        const container =
            document.getElementById("actuacionesContainer");

        if (!container) {
            return;
        }

        const response =
            await fetch(`/Caso/Actuaciones?casoId=${casoId}`);

        if (!response.ok) {
            console.error(
                "No se pudieron cargar las actuaciones.");
            return;
        }

        container.innerHTML =
            await response.text();
    }


    /**
     * Actualiza únicamente los documentos del expediente.
     */
    async function recargarDocumentos() {

        const container =
            document.getElementById("documentosContainer");

        if (!container) {
            return;
        }

        const response =
            await fetch(`/Caso/Documentos?casoId=${casoId}`);

        if (!response.ok) {
            console.error(
                "No se pudieron cargar los documentos.");
            return;
        }

        container.innerHTML =
            await response.text();
    }


    /*
     * Dejamos disponibles estas funciones para los
     * modales que incorporaremos posteriormente.
     */
    window.sigevigExpediente = {
        recargarActuaciones,
        recargarDocumentos
    };
});