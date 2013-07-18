/*
 *	Setup events and listeners
 */
function SetupEvents() {
    
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

    /* Setup events */

    var colorPickerContainerToggle = function () { dColorContainer.toggle(); };

    dRootCell.hover(colorPickerContainerToggle);
    dColorContainer.hover(colorPickerContainerToggle);
}

$(document).ready(SetupEvents);