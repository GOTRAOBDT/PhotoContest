$(document).ready(function () {
    console.log("Ready for hub");
    var contestsHub = $.connection.contests;
    contestsHub.client.receiveMessage = function (id) {
        console.log(id);
        $.ajax({
            url: "/Contests/GetContestHubMessagePartial/" + id,
            dataType: "html",
            type: "GET",
            contentType: 'application/json; charset=utf-8',
            async: true,
            processData: false,
            cache: false,
            success: function(notificationPartial) {
                $("#hub-message").append(notificationPartial);
            },
            error: function(xhr) {
                alert('error');
            }
        });
    };

    $.connection.hub.start();
})