// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function route_to_check_by_id(id) {
    window.location.href = "/Check/Details/" + id;
}

function analize_QR_text(text) {
    var prefix = "CovidWeb";
    if (!text.startsWith(prefix)) return false;
    var id = text.slice(prefix.length);
    if (isNaN(id)) return false;
    route_to_check_by_id(id);
}