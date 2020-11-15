(function () {
    appModule.controller('quanlyhoso.thutuc99.views.motcuarasoat.index', [
        '$sce', '$rootScope', 'appSession', 'quanlyhoso.thutuc99.services.appChuKySo',
        'abp.services.app.xuLyHoSoPhanCong99',
        function ($sce, $rootScope, appSession, appChuKySo,
            xuLyHoSoPhanCongService) {
            var vm = this;
            function initThuTuc() {
                $rootScope.currentThuTuc = appChuKySo.THU_TUC_ID;
                $rootScope.currentMaThuTuc = appChuKySo.MA_THU_TUC;
            }

            initThuTuc();
            vm.form = 'mot_cua_ra_soat';
            vm.formId = 22;
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
            vm.openMotCuaRaSoat = function (dataItem) {
                var params = {
                    loaiHoSoId: null,
                    tenLoaiHoSo: null,
                    arrPhongBanXuLy: []
                };
                vm.phanCongInfo_Reset();
                if (dataItem) {
                    vm.dataItem = dataItem;
                    vm.phanCongInfo.title = "Rà soát hồ sơ [" + dataItem.maHoSo + "] - (loại hồ sơ : " + dataItem.strLoaiHoSo + ")";
                    vm.phanCongInfo.arrHoSoId = [];
                    vm.phanCongInfo.arrHoSoId.push(dataItem.id);
                }
                vm.show_mode = 'mot_cua_ra_soat';
            };
            vm.summernote_options = {
                toolbar: [
                    ['style', ['bold', 'italic', 'underline', 'clear']],
                    ['view', ['fullscreen', 'codeview']]
                ],
                lineHeight: 30,
                height: 200,
                callbacks: {
                    onPaste: function (e) {
                        var bufferText = ((e.originalEvent || e).clipboardData || window.clipboardData).getData('Text');
                        e.preventDefault();
                        setTimeout(function () {
                            document.execCommand('insertText', false, bufferText);
                        }, 10);
                    }
                }
            };

            vm.luuMotCuaPhanCong = function () {
                abp.message.confirm(app.localize("Bạn muốn chuyển hồ sơ để doanh nghiệp thanh toán?"),
                    app.localize('Chuyển hồ sơ'),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            abp.ui.setBusy();
                            xuLyHoSoPhanCongService.yeuCauThanhToan(vm.phanCongInfo)
                                .then(function (result) {
                                    if (result.data) {
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

            vm.tuChoiTiepNhan = function () {
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