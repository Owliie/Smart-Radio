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