(function () {
    appModule.directive('quanlyhoso.thutuc99.directives.util.bienbanthamdinh', ['$compile', '$timeout', '$templateRequest', '$filter', 'appSession',
        function ($compile, $templateRequest, appSession) {

            var controller = ['$scope', 'abp.services.app.xuLyHoSoChuyenVien99',
                function ($scope, xuLyHoSoChuyenVienService) {
                    var vm = this;
                    vm.userId = appSession.user.id;

                    vm.dataItem = $scope.dataitem;
                    vm.hoSoXuLy = $scope.hosoxuly;
                    vm.bienBanThamDinh = $scope.bienbanthamdinh;
                    vm.nguoiDuyet = $scope.nguoiduyet;
                    vm.readonly = $scope.readonly;

                    function init() {
                        var _id = vm.dataItem.hoSoXuLyId_Active;
                        if (_id > 0) {
                            var params = {
                                hoSoXuLyId: _id,
                                hoSoId: vm.dataItem.id
                            };
                            xuLyHoSoChuyenVienService.loadTongHopThamDinh(params)
                                .then(function (result) {
                                    if (result.data) {
                                        vm.hoSoXuLy = result.data.hoSoXuLy;
                                        //Người duyệt
                                        vm.nguoiDuyet = result.data.nguoiDuyet;

                                        if (result.data.bienBanThamDinhCV1) {
                                            vm.bienBanThamDinhCV1 = result.data.bienBanThamDinhCV1;
                                        }
                                        if (result.data.bienBanThamDinhCV2) {
                                            vm.bienBanThamDinhCV2 = result.data.bienBanThamDinhCV2;
                                        }
                                        if (result.data.duyetHoSo) {
                                            vm.duyetHoSo = result.data.duyetHoSo;
                                        }
                                    }
                                }).finally(function () {
                                    abp.ui.clearBusy();
                                });
                        }
                    }
                    init();
                }];

            return {
                restrict: 'EA',
                scope: {
                    dataitem: '=',
                    hosoxuly: '=',
                    nguoiduyet: '=',
                    bienbanthamdinh: '=?',
                    readonly:'=?'
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/quanlyhoso/thutuc99/directives/util/bienBanThamDinh.cshtml").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
})();