(function () {
    appModule.directive('checkValidateMinMax', ['appSession',
        function (appSession) {
            return {
                restrict: 'E',
                replace: true,
                template: `<small ng-show="checkValidate()" class="help-block cls-hide" ng-class="checkValidate() ? 'custom-error-validate':''"  style="display: none;">{{message}}</small>`,
                scope: {
                    datacheck: '=?',
                    min: '=?',
                    max: '=?'
                },
                link: function ($scope, element, attrs) {
                    $scope.checkValidate = function () {
                        let form = $scope.datacheck;
                        let minValue = $scope.min;
                        let maxValue = $scope.max;
                        if (app.isNullOrEmpty(form)) {
                            $scope.message = "Không để trống trường này";
                            element.removeClass('cls-hide');
                            return true;
                        }
                        else if (form < minValue) {
                            $scope.message = "Giá trị phải lớn hơn hoặc bằng " + minValue;
                            element.removeClass('cls-hide');
                            return true;
                        }
                        else if (form > maxValue) {
                            $scope.message = "Giá trị phải nhỏ hơn hoặc bằng " + maxValue;
                            return true;
                        }
                        else {
                            element.addClass('cls-hide');
                            return false;
                        }
                    };
                }
            };
        }
    ]);
})();