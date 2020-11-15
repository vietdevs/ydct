(function () {
    appModule.directive('viewHoaDon', ['$compile', '$sce', '$timeout', '$templateRequest', '$filter', 'baseService', 'appSession',
        function ($compile, $sce, $timeout, $templateRequest, $filter, baseService, appSession) {
            var controller = ['$scope', '$timeout', '$window', '$filter', '$uibModal', function ($scope, $timeout, $window, $filter, $uibModal) {
                var vm = this;
                vm.dataItem = $scope.thanhtoan;

                vm.hoaDonUrl = "";

                var init = function () {
                    vm.hoaDonUrl = "/File/GoToViewTaiLieu?url=" + vm.dataItem.duongDanHoaDonThanhToan;
                }
                init();

                //Function
                vm.trustSrc = function (src) {
                    return $sce.trustAsResourceUrl(src);
                }
            }];
            return {
                restrict: 'EA',
                scope: {
                    thanhtoan: '=',
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlythanhtoan/directives/viewHoaDon.cshtml").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
})();