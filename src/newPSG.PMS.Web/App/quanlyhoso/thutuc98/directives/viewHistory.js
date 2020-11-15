(function () {
    appModule.directive('quanlyhoso.thutuc98.directives.viewhistory', ['$compile','$templateRequest', 'abp.services.app.xuLyHoSoView98', 'baseService',
        function ($compile, $templateRequest, xuLyHoSoViewService, baseService) {

            var controller = ['$scope', '$uibModal', function ($scope,  $uibModal) {
                var vm = this;
                vm.hosoid = $scope.hosoid;
                vm.listYKien = [];
                var firstload = true;

                var init = function () {
                    $scope.$watch("show", function () {
                        if ($scope.show && firstload) {
                            xuLyHoSoViewService.getHistory(vm.hosoid)
                                .then(function (result) {
                                    if (result.data) {
                                        vm.listYKien = result.data.listYKien;
                                    }
                                }).finally(function () {
                                    firstload = false;
                                });
                        }
                    });

                };
                init();

                vm.getYKien = function (item) {
                    var strYKien = "";
                    strYKien = baseService.formatBreakLineTextareaToHTML(item.noiDungYKien);
                    return strYKien;
                };

                vm.itemYKien = {};
                vm.xemChiTiet = function (item) {

                    var modalInstance = $uibModal.open({
                        templateUrl: '~/App/quanlyhoso/thutuc98/directives/modal/viewHistoryChiTietModal.cshtml',
                        controller: 'quanlyhoso.thutuc98.directives.modal.viewHistoryChiTietModal as vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            modalData: {
                                yKien: item
                            }
                        }
                    });

                    modalInstance.result.then(function (result) {
                    });
                };
            }];

            return {
                restrict: 'EA',
                scope: {
                    hosoid: '=',
                    show: '=?'
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/thutuc98/directives/viewHistory.cshtml").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
})();