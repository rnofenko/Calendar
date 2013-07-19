/*
 *	Setup events and listeners
 */
function SetupEvents() {

    var hiddenField = $("#Extra_Color");
    var dRootCell = $("#pick_color");	/* Cell used to display current color */
    var dColorContainer = $("#color_picker_container"); /* Container DOM element used to attach color picker control to */

    var basePosition = dRootCell.offset();
    dColorContainer.offset({ top: basePosition.top, left: basePosition.left + dRootCell.outerWidth() });	/* Move color picker panel to the initial position */

    /*
     *	Color picker mouse interaction
     */
    createColorPicker(
		dColorContainer,
		2,
		10,
		function () {
		    dRootCell
            .css(
                'background-color',
                $(this).css('background-color'));
		});	/* Color picker control instance */

    $(dColorContainer).hide();  /* Intially hide container for the color picker control */

    dRootCell.css("background-color", hiddenField.attr("value"));

    /* Setup events */

    $("#submit_button").click(function (event)
    {
        //event.preventDefault(); /* Disable default behaviour (don't submit data) */
        
        var color = dRootCell.css("background-color");

        hiddenField.attr("value", new ColorUtilities().RgbToHex(color));
    });

    var colorPickerContainerToggle = function () { dColorContainer.toggle(); };

    dRootCell.hover(colorPickerContainerToggle);
    dColorContainer.hover(colorPickerContainerToggle);
}

var docElement = $(document);

docElement.ready(SetupEvents);