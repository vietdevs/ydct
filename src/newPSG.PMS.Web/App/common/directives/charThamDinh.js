(function () {
    appModule.directive('charThamDinh', function () {
        return {
            restrict: 'A',
            scope: {
                charThamDinh: "=?"
            },
            require: 'ngModel',
            link: function (scope, element, attr, ngModelCtrl) {
                function fromUser(text) {
                    console.log(text);
                    if (text) {
                        var transformedInput = text.replace(/[^oxOX]/g, '').slice(-1);
                        if (transformedInput !== text) {
                            ngModelCtrl.$setViewValue(transformedInput);
                            ngModelCtrl.$render();
                        }

                        return transformedInput;
                    }
                    return undefined;
                }
                ngModelCtrl.$parsers.push(fromUser);
            }
        };
    });
})();