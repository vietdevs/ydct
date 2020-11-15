(function () {
    appModule.controller('quanlyhoso.thutuc37.views.motcuaphancong.index', [
        '$sce', '$rootScope', 'baseService', 'appSession', 'quanlyhoso.thutuc37.services.appChuKySo',
        'abp.services.app.xuLyHoSoPhanCong37', 'abp.services.app.xuLyHoSoView37',
        function ($sce, $rootScope, baseService, appSession, appChuKySo,
            xuLyHoSoPhanCongService, xuLyHoSoView) {
            var vm = this;
            function initThuTuc() {
                $rootScope.currentThuTuc = appChuKySo.THU_TUC_ID;
                $rootScope.currentMaThuTuc = appChuKySo.MA_THU_TUC;
            }
            initThuTuc();
            console.log(appSession, 'appSessionappSession');
            vm.form = 'mot_cua_phan_cong';
            vm.formId = 2;
            vm.dangKyHoSoUrl = "";
            vm.lstTaiLieu = [];
            vm.show_mode = null; //'mot_cua_phan_cong'
            vm.closeModal = function () {
                $rootScope.$broadcast('refreshGridHoSo', 'ok');
                vm.show_mode = null;
                vm.phanCongInfo = {
                    arrHoSoId: [],
                    phongBanId: null,
                    tenPhongBan: null
                };
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
                phongBanId: null,
                tenPhongBan: null
            };


            // ================= function =============================//

            vm.openMotCuaPhanCong = function (dataItem) {
                vm.dataItem = dataItem;
                var params = {
                    arrPhongBanXuLy: []
                };

                if (dataItem) {
                    var hoSo = dataItem;
                    if (hoSo && hoSo.arrPhongBanXuLy && hoSo.arrPhongBanXuLy.length > 0) {

                        params.arrPhongBanXuLy = hoSo.arrPhongBanXuLy;
                        vm.lstPhongBan = hoSo.arrPhongBanXuLy;
                        
                    }
                    else {
                        // lấy từ appSession 

                        params.arrPhongBanXuLy = appSession.get_phongban();
                        vm.lstPhongBan = appSession.get_phongban();
                        
                    }

                    vm.phanCongInfo.title = "Chuyển hồ sơ [" + hoSo.maHoSo + "] - (loại hồ sơ : " + hoSo.strLoaiHoSo + ")";
                    
                    if (hoSo.phongBanId) {
                        vm.phanCongInfo.phongBanId = hoSo.phongBanId;
                    }

                    vm.phanCongInfo.arrHoSoId.push(hoSo.id);

                }
                //Load Data

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

                vm.show_mode = 'mot_cua_phan_cong';
            };

            vm.luuMotCuaPhanCong = function () {
                if (!app.checkValidateForm("#form-chuyen-ho-so")) {
                    abp.notify.error("Vui lòng nhập đầy đủ thông tin!");
                    return;
                }
                else {
                    if (vm.lstPhongBan && vm.lstPhongBan.length > 0) {
                        var find = vm.lstPhongBan.find(function (item) {
                            return item.id == vm.phanCongInfo.phongBanId;
                        });
                        if (find) {
                            vm.phanCongInfo.tenPhongBan = find.name;
                        }
                    }
                    console.log(vm.phanCongInfo);
                    //return;
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

            vm.trustSrc = function (src) {
                return $sce.trustAsResourceUrl(src);
            };

        }
    ]);
})();