(function () {
    appModule.directive('quanlyhoso.thutuc99.views.thamxethoso.directives.thamdinh.index', ['$compile', '$templateRequest',
        function ($compile, $templateRequest) {
            var controller = ['$scope', '$rootScope', 'abp.services.app.xuLyHoSoChuyenVien99', 'appSession', 'quanlyhoso.thutuc99.services.appChuKySo',
                function ($scope, $rootScope, xuLyHoSoChuyenVienService, appSession, appChuKySo) {
                    var vm = this;
                    vm.QUI_TRINH_THAM_DINH = app.QUI_TRINH_THAM_DINH;

                    vm.dataItem = {};
                    vm.hoSoXuLy = {};

                    function init() {
                        vm.dataItem = $scope.dataitem;
                        var _id = vm.dataItem.hoSoXuLyId_Active;
                        if (_id > 0) {

                            //RESET-INFO
                            //vm.hoSoXuLy_Reset();
                            var params = {
                                hoSoXuLyId: _id,
                                hoSoId: vm.dataItem.id
                            };
                            xuLyHoSoChuyenVienService.loadThamDinh(params)
                                .then(function (result) {
                                    if (result.data) {
                                        vm.hoSoXuLy = result.data.hoSoXuLy;
                                        //Người duyệt
                                        vm.nguoiDuyet = result.data.nguoiDuyet;

                                        if (result.data.bienBanThamDinhCV1 && vm.hoSoXuLy.chuyenVienThuLyId == appSession.user.id) {
                                            vm.duyetHoSo = result.data.bienBanThamDinhCV1;
                                        } else if (result.data.bienBanThamDinhCV2 && vm.hoSoXuLy.chuyenVienPhoiHopId == appSession.user.id) {
                                            vm.duyetHoSo = result.data.bienBanThamDinhCV2;
                                        }
                                    }
                                }).finally(function () {
                                    abp.ui.clearBusy();
                                });
                        }

                    }


                    vm.saveThamDinhHoSo = function () {
                        vm.saving = true;
                        let pram = {
                            hoSoXuLyId: vm.hoSoXuLy.id,
                            bienBanThamDinh: vm.duyetHoSo
                        };
                        xuLyHoSoChuyenVienService.thamDinh_Luu(pram)
                            .then(function (result) {
                                abp.notify.info(app.localize('SavedSuccessfully'));
                                $rootScope.$broadcast('refreshGridHoSo', 'ok');
                                vm.closeModal();
                            }).finally(function () {
                                vm.saving = false;
                            });
                    };

                    vm.chuyenThamDinhHoSo = function () {
                        if (app.checkValidateForm("#ThamDinhHoSo")) {
                            //validate
                            if (vm.duyetHoSo.isThamXetDat == null || (vm.duyetHoSo.isThamXetDat == null && (vm.duyetHoSo.yKienBoSung == null || vm.duyetHoSo.yKienBoSung == ""))) {
                                abp.notify.error("Mời nhập dữ liệu");
                                return;
                            }

                            vm.saving = true;
                            let pram = {
                                hoSoXuLyId: vm.hoSoXuLy.id,
                                bienBanThamDinh: vm.duyetHoSo
                            };
                            xuLyHoSoChuyenVienService.thamDinh_Chuyen(pram)
                                .then(function (result) {
                                    abp.notify.info(app.localize('SavedSuccessfully'));
                                    $rootScope.$broadcast('refreshGridHoSo', 'ok');
                                    vm.closeModal();
                                }).finally(function () {
                                    vm.saving = false;
                                });
                        }
                        else {
                            abp.notify.error("Vui lòng nhập đầy đủ thông tin!");
                        }
                    };

                    vm.xemBienBanThamDinh = function () {
                        if (vm.dataItem) {
                            var item = {
                                id: vm.dataItem.id
                            };
                            appChuKySo.xemTruocBienBanThamDinh(item, function () {

                            });
                        }
                    };

                    vm.closeModal = function () {
                        $scope.showmode = null;
                    };

                    init();
                }];

            return {
                restrict: 'EA',
                scope: {
                    dataitem: '=',
                    showmode: '='
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("~/App/quanlyhoso/thutuc99/views/3_thamdinhhoso/directives/thamDinh/index.cshtml").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
})();