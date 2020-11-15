(function () {
    appModule.factory('appSession', ['$linq',
        function ($linq) {
            var _session = {
                user: null,
                tenant: null
            };
            abp.services.app.session.getCurrentLoginInformations({ async: false }).done(function (result) {
                _session.user = result.user;
                _session.tenant = result.tenant;

                //Load data khi user là doanh nghiệp
                if (_session.user && _session.user.doanhNghiepId != null && _session.user.doanhNghiepId > 0) {
                    abp.services.app.doanhNghiep.getDoanhNghiepByCurrentUser({ async: false }).done(function (result) {
                        _session.doanhNghiepInfo = result;
                    });
                }

                //Load data khi user là cán bộ cục
                if (_session.user && _session.user.phongBanId != null && _session.user.phongBanId > 0) {
                    abp.services.app.phongBan.getPhongBanByCurrentUser({ async: false }).done(function (result) {
                        _session.phongBan = result;
                    });
                }
            });

            //Load dữ liệu danh mục
            {
                _session.get_quocgia = function () {
                    _session.quocgia = app.sessionStorage.get("hcc_quocgias");
                    if (_session.quocgia === null) {
                        abp.services.app.quocGia.getAllToDDL({ async: false }).done(function (result) {
                            angular.forEach(result, function (it) {
                                it.khongDau = app.removeDau(it.name);
                            });
                            _session.quocgia = result;
                            app.sessionStorage.set("hcc_quocgias", result);
                        });
                    }
                    return _session.quocgia;
                };

                _session.get_tinh = function () {
                    _session.tinh = app.sessionStorage.get("hcc_tinhs");
                    if (_session.tinh === null) {
                        abp.services.app.tinh.getAll({ async: false }).done(function (result) {
                            _session.tinh = result;
                            app.sessionStorage.set("hcc_tinhs", result);
                        });
                    }
                    return _session.tinh;
                };

                _session.get_huyen = function () {
                    _session.huyen = app.sessionStorage.get("hcc_huyens");
                    if (_session.huyen === null) {
                        abp.services.app.huyen.getAll({ async: false }).done(function (result) {
                            _session.huyen = result;
                            app.sessionStorage.set("hcc_huyens", result);
                        });
                    }
                    return _session.huyen;
                };

                _session.get_xa = function () {
                    _session.xa = app.sessionStorage.get("hcc_xas");
                    if (_session.xa === null) {
                        abp.services.app.xa.getAll({ async: false }).done(function (result) {
                            _session.xa = result;
                            app.sessionStorage.set("hcc_xas", result);
                        });
                    }
                    return _session.xa;
                };

                //get_level
                _session.get_level = function () {
                    _session.level = app.sessionStorage.get("arv_levels");
                    if (_session.level == null) {
                        abp.services.app.commonLookup.getRoleLevel({ async: false }).done(function (result) {
                            _session.level = result;
                            app.sessionStorage.set("arv_levels", result);
                        });
                    }
                    return _session.level;
                };

                _session.get_loaihoso = function () {
                    _session.loaihoso = app.sessionStorage.get("hcc_loaihosos");
                    if (_session.loaihoso === null) {
                        abp.services.app.loaiHoSo.getAllToDDL({ async: false }).done(function (result) {
                            _session.loaihoso = result;
                            app.sessionStorage.set("hcc_loaihosos", result);
                        });
                    }
                    return _session.loaihoso;
                };

                //Quy trinh tham dinh
                _session.get_quytrinhthamdinh = function () {
                    _session.quytrinhthamdinh = app.sessionStorage.get("hcc_quitrinhs");
                    if (_session.quytrinhthamdinh === null) {
                        abp.services.app.commonLookup.getQuyTrinhThamDinh({ async: false }).done(function (result) {
                            _session.quytrinhthamdinh = result;
                        });
                    }
                    return _session.quytrinhthamdinh;
                };

                _session.get_loaihinh = function () {
                    _session.loaihinh = app.sessionStorage.get("hcc_loaihinhs");
                    if (_session.loaihinh === null) {
                        abp.services.app.loaiHinhDoanhNghiep.getAll({ async: false }).done(function (result) {
                            _session.loaihinh = result;
                            app.sessionStorage.set("hcc_loaihinhs", result);
                        });
                    }
                    return _session.loaihinh;
                };

                _session.get_chucvu = function () {
                    _session.chucvu = app.sessionStorage.get("hcc_chucvus");
                    if (_session.chucvu === null) {
                        abp.services.app.chucVu.getAllToDDL({ async: false }).done(function (result) {
                            _session.chucvu = result;
                            app.sessionStorage.set("hcc_chucvus", result);
                        });
                    }
                    return _session.chucvu;
                };

                //Dữ liệu khác
                _session.get_chucnanghethong = function () {
                    _session.chucnanghethong = app.sessionStorage.get("hcc_chucnangs");
                    if (_session.chucnanghethong === null) {
                        abp.services.app.thuTucCommon.getListFormFunction({ async: false }).done(function (result) {
                            _session.chucnanghethong = result;
                            app.sessionStorage.set("hcc_chucnangs", result);
                        });
                    }
                    return _session.chucnanghethong;
                };

                _session.get_tenantchicuc = function () {
                    _session.tenantchicuc = app.sessionStorage.get("hcc_tenantchicucs");
                    if (_session.tenantchicuc === null) {
                        abp.services.app.customTennant.getAllTenantChiCuc({ async: false }).done(function (result) {
                            _session.tenantchicuc = result;
                            app.sessionStorage.set("hcc_tenantchicucs", result);
                        });
                    }
                    return _session.tenantchicuc;
                };
            }

            if (_session.tenant && _session.tenant.customCssId) {
                $('head').append('<link id="TenantCustomCss" href="' + abp.appPath + 'TenantCustomization/GetCustomCss?id=' + _session.tenant.customCssId + '" rel="stylesheet"/>');
            }

            _session.payment = {
                keyPay: {
                    '00': 'Thành công'
                    , '01': 'Đại lý không tồn tại trong hệ thống'
                    , '02': 'Chuỗi mã hóa không hợp lệ'
                    , '03': 'Mã giao dịch đại lý không hợp lệ'
                    , '04': 'Không tìm thấy giao dịch trong hệ thống'
                    , '05': 'Mã dịch vụ không hợp lệ'
                    , '06': 'Lỗi xác nhận giao dịch: giao dịch đã được xác nhận(thành công / không thành công trước đó và không thể xác nhận lại)'
                    , '07': 'Mã quốc gia không hợp lệ'
                    , '08': 'Lỗi timeout xảy ra do không nhận được thông điệp trả về từ Ngân Hàng'
                    , '09': 'Mô tả đơn hàng không hợp lệ'
                    , '10': 'Mã đơn hàng không hợp lệ'
                    , '11': 'Số tiền không hợp lệ'
                    , '12': 'Phí vận chuyển không hợp lệ'
                    , '13': 'Thuế không hợp lệ'
                    , '14': 'Đại lý chưa được cấu hình phí'
                    , '15': 'Sai mã Ngân hàng'
                    , '16': 'Số tiền thanh toán của Đại lý không nằm trong khoảng cho phép'
                    , '17': 'Tài khoản không đủ tiền'
                    , '18': 'Khách hàng nhấn Hủy giao dịch trên giao diện Payment'
                    , '19': 'Thời gian thanh toán không hợp lệ'
                    , '20': 'Kiểu nhận mã OTP không hợp lệ'
                    , '21': 'Mã OTP sai'
                    , '25': 'Nhập sai thông tin chủ thẻ lần 1'
                    , '26': 'Nhập sai thông tin chủ thẻ lần 2'
                    , '27': 'Nhập sai thông tin chủ thẻ lần 3 - Thanh toán không thành công'
                    , '30': 'Phiên bản không hợp lệ'
                    , '31': 'Mã lệnh không hợp lệ'
                    , '32': 'Loại tiền tệ không hợp lệ'
                    , '33': 'Ngôn ngữ không hợp lệ'
                    , '34': 'Thông tin thêm(desc 1) không hợp lệ'
                    , '35': 'Thông tin thêm(desc 2) không hợp lệ'
                    , '36': 'Thông tin thêm(desc 3) không hợp lệ'
                    , '37': 'Thông tin thêm(desc 4) không hợp lệ'
                    , '38': 'Thông tin thêm(desc 5) không hợp lệ'
                    , '39': 'Chuỗi trả về - Return URL không hợp lệ'
                    , '40': 'Loại thẻ không hợp lệ'
                    , '41': 'Thẻ nghi vấn(thẻ đánh mất, hot card)'
                    , '54': 'Thẻ hết hạn'
                    , '57': 'Chưa đăng ký dịch vụ thanh toán trực tuyến'
                    , '61': 'Quá hạn mức giao dịch trong ngày'
                    , '62': 'Thẻ bị khóa'
                    , '65': 'Quá hạn mức 1 lần giao dịch'
                    , '97': 'Ngân hàng chưa sẵn sàng'
                    , '98': 'Giao dịch không hợp lệ'
                    , '99': 'Lỗi hệ thống'
                }
            };

            return _session;
        }
    ]);
})();