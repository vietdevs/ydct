(function () {
    appModule.controller('quanlyhoso.thutuc37.views.dangkyhoso.index', [
        '$stateParams', '$rootScope', '$uibModal', '$filter', 'abp.services.app.xuLyHoSoDoanhNghiep37',
        'FileUploader', 'baseService', 'appSession', 'quanlyhoso.thutuc37.services.appChuKySo', 'abp.services.app.loaiHoSo',
        function ($stateParams, $rootScope, $uibModal, $filter, xuLyHoSoDoanhNghiepService,
            fileUploader, baseService, appSession, appChuKySo, loaiHoSoService) {
            abp.ui.clearBusy();
            // -----------------------variable-------------------------//
            var vm = this;
            vm.now = new Date();
            vm.currentTab = 0;
            console.log(appSession, 'appSessionappSession');
            vm.arrLoaiHoSo = [];
            vm.phamViHoatDongIdArr = [];
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
            vm.LOAI_TAI_LIEU_DINH_KEM = app.TT37_LOAI_TAI_LIEU_DINH_KEM;
            vm.taiLieu = [];
            vm.taiLieuDefaultInit = [
                {
                    index: 1,
                    tenTep: null,
                    moTaTep: 'Bản sao văn bằng chuyên môn',
                    loaiTepDinhKem: vm.LOAI_TAI_LIEU_DINH_KEM.VAN_BANG_CHUYEN_MON,
                    duongDanTep: null,
                    uploadFileId: null
                },
                {
                    index: 2,
                    tenTep: null,
                    moTaTep: 'Giấy xác nhận quá trình thực hành',
                    loaiTepDinhKem: vm.LOAI_TAI_LIEU_DINH_KEM.QUA_TRINH_THUC_HANH,
                    duongDanTep: null,
                    uploadFileId: null
                },
                {
                    index: 3,
                    tenTep: null,
                    moTaTep: 'Giấy chứng nhận đủ điều kiện sức khỏe',
                    loaiTepDinhKem: vm.LOAI_TAI_LIEU_DINH_KEM.DU_DIEU_KIEN_SUC_KHOE,
                    duongDanTep: null,
                    uploadFileId: null
                },
                {
                    index: 4,
                    tenTep: null,
                    moTaTep: 'Phiếu lý lịch tư pháp',
                    loaiTepDinhKem: vm.LOAI_TAI_LIEU_DINH_KEM.LY_LICH_TU_PHAP,
                    duongDanTep: null,
                    uploadFileId: null
                },
                {
                    index: 5,
                    tenTep: null,
                    moTaTep: 'Ảnh 4x6',
                    loaiTepDinhKem: vm.LOAI_TAI_LIEU_DINH_KEM.ANH_4X6,
                    duongDanTep: null,
                    uploadFileId: null
                }
            ];


            // --------------------------control----------------------//

            vm.vanBangOptions = {
                dataSource: {
                    transport: {
                        read: function (options) {
                            xuLyHoSoDoanhNghiepService.getListVanBangToDDL().then(function (result) {
                                options.success(result.data);
                            })
                        }
                    }
                },
                optionLabel: 'Chọn...',
                dataTextField: 'name',
                dataValueField: 'id',
                filter: 'contains',
            }




            // --------------------------function----------------------//

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
                    email: appSession.doanhNghiepInfo.doanhNghiep.emailXacNhan,
                    fax: appSession.doanhNghiepInfo.doanhNghiep.fax,
                    maSoThue: appSession.doanhNghiepInfo.doanhNghiep.maSoThue,
                    soDangKy: null,
                    //thong tin ho so
                    tenCoSo: appSession.doanhNghiepInfo.doanhNghiep.tenDoanhNghiep,
                    diaChiCoSo: appSession.doanhNghiepInfo.doanhNghiep.diaChi,
                    trangThaiHoSo: null,
                    ngayDeNghi: vm.now,

                    hoTenNguoiDeNghi: appSession.doanhNghiepInfo.doanhNghiep.tenNguoiDaiDien,
                    diaChiCuTru: appSession.doanhNghiepInfo.doanhNghiep.diaChi,
                    dienThoaiNguoiDeNghi: appSession.doanhNghiepInfo.doanhNghiep.soDienThoai,
                    emailNguoiDeNghi: appSession.doanhNghiepInfo.doanhNghiep.emailXacNhan
                };
                return d;
            };

            var getListPhamViHoatDong = () => {
                xuLyHoSoDoanhNghiepService.getListPhamViToDDL().then(function (result) {
                    vm.listPhamViHoatDong = result.data;
                })
            }
            getListPhamViHoatDong();

            //Function Common
            vm.closeModal = function () {
                vm.show_mode = null;
                vm.listPhamViHoatDong.forEach(function (item) {
                    item.checked = false;
                })
            };

            vm.xemHuongDanDangKyCongBo = function () {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/quanlyhoso/thutuc37/views/1_dangkyhoso/modal/xemVideoHuongDan.cshtml',
                    controller: 'quanlyhoso.thutuc37.views.quanlyhoso.thutuc37.modal.xemVideoHuongDan as vm',
                    backdrop: 'static',
                    size: 'lg'
                });
                modalInstance.result.then(function (result) {
                });
            };

            //----------------------Open AddOrEdit -------------------------//
            vm.addHoSo = function () {
                vm.currentTab = 0;
                var thuTucEnum = 37;
                loaiHoSoService.getLoaiHoSoByThuTucToDDL(thuTucEnum).then(function (result) {
                    if (result.data) {
                        vm.hoso = vm.hosoInit();
                        vm.loaiHoSo = result.data;
                        vm.hoso.loaiHoSoId = vm.loaiHoSo[0].id;
                        vm.show_mode = 'dangkyhoso';
                        vm.taiLieu = angular.copy(vm.taiLieuDefaultInit);
                    }
                });
            };

            vm.viewOrEditHoSo = function (dataItem) {
                xuLyHoSoDoanhNghiepService.getHoSoById(dataItem.id)
                    .then(function (result) {
                        if (result.data.status == true) {
                            vm.hoso = result.data.hoSo;

                            //cập nhật strThuMucHoSo - data cũ
                            if (!vm.hoso.strThuMucHoSo) {
                                vm.hoso.strThuMucHoSo = app.getStrThuMucHoSo();
                            }
                            if (!vm.hoso.loaiHoSoId) {
                                vm.hoso.loaiHoSoId = vm.loaiHoSoIdDefault;
                            }
                            if (result.data.teps && result.data.teps.length > 0) {
                                vm.taiLieu = angular.copy(vm.taiLieuDefaultInit);
                                vm.taiLieu.forEach(function (item) {
                                    result.data.teps.forEach(function (tep) {
                                        if (item.loaiTepDinhKem == tep.loaiTepDinhKem) {
                                            item.tenTep = tep.tenTep;
                                            item.duongDanTep = tep.duongDanTep; 
                                        }
                                    })
                                })
                                result.data.teps.forEach(function (tep) {
                                    if (tep.loaiTepDinhKem == vm.LOAI_TAI_LIEU_DINH_KEM.TAI_LIEU_KHAC) {
                                        vm.taiLieu.push(tep);
                                    }
                                })
                            }
                            else {
                                vm.taiLieu = angular.copy(vm.taiLieuDefaultInit);
                            }

                            if (result.data.phamViHoatDongIdArr && result.data.phamViHoatDongIdArr.length > 0) {
                                vm.listPhamViHoatDong.forEach(function (item) {
                                    result.data.phamViHoatDongIdArr.forEach(function (id) {
                                        if (item.id == id) {
                                            item.checked = true;
                                        }
                                    })
                                })
                            }

                            vm.show_mode = 'dangkyhoso';
                        }

                    }).finally(function () {
                        vm.currentTab = 0;
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
                                objTailieuItem[0].duongDanTep = ajaxResponse.result.filePath;
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
                        loaiTepDinhKem: vm.LOAI_TAI_LIEU_DINH_KEM.TAI_LIEU_KHAC,
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

                vm.xemTaiLieu = function (taiLieu) {
                    appChuKySo.xemFilePDF(taiLieu.duongDanTep, 'Xem tài liệu');
                }


                //Save
                vm.checkValidate = function () {
                    if (!app.checkValidateForm("#FrmThongTinHoSo")) {
                        abp.notify.error("Vui lòng nhập đẩy đủ thông tin");
                        return false;
                    }

                    let uploadedFile = false;
                    for (let i = 0; i < vm.taiLieuDefaultInit.length;i++) {
                        if (vm.taiLieu[i].tenTep) {
                            uploadedFile = true;
                            break;
                        }
                    }
                    if (!uploadedFile) {
                        abp.notify.error("Thông tin tài liệu đính kèm chưa có");
                        return false;
                    }

                    if (vm.taiLieu.length > vm.taiLieuDefaultInit.length) {
                        if (!app.checkValidateForm("#uploadform")) {
                            abp.notify.error("Thông tin tài liệu không được để trống");
                            return false;
                        }
                    }

                    let choosed = 0;
                    vm.phamViHoatDongIdArr = [];
                    vm.listPhamViHoatDong.forEach(function (item) {
                        if (item.checked) {
                            choosed ++;
                            vm.phamViHoatDongIdArr.push(item.id);
                        }
                    })
                    if (choosed < 1) {
                        abp.notify.error("Phạm vi hoạt đông không được để trống");
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

                    console.log(vm.hoso, 'hossssssssssssssssssssssss');

                    vm.hoso.thuTucId = appChuKySo.THU_TUC_ID;
                    vm.hoso.isChiCuc = false;
                    if (vm.hoso.trangThaiHoSo == null || vm.hoso.trangThaiHoSo == 0) {
                        vm.hoso.trangThaiHoSo = trangthailuu;
                    }
                    vm.taiLieu = vm.taiLieu.filter(x => x.tenTep != null);
                    var _data = {
                        Hoso: vm.hoso,
                        Teps: vm.taiLieu,
                        PhamViHoatDongIdArr: vm.phamViHoatDongIdArr
                    };

                    console.log(_data, '_data_data_data_data_data');
                    //return;
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

                vm.nopHoSoBiTraLai = function (dataItem) {
                    xuLyHoSoDoanhNghiepService.nopHoSoTraLai(dataItem.id).then(function (result) {
                        abp.notify.success(app.localize('Nộp hồ sơ thành công'));
                        $rootScope.$broadcast('refreshGridHoSo', vm.filter);
                    })
                }

            }
        }
    ]);
})();