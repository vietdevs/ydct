(function () {
    appModule.controller('quanlyhoso.thutuc37.views.lanhdaocucduyetthamdinh.index', [
        '$rootScope', '$sce', '$uibModal', 'appSession', 'quanlyhoso.thutuc37.services.appChuKySo', 'abp.services.app.xuLyHoSoLanhDaoCuc37',
        function ($rootScope, $sce, $uibModal, appSession, appChuKySo, xuLyHoSoLanhDaoCucService) {
            var vm = this;
            vm.DON_VI_XU_LY = app.DON_VI_XU_LY;

            function initThuTuc() {
                $rootScope.currentThuTuc = appChuKySo.THU_TUC_ID;
                $rootScope.currentMaThuTuc = appChuKySo.MA_THU_TUC;
            }
            initThuTuc();

            vm.form = 'lanh_dao_cuc_duyet_tham_dinh';
            vm.formId = 374;
            vm.show_mode = null; //'lanh_dao_cuc_duyet'
            vm.saving = false;
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

            //Xủ lý nhiều hồ sơ
            vm.arrCheckbox = [];
            vm.updateArrCheckbox = function (arrCheckbox) {
                vm.arrCheckbox = arrCheckbox;
            };

            //Begin
            vm.yKienTruongPhong = {
                tenTruongPhong: null,
                yKien: null
            };

            vm.hoSoXuLy = {};
            vm.duyetHoSo = {
                hoSoXuLyId: null,
                hoSoIsDat: null,
                trangThaiCV: null,
                donViKeTiep: null,
                noiDungYKien: null
            };

            vm.hoSoXuLy_Reset = function () {
                vm.hoSoXuLy = {};
                vm.duyetHoSo = {
                    hoSoXuLyId: null,
                    hoSoIsDat: null,
                    trangThaiCV: null,
                    donViKeTiep: null,
                    noiDungYKien: null
                };
            };

            vm.dataItem = {};
            vm.openLanhDaoDuyetThamDinh = function (dataItem) {
                vm.dataItem = dataItem;
                var _id = dataItem.hoSoXuLyId_Active;
                if (_id > 0) {

                    //RESET-INFO
                    vm.hoSoXuLy_Reset();

                    var params = {
                        HoSoXuLyId: _id
                    };
                    xuLyHoSoLanhDaoCucService.loadLanhDaoCucDuyet(params)
                        .then(function (result) {

                            if (result.data) {
                                vm.hoSoXuLy = result.data.hoSoXuLy;
                                vm.yKienTruongPhong = result.data.yKienTruongPhong;

                                if (result.data.duyetHoSo) {
                                    vm.duyetHoSo = result.data.duyetHoSo;
                                }
                                vm.duyetHoSo.hoSoXuLyId = vm.hoSoXuLy.id;
                                vm.duyetHoSo.HoSoId = dataItem.id;
                                vm.pathGiayTiepNhan = "/Report37/TemplateCongVan?hoSoId=" + dataItem.id;
                                vm.show_mode = 'lanh_dao_cuc_duyet_tham_dinh';
                            }
                        });
                }
            };

            vm.kyVaChuyenVanThu = function () {
                //Validate
                var flagNull = vm.duyetHoSo.donViKeTiep != null && vm.duyetHoSo.donViKeTiep != undefined;
                var flag = true;
                if (vm.duyetHoSo.donViKeTiep == vm.DON_VI_XU_LY.TRUONG_PHONG) {
                    flag = !(vm.duyetHoSo.yKien == null || vm.duyetHoSo.yKien == "");
                }
                if (!flagNull || !flag) {
                    abp.notify.error('Mời nhập dữ liệu');
                    return flag;
                } else {

                    if (vm.dataItem && vm.dataItem.id > 0) {

                        //Ký số Công Văn Bổ Sung
                        appChuKySo.kySoCongVan(vm.dataItem, function (paramKySo) {
                            var duyetHoSo = {};
                            duyetHoSo.hoSoXuLyId = vm.dataItem.hoSoXuLyId_Active;
                            duyetHoSo.hoSoId = vm.dataItem.id;
                            duyetHoSo.hoSoIsDat = false;
                            vm.duyetHoSo.donViKeTiep = vm.DON_VI_XU_LY.VAN_THU;
                            duyetHoSo.duongDanTepCA = paramKySo.duongDanTep;

                            vm.saving = true;
                            //Chuyển đơn vị xử lý
                            xuLyHoSoLanhDaoCucService.kyVaChuyenVanThuLuongThamDinh(duyetHoSo)
                                .then(function (result) {
                                    abp.notify.info(app.localize('SavedSuccessfully'));

                                    $rootScope.$broadcast('refreshGridHoSo', 'ok');
                                    vm.show_mode = null;

                                    appChuKySo.xemFilePDF(paramKySo.duongDanTep, 'Công văn đã ký số');

                                }).finally(function () {
                                    vm.saving = false;
                                });
                        });

                    }
                }
            };

            vm.chuyenLaiTruongPhong = function () {
                //Validate
                var flagNull = vm.duyetHoSo.donViKeTiep != null;
                var flag = (vm.duyetHoSo.donViKeTiep == vm.DON_VI_XU_LY.TRUONG_PHONG && !(vm.duyetHoSo.noiDungYKien == null || vm.duyetHoSo.noiDungYKien == ""));

                if (!flagNull || !flag) {
                    abp.notify.error('Mời nhập dữ liệu');
                    return;
                } else {
                    vm.saving = true;
                    xuLyHoSoLanhDaoCucService.chuyenLaiTruongPhongThamDinhLai(vm.duyetHoSo)
                        .then(function (result) {
                            if (result.data) {
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

            vm.setKyKien = function () {
                if (vm.duyetHoSo.donViKeTiep == vm.DON_VI_XU_LY.VAN_THU) {
                    vm.duyetHoSo.noiDungYKien = 'Nhất trí với ý kiến của phòng';
                }
                else {
                    vm.duyetHoSo.noiDungYKien = '';
                }
            };

            vm.xemTaiLieuFull = function (dataItem) {
                var mainTab = document.getElementById(dataItem);
                var tabActive = mainTab.getElementsByClassName("active");
                var modalData = {
                    dataHtml: $sce.trustAsHtml(tabActive[1].querySelector("iframe").contentWindow.document.body.innerHTML),
                    title: tabActive[1].querySelector("iframe").title
                };
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/quanlyhoso/thutuc37/directives/modal/fullDocumentModal.cshtml',
                    controller: 'quanlyhoso.thutuc37.directives.modal.fullDocumentModal as vm',
                    backdrop: 'static',
                    size: 'lg',
                    resolve: {
                        modalData: modalData
                    }
                });
            };

        }
    ]);
})();