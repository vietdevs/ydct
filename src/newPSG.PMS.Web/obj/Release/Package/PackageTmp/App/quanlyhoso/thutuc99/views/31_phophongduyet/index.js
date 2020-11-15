(function () {
    appModule.controller('quanlyhoso.thutuc99.views.phophongduyet.index', [
        '$scope', '$rootScope', 'appSession', 'quanlyhoso.thutuc99.services.appChuKySo',
        'abp.services.app.xuLyHoSoPhoPhong99',
        function ($scope, $rootScope, appSession, appChuKySo,
            xuLyHoSoPhoPhongService) {
            var vm = this;
            vm.DON_VI_XU_LY = app.DON_VI_XU_LY;
            vm.QUI_TRINH_THAM_DINH = app.QUI_TRINH_THAM_DINH;

            function initThuTuc() {
                $rootScope.currentThuTuc = appChuKySo.THU_TUC_ID;
                $rootScope.currentMaThuTuc = appChuKySo.MA_THU_TUC;
            }
            initThuTuc();

            vm.form = 'pho_phong_duyet';
            vm.formId = 31;
            vm.show_mode = null; //'pho_phong_duyet'
            vm.saving = false;
            vm.closeModal = function () {
                vm.show_mode = null;
            };
            vm.summernote_options = {
                toolbar: [
                    ['style', ['clear']]
                ],
                height: 80,
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

            vm.filter = {
                formId: vm.formId,
                formCase: 1, //0: getAll(), 1: hồ sơ chưa duyệt, 2: hồ sơ đã duyệt và đang theo dõi
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

            //Common
            vm.showLichSu = false;
            vm.toggleLichSu = function () {
                vm.showLichSu = !vm.showLichSu;
            };

            //--- Phó phòng duyệt ---//
            vm.listLanhDaoCuc = [];

            vm.paramInit = {
                phongBanId: appSession.user.phongBanId,
                userId: appSession.user.id,
                roleLevel: appSession.user.roleLevel
            };
            var init = function () {
                xuLyHoSoPhoPhongService.initPhoPhongDuyet(vm.paramInit)
                    .then(function (result) {
                        if (result.data) {
                            vm.listLanhDaoCuc = result.data.listLanhDaoCuc;
                            vm.listTruongPhong = result.data.listTruongPhong;
                        }
                    }).finally(function () {
                        //vm.loading = false;
                    });
            };
            init();

            vm.dataItem = {};
            vm.hoSoXuLy = {};

            vm.duyetHoSo = {
                hoSoIsDatChuyenVien: null,
                hoSoXuLyId: null,
                hoSoIsDat: null,
                noiDungCV: null,
                tieuDeCV: null,
                trangThaiCV: null,
                donViKeTiep: null,
                truongPhongId: null,
                //***
                isSuaCV: null,
                noiDungYKien: null
            };

            vm.hoSoXuLy_Reset = function () {
                vm.showLichSu = false;
                vm.hoSoXuLy = {};
                vm.thoiGianHieuLucJson = {
                    thoiHan: null,
                    donVi: null
                };
                vm.duyetHoSo = {
                    hoSoIsDatChuyenVien: null,
                    hoSoXuLyId: null,
                    hoSoIsDat: null,
                    noiDungCV: null,
                    tieuDeCV: null,
                    trangThaiCV: null,
                    donViKeTiep: null,
                    truongPhongId: null,
                    //***
                    isSuaCV: null,
                    noiDungYKien: null
                };
            };

            //Change Noi dung CV khi chon đạt và lưu lại
            vm.changeNoiDungCV = function (hoSoIsDat) {
                if (hoSoIsDat == false && vm.hoSoXuLy.noiDungCV) {
                    vm.duyetHoSo.noiDungCV = vm.hoSoXuLy.noiDungCV;
                }
            };

            vm.openPhoPhongDuyet = function (dataItem) {
                vm.dataItem = dataItem;
                var _id = dataItem.hoSoXuLyId_Active;
                if (_id > 0) {

                    //RESET-INFO
                    vm.hoSoXuLy_Reset();

                    var params = {
                        hoSoXuLyId: _id,
                        hoSoId: dataItem.hoSoId
                    };
                    xuLyHoSoPhoPhongService.loadPhoPhongDuyet(params)
                        .then(function (result) {
                            if (result.data) {
                                vm.hoSoXuLy = result.data.hoSoXuLy;
                                vm.nguoiDuyet = result.data.nguoiDuyet;
                                vm.hosoxl = {
                                    hoSoXuLy: vm.hoSoXuLy,
                                    nguoiDuyet: vm.nguoiDuyet
                                };
                                if (result.data.duyetHoSo) {
                                    vm.duyetHoSo = result.data.duyetHoSo;
                                } else {
                                    vm.duyetHoSo.hoSoIsDat = vm.hoSoXuLy.hoSoIsDat;
                                    vm.duyetHoSo.tieuDeCV = vm.hoSoXuLy.tieuDeCV;
                                    vm.duyetHoSo.noiDungCV = vm.hoSoXuLy.noiDungCV;
                                }
                                if (result.data.thoiGianHieuLuc) {
                                    vm.thoiGianHieuLucJson = result.data.thoiGianHieuLuc;
                                }
                                vm.duyetHoSo.yKienChung = vm.hoSoXuLy.yKienChung;
                                vm.duyetHoSo.hoSoXuLyId = vm.hoSoXuLy.id;
                                vm.duyetHoSo.truongPhongId = vm.hoSoXuLy.truongPhongId;
                                vm.duyetHoSo.phoPhongId = vm.hoSoXuLy.phoPhongId;

                                vm.show_mode = 'pho_phong_duyet';
                            }
                        }).finally(function () {
                            //vm.loading = false;
                        });
                }
            };

            vm.savePhoPhongDuyet = function () {
                vm.saving = true;
                if (vm.duyetHoSo.hoSoIsDat == true) {
                    vm.duyetHoSo.donViKeTiep = vm.DON_VI_XU_LY.TRUONG_PHONG
                }
                xuLyHoSoPhoPhongService.phoPhongDuyet_Luu(vm.duyetHoSo)
                    .then(function (result) {
                        vm.saving = false;
                        abp.notify.info(app.localize('SavedSuccessfully'));
                        $rootScope.$broadcast('refreshGridHoSo', 'ok');
                        vm.show_mode = null;

                    }).finally(function () {
                        //vm.loading = false;
                    });
            }

            vm.chuyenPhoPhongDuyet = function () {
                vm.saving = true;

                //Validate
                var flagNull = true;
                if (!app.checkValidateForm("#duyet-tham-dinh-ho-so"))
                    flagNull = false;
                if (vm.duyetHoSo.hoSoIsDat == null
                    || (vm.duyetHoSo.hoSoIsDat == true && vm.duyetHoSo.truongPhongId == null)
                    || (vm.duyetHoSo.hoSoIsDat == false && vm.duyetHoSo.donViKeTiep == null)) {
                    flagNull = false;
                }
                if (vm.duyetHoSo.hoSoIsDat == false && vm.duyetHoSo.donViKeTiep == vm.DON_VI_XU_LY.TRUONG_PHONG && (vm.duyetHoSo.noiDungCV == null || vm.duyetHoSo.noiDungCV == "")) {
                    flagNull = false;
                }
                else if (vm.duyetHoSo.donViKeTiep == vm.DON_VI_XU_LY.TRUONG_PHONG && vm.duyetHoSo.truongPhongId == null) {
                    flagNull = false;
                }
                else if (vm.duyetHoSo.donViKeTiep == vm.DON_VI_XU_LY.CHUYEN_VIEN_THAM_XET_TONG_HOP && (vm.duyetHoSo.noiDungYKien == null || vm.duyetHoSo.noiDungYKien == "")) {
                    flagNull = false;
                }

                if (!flagNull) {
                    abp.notify.error('Mời nhập dữ liệu');
                    vm.saving = false;
                    return;
                } else {
                    if (vm.duyetHoSo.hoSoIsDat == true) {
                        vm.duyetHoSo.donViKeTiep = vm.DON_VI_XU_LY.TRUONG_PHONG
                    }
                    xuLyHoSoPhoPhongService.phoPhongDuyet_Chuyen(vm.duyetHoSo)
                        .then(function (result) {
                            vm.saving = false;
                            abp.notify.info(app.localize('SavedSuccessfully'));
                            $rootScope.$broadcast('refreshGridHoSo', 'ok');
                            vm.show_mode = null;

                        }).finally(function () {
                            //vm.loading = false;
                        });
                }
            }

            vm.resetSummernote = function (val) {
                var control = angular.element("#summernote_congvan");
                if (val == true && val != null)
                    control.summernote('enable');
                else
                    control.summernote('disable');
            }
            $scope.$watchCollection('vm.duyetHoSo.isSuaCV', function (newValue, oldValue) {
                vm.resetSummernote(newValue);
            });


            $scope.$watchCollection('vm.duyetHoSo.hoSoIsDat', function (newValue, oldValue) {
                if (newValue) {
                    vm.duyetHoSo.isSuaCV = false;
                    //vm.duyetHoSo.noiDungCV = null;
                    vm.duyetHoSo.noiDungYKien = null;
                    vm.duyetHoSo.truongPhongId = null;
                    vm.duyetHoSo.donViKeTiep = null;
                } else {
                    vm.duyetHoSo.truongPhongId = null;
                }
            });


            vm.xemTruocCongVan = function () {
                if (vm.dataItem) {
                    var item = {
                        id: vm.dataItem.id,
                        noiDungCV: vm.duyetHoSo.noiDungCV,
                        hoSoIsDat: vm.duyetHoSo.hoSoIsDat
                    }
                    appChuKySo.xemTruocCongVan(item, function () {

                    });
                }
            }

            vm.xemBienBanThamDinh = function () {
                if (vm.dataItem) {
                    var item = {
                        id: vm.dataItem.id,
                        noiDungThamXetJson: "",
                        yKienChung: vm.duyetHoSo.yKienChung
                    }

                    appChuKySo.xemTruocBienBanThamDinh(item, function () {

                    });
                }
            }
        }
    ]);
})();