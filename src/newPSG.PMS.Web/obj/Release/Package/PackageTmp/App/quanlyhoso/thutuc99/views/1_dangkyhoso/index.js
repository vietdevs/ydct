(function () {
    appModule.controller('quanlyhoso.thutuc99.views.dangkyhoso.index', [
        '$stateParams', '$rootScope', '$uibModal', '$filter', 'abp.services.app.xuLyHoSoDoanhNghiep99',
        'FileUploader', 'baseService', 'appSession', 'quanlyhoso.thutuc99.services.appChuKySo',
        function ($stateParams, $rootScope, $uibModal, $filter, xuLyHoSoDoanhNghiepService,
            fileUploader, baseService, appSession, appChuKySo) {

            var vm = this;
            vm.arrLoaiHoSo = [];
            vm.loaiHoSoIdDefault = null;
            function initThuTuc() {
                $rootScope.currentThuTuc = appChuKySo.THU_TUC_ID;
                $rootScope.currentMaThuTuc = appChuKySo.MA_THU_TUC;
                const _arr = appSession.get_loaihoso().filter(item => item.thuTucId == appChuKySo.THU_TUC_ID);
                if (_arr && _arr.length > 0) {
                    vm.arrLoaiHoSo = _arr;
                    vm.loaiHoSoIdDefault = _arr[0].id;
                }
            }
            initThuTuc();

            function checkStatusCodeThanhToanKeyPay() {
                var _statuscode = $stateParams.kp_statuscode;
                if (typeof (_statuscode) != 'undefined') {
                    if (_statuscode == '00') {
                        abp.notify.info("Thanh toán thành công");
                    }
                    else {
                        abp.notify.error("Thanh toán thất bại. Vui lòng thử lại!");
                    }

                }
            }
            checkStatusCodeThanhToanKeyPay();

            vm.saving = false;
            vm.form = 'dang_ky_ho_so';
            vm.formId = app.FORM_ID.FORM_DANG_KY_HO_SO;
            vm.show_mode = null; //'danh sach'
            vm.filter = {
                formId: vm.formId,
                formCase: 1,
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

            //Begin
            vm.opening = false;
            vm.now = new Date();
            vm.taiLieu = [];

            //Variable
            vm.hosoInit = function () {
                var d =
                {
                    //thu muc chua file hoso
                    strThuMucHoSo: app.getStrThuMucHoSo(),
                    //thong tin doanh nghiep
                    doanhNghiepId: appSession.doanhNghiepInfo.doanhNghiep.id,
                    diaChi: appSession.doanhNghiepInfo.doanhNghiep.diaChi,
                    tenDoanhNghiep: appSession.doanhNghiepInfo.doanhNghiep.tenDoanhNghiep,
                    tenNguoiDaiDien: appSession.doanhNghiepInfo.doanhNghiep.tenNguoiDaiDien,
                    tinhId: appSession.doanhNghiepInfo.doanhNghiep.tinhId,
                    huyenId: appSession.doanhNghiepInfo.doanhNghiep.huyenId,
                    xaId: appSession.doanhNghiepInfo.doanhNghiep.xaId,
                    soDienThoai: appSession.doanhNghiepInfo.doanhNghiep.soDienThoai,
                    email: appSession.doanhNghiepInfo.doanhNghiep.emailDoanhNghiep,
                    fax: appSession.doanhNghiepInfo.doanhNghiep.fax,
                    maSoThue: appSession.doanhNghiepInfo.doanhNghiep.maSoThue,
                    soDangKy: null,
                    //thong tin ho so
                    tenCoSo: appSession.doanhNghiepInfo.doanhNghiep.tenDoanhNghiep,
                    diaChiCoSo: appSession.doanhNghiepInfo.doanhNghiep.diaChi,
                    trangThaiHoSo: null,
                    loaiHoSoId: vm.loaiHoSoIdDefault
                };
                return d;
            };

            //Function Common
            vm.closeModal = function () {
                vm.show_mode = null;
            };

            vm.xemHuongDanDangKyCongBo = function () {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/quanlyhoso/thutuc99/views/1_dangkyhoso/modal/xemVideoHuongDan.cshtml',
                    controller: 'quanlyhoso.thutuc99.views.quanlyhoso.thutuc99.modal.xemVideoHuongDan as vm',
                    backdrop: 'static',
                    size: 'lg'
                });
                modalInstance.result.then(function (result) {
                });
            };
            
            //----------------------Open AddOrEdit -------------------------//
            vm.addHoSo = function () {
                vm.show_mode = 'dangkyhoso';
                vm.hoso = vm.hosoInit();
                vm.taiLieu = [];
                vm.currentTab = 1;
            };

            vm.viewOrEditHoSo = function (dataItem) {
                xuLyHoSoDoanhNghiepService.getHoSoById(dataItem.id)
                    .then(function (result) {
                        if (result.data.status == true) {
                            vm.hoso = result.data.hoSo;

                            //cập nhật strThuMucHoSo - data cũ
                            if (!vm.hoso.strThuMucHoSo){
                                vm.hoso.strThuMucHoSo = app.getStrThuMucHoSo();
                            }
                            if (!vm.hoso.loaiHoSoId) {
                                vm.hoso.loaiHoSoId = vm.loaiHoSoIdDefault;
                            }
                            if (result.data.teps && result.data.teps.length > 0) {
                                var i = 1;
                                vm.taiLieu = result.data.teps;
                                vm.taiLieu.forEach(function (tailieu) {
                                    tailieu.index = i++;
                                });
                            }
                            else {
                                vm.taiLieu = [];
                            }
                            vm.show_mode = 'dangkyhoso';
                        }

                    }).finally(function () {
                        vm.currentTab = 1;
                    });
            };
            
            vm.totalFile = 0;
            //----------------------From AddOrEdit -------------------------//
            {
                vm.uploader = function (tepItem) {
                    var _maThuTuc = appChuKySo.MA_THU_TUC;
                    var _maSoThue = vm.hoso.maSoThue;
                    var _strThuMucHoSo = vm.hoso.strThuMucHoSo;
                    var _folderName = "tepdinhkem";

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
                                    $("#tailieu_" + tepItem.code).val('');
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
                            var objTailieuItem = $filter('filter')(vm.taiLieu, { index: tepItem.index }, true);
                            if (objTailieuItem && objTailieuItem[0]) {
                                objTailieuItem[0].tenTep = ajaxResponse.result.fileName;
                                objTailieuItem[0].uploadFileId = ajaxResponse.result.uploadFileId;
                            }

                            abp.notify.info(app.localize('SavedSuccessfully'));
                        } else {
                            abp.message.error(ajaxResponse.error.message);
                        }
                    };
                    return uploader;
                };

                vm.themUploadFile = function () {
                    vm.totalFile = vm.totalFile + 1;
                    vm.taiLieu.push({
                        index: vm.taiLieu.length + 1,
                        isCA: false,
                        code: vm.totalFile
                    });
                };

                vm.xoaUploadFile = function (item) {
                    if (vm.taiLieu.length > 0) {
                        var index = vm.taiLieu.indexOf(item);
                        vm.taiLieu.splice(index, 1);
                    }
                    var idx = 0;
                    angular.forEach(vm.taiLieu, function (value, key) {
                        value.index = ++idx;
                    });
                };

                //Save
                vm.checkValidate = function () {
                    baseService.ValidatorForm('#FrmThongTinToChuc');
                    var frmValidatorForm1 = angular.element(document.querySelector('#FrmThongTinToChuc'));
                    var frmValidator1 = frmValidatorForm1.data('formValidation').validate();
                    if (!(frmValidator1 && frmValidator1.isValid())) {
                        var msgString1 = "Xin vui lòng xem lại mục 'Thông tin tổ chức'!";
                        abp.notify.error(msgString1, "Dữ liệu đang bị trống");
                        return false;
                    }
                    baseService.ValidatorForm('#FrmThongTinHoSo');
                    var frmValidatorForm2 = angular.element(document.querySelector('#FrmThongTinHoSo'));
                    var frmValidator2 = frmValidatorForm2.data('formValidation').validate();
                    if (!(frmValidator2 && frmValidator2.isValid())) {
                        var msgString2 = "Xin vui lòng xem lại mục 'Thông tin về hồ sơ'!";
                        abp.notify.error(msgString2, "Dữ liệu đang bị trống");
                        return false;
                    }
                    var vld = false;
                    if (!app.checkValidateForm("#uploadform"))
                        vld = true;
                    vm.taiLieu.forEach(function (item) {
                        if (!item.tenTep) {
                            vld = true;
                        }
                    });
                    if (vld) {
                        abp.notify.error("Thông tin tài liệu đính kèm chưa có");
                        return false;
                    }
                    return true;
                };

                vm.save = function () {
                    var validated = vm.checkValidate();
                    if (validated) {
                        vm.saveDraft(0);
                    }
                };

                vm.saveDraft = function (trangthailuu) {
                    vm.hoso.thuTucId = appChuKySo.THU_TUC_ID;
                    vm.hoso.isChiCuc = false;
                    if (vm.hoso.trangThaiHoSo == null || vm.hoso.trangThaiHoSo == 0) {
                        vm.hoso.trangThaiHoSo = trangthailuu;
                    }
                    var _data = {
                        Hoso: vm.hoso,
                        Teps: vm.taiLieu
                    };
                    vm.saving = true;
                    xuLyHoSoDoanhNghiepService.createOrUpdateHoSo(_data).then(function (result) {
                        if (result) {
                            abp.notify.success(app.localize('SavedSuccessfully'));
                            $rootScope.$broadcast('refreshGridHoSo', vm.filter);
                            vm.closeModal();
                        }
                    }).finally(function () {
                        vm.saving = false;
                    });
                };
            }
        }
    ]);
})();