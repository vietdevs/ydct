(function () {
    appModule.directive('quanlyhoso.thutuc99.directives.util.boxngayquahan', ['$compile', '$templateRequest',
        function ($compile, $templateRequest) {

            var controller = ['$scope', function ($scope) {
                var vm = this;
                vm.dataItem = {};

                function init() {

                    if ($scope.dataitem) {
                        vm.dataItem = $scope.dataitem;
                    }
                }
                init();
            }];

            return {
                restrict: 'EA',
                scope: {
                    dataitem: '='
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/thutuc99/directives/util/boxNgayQuaHan.cshtml").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
})();