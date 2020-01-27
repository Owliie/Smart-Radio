var connection = new signalR.HubConnectionBuilder().withUrl("/FriendsActivity").build();
var userId = null;

$(document).ready(function () {
    
    userId = readCookie("userId");

    console.log("function: ", userId);

    connection.start().then(function () {
        connection.invoke("JoinRoom", userId).catch(function (err) {
            return console.error(err.toString());
        });
    }).catch(function (err) {
        return console.error(err.toString());
    });

    connection.on("DisplayFriends", function (friends) {
        for (let friend of friends) {
            console.log(friend);
            $("#friends").append($(`<li class="list-group-item d-flex justify-content-between align-items-center" id="friend-item-${friend.id}">
                                        <h5 class="mb-1">${friend.userName}</h5>
                                        <div>
                                            <span id="fm" class="badge badge-secondary">${friend.radioStation}</span>
                                            <div class="btn-group dropright d-inline">
                                                <span class="more" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                                                    &#xFE19;
                                                </span>
                                                <div class="dropdown-menu">
                                                    <p class="dropdown-item" onclick="deleteFriend('${friend.id}')">Unfollow</p>
                                                </div>
                                            </div>
                                        </div>
                                    </li>`));
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

function deleteFriend(id) {
    connection.invoke("DeleteFriend", userId, id).catch(function(err) {
        return console.log(err.toString());
    });
}

connection.on("DeleteFriend", function(userId, id) {
    $(`#friend-item-${id}`).remove();
});