var userId = null;

$(document).ready(function() {
    var connection = new signalR.HubConnectionBuilder().withUrl("/MusicListing").build();

    userId = readCookie("userId");

    connection.start().then(function() {
        connection.invoke("JoinRoom", userId).catch(function(err) {
            return console.error(err.toString());
        });
    }).catch(function(err) {
        return console.error(err.toString());
    });

    connection.on("UpdateMusicList", function (song) {
        var day = getParameterByName("day");
        var now = new Date();
        if (day !== null) {
            var month = getParameterByName("month");
            var year = getParameterByName("year");

            if (day !== now.getDate() || month !== now.getMonth()+1 || year !== now.getYear()) {
                return;
            }
        }
        $("tbody").prepend(`<tr>
                                <td>${song.name}</td>
                                <td>${song.artist}</td>
                                <td>${song.radioStation}</td>
                                <td>${formatAMPM(now)}</td>
                            </tr>`
        );
    });
});

function getParameterByName(name) {
    var match = RegExp('[?&]' + name + '=([^&]*)').exec(window.location.search);
    return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
}

function formatAMPM(date) {
    var hours = date.getHours();
    var minutes = date.getMinutes();
    var ampm = hours >= 12 ? 'PM' : 'AM';
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'
    minutes = minutes < 10 ? '0' + minutes : minutes;
    var strTime = hours + ':' + minutes + ' ' + ampm;
    return strTime;
}