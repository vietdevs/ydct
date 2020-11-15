(function () {
    appModule.directive('maskedDatePicker', [
        function () {
            return {
                restrict: 'AE',
                link: function (scope, elem, attrs) {
                    $(elem).kendoMaskedTextBox({
                        mask: "00/00/0000"
                    });
                    $(elem).removeClass("k-textbox");
                }
            };
        }
    ]);
})();