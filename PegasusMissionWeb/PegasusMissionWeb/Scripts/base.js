$(function () {
    $('.subnavbar').find('li').each(function (i) {
        var mod = i % 3;
        if (mod === 2) {
            $(this).addClass('subnavbar-open-right');
        }
    });

    $('.widget-header').on('click', function () {
        $(this).parent('.widget').find('.widget-content').toggle("slow");
        $(this).find('.minimizeWidget').toggleClass('fa-minus-square-o').toggleClass('fa-plus-square-o');
    });
});
var locationNameArray = ["Launch", "Mobile", "Balloon"];
var map = new google.maps.Map(document.getElementById('map-canvas'));
var balloonTriangle = new google.maps.Polygon({
    paths: [],
    strokeColor: '#FF0000',
    strokeOpacity: 0.8,
    strokeWeight: 2,
    fillColor: '#FF0000',
    fillOpacity: 0.05
});

var imageAntenna = 'images/antenna232.gif';
var imageMobile = 'images/mobile232.gif';
var imageBalloon = 'images/balloon232.gif';

var markerLaunch = new google.maps.Marker({
    position: new google.maps.LatLng(0, 0),
    map: map,
    title: locationNameArray[0],
    icon: imageAntenna
});

var markerMobile = new google.maps.Marker({
    position: new google.maps.LatLng(0, 0),
    map: map,
    title: locationNameArray[1],
    icon: imageMobile
});

var markerBalloon = new google.maps.Marker({
    position: new google.maps.LatLng(0, 0),
    map: map,
    title: locationNameArray[2],
    icon: imageBalloon
});


function centerMapOnLocations(displayMap, launchLatLon, mobileLatLon, balloonLatLon) {
    var bounds = new google.maps.LatLngBounds();

    bounds.extend(launchLatLon);
    bounds.extend(mobileLatLon);
    bounds.extend(balloonLatLon);

    displayMap.fitBounds(bounds);
    displayMap.panToBounds(bounds);

}

function embedElevation(displayMap, marker, elevatorService, locationLatLon, content) {
    var elevationData;

    var locations = [];

    // Push the location on the array
    locations.push(locationLatLon);

    // Create a LocationElevationRequest object using the array's one value
    var positionalRequest = {
        'locations': locations
    }

    // Initiate the location request
    elevatorService.getElevationForLocations(positionalRequest, function (results, status) {
        if (status === google.maps.ElevationStatus.OK) {

            // Retrieve the first result
            if (results[0]) {

                // Open an info window indicating the elevation at the clicked position
                elevationData = 'Ground Elevation: ' + results[0].elevation + ' meters.';

                content += elevationData;
            }

            content += '</p>' +
                  '</div>' +
                  '</div>';

            var infowindow = new google.maps.InfoWindow({
                content: content
            });


            google.maps.event.addListener(marker, 'click', function () {
                infowindow.open(displayMap, marker);
            });
        } else {
            alert('Elevation service failed due to: ' + status);
        }
    });

}

function initialize(ltd, lng, ltd2, lng2) {
    //44.8292, -117.8703
    var launch = new google.maps.LatLng(46.8301, -119.1645);
    var mobile = new google.maps.LatLng(ltd2, lng2);
    var balloon = new google.maps.LatLng(ltd, lng);

    markerLaunch.setPosition(launch);
    markerMobile.setPosition(mobile);
    markerBalloon.setPosition(balloon);

    var elevator = new google.maps.ElevationService();

    var contentStringBalloon = '<div id="content">' +
      '<div id="siteNotice">' +
      '</div>' +
      '<h1 id="firstHeading" class="firstHeading">Balloon</h1>' +
      '<div id="bodyContent">' +
      'Altitude: 22,500 ft<br>' +
      'Direction: SE<br>';

    var contentStringLaunch = '<div id="content">' +
      '<div id="siteNotice">' +
      '</div>' +
      '<h1 id="firstHeading" class="firstHeading">Launch</h1>' +
      '<div id="bodyContent">' +
      'Balloon: 10.5 miles downrange<br>' +
      '- 22,500 ft altitude<br>' +
      '- 12.75 miles actual distance<br>';

    var contentStringMobile = '<div id="content">' +
      '<div id="siteNotice">' +
      '</div>' +
      '<h1 id="firstHeading" class="firstHeading">Chase</h1>' +
      '<div id="bodyContent">' +
      'Balloon: 2.65 miles downrange<br>' +
      '- 22,500 ft altitude<br>' +
      '- 5.25 miles actual distance<br>';

    embedElevation(map, markerBalloon, elevator, balloon, contentStringBalloon);
    embedElevation(map, markerLaunch, elevator, launch, contentStringLaunch);
    embedElevation(map, markerMobile, elevator, mobile, contentStringMobile);

    var triangleCoords = [
        new google.maps.LatLng(ltd, lng),
        new google.maps.LatLng(ltd2, lng2),
        new google.maps.LatLng(46.8301, -119.1645),
        new google.maps.LatLng(ltd, lng)
    ];

    balloonTriangle.setPaths(triangleCoords);

    balloonTriangle.setMap(map);
    centerMapOnLocations(map, launch, mobile, balloon);
}



//var mapInitialized = false; 
//var mapOptions = {
//    center: new google.maps.LatLng(0, 0),
//    zoom: 10,
//    mapTypeId: google.maps.MapTypeId.ROADMAP
//};
//var map = new google.maps.Map(document.getElementById("map_canvas"),
//  mapOptions);

//function initialize(ltd, lng) {
//    mapInitialized = true;
//    var latLng = new google.maps.LatLng(ltd, lng);
//    map.panTo(latLng);
//    //// create a marker
//    //var latlng = new google.maps.LatLng();
//    //var marker = new google.maps.Marker({
//    //  position: latlng,
//    //  map: map,
//    //  title: 'My Place'
//    //});

//}

function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}
