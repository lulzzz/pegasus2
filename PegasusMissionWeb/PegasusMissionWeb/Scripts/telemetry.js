var app = angular.module('pegasus', []);

app.controller('telemetry', ['$scope', function ($scope) {

    $(document).ready(function () {

        var b1 = 44.8292;
        var b2 = -117.8703;
        var c1 = 43.8292;
        var c2 = -118.8703;
        //44.8292, -117.8703
        //43.8292, -118.8703
        var chat = $.connection.pegasusHub;
        chat.client.addNewMessageToPage = function (jsonString, telemetryType) {
            var obj = JSON.parse(jsonString);
            if (telemetryType == "balloon") {
                $("#gpsAltitude").text(obj.gpsAltitude);
            }

            if (typeof (obj.gpsAltitude) != "undefined") {
                $.extend($scope, obj);
                $scope.$apply();
            }

            initialize(obj.gpsLatitude, obj.gpsLongitude, telemetryType);

        };

        $.connection.hub.start().done(function () {
            chat.server.send(1, 1);
        });
    });
}]);