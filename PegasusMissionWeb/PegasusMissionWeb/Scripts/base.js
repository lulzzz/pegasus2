$(function () {
    $('.subnavbar').find('li').each(function (i) {
        var mod = i % 3;
        if (mod === 2) {
            $(this).addClass('subnavbar-open-right');
        }
    });

    $('.widget-header').on('click', function() {
        $(this).parent('.widget').find('.widget-content').toggle("slow");
        $(this).find('.minimizeWidget').toggleClass('fa-minus-square-o').toggleClass('fa-plus-square-o');
    });
});

var mapInitialized = false;
var mapOptions = {
    center: new google.maps.LatLng(0, 0),
    zoom: 10,
    mapTypeId: google.maps.MapTypeId.ROADMAP
};
var map = new google.maps.Map(document.getElementById("map_canvas"), mapOptions);

function initialize(ltd, lng) {
    mapInitialized = true;
    var latLng = new google.maps.LatLng(ltd, lng);
    map.panTo(latLng);
    //// create a marker
    //var latlng = new google.maps.LatLng();
    //var marker = new google.maps.Marker({
    //  position: latlng,
    //  map: map,
    //  title: 'My Place'
    //});

}

function htmlEncode(value) {
    var encodedValue = $('<div />').text(value).html();
    return encodedValue;
}