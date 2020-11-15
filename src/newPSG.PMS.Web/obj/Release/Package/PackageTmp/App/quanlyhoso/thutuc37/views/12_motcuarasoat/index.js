(function () {
    appModule.controller('quanlyhoso.thutuc37.views.motcuarasoat.index', [
        '$sce', '$rootScope', 'appSession', 'quanlyhoso.thutuc37.services.appChuKySo',
        'abp.services.app.xuLyHoSoVanThu37', 'FileUploader', '$uibModal', 'abp.services.app.thanhToan',
        function ($sce, $rootScope, appSession, appChuKySo,
            xuLyHoSoVanThuService, fileUploader, $uibModal, thanhToanService) {
            var vm = this;
            function initThuTuc() {
                $rootScope.currentThuTuc = appChuKySo.THU_TUC_ID;
                $rootScope.currentMaThuTuc = appChuKySo.MA_THU_TUC;
            }

            initThuTuc();

            // variable
            vm.isGuiLanhDao = false;
            vm.hoSoXuLy = {
                trangThaiXuLy: 1,
                phiDaNop: null
            };
            vm.isDaNopPhi = false;
            vm.form = 'mot_cua_ra_soat';
            vm.formId = 12;
            vm.show_mode = null; //'mot_cua_phan_cong'
            vm.filter = {
                formId: vm.formId,
                formCase: 1, //0:TAT_CA, 1:CHUA_PHAN_CONG, 2:DA_PHAN_CONG, 3:DA_PHAN_CONG_TU_DONG
                formCase2: 0,
                page: 1,
                pageSize: 10,
                keyword: null,
                ngayGuiTu: null,
                ngayGuiToi: null,
                loaiHoSoId: null,
                tinhId: null,
                doanhNghiepId: null,
                phongBanId: null
            };
            if (appSession.user) {
                vm.filter.doanhNghiepId = appSession.user.doanhNghiepId;
                vm.filter.phongBanId = appSession.user.phongBanId;
            }
            vm.phanCongInfo = {};

            // function  
            vm.lstautoCompleteMain = [];
            vm.itemValue = 1000;
            vm.itemValueOrg = angular.copy(vm.itemValue);
            vm.initListAutoComplete = function () {
                vm.lstautoCompleteMain = [];
                vm.itemValue = angular.copy(vm.itemValueOrg);
                if (parseInt(vm.hoSoXuLy.phiDaNop) == 0) {
                    return;
                } else {
                    var outInit = vm.itemValue * vm.hoSoXuLy.phiDaNop;
                    vm.lstautoCompleteMain.push(outInit);
                    for (var i = 0; i < 4; i++) {
                        outInit = outInit * 10;
                        vm.lstautoCompleteMain.push(outInit);
                    }
                }
            };

            vm.openMotCuaRaSoat = function (dataItem) {
                if (dataItem) {
                    vm.dataItem = dataItem;
                    vm.title = "Rà soát hồ sơ [" + dataItem.maHoSo + "]";
                    thanhToanService.getListThanhToanByHoSoId(vm.dataItem.thuTucId, vm.dataItem.id).then(function (result) {
                        if (result.data && result.data.length > 0) {
                            vm.isDaNopPhi = true;
                            vm.hoSoXuLy.phiDaNop = result.data[0].phiDaNop;
                        }
                    })
                }
                vm.show_mode = 'mot_cua_ra_soat';
            };

            vm.duyetHoSo = function () {
                if (!app.checkValidateForm("#form-chuyen-ho-so")) {
                    abp.notify.error(app.localize('Dữ liệu không được để trống'));
                    return;
                }
                abp.message.confirm(app.localize("Bạn muốn duyệt hồ sơ?"),
                    app.localize('Duyệt hồ sơ'),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            let input = {
                                HoSoXuLyId: vm.dataItem.hoSoXuLyId,
                                HoSoId: vm.dataItem.id,
                                TrangThaiXuLy: vm.hoSoXuLy.trangThaiXuLy,
                                LyDoTuChoi: vm.hoSoXuLy.lyDoTuChoi
                            };
                            abp.ui.setBusy();
                            xuLyHoSoVanThuService.vanThuRaSoatHoSo(input)
                                .then(function (result) {
                                    if (result) {
                                        abp.notify.info(app.localize('SavedSuccessfully'));
                                        vm.closeModal();
                                    }
                                }).finally(function () {
                                    abp.ui.clearBusy();
                                });
                        }
                    }
                );
            };

            vm.lapPhieuTiepNhan = function () {
                if (!app.checkValidateForm("#form-chuyen-ho-so")) {
                    abp.notify.error(app.localize('Dữ liệu không được để trống'));
                    return;
                }
                vm.dataItem.phiDaNop = vm.hoSoXuLy.phiDaNop;
                var modelInstance = $uibModal.open({
                    templateUrl: '~/App/quanlyhoso/thutuc37/views/12_motcuarasoat/Model/traGiayTiepNhanModel.cshtml',
                    controller: 'app.quanlyhoso.thutuc37.views.motcuarasoat.model.tragiaytiepnhan as vm',
                    backdrop: 'static',
                    size: 'lg',
                    resolve: {
                        dataItem: function () {
                            return vm.dataItem;
                        }
                    }
                })
                modelInstance.result.then(function (data) {
                    appChuKySo.xemFilePDF(data, 'Phiếu tiếp nhận');
                    vm.isGuiLanhDao = true;
                })
            }

            vm.motCuaGuiPhanCong = function (dataItem) {
                abp.message.confirm(app.localize("Bạn muốn gửi phần công hồ sơ này?"),
                    app.localize('Gửi phân công'),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            abp.ui.setBusy();
                            xuLyHoSoVanThuService.guiLanhDaoCucPhanCongHoSo(dataItem.id)
                                .then(function (result) {
                                    if (result) {
                                        abp.notify.info(app.localize('SavedSuccessfully'));
                                        vm.closeModal();
                                    }
                                }).finally(function () {
                                    abp.ui.clearBusy();
                                });
                        }
                    }
                );
            }

            vm.closeModal = function () {
                $rootScope.$broadcast('refreshGridHoSo', 'ok');
                vm.show_mode = null;
                vm.isDaNopPhi = false;
                vm.isGuiLanhDao = false;
                vm.hoSoXuLy = {
                    trangThaiXuLy: 1,
                    phiDaNop: null
                };
            };

            vm.trustSrc = function (src) {
                return $sce.trustAsResourceUrl(src);
            };
        }
    ]);
})();