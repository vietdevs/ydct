(function () {
    appModule.directive('ngayguicombo', ['$compile', '$timeout', '$templateRequest', '$location',
        function ($compile, $timeout, $templateRequest, $location) {
            var controller = ['$scope',
                function ($scope) {
                    var today = new Date();
                    $scope.ngayGuiOptions = app.createDateRangePickerOptions();
                    $scope.ngaynophoso = app.dateDefault;
                }]

            return {
                restrict: 'EA',
                scope: {
                    ngaynophoso: '=',
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("../App/common/filters/ngay-dn-nop-hs/filter-ngay-nop-ho-so.html").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
})();