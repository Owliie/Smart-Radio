var page = 0;
var maxPages = 0;
var peoplePerPage = 8;
var peopleCount = 0;

$(document).ready(function () {
    $("#appendix").prepend($("<input class='btn btn-important' type='button' onclick='redirectToMusic()' value='X'/>"));
    $("#prev-arrow").prop("disabled", true);

    peopleCount = $("#results").children().length;

    maxPages = Math.ceil(peopleCount / peoplePerPage);

    if (peopleCount <= peoplePerPage) {
        $("#search-navigation").hide();

        for (var person of $("#results").children()) {
            $(person).attr("hidden", false);
        }
    } else {
        for (var i = 0; i < peoplePerPage; i++) {
            $($("#results").children()[i]).attr("hidden", false);
        }
    }
});

function redirectToMusic() {
    location.href = '/Music';
}

function navigatePrevious() {
    $("#next-arrow").prop("disabled", false);

    shouldShowPeople(true);

    page--;
    if (page === 0) {
        $("#prev-arrow").prop("disabled", true);
    }

    shouldShowPeople(false);
}

function navigateNext() {
    $("#prev-arrow").prop("disabled", false);

    shouldShowPeople(true);

    page++;
    if (page === maxPages - 1) {
        $("#next-arrow").prop("disabled", true);
    }

    shouldShowPeople(false);
}

function shouldShowPeople(show) {
    for (let i = peoplePerPage * page; i < peoplePerPage * page + peoplePerPage; i++) {
        $($("#results").children()[i]).attr("hidden", show);
    }
}