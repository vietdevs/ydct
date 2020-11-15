(function () {
    appModule.controller('quanlyhoso.thutuc37.views.vanthuduyet.index', [
        '$rootScope', '$sce', 'abp.services.app.xuLyHoSoVanThu37', 'appSession', 'quanlyhoso.thutuc37.services.appChuKySo',
        function ($rootScope, $sce, xuLyHoSoVanThuService, appSession, appChuKySo) {

            var vm = this;
            function initThuTuc() {
                $rootScope.currentThuTuc = appChuKySo.THU_TUC_ID;
                $rootScope.currentMaThuTuc = appChuKySo.MA_THU_TUC;
            }
            initThuTuc();

            vm.form = 'van_thu_duyet';
            vm.formId = 7;
            vm.show_mode = null; //'van_thu_duyet'

            vm.closeModal = function () {
                vm.show_mode = null;
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

            //Ký nhiều công văn đạt
            vm.arrCheckbox = [];
            vm.updateArrCheckbox = function (arrCheckbox) {
                vm.arrCheckbox = arrCheckbox;
            };

            //Begin
            vm.objInfo = {
                tenTruongPhong: null,
                tenLanhDaoCuc: null
            };
            vm.hoSoXuLy = {};
            vm.duyetHoSo = {
                hoSoXuLyId: null,
                trangThaiCV: null,
                donViKeTiep: null,
                yKien: null,
                isCoSaiSot: null
            };

            vm.hoSoXuLy_Reset = function () {
                vm.hoSoXuLy = {};
                vm.duyetHoSo = {
                    hoSoXuLyId: null,
                    trangThaiCV: null,
                    donViKeTiep: null,
                    yKien: null,
                    isCoSaiSot: null
                };
            };

            vm.dataItem = {};
            vm.openVanThuDuyet = function (dataItem) {
                vm.dataItem = dataItem;
                var _id = dataItem.hoSoXuLyId_Active;
                if (_id > 0) {

                    //RESET-INFO
                    vm.hoSoXuLy_Reset();

                    var params = {
                        HoSoXuLyId: _id
                    };
                    xuLyHoSoVanThuService.loadVanThuDuyet(params)
                        .then(function (result) {

                            if (result.data) {
                                vm.hoSoXuLy = result.data.hoSoXuLy;
                                vm.objInfo = result.data.objInfo;

                                vm.duyetHoSo.hoSoXuLyId = vm.hoSoXuLy.id;
                                vm.duyetHoSo.HoSoId = dataItem.id;

                                vm.duongDanCA = "/File/GoToViewTaiLieu?url=" + vm.hoSoXuLy.duongDanTepCA;
                                if (vm.hoSoXuLy.hoSoIsDat) {
                                    vm.duongDanCA = "/File/GoToViewTaiLieu?url=" + vm.hoSoXuLy.giayTiepNhanCA + "#zoom=105";
                                }

                                vm.show_mode = 'van_thu_duyet';
                            }
                        });
                }
            };

            vm.dongDauGiayTo = function () {
                abp.message.confirm(app.localize(""),
                    app.localize('Bạn chắc chắn muốn đóng dấu văn bản?'),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            var params = {
                                id: vm.dataItem.id,
                                hoSoIsDat: vm.dataItem.hoSoIsDat
                            };
                            //Ký số
                            appChuKySo.vanThuDongDau(params, function (paramKySo) {
                                var params = {
                                    hoSoId: vm.dataItem.id,
                                    hoSoXuLyId: vm.dataItem.hoSoXuLyId_Active,
                                    duongDanTepCA: vm.dataItem.hoSoIsDat != true ? paramKySo.duongDanTep : null,
                                };

                                vm.saving = true;
                                xuLyHoSoVanThuService.dongDau(params)
                                    .then(function (result) {
                                        if (result.data == 0) {
                                            abp.notify.success(app.localize('Kết nối hải quan không thành công'));
                                        }
                                        else {
                                            abp.notify.success(app.localize('Đóng dấu thành công'));
                                            vm.saving = false;
                                            $rootScope.$broadcast('refreshGridHoSo', 'ok');
                                            vm.show_mode = null;

                                            appChuKySo.xemFilePDF(paramKySo.duongDanTep, 'Công văn đã ký số');

                                        }
                                    });
                            });
                        }
                    }
                );
            };

            vm.chuyenLaiTruongPhong = function () {
                vm.saving = true;
                if (!app.checkValidateForm("#form-van-thu-duyet")) {
                    abp.notify.error("Vui lòng nhập đầy đủ thông tin!");
                    vm.saving = false;
                    return;
                } else {
                    xuLyHoSoVanThuService.baoCaoSaiSot(vm.duyetHoSo)
                        .then(function (result) {
                            if (result) {
                                vm.saving = false;
                                abp.notify.success(app.localize('SavedSuccessfully'));
                                $rootScope.$broadcast('refreshGridHoSo', 'ok');
                                vm.show_mode = null;
                            }
                        });
                }
            };
            //*** Function ***
            vm.trustSrc = function (src) {
                return $sce.trustAsResourceUrl(src);
            };
        }
    ]);
})();