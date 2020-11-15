(function () {
    appModule.controller('quanlydoanhnghiep.views.danhmucdoanhnghiep.changePasswordModal', [
        '$scope', '$location', '$state', '$uibModalInstance', 'detailData', 'appSession', 'abp.services.app.doanhNghiep', 'abp.services.app.user', 'baseService',
        function ($scope, $location, $state, $uibModalInstance, detailData, appSession, doanhNghiepService, userService, baseService) {
            var vm = this;
            vm.dataForm = angular.copy(detailData);
            vm.confirm = false;
            vm.matkhau = {
                matKhauMoi: "",
                matKhauXacNhan: ""
            }
            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };
            vm.changePassword = function () {
                baseService.ValidatorForm("#changePassForm");
                var frmValidatorForm = angular.element(document.querySelector('#changePassForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (formValidation.isValid()) {
                    if (vm.matkhau.matKhauMoi == vm.matkhau.matKhauXacNhan) {
                        var input = {
                            Password: vm.matkhau.matKhauMoi,
                            MaSoThue: vm.dataForm.maSoThue
                        }
                        doanhNghiepService.changePasswordDoanhNghiep(input).then(function (result) {
                            if (result.data == 'OK') {
                                abp.notify.success(app.localize('SavedSuccessfully'));
                                $uibModalInstance.close();
                            }
                            else {
                                abp.notify.error('Đã có lỗi xảy ra xin vui lòng thử lại sau.');
                                return;
                            }
                        });
                    }
                    else {
                        abp.notify.error('Mật khẩu xác thực chưa đúng !');
                    }
                }
            }
        }
    ]);
})();