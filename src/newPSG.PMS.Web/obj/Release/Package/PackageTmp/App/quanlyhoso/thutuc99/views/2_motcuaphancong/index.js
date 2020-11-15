(function () {
    appModule.controller('quanlyhoso.thutuc99.views.motcuaphancong.index', [
        '$sce', '$rootScope', 'baseService', 'appSession', 'quanlyhoso.thutuc99.services.appChuKySo',
        'abp.services.app.xuLyHoSoPhanCong99', 'abp.services.app.xuLyHoSoView99',
        function ($sce, $rootScope, baseService, appSession, appChuKySo,
            xuLyHoSoPhanCongService, xuLyHoSoView) {
            var vm = this;
            function initThuTuc() {
                $rootScope.currentThuTuc = appChuKySo.THU_TUC_ID;
                $rootScope.currentMaThuTuc = appChuKySo.MA_THU_TUC;
            }
            initThuTuc();

            vm.form = 'mot_cua_phan_cong';
            vm.formId = 2;
            vm.dangKyHoSoUrl = "";
            vm.lstTaiLieu = [];
            vm.show_mode = null; //'mot_cua_phan_cong'
            vm.closeModal = function () {
                $rootScope.$broadcast('refreshGridHoSo', 'ok');
                vm.show_mode = null;
            };
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

                //app-session
                doanhNghiepId: null,
                phongBanId: null
            };

            if (appSession.user) {
                vm.filter.doanhNghiepId = appSession.user.doanhNghiepId;
                vm.filter.phongBanId = appSession.user.phongBanId;
            }

            vm.phanCongInfo = {
                arrHoSoId: [],
                arrPhongBan: []
            };
            vm.phanCongInfo_Reset = function () {
                vm.phanCongInfo = {
                    arrHoSoId: [],
                    arrPhongBan: []
                };
            };
            
            vm.openMotCuaPhanCong = function (dataItem) {
                var params = {
                    loaiHoSoId: null,
                    tenLoaiHoSo: null,
                    arrPhongBanXuLy: []
                };

                vm.phanCongInfo_Reset();

                if (dataItem) {
                    var hoSo = dataItem;
                    if (hoSo && hoSo.arrPhongBanXuLy && hoSo.arrPhongBanXuLy.length > 0) {
                        params = {
                            loaiHoSoId: hoSo.loaiHoSoId,
                            tenLoaiHoSo: hoSo.tenLoaiHoSo,
                            arrPhongBanXuLy: hoSo.arrPhongBanXuLy
                        };
                        vm.lstPhongBan = hoSo.arrPhongBanXuLy;
                        vm.phanCongInfo.title = "Chuyển hồ sơ [" + hoSo.maHoSo + "] - (loại hồ sơ : " + hoSo.tenLoaiHoSo + ")";
                        xuLyHoSoView.getViewHoSo(hoSo.id).then(function (result) {
                            vm.dangKyHoSoUrl = result.data.urlBanDangKy + "#zoom=70";
                            vm.lstTaiLieu = result.data.danhSachTepDinhKem;
                            if (vm.lstTaiLieu != null && vm.lstTaiLieu.length > 0) {
                                vm.lstTaiLieu.forEach(function (item, idx) {
                                    if (baseService.isNullOrEmpty(item.moTaTep)) {
                                        item.moTaTep = "Têp đính kèm khác " + (idx + 1);
                                    }
                                });
                                vm.xemTaiLieu(vm.lstTaiLieu[0]);
                            }
                        });
                    }
                    if (hoSo.phongBanId) {
                        vm.phanCongInfo.phongBanId = hoSo.phongBanId;
                    }
                    vm.phanCongInfo.arrHoSoId = [];
                    vm.phanCongInfo.arrHoSoId.push(hoSo.id);

                }
                vm.xemTaiLieu = function (item) {
                    vm.lstTaiLieu.forEach(function (taiLieu) {
                        taiLieu.active = false;
                    });
                    if (item != null) {
                        item.active = true;
                        if (item.duongDanTep && item.duongDanTep.includes("readfile?id")) {
                            vm.taiLieuDinhKemUrl = item.duongDanTep + "#zoom=100";
                        } else {
                            vm.taiLieuDinhKemUrl = "/File/GoToViewTaiLieu?url=" + item.duongDanTep + "#zoom=100";
                        }
                    }
                };

                //Load Data
                if (params.loaiHoSoId) {
             
                    vm.loading = true;
                    xuLyHoSoPhanCongService.loadPhanCongPhongBan(params)
                        .then(function (result) {
                    
                            if (result.data && result.data.listThongKePhanCong) {
                                vm.listThongKePhanCong = result.data.listThongKePhanCong;
                            } else {
                                vm.listThongKePhanCong = [];
                            }
                        }).finally(function () {
                            vm.loading = false;
                        });
                    
                }
                else
                {
                    abp.notify.error("Loại hồ sơ bị null hoặc chưa có phòng ban nào xử lý.");
                }
                vm.show_mode = 'mot_cua_phan_cong';
            };

            vm.luuMotCuaPhanCong = function () {
                if (!app.checkValidateForm("#form-chuyen-ho-so")) {
                    abp.notify.error("Vui lòng nhập đầy đủ thông tin!");
                    return;
                } else {
                    if (vm.lstPhongBan && vm.lstPhongBan.length > 0) {
                        var find = vm.lstPhongBan.find(function (item) {
                            return item.id == vm.phanCongInfo.phongBanId;
                        });
                        if (find) {
                            vm.phanCongInfo.tenPhongBan = find.name;
                        }
                    }
                    abp.ui.setBusy();
                    xuLyHoSoPhanCongService.phanCongPhongBan(vm.phanCongInfo)
                        .then(function (result) {
                            if (result.data) {
                                abp.notify.info(app.localize('SavedSuccessfully'));
                                vm.closeModal();
                            }
                        }).finally(function () {
                            abp.ui.clearBusy();
                        });
                }
            };

            vm.tuChoiTiepNhanDonHang = function () {
                if (!app.checkValidateForm("#form-chuyen-ho-so")) {
                    abp.notify.error("Vui lòng nhập đầy đủ thông tin!");
                    return;
                } else {
                    abp.ui.setBusy();
                    xuLyHoSoPhanCongService.tuChoiTiepNhan(vm.phanCongInfo)
                        .then(function (result) {
                            if (result.data) {
                                abp.notify.info(app.localize('SavedSuccessfully'));
                                vm.closeModal();
                            }
                        }).finally(function () {
                            abp.ui.clearBusy();
                        });
                }
            };

            vm.trustSrc = function (src) {
                return $sce.trustAsResourceUrl(src);
            };
            
        }
    ]);
})();