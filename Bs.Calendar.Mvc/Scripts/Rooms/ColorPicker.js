select_color = function (color) {
    $(".roomColorSelected").removeClass('roomColorSelected');
    $("#color_" + String(color)).toggleClass('roomColorSelected');
    $("#Color").val(color);
}

$(document).ready(function() {
    select_color(0);
    $('#NumberOfPlaces').toggleClass("wide text input pull_right")
})