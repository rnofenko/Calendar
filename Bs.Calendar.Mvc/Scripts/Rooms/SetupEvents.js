function SetupPage()
{
    var dRootCell = $("#color_cell");           /* Cell used to display current color */
    var dColorContainer = $("#color_picker");   /* Container DOM element used to attach color picker control to */
    var dHiddenField = $("#Color");

    dColorContainer.hide(); /* Intially hide container for the color picker control */
    
    var colorPickerContainerToggle = function() { dColorContainer.toggle(); };

    dRootCell.hover(colorPickerContainerToggle);
    dColorContainer.hover(colorPickerContainerToggle);

    $("#color_picker > div").click(function() {

        var className = $(this).attr("class");

        dRootCell.attr("class", className);
        dHiddenField.attr("value", className.match(/\d+$/)[0]);
    });
}

var docElement = $(document);

docElement.ready(SetupPage);