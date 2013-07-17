$("input[type='text']").focus(function () {
    if (this.value == 'Find User')
        this.value = '';
}).blur(function () {
    if (this.value == '')
        this.value = 'Find User';
});