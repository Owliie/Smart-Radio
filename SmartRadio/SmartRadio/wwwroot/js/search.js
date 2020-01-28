$("#people-search").on("input", function () {
    var input = $("#people-search").val();
    console.log(input);

    var list = $("#search-results");

    $.ajax({
        method: "GET",
        url: `/Search/SearchPreview?name=${input}&limit=10`,
        success: function (users) {
            list.empty();
            for (var user of users) {
                list.append(`<li class="list-group-item">${user.userName}</li>`);
            }
            console.log(users);
        }
    });
});