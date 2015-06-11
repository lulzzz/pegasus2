var app = angular.module('pegasus', []);

app.controller('telemetry', ['$scope', function ($scope) {
        $(document).ready(function () {

        var chat = $.connection.pegasusHub;
        chat.client.addNewMessageToPage = function (jsonString) {
            var obj = JSON.parse(jsonString);
            if (typeof(obj.gpsAltitude) != "undefined") {
                $.extend($scope, obj);
                $scope.$apply();
            }

            initialize(55.770 + Math.random(), 37.424 + Math.random());
        };

        $.connection.hub.start().done(function () {
            chat.server.send(1);
        });
    });
}]);