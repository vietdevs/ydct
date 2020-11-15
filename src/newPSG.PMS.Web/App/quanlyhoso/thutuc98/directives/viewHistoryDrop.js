(function () {
    appModule.directive('quanlyhoso.thutuc98.directives.viewhistorydrop', ['$compile', '$templateRequest', 'abp.services.app.xuLyHoSoView98', 'appSession',
        function ($compile, $templateRequest, xuLyHoSoViewService, appSession) {

            var controller = ['$scope', '$uibModal', 'baseService', function ($scope, $uibModal, baseService) {
                var vm = this;
                vm.user = appSession.user;
                vm.hosoid = $scope.hosoid;
                vm.listYKien = [];
                vm.getYKien = function (item) {
                    var strYKien = "";
                    strYKien = baseService.formatBreakLineTextareaToHTML(item);
                    return strYKien;
                };

                vm.requestParams = {
                    skipCount: 0,
                    maxResultCount: 5,
                    sorting: null
                };
                vm.totalHistory = null;
                vm.currentPageHoSoDangKyCongBo = 1;
                vm.pageSize = 5;
                vm.pageChanged = function (newPage) {
                    vm.requestParams.skipCount = (newPage - 1) * vm.pageSize;
                    vm.requestParams.maxResultCount = vm.pageSize;
                };

                var init = function () {
                    $scope.$watch("hosoid", function () {
                        if ($scope.hosoid) {
                            abp.ui.setBusy();
                            xuLyHoSoViewService.getHistory($scope.hosoid)
                                .then(function (result) {
                                    if (result.data) {
                                        vm.listYKien = result.data.listYKien;
                                        vm.totalHistory = vm.listYKien.length;

                                    }
                                }).finally(function () {
                                    abp.ui.clearBusy();
                                });
                        }
                    });

                };
                init();

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
                        console.log(result, 'result');
                    });
                };

                vm.xuatExel = () => {
                    vm.exporting = true;
                    xuLyHoSoViewService.exportToExcel($scope.hosoid)
                        .then(function (result) {
                            console.log(result, "result");
                            app.downloadTempFile(result.data);
                        }).finally(function () {
                            vm.exporting = false;
                        });
                };


            }];

            return {
                restrict: 'EA',
                scope: {
                    hosoid: '=?'
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/thutuc98/directives/viewHistoryDrop.cshtml").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
})();