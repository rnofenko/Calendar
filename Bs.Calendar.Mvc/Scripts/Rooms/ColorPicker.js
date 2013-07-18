/*
 *	Color picker control builder
 */
var createColorPicker = function (
	dColorContainer,
	iRowCount,
    iColumnCount,
    callback)
{
    /*
     *    Generate random integer in specified range
     */
    var randomize = function (
    	min,
        max) {
        return Math.floor(Math.random() * (max - min + 1)) + min;
    };

    /*
     *    Generate array filled with random integers
     */
    var randomArray = function (
    	iCount,
    	iMin,
    	iMax) {
        var arr = [];

        for (var i = 0; i < iCount; ++i) {

            arr[i] = randomize(iMin, iMax);
        }

        return arr;
    };


    var tColorTable = $("<table>")
    							.appendTo(dColorContainer)
    							.addClass("color_picker_table");

    for (var i = 0; i < iRowCount; ++i)
        /* Create rows */ {
        
        var trRow = $("<tr>").appendTo(tColorTable);

        for (var j = 0; j < iColumnCount; ++j)
            /* Create cells */ {
            
            var RGB = randomArray(3, 0, 256);	/* Generate array filled with random colors */

            var tdCell = $("<td>")
            					.appendTo(trRow)
            					.addClass("color_picker_cell")
            					.click(callback)
            					.css("background-color", "rgb(" + RGB.join(",") + ")");
        }
    }
}