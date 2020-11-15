(function () {
    appModule.controller('quanlyhoso.thutuc99.views.thamxethoso.index', [
        '$rootScope', 'appSession', 'quanlyhoso.thutuc99.services.appChuKySo',
        'abp.services.app.xuLyHoSoChuyenVien99',
        function ($rootScope, appSession, appChuKySo,
            xuLyHoSoChuyenVienService) {
            var vm = this;
            vm.userId = appSession.user.id;
            vm.QUI_TRINH_THAM_DINH = app.QUI_TRINH_THAM_DINH;
            vm.TRANG_THAI_DUYET_NHAP = app.TRANG_THAI_DUYET_NHAP;

            function initThuTuc() {
                $rootScope.currentThuTuc = appChuKySo.THU_TUC_ID;
                $rootScope.currentMaThuTuc = appChuKySo.MA_THU_TUC;
            }
            initThuTuc();

            vm.form = 'tham_dinh_ho_so';
            vm.formId = app.FORM_ID.FORM_THAM_XET_HO_SO;
            vm.show_mode = null; //'tham_dinh_ho_so', 'tham_dinh_ho_so_cv1', 'tham_dinh_ho_so_cv2', 'tong_hop_tham_dinh', 'tham_dinh_bo_sung', 'tham_dinh_lai'

            // control grid
            vm.controlGridHoSo = {};
            //cửa khẩu
            vm.cuaKhau = {};
            vm.cuakhaucheck = false;

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
                formCase: '1', //0: all, 1: hồ sơ thẩm định mới, 2: hồ sơ thẩm định bổ sung, 3: hồ sơ thẩm định lại, 4: hồ sơ đang theo dõi
                formCase2: 0,  //0: getAll(), 1: hồ sơ thẩm định 1, 2: hồ sơ thẩm định 2
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

            vm.closeModal = function () {
                vm.show_mode = null;
            };

            //Common
            vm.showLichSu = false;
            vm.toggleLichSu = function () {
                vm.showLichSu = !vm.showLichSu;
            };

            //Chuyên viên 1 tổng hợp thẩm định
            vm.listTruongPhong = [];
            vm.listPhoPhong = [];
            vm.phanCongConfigLocalStorage = [];

            vm.paramInit = {
                phongBanId: appSession.user.phongBanId,
                userId: appSession.user.id,
                roleLevel: appSession.user.roleLevel
            };
            var init = function () {
                xuLyHoSoChuyenVienService.initThamDinh(vm.paramInit)
                    .then(function (result) {
                        if (result.data) {
                            vm.listNguoiXuLy = {
                                listTruongPhong: result.data.listTruongPhong,
                                listPhoPhong: result.data.listPhoPhong
                            };
                            vm.listTruongPhong = result.data.listTruongPhong;
                            vm.listPhoPhong = result.data.listPhoPhong;
                        }
                    }).finally(function () {
                        //vm.loading = false;
                    });

                var phongBanId = undefined;
                if (appSession.phongBan) {
                    phongBanId = appSession.phongBan.id;
                }
                vm.phanCongConfigLocalStorage = getPhanCongConfig(phongBanId);
            };
            init();

            //--- Thẩm định hồ sơ ---//
            vm.dataItem = {};
            vm.hoSoXuLy = {};
            vm.duyetHoSo = {
                hoSoIsDat_Input: null,
                trangThaiCV_Input: null,
                tieuDeCV_Input: null,
                noiDungCV_Input: null,
                donViKeTiep_Input: null,
                //***
                isChuyenNhanh: null,
                lyDoChuyenNhanh: null
            };

            vm.hoSoXuLy_Reset = function () {
                vm.showLichSu = false;
                vm.hoSoXuLy = {};
                vm.bienBanThamXet = {};
                vm.thoiGianHieuLucJson = {
                    thoiHan: null,
                    donVi: null
                };
                vm.duyetHoSo = {
                    hoSoIsDat_Input: null,
                    trangThaiCV_Input: null,
                    donViKeTiep_Input: null,
                    tieuDeCV_Input: null,
                    noiDungCV_Input: null,
                    thoiGianHieuLucJson_Input: null,
                    //***
                    isChuyenNhanh: null,
                    lyDoChuyenNhanh: null
                };
                vm.cuakhaucheck = false;
            };

            //change hồ sơ đạt or không khi duyệt
            vm.changeDuyetNhap = function (duyetNhap, lyDoNo) {
                if (duyetNhap == 1 || duyetNhap == 2) {
                    vm.duyetHoSo.hoSoIsDat = true;
                }
                else {
                    vm.duyetHoSo.hoSoIsDat = false;
                    if (lyDoNo) {
                        vm.duyetHoSo.noiDungCV = angular.copy(lyDoNo);
                    }
                }
            };

            function getInputThamDinh() {
                var _bienban = angular.copy(vm.bienBanThamXet);

                //Xử lý copy thẩm xét
                if (vm.hoSoXuLy && vm.hoSoXuLy.quiTrinh == vm.QUI_TRINH_THAM_DINH.QT_2_CHUYEN_VIEN) {
                    if (vm.hoSoXuLy.chuyenVienThuLyId == appSession.user.id) {
                        var _ret = angular.copy(vm.hoSoXuLy.bienBanThamXet_ChuyenVienPhoiHop);
                    }
                    else if (vm.hoSoXuLy.chuyenVienPhoiHopId == appSession.user.id) {

                        var _ret = angular.copy(vm.hoSoXuLy.bienBanThamXet_ChuyenVienThuLy);

                        if (_bienban.arrNoiDungThamXet && _bienban.arrNoiDungThamXet.length > 0) {
                            var noiDungThamXet = _bienban.arrNoiDungThamXet[0];
                            _bienban.isCopyThamXet = angular.copy(noiDungThamXet.isCopyThamXet);
                            if (_bienban.isCopyThamXet == true) {
                                _bienban.arrNoiDungThamXet = angular.copy(_ret.arrNoiDungThamXet);
                                _bienban.copyThamXetId = angular.copy(_ret.id);

                                //Gán lại trạng thái copy
                                _bienban.arrNoiDungThamXet.forEach(function (_item) {
                                    _item.isCopyThamXet = angular.copy(_bienban.isCopyThamXet);
                                });
                            }
                        }
                    }
                }

                _bienban.noiDungThamXetJson = JSON.stringify(_bienban.arrNoiDungThamXet);

                if (_bienban.arrNoiDungThamXet && _bienban.arrNoiDungThamXet.length > 0) {
                    var noiDungThamXet = _bienban.arrNoiDungThamXet[0];
                    if (noiDungThamXet.TrangThaiDuyetNhap == vm.TRANG_THAI_DUYET_NHAP.KHONG_DUYET) {
                        _bienban.isThamXetDat = false;
                    } else {
                        _bienban.isThamXetDat = true;
                    }
                }

                var param = angular.copy(vm.hoSoXuLy);
                param.hoSoXuLyId = param.id;
                param.bienBanThamXet = angular.copy(_bienban);

                return param;
            }

            vm.taoCongVanDong = function () {
                var _bienBanThamDinh = getInputThamDinh();
                var _noiDungCV = "";

                if (vm.hoSoXuLy.quiTrinh == vm.QUI_TRINH_THAM_DINH.QT_1_CHUYEN_VIEN) {
                    if (_bienBanThamDinh.bienBanThamXet && _bienBanThamDinh.bienBanThamXet.arrNoiDungThamXet) {
                        var item = _bienBanThamDinh.bienBanThamXet.arrNoiDungThamXet[0];
                        if (item && item.TrangThaiDuyetNhap == vm.TRANG_THAI_DUYET_NHAP.KHONG_DUYET) {
                            _noiDungCV += item.lyDoKhongDuyet;
                        }
                    }
                }
                else if (vm.hoSoXuLy.quiTrinh == vm.QUI_TRINH_THAM_DINH.QT_2_CHUYEN_VIEN) {
                    var td_thuly = {};
                    var td_phoihop = {};

                    if (_bienBanThamDinh.bienBanThamXet_ChuyenVienThuLy && _bienBanThamDinh.bienBanThamXet_ChuyenVienThuLy.arrNoiDungThamXet) {
                        td_thuly = _bienBanThamDinh.bienBanThamXet_ChuyenVienThuLy.arrNoiDungThamXet[0];
                    }

                    if (_bienBanThamDinh.bienBanThamXet_ChuyenVienPhoiHop && _bienBanThamDinh.bienBanThamXet_ChuyenVienPhoiHop.arrNoiDungThamXet) {
                        td_phoihop = _bienBanThamDinh.bienBanThamXet_ChuyenVienPhoiHop.arrNoiDungThamXet[0];
                    }

                    if (td_thuly && td_thuly.TrangThaiDuyetNhap == vm.TRANG_THAI_DUYET_NHAP.KHONG_DUYET) {
                        _noiDungCV += td_thuly.lyDoKhongDuyet;
                    }

                    if (td_phoihop && td_phoihop.TrangThaiDuyetNhap == vm.TRANG_THAI_DUYET_NHAP.KHONG_DUYET) {
                        _noiDungCV += td_thuly.lyDoKhongDuyet;
                    }
                }

                _noiDungCV = _noiDungCV == null || _noiDungCV == 'null' ? '' : _noiDungCV;
                vm.duyetHoSo.noiDungCV = _noiDungCV;
            };

            //== tham-dinh-moi
            {
                vm.openThamDinhHoSo = function (dataItem) {
                    abp.ui.setBusy();
                    vm.dataItem = dataItem;
                    vm.show_mode = 'tham_dinh_ho_so_2cv';
                };
            }

            //== tong-hop-tham-dinh
            {
                vm.openTongHopThamDinh = function (dataItem) {
                    abp.ui.setBusy();
                    vm.dataItem = dataItem;
                    vm.show_mode = 'tong_hop_tham_dinh';
                };
            }

            //== tham-dinh-lai
            {
                vm.openThamDinhLai = function (dataItem) {
                    vm.dataItem = dataItem;

                    var _id = dataItem.hoSoXuLyId_Active;
                    if (_id > 0) {

                        //RESET-INFO
                        vm.hoSoXuLy_Reset();

                        var params = {
                            hoSoXuLyId: _id,
                            hoSoId: dataItem.id
                        };
                        xuLyHoSoChuyenVienService.loadThamDinhLai(params)
                            .then(function (result) {

                                if (result.data) {

                                    vm.hoSoXuLy = result.data.hoSoXuLy;

                                    if (result.data.thoiGianHieuLuc) {
                                        vm.thoiGianHieuLucJson = result.data.thoiGianHieuLuc;
                                    }
                                    //Người duyệt
                                    vm.nguoiDuyet = result.data.nguoiDuyet;
                                    //cửa khẩu
                                    vm.cuaKhau = result.data.cuaKhau;
                                    for (var i = 0; i < vm.cuaKhau.length; i++) {
                                        if (vm.cuaKhau[i].tenCuaKhauNuocNgoai == null || vm.cuaKhau[i].tenCuaKhauNuocNgoai == "") {
                                            vm.cuakhaucheck = true;
                                            break;
                                        }
                                    }

                                    if (result.data.bienBanThamXet) {
                                        vm.bienBanThamXet = result.data.bienBanThamXet;
                                        vm.bienBanThamXet.arrNoiDungThamXet = [];
                                        if (vm.bienBanThamXet.noiDungThamXetJson) {
                                            vm.bienBanThamXet.arrNoiDungThamXet = JSON.parse(vm.bienBanThamXet.noiDungThamXetJson);
                                        }
                                    }

                                    //View Biên Bản Thẩm Định
                                    {
                                        if (result.data.bienBanThamXet_ChuyenVienThuLy) {
                                            var _bienBanThamXetThuLy = result.data.bienBanThamXet_ChuyenVienThuLy;
                                            _bienBanThamXetThuLy.arrNoiDungThamXet = [];
                                            if (_bienBanThamXetThuLy.noiDungThamXetJson) {
                                                _bienBanThamXetThuLy.arrNoiDungThamXet = JSON.parse(_bienBanThamXetThuLy.noiDungThamXetJson);
                                            }
                                            vm.hoSoXuLy.bienBanThamXet_ChuyenVienThuLy = _bienBanThamXetThuLy;
                                        }
                                        if (result.data.bienBanThamXet_ChuyenVienPhoiHop) {
                                            var _bienBanThamXetPhoiHop = result.data.bienBanThamXet_ChuyenVienPhoiHop;
                                            _bienBanThamXetPhoiHop.arrNoiDungThamXet = [];
                                            if (_bienBanThamXetPhoiHop.noiDungThamXetJson) {
                                                _bienBanThamXetPhoiHop.arrNoiDungThamXet = JSON.parse(_bienBanThamXetPhoiHop.noiDungThamXetJson);
                                            }

                                            vm.hoSoXuLy.bienBanThamXet_ChuyenVienPhoiHop = _bienBanThamXetPhoiHop;
                                        }
                                    }

                                    //Duyệt Hồ Sơ
                                    if (result.data.duyetHoSo) {
                                        vm.duyetHoSo = result.data.duyetHoSo;
                                    }

                                    vm.duyetHoSo.hoSoXuLyId = vm.hoSoXuLy.id;
                                    vm.duyetHoSo.truongPhongId = vm.hoSoXuLy.truongPhongId;
                                    vm.duyetHoSo.hoSoIsDat = vm.hoSoXuLy.hoSoIsDat;
                                    if (!vm.duyetHoSo.noiDungCV && !vm.hoSoXuLy.hoSoIsDat) {
                                        vm.duyetHoSo.noiDungCV = angular.copy(vm.hoSoXuLy.noiDungCV);
                                    }
                                    ////Get phoPhongId trong localStorage
                                    //var _config = vm.phanCongConfigLocalStorage.find(function (item) {
                                    //    return item.chuyenVienThuLyId == appSession.user.id;
                                    //});
                                    //if (_config && _config.phoPhongId) {
                                    //    vm.duyetHoSo.phoPhongId = _config.phoPhongId;
                                    //}
                                    vm.show_mode = 'tham_dinh_lai';
                                }
                            }).finally(function () {
                                //vm.loading = false;
                            });
                    }
                };

                vm.saveThamDinhLai = function () {
                    var _req = getInputThamDinh();
                    //Xử lý lại data C#
                    vm.duyetHoSo.hoSoIsDat_Input = vm.duyetHoSo.hoSoIsDat;
                    vm.duyetHoSo.trangThaiCV_Input = vm.duyetHoSo.trangThaiCV;
                    vm.duyetHoSo.tieuDeCV_Input = vm.duyetHoSo.tieuDeCV;
                    if (vm.duyetHoSo.hoSoIsDat == false) {
                        vm.duyetHoSo.noiDungCV_Input = vm.duyetHoSo.noiDungCV;
                    } else {
                        vm.duyetHoSo.noiDungCV_Input = null;
                    }
                    vm.duyetHoSo.donViKeTiep_Input = vm.duyetHoSo.donViKeTiep;
                    _req = $.extend(angular.copy(_req), angular.copy(vm.duyetHoSo));
                    _req.id = vm.hoSoXuLy.id;
                    _req.thoiGianHieuLucJson_Input = vm.thoiGianHieuLucJson;
                    xuLyHoSoChuyenVienService.thamDinhLai_Luu(_req)
                        .then(function (result) {
                            abp.notify.info(app.localize('SavedSuccessfully'));
                            $rootScope.$broadcast('refreshGridHoSo', 'ok');
                            vm.show_mode = null;
                        }).finally(function () {
                            //vm.loading = false;
                        });
                };

                vm.chuyenThamDinhLai = function () {
                    if (app.checkValidateForm("#tham-dinh-lai-ho-so")) {
                        if (vm.duyetHoSo.hoSoIsDat == true) {
                            vm.duyetHoSo.noiDungCV = null;
                        }

                        //Validate
                        var flag = true;
                        if (vm.duyetHoSo.hoSoIsDat == null) {
                            flag = false;
                        }
                        if (vm.duyetHoSo.hoSoIsDat == false && (vm.duyetHoSo.noiDungCV == null || vm.duyetHoSo.noiDungCV == "")) {
                            flag = false;
                        }
                        if (vm.duyetHoSo.isChuyenNhanh == null) {
                            abp.notify.error("Mời bạn chọn người gửi hồ sơ");
                            return flag;
                        }
                        if (vm.duyetHoSo.isChuyenNhanh == true && (vm.duyetHoSo.lyDoChuyenNhanh == null || vm.duyetHoSo.lyDoChuyenNhanh == "")) {
                            flag = false;
                        }
                        if (vm.duyetHoSo.isChuyenNhanh == false && !(vm.duyetHoSo.phoPhongId)) {
                            flag = false;
                        }
                        if (!flag) {
                            abp.notify.error('Mời nhập dữ liệu');
                            return flag;
                        } else {

                            var _req = getInputThamDinh();
                            //Xử lý lại data C#
                            vm.duyetHoSo.hoSoIsDat_Input = vm.duyetHoSo.hoSoIsDat;
                            vm.duyetHoSo.trangThaiCV_Input = vm.duyetHoSo.trangThaiCV;
                            vm.duyetHoSo.tieuDeCV_Input = vm.duyetHoSo.tieuDeCV;
                            if (vm.duyetHoSo.hoSoIsDat == false) {
                                vm.duyetHoSo.noiDungCV_Input = vm.duyetHoSo.noiDungCV;
                            } else {
                                vm.duyetHoSo.noiDungCV_Input = null;
                            }
                            vm.duyetHoSo.donViKeTiep_Input = vm.duyetHoSo.donViKeTiep;

                            _req = $.extend(angular.copy(_req), angular.copy(vm.duyetHoSo));

                            _req.id = vm.hoSoXuLy.id;
                            _req.thoiGianHieuLucJson_Input = vm.thoiGianHieuLucJson;
                            xuLyHoSoChuyenVienService.thamDinhLai_Chuyen(_req)
                                .then(function (result) {
                                    if (result) {
                                        abp.notify.info(app.localize('SavedSuccessfully'));
                                        $rootScope.$broadcast('refreshGridHoSo', 'ok');
                                        vm.show_mode = null;
                                    }
                                }).finally(function () {
                                    //vm.loading = false;
                                });
                        }
                    } else {
                        abp.notify.error("Vui lòng nhập đầy đủ thông tin!");
                    }
                };
            }

            //== tham-dinh-bo-sung
            {
                vm.openThamDinhBoSung = function (dataItem) {
                    vm.dataItem = dataItem;
                    var _id = dataItem.hoSoXuLyId_Active;
                    if (_id > 0) {

                        //RESET-INFO
                        vm.hoSoXuLy_Reset();

                        var params = {
                            hoSoXuLyId: _id,
                            hoSoId: dataItem.id
                        };
                        xuLyHoSoChuyenVienService.loadThamDinhBoSung(params)
                            .then(function (result) {

                                if (result.data) {

                                    vm.hoSoXuLy = result.data.hoSoXuLy;
                                    vm.thoiGianHieuLucJson = result.data.thoiGianHieuLuc;

                                    //Người duyệt
                                    vm.nguoiDuyet = result.data.nguoiDuyet;
                                    //cửa khẩu
                                    vm.cuaKhau = result.data.cuaKhau;
                                    for (var i = 0; i < vm.cuaKhau.length; i++) {
                                        if (vm.cuaKhau[i].tenCuaKhauNuocNgoai == null || vm.cuaKhau[i].tenCuaKhauNuocNgoai == "") {
                                            vm.cuakhaucheck = true;
                                            break;
                                        }
                                    }

                                    if (result.data.bienBanThamXet) {
                                        vm.bienBanThamXet = result.data.bienBanThamXet;
                                        vm.bienBanThamXet.arrNoiDungThamXet = [];
                                        if (vm.bienBanThamXet.noiDungThamXetJson) {
                                            vm.bienBanThamXet.arrNoiDungThamXet = JSON.parse(vm.bienBanThamXet.noiDungThamXetJson);
                                        }
                                    }

                                    //View Biên Bản Thẩm Định
                                    {
                                        if (result.data.bienBanThamXet_ChuyenVienThuLy) {
                                            var _bienBanThamXetThuLy = result.data.bienBanThamXet_ChuyenVienThuLy;
                                            _bienBanThamXetThuLy.arrNoiDungThamXet = [];
                                            if (_bienBanThamXetThuLy.noiDungThamXetJson) {
                                                _bienBanThamXetThuLy.arrNoiDungThamXet = JSON.parse(_bienBanThamXetThuLy.noiDungThamXetJson);
                                            }
                                            vm.hoSoXuLy.bienBanThamXet_ChuyenVienThuLy = _bienBanThamXetThuLy;
                                        }
                                        if (result.data.bienBanThamXet_ChuyenVienPhoiHop) {
                                            var _bienBanThamXetPhoiHop = result.data.bienBanThamXet_ChuyenVienPhoiHop;
                                            _bienBanThamXetPhoiHop.arrNoiDungThamXet = [];
                                            if (_bienBanThamXetPhoiHop.noiDungThamXetJson) {
                                                _bienBanThamXetPhoiHop.arrNoiDungThamXet = JSON.parse(_bienBanThamXetPhoiHop.noiDungThamXetJson);
                                            }

                                            vm.hoSoXuLy.bienBanThamXet_ChuyenVienPhoiHop = _bienBanThamXetPhoiHop;
                                        }
                                    }

                                    //Duyệt Hồ Sơ
                                    if (result.data.duyetHoSo) {
                                        vm.duyetHoSo = result.data.duyetHoSo;
                                    }

                                    vm.duyetHoSo.hoSoXuLyId = vm.hoSoXuLy.id;
                                    vm.duyetHoSo.truongPhongId = vm.hoSoXuLy.truongPhongId;

                                    vm.show_mode = 'tham_dinh_bo_sung';
                                }
                            }).finally(function () {
                                //vm.loading = false;
                            });
                    }
                };

                vm.saveThamDinhBoSung = function () {
                    var _req = getInputThamDinh();
                    //Xử lý lại data C#
                    vm.duyetHoSo.hoSoIsDat_Input = vm.duyetHoSo.hoSoIsDat;
                    vm.duyetHoSo.trangThaiCV_Input = vm.duyetHoSo.trangThaiCV;
                    vm.duyetHoSo.tieuDeCV_Input = vm.duyetHoSo.tieuDeCV;
                    if (vm.duyetHoSo.hoSoIsDat == false) {
                        vm.duyetHoSo.noiDungCV_Input = vm.duyetHoSo.noiDungCV;
                    } else {
                        vm.duyetHoSo.noiDungCV_Input = null;
                    }
                    vm.duyetHoSo.donViKeTiep_Input = vm.duyetHoSo.donViKeTiep;

                    _req = $.extend(angular.copy(_req), angular.copy(vm.duyetHoSo));

                    _req.id = vm.hoSoXuLy.id;

                    _req.thoiGianHieuLucJson_Input = vm.thoiGianHieuLucJson;
                    xuLyHoSoChuyenVienService.thamDinhBoSung_Luu(_req)
                        .then(function (result) {

                            abp.notify.info(app.localize('SavedSuccessfully'));
                            $rootScope.$broadcast('refreshGridHoSo', 'ok');
                            vm.show_mode = null;

                        }).finally(function () {
                            //vm.loading = false;
                        });
                };

                vm.chuyenThamDinhBoSung = function () {

                    if (vm.duyetHoSo.hoSoIsDat == true) {
                        vm.duyetHoSo.noiDungCV = null;
                    }

                    //Validate
                    var flag = true;
                    if (vm.duyetHoSo.hoSoIsDat == null) {
                        flag = false;
                    }
                    if (vm.duyetHoSo.hoSoIsDat == false && (vm.duyetHoSo.noiDungCV == null || vm.duyetHoSo.noiDungCV == "")) {
                        flag = false;
                    }
                    if (vm.duyetHoSo.isChuyenNhanh == null) {
                        abp.notify.error("Mời bạn chọn người gửi hồ sơ");
                        return flag;
                    }
                    if (vm.duyetHoSo.isChuyenNhanh == true && (vm.duyetHoSo.lyDoChuyenNhanh == null || vm.duyetHoSo.lyDoChuyenNhanh == "")) {
                        flag = false;
                    }
                    if (vm.duyetHoSo.isChuyenNhanh == false && !(vm.duyetHoSo.phoPhongId)) {
                        flag = false;
                    }
                    if (!flag) {
                        abp.notify.error('Mời nhập dữ liệu');
                        return flag;
                    } else {

                        var _req = getInputThamDinh();
                        //Xử lý lại data C#
                        vm.duyetHoSo.hoSoIsDat_Input = vm.duyetHoSo.hoSoIsDat;
                        vm.duyetHoSo.trangThaiCV_Input = vm.duyetHoSo.trangThaiCV;
                        vm.duyetHoSo.tieuDeCV_Input = vm.duyetHoSo.tieuDeCV;
                        if (vm.duyetHoSo.hoSoIsDat == false) {
                            vm.duyetHoSo.noiDungCV_Input = vm.duyetHoSo.noiDungCV;
                        } else {
                            vm.duyetHoSo.noiDungCV_Input = null;
                        }
                        vm.duyetHoSo.donViKeTiep_Input = vm.duyetHoSo.donViKeTiep;

                        _req = $.extend(angular.copy(_req), angular.copy(vm.duyetHoSo));

                        _req.id = vm.hoSoXuLy.id;

                        _req.thoiGianHieuLucJson_Input = vm.thoiGianHieuLucJson;
                        xuLyHoSoChuyenVienService.thamDinhBoSung_Chuyen(_req)
                            .then(function (result) {

                                abp.notify.info(app.localize('SavedSuccessfully'));
                                $rootScope.$broadcast('refreshGridHoSo', 'ok');
                                vm.show_mode = null;

                            }).finally(function () {
                                //vm.loading = false;
                            });
                    }
                };
            }

            /*** Function ***/

            function getPhanCongConfig(_phongBanId) {

                var _key = "phanCongConfig" + "_phongBanId_" + _phongBanId;

                var phanCongConfig = app.localStorage.get(_key);
                if (phanCongConfig == null) {
                    return [];
                }
                return phanCongConfig;
            }

            function setPhanCongConfig(_phongBanId) {

                var _key = "phanCongConfig" + "_phongBanId_" + _phongBanId;
                if (vm.phanCongConfigLocalStorage && vm.phanCongConfigLocalStorage.length > 0) {
                    app.localStorage.set(_key, vm.phanCongConfigLocalStorage);
                }
            }
            /*** End Function ***/

            vm.xemTruocCongVan = function () {
                if (vm.dataItem) {
                    var item = {
                        id: vm.dataItem.id,
                        noiDungCV: vm.duyetHoSo.noiDungCV,
                        hoSoIsDat: vm.duyetHoSo.hoSoIsDat,
                        thoiGianHieuLucJson: JSON.stringify(vm.thoiGianHieuLucJson)
                    };
                    if (vm.bienBanThamXet.arrNoiDungThamXet) {
                        var _noiDungThamXetJson = JSON.stringify(vm.bienBanThamXet.arrNoiDungThamXet);
                        item.noiDungThamXetJson = _noiDungThamXetJson;
                    }
                    appChuKySo.xemTruocCongVan(item, function () {

                    });
                }
            };

            vm.xemBienBanThamDinh = function () {
                if (vm.dataItem) {
                    var item = {
                        id: vm.dataItem.id,
                        noiDungThamXetJson: null
                    };
                    if (vm.bienBanThamXet.arrNoiDungThamXet) {
                        var _noiDungThamXetJson = JSON.stringify(vm.bienBanThamXet.arrNoiDungThamXet);
                        item.noiDungThamXetJson = _noiDungThamXetJson;
                    }
                    appChuKySo.xemTruocBienBanThamDinh(item, function () {

                    });
                }
            };

            vm.setLyDoKhongDuyet = function () {
                if (vm.bienBanThamXet.arrNoiDungThamXet.length > 0 && vm.duyetHoSo.hoSoIsDat == false && (vm.duyetHoSo.noiDungCV == null || vm.duyetHoSo.noiDungCV == '')) {
                    vm.duyetHoSo.noiDungCV = vm.bienBanThamXet.arrNoiDungThamXet[0].lyDoKhongDuyet;
                }
            };
        }
    ]);
})();