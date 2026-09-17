// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {

    const sidebar = document.getElementById("appSidebar");
    const backdrop = document.getElementById("sidebarBackdrop");

    const btnOpen = document.getElementById("btnOpenSidebar");
    const btnClose = document.getElementById("btnCloseSidebar");


    function openSidebar() {

        sidebar?.classList.add("show");
        backdrop?.classList.add("show");

    }


    function closeSidebar() {

        sidebar?.classList.remove("show");
        backdrop?.classList.remove("show");

    }


    btnOpen?.addEventListener("click", openSidebar);

    btnClose?.addEventListener("click", closeSidebar);

    backdrop?.addEventListener("click", closeSidebar);

});