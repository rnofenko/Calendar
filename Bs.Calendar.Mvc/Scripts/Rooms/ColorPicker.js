select_color = function (color) {
    $(".roomColorSelected").removeClass('roomColorSelected');
    $("#color_" + String(color)).toggleClass('roomColorSelected');
    $("#Color").val(color);
}
