(function () {
    appModule.controller('quanlyadmin.views.modal.changePasswordModal', [
        '$scope', '$location', '$state', '$uibModalInstance', 'detailData', 'appSession', 'abp.services.app.user', 'baseService',
        function ($scope, $location, $state, $uibModalInstance, detailData, appSession, userService, baseService) {
            var vm = this;
            vm.dataForm = angular.copy(detailData);
            vm.confirm = false;
            vm.saving = false;
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
                        vm.saving = true;
                        var input = {
                            NewPassword: vm.matkhau.matKhauMoi,
                            Id: vm.dataForm.id
                        }
                        userService.changePasswordAdmin(input).then(function (result) {
                            abp.notify.success(app.localize('SavedSuccessfully'));
                            $uibModalInstance.close();
                            vm.saving = false;
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