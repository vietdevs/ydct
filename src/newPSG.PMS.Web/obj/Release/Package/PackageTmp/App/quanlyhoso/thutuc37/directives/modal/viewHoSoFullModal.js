(function () {
    appModule.controller('quanlyhoso.thutuc37.directives.modal.viewHoSoFullModal', [
        '$sce', '$uibModalInstance', '$filter', 'modalData', 'baseService', 'abp.services.app.xuLyHoSoView37', 'appSession',
        function ($sce, $uibModalInstance, $filter, modalData, baseService, xuLyHoSoView, appSession) {
            var vm = this;
            vm.dataItem = modalData;
            vm.hoSoId = modalData.id;
            vm.dangKyHoSoUrl = "";
            vm.taiLieuDinhKemUrl = "";
            vm.congVanUrl = "";
            vm.giayTiepNhanUrl = "";
            vm.isShowGiayTiepNhan = false;
            vm.lstTaiLieu = [];
            vm.lstCongVan = [];

            vm.lstHoSoXuLy = [];
            vm.xemTaiLieu = function (item) {
                vm.lstTaiLieu.forEach(function (taiLieu) {
                    taiLieu.active = false;
                });
                if (item.duongDanTep != null) {
                    item.active = true;
                    if (item.duongDanTep && item.duongDanTep.includes("readfile?id")) {
                        vm.taiLieuDinhKemUrl = item.duongDanTep + "#zoom=100";
                    } else {
                        vm.taiLieuDinhKemUrl = "/File/GoToViewTaiLieu?filename=" + item.tenTep + "&url=" + item.duongDanTep + "#zoom=100";
                    }
                }
                else
                    vm.taiLieuDinhKemUrl = "";
            };

            vm.xemCongVanBoSung = function (item) {
                if (vm.lstCongVan.length > 0) {
                    vm.lstCongVan.forEach(function (taiLieu) {
                        taiLieu.active = false;
                    });
                    if (item != null) {
                        item.active = true;
                        if (!baseService.isNullOrEmpty(item.duongDanTepCA) && item.lanhDaoCucIsCA == true) {
                            vm.congVanUrl = "/File/GoToViewTaiLieu?filename=congvan.pdf&url=" + item.duongDanTepCA + "#zoom=70";
                        }
                        else {
                            vm.congVanUrl = "/Report37/TemplateCongVan?hoSoId=" + item.hoSoId + "#zoom=70";
                        }
                    }
                }
            };

            vm.showLichSu = false;
            vm.hoSo = {};
            var init = function () {
                xuLyHoSoView.getViewHoSo(vm.dataItem.id).then(function (result) {
                    if (result.data) {
                        if (result.data.trangThaiHoSo && result.data.trangThaiHoSo >= 2) {
                            if (appSession.user && appSession.user.roleLevel != 1) {
                                vm.showLichSu = true;
                            }
                        }
                        if (result.data.hoSo) {
                            vm.hoSo = result.data.hoSo;
                        }
                        else {
                            vm.hoSo = {};
                        }

                        vm.dangKyHoSoUrl = result.data.urlBanDangKy + "#zoom=70";

                        if (!baseService.isNullOrEmpty(result.data.giayTiepNhanUrl)) {
                            vm.isShowGiayTiepNhan = true;
                            vm.giayTiepNhanUrl = "/File/GoToViewTaiLieu?url=" + result.data.giayTiepNhanUrl + "#zoom=70";
                        }

                        if (!baseService.isNullOrEmpty(result.data.bienBanTongHopUrl)) {
                            vm.isShowbienBanTongHopUrl = true;
                            vm.bienBanTongHopUrl = "/File/GoToViewTaiLieu?url=" + result.data.bienBanTongHopUrl + "#zoom=70";
                        }

                        vm.lstTaiLieu = result.data.danhSachTepDinhKem;

                        if (vm.lstTaiLieu != null && vm.lstTaiLieu.length > 0) {
                            vm.lstTaiLieu.forEach(function (item, idx) {
                                if (baseService.isNullOrEmpty(item.moTaTep)) {
                                    item.moTaTep = "Têp đính kèm khác " + (idx + 1);
                                }

                            });
                        }
                        if (vm.lstTaiLieu.length > 0) {
                            vm.xemTaiLieu(vm.lstTaiLieu[0]);
                        }

                        vm.lstCongVan = result.data.danhSachCongVan;
                        vm.lstHoSoXuLy = result.data.hosoXuLy;
                        vm.activeIndex = vm.lstCongVan.length > 0 ? 0 : 1;

                        if (vm.lstCongVan != null && vm.lstCongVan.length > 0) {
                           
                            vm.lstCongVan.forEach(function (item, idx) {
                                item.active = false;
                                var str_date = "(" + new Date(item.ngayTra).getDate() + "/" + (new Date(item.ngayTra).getMonth() + 1) + "/" + new Date(item.ngayTra).getFullYear() + ")";
                                if (idx + 1 == vm.lstCongVan.length) {
                                    vm.lstCongVan[idx].displayText = "Công văn " + str_date;
                                } else {
                                    item.displayText = "Thông báo " + str_date;
                                }
                                if (idx == 0) {
                                    item.active = true;
                                    if (!baseService.isNullOrEmpty(item.duongDanTepCA)) {
                                        vm.congVanUrl = "/File/GoToViewTaiLieu?url=" + item.duongDanTepCA + "#zoom=70";
                                    }
                                    else {
                                        vm.congVanUrl = "/Report37/TemplateCongVan?hoSoId=" + item.hoSoId + "#zoom=70";
                                    }
                                }
                            });

                        }
                    }
                });
            };
            init();

            //Modal
            vm.ok = function () {
                var result = { status: true };
                $uibModalInstance.close(result);
            };

            vm.cancel = function () {
                $uibModalInstance.dismiss('cancel');
            };

            //Function
            vm.trustSrc = function (src) {
                return $sce.trustAsResourceUrl(src);
            };
        }
    ]);
})();