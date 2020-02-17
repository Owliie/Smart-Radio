var connection = new signalR.HubConnectionBuilder().withUrl("/FollowingActivity").build();
var userId = null;

$(document).ready(function () {
    
    userId = readCookie("userId");

    connection.start().then(function () {
        connection.invoke("JoinRoom", userId).catch(function (err) {
            return console.error(err.toString());
        });
    }).catch(function (err) {
        return console.error(err.toString());
    });

    connection.on("DisplayFollowing", function (followingUsers) {
        for (let following of followingUsers) {
            console.log(following);
            $("#following").append(followingInfo(following));
        }
    });

    connection.on("UnFollow", function (id) {
        $(`#following-item-${id}`).remove();
    });

    connection.on("Follow", function (following) {
        $("#following").append(followingInfo(following));
        $(`#search-result-${following.id}`).remove();
    });
});

function followingInfo(following) {
    return $(`<li class="list-group-item d-flex justify-content-between align-items-center" id="following-item-${following.id}">
                <h5 class="mb-1">${following.userName}</h5>
                <div>
                    <span id="fm" class="badge badge-secondary">${following.radioStation}</span>
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

function unFollow(id) {
    connection.invoke("unFollow", userId, id).catch(function(err) {
        return console.log(err.toString());
    });
}

function follow(id) {
    connection.invoke("follow", userId, id).catch(function(err) {
        return console.log(err.toString());
    });
}