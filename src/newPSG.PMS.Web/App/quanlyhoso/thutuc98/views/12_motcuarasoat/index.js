(function () {
    appModule.controller('quanlyhoso.thutuc98.views.motcuarasoat.index', [
        '$sce', '$rootScope', 'appSession', 'quanlyhoso.thutuc98.services.appChuKySo',
        'abp.services.app.xuLyHoSoVanThu98','FileUploader',
        function ($sce, $rootScope, appSession, appChuKySo,
            xuLyHoSoVanThuService, fileUploader) {
            var vm = this;
            function initThuTuc() {
                $rootScope.currentThuTuc = appChuKySo.THU_TUC_ID;
                $rootScope.currentMaThuTuc = appChuKySo.MA_THU_TUC;
            }

            initThuTuc();

            // variable
            vm.taiLieuUpload = {};
            vm.hoSoXuLy = {
                trangThaiXuLy: 1
            };
            vm.form = 'mot_cua_ra_soat';
            vm.formId = 12;
            vm.dangKyHoSoUrl = "";
            vm.lstTaiLieu = [];
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

            vm.uploader = function () {
                var _maThuTuc = appChuKySo.MA_THU_TUC;
                var _maSoThue = vm.dataItem.maSoThue;
                var _strThuMucHoSo = vm.dataItem.strThuTucHoSo;
                var _folderName = "giaychungnhan";

                var uploader = new fileUploader({
                    url: abp.appPath + 'File/UploadTaiLieuHoSo?maThuTuc=' + _maThuTuc + '&maSoThue=' + _maSoThue + '&strThuMucHoSo=' + _strThuMucHoSo + '&folderName=' + _folderName,
                    headers: {
                        "X-XSRF-TOKEN": abp.security.antiForgery.getToken()
                    },
                    queueLimit: 1,
                    autoUpload: true,
                    removeAfterUpload: true,
                    filters: [{
                        name: 'excelFilter',
                        fn: function (item, options) {
                            //File type check
                            var _extension = "." + item.name.split('.').pop().toLowerCase();
                            if (['.pdf'].indexOf(_extension) === -1) {
                                abp.message.error(app.localize('Không đúng định dạng tập tin cho phép (.pdf)'));
                                $("taiLieuUploadUrl").val('');
                                return false;
                            }
                            else {
                                //File size check
                                if (item.size > 15145728) //3MB
                                {
                                    abp.message.error(app.localize('Dung lượng tập tin đính kèm vượt quá giới hạn cho phép (15MB)'));
                                    return false;
                                }
                                return true;
                            }
                        }
                    }]
                });
                uploader.onSuccessItem = function (item, ajaxResponse, status) {
                    if (ajaxResponse.success) {
                        //vm.taiLieuUpload.tenTep = ajaxResponse.result.fileName;
                        vm.taiLieuUpload.uploadFileId = ajaxResponse.result.uploadFileId;
                        vm.taiLieuUpload.tenTep = ajaxResponse.result.fileName;
                        vm.taiLieuUpload.duongDanTep = ajaxResponse.result.filePath;
                        abp.notify.info(app.localize('SavedSuccessfully'));
                    } else {
                        abp.message.error(ajaxResponse.error.message);
                    }
                };
                return uploader;
            };

            vm.xemTaiLieu = function () {
                appChuKySo.xemFilePDF(vm.taiLieuUpload.duongDanTep, 'Xem tài liệu');
            };

            vm.openMotCuaRaSoat = function (dataItem) {
                if (dataItem) {
                    vm.dataItem = dataItem;
                    vm.title = "Rà soát hồ sơ [" + dataItem.maHoSo + "]";
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
                                DuongDanTepCA: vm.taiLieuUpload.duongDanTep,
                                TrangThaiXuLy: vm.hoSoXuLy.trangThaiXuLy,
                                NoiDungYKien: vm.hoSoXuLy.noiDungYKien
                            };
                            abp.ui.setBusy();
                            xuLyHoSoVanThuService.vanThuDuyet_Chuyen(input)
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

            vm.closeModal = function () {
                $rootScope.$broadcast('refreshGridHoSo', 'ok');
                vm.show_mode = null;
                vm.taiLieuUpload = {};
                vm.hoSoXuLy = {
                    trangThaiXuLy: 1
                };
            };

            vm.trustSrc = function (src) {
                return $sce.trustAsResourceUrl(src);
            };
        }
    ]);
})();