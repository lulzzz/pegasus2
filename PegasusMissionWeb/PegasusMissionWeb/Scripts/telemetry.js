var app = angular.module('pegasus', []);


        var chat = $.connection.pegasusHub;
        chat.client.addNewMessageToPage = function (jsonString) {
            var obj = JSON.parse(jsonString);
//live telemetry
               $("#runId").text(obj.runId);
                $("#gpsSpeedKph").text(obj.gpsSpeedKph);
                $("#gpsSpeedMph").text(obj.gpsSpeedMph);
                $("#gpsDirection").text(obj.gpsDirection);
                $("#atmTemp").text(obj.atmTemp);
                $("#atmHumidity").text(obj.atmHumidity);
                $("#atmPressure").text(obj.atmPressure);
$("#imuLinAccelX").text(obj.imuLinAccelX);
$("#imuLinAccelY").text(obj.imuLinAccelY);
$("#imuLinAccelZ").text(obj.imuLinAccelZ);
$("#soundAmp").text(obj.soundAmp);
$("#imuHeading").text(obj.imuHeading);

        };

        chat.client.UpdateOnboard = function (jsonString) {
            var obj = JSON.parse(jsonString);
            $("#stickPosition").text(obj.stickPosition);
            $("#steeringBoxPositionDegrees").text(obj.steeringBoxPositionDegrees);
            $("#noseWeightLbf").text(obj.noseWeightLbf);
            $("#airSpeedKph").text(obj.airSpeedKph);
            $("#throttlePosition").text(obj.throttlePosition);
            $("#leftRearWeightLbf").text(obj.leftRearWeightLbf);
            $("#rightRearWeightLbf").text(obj.rightRearWeightLbf);
            $("#accelXG").text(obj.accelXG);
            $("#accelYG").text(obj.accelYG);
            $("#accelZG").text(obj.accelZG);
            $("#steerBoxAccelXG").text(obj.steerBoxAccelXG);
            $("#steerBoxAccelYG").text(obj.steerBoxAccelYG);
            $("#steerBoxAccelZG").text(obj.steerBoxAccelZG);
        }

        chat.client.updateVideos = function (jsonString) {
            var obj = JSON.parse(jsonString);           
            $('#video-block').html("<hr class='well-sm' /><div class='third-video'><div id='videoControl' class='margin-left-main-shift'><video id='payloadVideo' controls class='azuremediaplayer amp-default-skin videoStyle'><source id='cockpitVideo' src=" + obj[2] + " type='video/mp4'></video></div></div><table border='0' class='margin-left-main-shift'><tr><td style='text-align:center'>Drone 1:</td><td style='text-align:center'>Drone 2</td></tr><tr><td><div id='videoControl'><video id='payloadVideo' controls class='azuremediaplayer amp-default-skin videoStyle'><source id='drone1Video' src=" + obj[0] + " type='video/mp4'></video></div></td><td><div id='videoControl'><video id='payloadVideo' controls class='azuremediaplayer amp-default-skin videoStyle' >Drone 2:<source id='drone2Video' src=" + obj[1] + " type='video/mp4'></video></div></td></tr></table>")

            
        };

        $.connection.hub.start().done(function () {
            chat.server.send(1, 1);
        });
        function invokeCode(runId) {
            var e = document.getElementById("runmenu");
            var strUser = e.options[e.selectedIndex].text;
            chat.server.onboard(strUser);
            chat.server.send()
        }
