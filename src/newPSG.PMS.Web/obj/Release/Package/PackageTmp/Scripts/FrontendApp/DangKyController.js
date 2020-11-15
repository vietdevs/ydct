MyApp.controller('ctrl.dangky', [
    '$scope', '$uibModal', '$location', '$sce', '$filter', '$http', '$window', 'FileUploader',
    function ($scope, $uibModal, $location, $sce, $filter, $http, $window, fileUploader) {
        var vm = this;
        vm.saving = false;
        vm.dataFormInit = {
            MaSoThue: "",
            TenDoanhNghiep: "",
            TenTiengAnh: "",
            TenVietTat: "",
            DiaChi: "",
            LoaiHinhDoanhNghiepId: 1,
            SoDienThoai: "",
            Fax: "",
            Website: "",
            TinhId: null,
            HuyenId: null,
            XaId: null,
            EmailDoanhNghiep: "",

            TenDangNhap: "",
            EmailXacNhan: "",
            TenNguoiDaiDien: "",
            ChucVuNguoiDaiDienId: "",
            DienThoaiNguoiDaiDien: "",

            IsActive: true,
            Name: "",
            SurName: "",
            nguoiLienHes: [{ hoTen: '', chucVuId: '', dienThoai: '', email: '' }]
        };
        vm.dataForm = angular.copy(vm.dataFormInit);
        vm.list = {
            Tinh: [],
            Huyen: [],
            Xa: [],
            LoaiHinhDoanhNghiep: [],
            ChucVu: []
        };
        vm.taiLieu = [];

        vm.loaiHinhOption = {
            dataSource: {
                transport: {
                    read: function (options1) {
                        $http.post('/api/services/app/loaihinhdoanhnghiep/getAllToDDL').then(function (response) {
                            vm.list.LoaiHinhDoanhNghiep = response.data.result;
                            options1.success(vm.list.LoaiHinhDoanhNghiep);
                        });
                    }
                }
            },
            dataValueField: "id",
            dataTextField: "name",
            optionLabel: app.localize('Chọn ...'),
            filter: "contains"
        };

        vm.chucVuOption = {
            dataSource: {
                transport: {
                    read: function (options1) {
                        $http.post('/api/services/app/chucvu/getAllToDDL').then(function (response) {
                            vm.list.ChucVu = response.data.result;
                            options1.success(vm.list.ChucVu);
                        });
                    }
                }
            },
            dataValueField: "id",
            dataTextField: "name",
            optionLabel: app.localize('Chọn ...'),
            filter: "contains"
        };

        vm.ddlTinhOptions = {
            dataSource: {
                transport: {
                    read: function (options) {
                        $http.post('/api/services/app/tinh/getAllToDDL').then(function (response) {
                            vm.list.Tinh = response.data.result;
                            options.success(response.data.result);
                        });
                    }
                }
            },
            dataValueField: "id",
            dataTextField: "ten",
            optionLabel: "Chọn Tỉnh",
            filter: "contains",
            dataBound: function () {
            }
        };

        vm.ddlHuyenOptions = {
            dataSource: {
                transport: {
                    read: function (options) {
                        vm.huyenCallBack = options;
                        $http.post('/api/services/app/huyen/getAllToDDL')
                            .then(function (response) {
                                vm.list.Huyen = response.data.result;
                                console.log("select tinh", vm.dataForm.TinhId);
                                options.success(response.data.result);
                            });
                    }
                }
            },
            dataValueField: "id",
            dataTextField: "name",
            optionLabel: "Chọn Huyện",
            filter: "contains",
            dataBound: function () {
            }
        };

        vm.ddlXaOptions = {
            dataSource: {
                transport: {
                    read: function (options) {
                        $http.post('/api/services/app/xa/getAllToDDL').then(function (response) {
                            vm.list.Xa = response.data.result;
                            options.success(response.data.result);
                        });
                    }
                }
            },
            dataValueField: "id",
            dataTextField: "name",
            optionLabel: "Chọn Xã",
            filter: "contains",
            dataBound: function () {
            }
        };
        vm.SubmitForm = function () {
            if (validForm() == false) {
                return;
            }
            vm.saving = true;
            vm.dataForm.SurName = angular.copy(vm.dataForm.TenNguoiDaiDien).split(" ").slice(0, -1).join(' ');
            vm.dataForm.Name = angular.copy(vm.dataForm.TenNguoiDaiDien).split(" ").slice(-1).join(' ');
            if (vm.dataForm.SurName != "" && vm.dataForm.SurName != null && vm.dataForm.SurName != undefined && vm.dataForm.Name != "" && vm.dataForm.Name != null && vm.dataForm.Name != undefined) {
                var input = {
                    DoanhNghiep: vm.dataForm
                };
                $http.post('/api/services/app/doanhnghiep/taoTaiKhoanDoanhNghiep', input)
                    .then(function (response) {
                        mess = response.data.result;
                        console.log(response, "response");
                        switch (mess) {
                            case "ok":
                                swal({
                                    title: "Bạn đã đăng ký thành công!",
                                    text: "Vui lòng kiểm tra email để xác nhận thông tin đăng ký của bạn!",
                                    icon: "success"
                                })
                                    .then(() => {
                                        window.location.href = "/";
                                    });
                                break;
                            case "email_da_co":
                                vm.saving = false;
                                abp.notify.error('', 'Email xác nhận đã được dùng để đăng ký trên hệ thống!');
                                break;
                            case "email_khong_ton_tai":
                                vm.saving = false;
                                abp.notify.error('', 'Email xác nhận không tồn tại!');
                                break;
                            case "ten_dang_nhap_khong_hop_le":
                                vm.saving = false;
                                abp.notify.error('', 'Tên đăng nhập không hợp lệ!');
                                break;
                            case "ten_dang_nhap_da_co":
                                vm.saving = false;
                                abp.notify.error('Hãy liên hệ với quản trị viên để xác minh', 'Mã số thuế đã được đăng ký!');
                                break;
                            case "vai_tro_doanh_nghiep_khong_ton_tai":
                                vm.saving = false;
                                abp.notify.error('Chức năng cần liên hệ với quản trị viên để xử lý', 'Vai trò doanh nghiệp không tồn tại!');
                                break;
                            case "co_loi_xay_ra":
                                vm.saving = false;
                                abp.notify.error('Hãy thử lại hoặc liên hệ với quản trị viên', 'Đã có lỗi xảy ra!');
                                break;
                        }
                    });
            }
            else {
                vm.saving = false;
                abp.notify.error('', 'Hãy nhập cả họ và tên người đại diện!');
            }
        };

        vm.ResetForm = function () {
            vm.dataForm = angular.copy(vm.dataFormInit);
        };
        vm.checkExistEmail = function () {
            if (vm.dataForm.EmailXacNhan != null && vm.dataForm.EmailXacNhan != "" && vm.dataForm.EmailXacNhan != undefined) {
                $http.post('/api/services/app/doanhnghiep/checkExistEmail?email=' + vm.dataForm.EmailXacNhan).then(function (response) {
                    if (response.data.result == true) {
                        abp.notify.error('', 'Email xác nhận đã tồn tại trên hệ thống!');
                    }
                });
            }
        };
        vm.checkExistEmailDoanhNghiep = function () {
            if (vm.dataForm.EmailDoanhNghiep != null && vm.dataForm.EmailDoanhNghiep != "" && vm.dataForm.EmailDoanhNghiep != undefined) {
                $http.post('/api/services/app/doanhnghiep/checkExistEmail?email=' + vm.dataForm.EmailDoanhNghiep)
                    .then(function (response) {
                        if (response.data.result == true) {
                            abp.notify.error('', 'Email doanh nghiệp đã tồn tại trên hệ thống!');
                        }
                    });
            }
        };
        vm.checkExistTenDangNhap = function () {
            if (vm.dataForm.TenDangNhap != null && vm.dataForm.TenDangNhap != "" && vm.dataForm.TenDangNhap != undefined) {
                $http.post('/api/services/app/doanhnghiep/checkExistTenDangNhap?tenDangNhap=' + vm.dataForm.TenDangNhap).then(function (response) {
                    if (response.data.result == true) {
                        abp.notify.error('', 'Tên đăng nhập đã được sử dụng!');
                    }
                });
            }
        };

        vm.checkValidURL = function () {
            if (vm.dataForm.Website != null && vm.dataForm.Website != "" && vm.dataForm.Website != undefined) {
                if (ValidURL(vm.dataForm.Website) == false) {
                    abp.notify.error('', 'Tên website không hợp lệ');
                }
            }
        };

        function validForm() {
            var valid = true;
            var PHONE_REGEXP = /^[0-9]*$/;
            if (vm.dataForm.MaSoThue == null || vm.dataForm.MaSoThue == '') {
                abp.notify.error('', 'Mã số thuế doanh nghiệp không được để trống'); valid = false;
            }
            else if (vm.dataForm.TenDoanhNghiep == null || vm.dataForm.TenDoanhNghiep == '') {
                abp.notify.error('', 'Tên tiếng Việt không được để trống'); valid = false;
            }
            else if (vm.dataForm.DiaChi == null || vm.dataForm.DiaChi == '') {
                abp.notify.error('', 'Địa chỉ chi tiết không được để trống'); valid = false;
            }
            else if ((vm.dataForm.TinhId == null || vm.dataForm.TinhId == '') && vm.dataForm.PhamViEnum == 1) {
                abp.notify.error('', 'Tỉnh/Thành phố không được để trống'); valid = false;
            }
            else if ((vm.dataForm.HuyenId == null || vm.dataForm.HuyenId == '') && vm.dataForm.PhamViEnum == 1) {
                abp.notify.error('', 'Huyện không được để trống'); valid = false;
            }
            else if ((vm.dataForm.XaId == null || vm.dataForm.XaId == '') && vm.dataForm.PhamViEnum == 1) {
                abp.notify.error('', 'Xã không được để trống'); valid = false;
            }
            //else if (vm.dataForm.LoaiHinhDoanhNghiepId == null || vm.dataForm.LoaiHinhDoanhNghiepId == '') {
            //    abp.notify.error('', 'Hình thức không được để trống'); valid = false;
            //}
            else if (vm.dataForm.SoDienThoai == null || vm.dataForm.SoDienThoai == '') {
                abp.notify.error('', 'Điện thoại không được để trống'); valid = false;
            }
            else if (vm.dataForm.EmailDoanhNghiep == null || vm.dataForm.EmailDoanhNghiep == '') {
                abp.notify.error('', 'Email doanh nghiệp trống hoặc không đúng định dạng'); valid = false;
            }
            else if (vm.dataForm.EmailXacNhan == null || vm.dataForm.EmailXacNhan == '') {
                abp.notify.error('', 'Email người đại diện trống hoặc không đúng định dạng'); valid = false;
            }
            else if (vm.dataForm.TenNguoiDaiDien == null || vm.dataForm.TenNguoiDaiDien == '') {
                abp.notify.error('', 'Họ và tên người đại diện không được để trống'); valid = false;
            }
            else if (vm.dataForm.DienThoaiNguoiDaiDien && !PHONE_REGEXP.test(vm.dataForm.DienThoaiNguoiDaiDien)) {
                abp.notify.error('', 'Số điện thoại người đại diện không đúng định dạng'); valid = false;
            }
            return valid;
        };

        vm.changeMaSoThue = function () {
            if (vm.dataForm.MaSoThue != null && vm.dataForm.MaSoThue != "" && vm.dataForm.MaSoThue != undefined) {
                vm.dataForm.TenDangNhap = locdau(vm.dataForm.MaSoThue);
            }
        };

        vm.layEmailDoanhNghiep = function () {
            vm.dataForm.EmailXacNhan = angular.copy(vm.dataForm.EmailDoanhNghiep);
            vm.checkExistEmail();
        };

        vm.xemHuongDan = function () {
            vm.modalInstanceInfo = $uibModal.open({
                animation: true,
                templateUrl: 'frmHuongDanModal.html',
                size: 'lg',
                keyboard: false,
                scope: $scope
            });
            vm.closeModal = function () {
                vm.modalInstanceInfo.dismiss('cancel');
            };
        };

        function locdau(str) {
            str = str.toLowerCase();
            str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
            str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
            str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
            str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
            str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
            str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
            str = str.replace(/đ/g, "d");
            str = str.replace(/!|@|\$|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\'| |\"|\&|\#|\[|\]|~/g, "_");
            str = str.replace(/-+-/g, "_");
            str = str.replace(/-/g, "_");
            str = str.replace(/^\-+|\-+$/g, "");
            return str;
        }

        vm.addNewNguoiLienHe = function () {
            var newItemNo = vm.dataForm.nguoiLienHes.length + 1;
            vm.dataForm.nguoiLienHes.push({ hoTen: '', chucVuId: '', dienThoai: '', email: '' });
        };

        vm.removeNguoiLienHe = function () {
            var lastItem = vm.dataForm.nguoiLienHes.length - 1;
            vm.dataForm.nguoiLienHes.splice(lastItem);
        };

        vm.xemFilePDF = function (pathPDF, title) {
            vm.pathPDF = "/File/GoToViewTaiLieu?url=" + pathPDF;
        };
        //Function
        vm.trustSrc = function (src) {
            return $sce.trustAsResourceUrl(src);
        }
        //Form chi tiet
        {
            vm.createGUID = function () {
                return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                    var r = Math.random() * 16 | 0, v = c == 'x' ? r : (r & 0x3 | 0x8);
                    return v.toString(16);
                });
            }
            vm.nameFolder = vm.createGUID();

            vm.uploader = function (tepItem) {
                var uploader = new fileUploader({
                    url: abp.appPath + 'File/UploadTempFile?maSoThue=' + vm.nameFolder,
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
                                //vm.uploader = null;
                                $("#tailieu_" + tepItem.index).val('');
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
                            objTailieuItem[0].duongDanTep = ajaxResponse.result.pathName;
                            objTailieuItem[0].IsNew = true;
                        }

                        abp.notify.info('Upload thành công');
                    } else {
                        abp.message.error(ajaxResponse.error.message);
                    }
                };
                return uploader;
            };

            vm.themUploadFile = function () {
                vm.taiLieu.push({
                    index: vm.taiLieu.length + 1,
                    tenTep: '',
                    duongDanTep: ''
                });
            };

            vm.xoaUploadFile = function (item) {
                if (vm.taiLieu.length > 0) {
                    var index = vm.taiLieu.indexOf(item);
                    vm.taiLieu.splice(index, 1);
                }
                var idx = 0;
                angular.forEach(vm.taiLieu, function (value, key) {
                    value.index = (++idx);
                });
            };
        }
    }
]);