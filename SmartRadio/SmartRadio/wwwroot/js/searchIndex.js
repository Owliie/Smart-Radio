$(document).ready(function() {
    $("#appendix").prepend($("<input class='btn btn-important' type='button' onclick='redirectToMusic()' value='X'/>"));
});

function redirectToMusic() {
    location.href = '/Music';
}