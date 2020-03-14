// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    var picker = $("#datepicker");
    picker.datepicker({
        dateFormat: "dd-mm-yyyy",
        firstDay: 1,
        onSelect: function () {
            var selectedDate = picker.datepicker("getDate");
            location.href = `/Music?day=${selectedDate.getDate()}&month=${selectedDate.getMonth() + 1}&year=${selectedDate.getFullYear()}`;
        }
    });
});

function readCookie(name) {
    var nameEQ = name + "=";
    var ca = document.cookie.split(';');
    for (var i = 0; i < ca.length; i++) {
        var c = ca[i];
        while (c.charAt(0) === ' ') c = c.substring(1, c.length);
        if (c.indexOf(nameEQ) === 0) return c.substring(nameEQ.length, c.length);
    }
    return null;
}

$("#people-search").on("change", function () {
    console.log(this.val());
    $("#search-btn").removeAttr("disabled");
})