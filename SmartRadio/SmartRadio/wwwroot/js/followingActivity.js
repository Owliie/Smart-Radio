var userId = null;
var connection = null;

$(document).ready(function () {
    connection = new signalR.HubConnectionBuilder().withUrl("/FollowingActivity").build();

    userId = readCookie("userId");

    connection.on("DisplayFollowing", function (followingUsers) {
        if (followingUsers.length === 0) {
            $("#following").append("<li class=\"list-group-item d-flex justify-content-between align-items-center\"><h4>You are not following anybody</h4></li>");
        } else {
            for (let following of followingUsers) {
                $("#following").append(followingInfo(following));
            }
        }
    });

    connection.on("UpdateRadioStation", function (followingId, radioStation) {
        console.log(followingId);
        $(`#fm-${followingId}`).text(radioStation);
    });

    connection.on("UnFollow", function (id) {
        $(`#following-item-${id}`).remove();
    });

    connection.on("Follow", function (following) {
        $("#following").append(followingInfo(following));
        $(`#search-result-${following.id}`).remove();
    });

    connection.start().then(function () {
        connection.invoke("JoinRoom", userId).catch(function (err) {
            return console.error(err.toString());
        });
    }).catch(function (err) {
        return console.error(err.toString());
    });

    function followingInfo(following) {
        return $(`<li class="list-group-item d-flex justify-content-between align-items-center" id="following-item-${following.id}">
                <h5 class="mb-1">${following.userName}</h5>
                <div>
                    <span id="fm-${following.id}" class="badge badge-secondary">${following.radioStation}</span>
                    <div class="btn-group dropright d-inline">
                        <span class="more" data-toggle="dropdown" aria-haspopup="true" aria-expanded="false">
                            &#xFE19;
                        </span>
                        <div class="dropdown-menu">
                            <p class="dropdown-item" onclick="unFollow('${following.id}')">Unfollow</p>
                        </div>
                    </div>
                </div>
            </li>`);
    }
});

function unFollow(id) {
    connection.invoke("UnFollow", userId, id).catch(function (err) {
        return console.log(err.toString());
    });
}

function follow(id) {
    connection.invoke("Follow", userId, id).catch(function (err) {
        return console.log(err.toString());
    });
}