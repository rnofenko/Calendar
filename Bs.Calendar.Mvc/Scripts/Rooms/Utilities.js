function ColorUtilities() {
    
    this.hexDigits = ["0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "a", "b", "c", "d", "e", "f"];

    this.RgbToHex = function(rgb) {

        rgb = rgb.match(/^rgb\((\d+),\s*(\d+),\s*(\d+)\)$/); /* example string: rgb(0,0,0) */
        return "#" + this.FormatHex(rgb[1]) + this.FormatHex(rgb[2]) + this.FormatHex(rgb[3]);
    };

    this.FormatHex = function(x) {

        return isNaN(x) ? "00" : this.hexDigits[(x - x % 16) / 16] + this.hexDigits[x % 16];
    };
};
