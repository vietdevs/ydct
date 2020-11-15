(function () {
    appModule.directive('quanlyhoso.thutuc99.directives.util.tablethamdinh', ['$compile', '$templateRequest',
        function ($compile, $templateRequest) {
            var controller = ['$scope', function ($scope) {
                var vm = this;

                vm.readonly = $scope.readonly;
                vm.bienBanThamDinh = $scope.itemthamdinh;
                vm.dataItem = $scope.dataitem;
                vm.hoSoXuLy = $scope.hosoxuly;
            }];

            return {
                restrict: 'EA',
                scope: {
                    dataitem: '=',
                    hosoxuly: '=',
                    itemthamdinh: '=?',
                    readonly: '='
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/thutuc99/directives/util/tableThamDinh.cshtml").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
})();