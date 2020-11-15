(function () {
    appModule.controller('quanlydoanhnghiep.views.doanhnghiepsuahoso.index', [
        '$scope', '$uibModal', '$interval', '$filter', 'appSession', 'abp.services.app.doanhNghiep', 'baseService', 'FileUploader', '$linq',
        function ($scope, $uibModal, $interval, $filter, appSession, doanhNghiepService, baseService, fileUploader, $linq) {
            var vm = this;
            var today = new Date();
            vm.now = new Date();
            vm.dsdoanhnghiep_loading = false;
            vm.saving = false;
            vm.visibleAdvanceSearch = false;
            console.log(appSession);
            vm.dataForm = {};

            vm.patternRegex = '^(?:[a-zA-Z\s]|[\u0391Z\s]|[\u4E00-\uFFE5]|[\u0e00\u9FFF]|[\u0E00-\u0e7e]|[\u00c0\u0E7F]|[\u00c0-\u1ef9])*$';

            //hoan add filter for directive

            vm.permissions = {
                create: abp.auth.hasPermission('Pages.QuanLyDoanhNghiep.Create'),
                edit: abp.auth.hasPermission('Pages.QuanLyDoanhNghiep.Edit'),
                delete: abp.auth.hasPermission('Pages.QuanLyDoanhNghiep.Delete')
            };

            vm.flags = {
                isActionDN: 1, //[1] open grid; [2] open add; [3] open view
            };
            //-----load combox-------//

            //huongcv----
            vm.tinhOptions = {
                dataSource: appSession.get_tinh(),
                dataValueField: "id",
                dataTextField: "ten",
                optionLabel: "Chọn ...",
                filter: "startswith",
                dataBound: function () {
                }
            };
            vm.huyenOptions = {
                dataSource: appSession.get_huyen(),
                dataValueField: "id",
                dataTextField: "ten",
                optionLabel: "Chọn ...",
                filter: "contains",
                dataBound: function () {
                }
            };
            vm.xaOptions = {
                dataSource: appSession.get_xa(),
                dataValueField: "id",
                dataTextField: "ten",
                optionLabel: "Chọn ...",
                filter: "contains",
                dataBound: function () {
                }
            }
            vm.loaiHinhOptions = {
                dataSource: appSession.get_loaihinh(),
                dataValueField: "id",
                dataTextField: "tenLoaiHinh",
                optionLabel: "Chọn ...",
                filter: "contains",
                dataBound: function () {
                }
            }

            vm.chucVuOptions = {
                dataSource: appSession.get_chucvu(),
                dataValueField: "id",
                dataTextField: "name",
                optionLabel: "Chọn ...",
                filter: "contains",
            }

            //-----Search khach hang-----//
            var init = function () {
                if (appSession.doanhNghiepInfo != null) {
                    vm.dataForm = appSession.doanhNghiepInfo.doanhNghiep;
                }
            }
            init();

            //huongcv -- update thong tin doanh nghiep btn
            vm.submitUpdate = function () {
                vm.dsdoanhnghiep_loading = true;
                baseService.ValidatorForm("#DoanhNghiepCreateOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#DoanhNghiepCreateOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (formValidation.isValid()) {
                    appSession.doanhNghiepInfo.doanhNghiep = vm.dataForm;
                    doanhNghiepService.updateDoanhNghiepInfo(vm.dataForm).then(function (result) {
                        if (result.data == 'ok') {
                            abp.notify.success("Cập nhật thông tin doanh nghiệp thành công");
                            vm.dsdoanhnghiep_loading = false;
                        }
                        else if (result.data == 'thieu_ten') {
                            abp.notify.error("Hãy nhập cả họ và tên người đại diện");
                            vm.dsdoanhnghiep_loading = false;
                        }
                        else if (result.data == 'loi_khong_biet') {
                            abp.notify.error("Đã có lỗi xảy ra. Xin vui lòng liên hệ quản trị viên");
                            vm.dsdoanhnghiep_loading = false;
                        }
                    });
                    console.log(vm.dataForm);
                }
                else {
                    vm.dsdoanhnghiep_loading = false;
                }
            }

            //-----Search ho so-----//

            function locdau(str) {
                str = str.toLowerCase();
                str = str.replace(/à|á|ạ|ả|ã|â|ầ|ấ|ậ|ẩ|ẫ|ă|ằ|ắ|ặ|ẳ|ẵ/g, "a");
                str = str.replace(/è|é|ẹ|ẻ|ẽ|ê|ề|ế|ệ|ể|ễ/g, "e");
                str = str.replace(/ì|í|ị|ỉ|ĩ/g, "i");
                str = str.replace(/ò|ó|ọ|ỏ|õ|ô|ồ|ố|ộ|ổ|ỗ|ơ|ờ|ớ|ợ|ở|ỡ/g, "o");
                str = str.replace(/ù|ú|ụ|ủ|ũ|ư|ừ|ứ|ự|ử|ữ/g, "u");
                str = str.replace(/ỳ|ý|ỵ|ỷ|ỹ/g, "y");
                str = str.replace(/đ/g, "d");
                str = str.replace(/!|@|\$|%|\^|\*|\(|\)|\+|\=|\<|\>|\?|\/|,|\.|\:|\'| |\"|\&|\#|\[|\]|~/g, "-");
                str = str.replace(/-+-/g, "-");
                str = str.replace(/^\-+|\-+$/g, "");
                return str;
            }
        }
    ]);
})();