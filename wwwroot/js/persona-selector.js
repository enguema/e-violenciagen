(function () {

    "use strict";


    /**
     * Inicializa todos los selectores de Persona
     * presentes en la página.
     */
    document.addEventListener(
        "DOMContentLoaded",
        function () {

            document
                .querySelectorAll(".persona-selector")
                .forEach(initializePersonaSelector);

        });


    function initializePersonaSelector(container) {

        const searchInput =
            container.querySelector(
                'input[type="search"]'
            );

        const hiddenInput =
            container.querySelector(
                'input[type="hidden"]'
            );

        const resultsContainer =
            container.querySelector(
                ".list-group"
            );

        const selectedContainer =
            container.querySelector(
                "[data-persona-selected]"
            );

        const selectedName =
            container.querySelector(
                "[data-persona-name]"
            );

        const selectedDocument =
            container.querySelector(
                "[data-persona-document]"
            );

        const photoContainer =
            container.querySelector(
                "[data-persona-photo]"
            );

        const removeButton =
            container.querySelector(
                "[data-persona-remove]"
            );

        const clearButton =
            container.querySelector(
                "[data-persona-clear]"
            );


        let debounceTimer = null;

        let abortController = null;


        // =====================================================
        // BÚSQUEDA
        // =====================================================

        searchInput.addEventListener(
            "input",
            function () {

                const term =
                    searchInput.value.trim();


                /*
                 * Si el usuario modifica el texto después de
                 * seleccionar una Persona, eliminamos la
                 * selección anterior.
                 */
                if (hiddenInput.value) {
                    clearSelection(false);
                }


                clearTimeout(debounceTimer);


                if (term.length < 2) {

                    hideResults();

                    return;
                }


                /*
                 * Debounce:
                 *
                 * evitamos enviar una petición HTTP con cada
                 * tecla inmediatamente.
                 */
                debounceTimer =
                    setTimeout(
                        function () {
                            searchPersonas(term);
                        },
                        350
                    );

            });


        // =====================================================
        // CONSULTA
        // =====================================================

        async function searchPersonas(term) {

            /*
             * Cancelamos la petición anterior si el usuario
             * continúa escribiendo.
             */
            if (abortController) {
                abortController.abort();
            }

            abortController =
                new AbortController();


            showLoading();


            try {

                const url =
                    `/Persona/Search?term=${encodeURIComponent(term)}`;


                const response =
                    await fetch(
                        url,
                        {
                            signal:
                                abortController.signal
                        });


                if (!response.ok) {

                    throw new Error(
                        `HTTP ${response.status}`
                    );

                }


                const personas =
                    await response.json();


                renderResults(personas);

            }
            catch (error) {

                /*
                 * AbortError es normal cuando el usuario
                 * escribe una nueva búsqueda antes de que
                 * termine la anterior.
                 */
                if (error.name === "AbortError") {
                    return;
                }


                console.error(
                    "Error buscando personas:",
                    error
                );


                resultsContainer.innerHTML = `
                    <div class="list-group-item text-danger">
                        No se pudo realizar la búsqueda.
                    </div>
                `;

                resultsContainer.classList.remove(
                    "d-none"
                );

            }

        }


        // =====================================================
        // MOSTRAR RESULTADOS
        // =====================================================

        function renderResults(personas) {

            resultsContainer.innerHTML = "";


            if (!personas ||
                personas.length === 0) {

                resultsContainer.innerHTML = `
                    <div class="list-group-item text-muted">
                        No se encontraron personas.
                    </div>
                `;

                resultsContainer.classList.remove(
                    "d-none"
                );

                return;
            }


            personas.forEach(
                function (persona) {

                    const button =
                        document.createElement("button");

                    button.type = "button";

                    button.className =
                        "list-group-item " +
                        "list-group-item-action";


                    const documentText =
                        buildDocumentText(persona);


                    button.innerHTML = `
                        <div class="d-flex
                                    align-items-center
                                    gap-3">

                            ${buildPhoto(persona)}

                            <div>

                                <div class="fw-semibold">
                                    ${escapeHtml(
                                        persona.nombreCompleto
                                    )}
                                </div>

                                <small class="text-muted">
                                    ${escapeHtml(
                                        documentText
                                    )}
                                </small>

                            </div>

                        </div>
                    `;


                    button.addEventListener(
                        "click",
                        function () {

                            selectPersona(persona);

                        });


                    resultsContainer.appendChild(
                        button
                    );

                });


            resultsContainer.classList.remove(
                "d-none"
            );

        }


        // =====================================================
        // SELECCIONAR
        // =====================================================

        function selectPersona(persona) {

            hiddenInput.value =
                persona.id;

            searchInput.value =
                persona.nombreCompleto;


            selectedName.textContent =
                persona.nombreCompleto;


            selectedDocument.textContent =
                buildDocumentText(persona);


            photoContainer.innerHTML =
                buildPhoto(persona);


            selectedContainer.classList.remove(
                "d-none"
            );


            clearButton?.classList.remove(
                "d-none"
            );


            hideResults();

        }


        // =====================================================
        // ELIMINAR SELECCIÓN
        // =====================================================

        removeButton?.addEventListener(
            "click",
            function () {

                clearSelection(true);

            });


        clearButton?.addEventListener(
            "click",
            function () {

                clearSelection(true);

            });


        function clearSelection(
            clearSearchText
        ) {

            hiddenInput.value = "";

            selectedContainer.classList.add(
                "d-none"
            );

            clearButton?.classList.add(
                "d-none"
            );


            if (clearSearchText) {

                searchInput.value = "";

                searchInput.focus();

            }

        }


        // =====================================================
        // UTILIDADES
        // =====================================================

        function showLoading() {

            resultsContainer.innerHTML = `
                <div class="list-group-item text-muted">
                    <span class="spinner-border
                                 spinner-border-sm
                                 me-2">
                    </span>

                    Buscando personas...
                </div>
            `;

            resultsContainer.classList.remove(
                "d-none"
            );

        }


        function hideResults() {

            resultsContainer.classList.add(
                "d-none"
            );

            resultsContainer.innerHTML = "";

        }


        function buildDocumentText(
            persona
        ) {

            if (!persona.numeroDocumento) {

                return "Sin documento registrado";

            }


            if (persona.tipoDocumento) {

                return `${persona.tipoDocumento}: ` +
                       `${persona.numeroDocumento}`;

            }


            return persona.numeroDocumento;

        }


        function buildPhoto(persona) {

            if (persona.rutaFoto) {

                return `
                    <img
                        src="${escapeHtml(
                            persona.rutaFoto
                        )}"
                        alt=""
                        width="44"
                        height="44"
                        class="rounded-circle
                               border
                               object-fit-cover"
                    />
                `;

            }


            const initial =
                persona.nombreCompleto
                    ?.trim()
                    ?.charAt(0)
                    ?.toUpperCase()
                ?? "?";


            return `
                <div
                    class="rounded-circle
                           bg-light
                           border
                           d-flex
                           align-items-center
                           justify-content-center
                           fw-semibold"
                    style="width:44px;height:44px;">

                    ${escapeHtml(initial)}

                </div>
            `;

        }


        function escapeHtml(value) {

            const div =
                document.createElement("div");

            div.textContent =
                value ?? "";

            return div.innerHTML;

        }

    }

})();