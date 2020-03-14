$(document).ready(function () {
    if (window.location.pathname !== "/Search/Search") {
        $("#people-search").on("input", function () {
            var input = $("#people-search").val();
            
            if (input === "") {
                $("#search-btn").attr("disabled", true);
            } else {
                $("#search-btn").removeAttr("disabled");
            }

            var list = $("#search-results");

            $.ajax({
                method: "GET",
                url: `/Search/SearchPreview?name=${input}&limit=10`,
                success: function (users) {
                    list.empty();
                    for (var user of users) {
                        list.append(`<li class="list-group-item">${user.userName}</li>`);
                    }
                }
            });
        });
    }

});

