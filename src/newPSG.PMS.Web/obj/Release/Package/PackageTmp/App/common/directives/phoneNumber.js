(function () {
    var PHONE_REGEXP = /^[(]{0,1}[0-9]{3}[)\.\- ]{0,1}[0-9]{3}[\.\- ]{0,1}[0-9]{4}$/;
    appModule.directive('checkValidatePhone', ['appSession',
        function (appSession) {
            return {
                restrict: 'E',
                replace: true,
                template: `<small ng-show="checkValidate()" class="help-block cls-hide" ng-class="checkValidate() ? 'custom-error-validate':''"  style="display: none;">{{message}}</small>`,
                scope: {
                    datacheck: '=?',
                },
                link: function ($scope, element, attrs) {
                    $scope.checkValidate = function () {
                        let form = $scope.datacheck;
                        if (app.isNullOrEmpty(form)) {
                            $scope.message = "Không để trống trường này";
                            element.removeClass('cls-hide');
                            return true;
                        }
                        else if (PHONE_REGEXP.test(form)) {
                            $scope.message = "Số điện thoại không đúng định dạng";
                            element.removeClass('cls-hide');
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