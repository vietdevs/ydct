(function () {
    appModule.directive('quanlyhoso.thutuc98.directives.util.ketquathamdinh', ['$compile', '$templateRequest',
        function ($compile, $templateRequest) {

            var controller = ['$scope', function ($scope) {
                var vm = this;
                vm.QUI_TRINH_THAM_DINH = app.QUI_TRINH_THAM_DINH;

                vm.dataItem = {};
                vm.hoSoXuLy = {};

                function init() {

                    if ($scope.dataitem) {
                        vm.dataItem = $scope.dataitem;
                    }

                    if ($scope.hosoxuly) {
                        vm.hoSoXuLy = $scope.hosoxuly;
                    }

                }
                init();
            }];

            return {
                restrict: 'EA',
                scope: {
                    dataitem: '=',
                    hosoxuly: '='
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/thutuc98/directives/util/ketQuaThamDinh.cshtml").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
})();