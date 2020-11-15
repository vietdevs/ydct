(function () {
    appModule.directive('quanlyhoso.thutuc99.views.thamxethoso.directives.tonghopthamdinh.index', ['$compile', '$templateRequest',
        function ($compile, $templateRequest) {
            var controller = ['$scope', '$rootScope', 'quanlyhoso.thutuc99.services.appChuKySo', 'abp.services.app.xuLyHoSoChuyenVien99',
                function ($scope, $rootScope, appChuKySo, xuLyHoSoChuyenVienService) {
                    var vm = this;
                    vm.QUI_TRINH_THAM_DINH = app.QUI_TRINH_THAM_DINH;

                    vm.dataItem = {};
                    vm.hoSoXuLy = {};
                    vm.listNguoiXuLy = $scope.listNguoiXuLy;

                    function init() {
                        vm.dataItem = $scope.dataitem;
                    }


                    vm.saveThamDinhHoSo = function () {
                        vm.saving = true;
                        let pram = {
                            hoSoXuLyId: vm.dataItem.hoSoXuLyId,
                            phoPhongId: vm.duyetHoSo.phoPhongId,
                            truongPhongId: vm.duyetHoSo.truongPhongId,
                            hoSoIsDat_Input: vm.duyetHoSo.hoSoIsDat,
                            isChuyenNhanh: vm.duyetHoSo.isChuyenNhanh,
                            noiDungCV_Input: vm.duyetHoSo.noiDungCV,
                            lyDoChuyenNhanh: vm.duyetHoSo.lyDoChuyenNhanh
                        };
                        xuLyHoSoChuyenVienService.tongHopThamDinh_Luu(pram)
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
                            if (vm.duyetHoSo.hoSoIsDat == null || (vm.duyetHoSo.hoSoIsDat == false && (vm.duyetHoSo.noiDungCV == null || vm.duyetHoSo.noiDungCV == ""))) {
                                abp.notify.error("Mời nhập dữ liệu");
                                return;
                            }

                            if (vm.duyetHoSo.isChuyenNhanh == null) {
                                abp.notify.error("Mời nhập dữ liệu");
                                return;
                            }
                            if (vm.duyetHoSo.isChuyenNhanh == true && (vm.duyetHoSo.truongPhongId == null || (vm.duyetHoSo.lyDoChuyenNhanh == null || vm.duyetHoSo.lyDoChuyenNhanh == ""))) {
                                abp.notify.error("Mời nhập dữ liệu");
                                return;
                            }
                            if (vm.duyetHoSo.isChuyenNhanh == false && vm.duyetHoSo.phoPhongId == null) {
                                abp.notify.error("Mời nhập dữ liệu");
                                return;
                            }

                            vm.saving = true;
                            let pram = {
                                hoSoXuLyId: vm.dataItem.hoSoXuLyId,
                                phoPhongId: vm.duyetHoSo.phoPhongId,
                                truongPhongId: vm.duyetHoSo.truongPhongId,
                                hoSoIsDat_Input: vm.duyetHoSo.hoSoIsDat,
                                isChuyenNhanh: vm.duyetHoSo.isChuyenNhanh,
                                noiDungCV_Input: vm.duyetHoSo.noiDungCV,
                                lyDoChuyenNhanh: vm.duyetHoSo.lyDoChuyenNhanh
                            };
                            xuLyHoSoChuyenVienService.tongHopThamDinh_Chuyen(pram)
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

                    vm.xemTruocCongVan = function () {
                        if (vm.dataItem) {
                            var item = {
                                id: vm.dataItem.id,
                                noiDungCV: vm.duyetHoSo.noiDungCV,
                                hoSoIsDat: vm.duyetHoSo.hoSoIsDat
                            };
                            appChuKySo.xemTruocCongVan(item, function () {

                            });
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
                    showmode: '=',
                    listNguoiXuLy: '=?'
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("~/App/quanlyhoso/thutuc99/views/3_thamdinhhoso/directives/tongHopThamDinh/index.cshtml").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
})();