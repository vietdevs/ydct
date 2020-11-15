(function () {
    appModule.controller('common.views.profile.changePassword', [
        '$scope', 'appSession', '$uibModalInstance', 'abp.services.app.profile',
        function ($scope, appSession, $uibModalInstance, profileService) {
            var vm = this;

            vm.saving = false;
            vm.passwordInfo = {
                currentPassword: null,
                newPassword: null,
                newPasswordRepeat: null
            };

            vm.passwordComplexitySetting = {};
            vm.passwordComplexityMessages = {};

            var init = function () {
                profileService.getPasswordComplexitySetting()
                    .then(function (result) {
                        vm.passwordComplexitySetting = result.data.setting;
                        vm.passwordComplexityMessages = {
                            minLenght: abp.utils.formatString(app.localize("PasswordComplexity_MinLength_Hint"), vm.passwordComplexitySetting.minLength),
                            maxLenght: abp.utils.formatString(app.localize("PasswordComplexity_MaxLength_Hint"), vm.passwordComplexitySetting.maxLength),
                            useUpperCaseLetters: app.localize("PasswordComplexity_UseUpperCaseLetters_Hint"),
                            useLowerCaseLetters: app.localize("PasswordComplexity_UseLowerCaseLetters_Hint"),
                            useNumbers: app.localize("PasswordComplexity_UseNumbers_Hint"),
                            usePunctuations: app.localize("PasswordComplexity_UsePunctuations_Hint")
                        }
                    });
            };

            vm.save = function () {
                vm.saving = true;
                //validate
                if (vm.passwordInfo.currentPassword == null || vm.passwordInfo.currentPassword == "" || vm.passwordInfo.newPassword == null || vm.passwordInfo.newPassword == "" || vm.passwordInfo.newPasswordRepeat == null || vm.passwordInfo.newPasswordRepeat == "") {
                    abp.notify.error("Mật khẩu không được để trống, xin vui lòng thử lại!");
                    vm.saving = false;
                    return;
                }
                if (vm.passwordInfo.newPassword != vm.passwordInfo.newPasswordRepeat) {
                    abp.notify.error("Mật khẩu không trùng khớp, xin vui lòng thử lại!");
                    vm.saving = false;
                    return;
                }
                const KiTuVaSo = /[a-zA-Z!@#$&*]+[0-9]+|[0-9]+[a-zA-z!@#$&*]+/gm;
                const daucach = /^[ \s]+|[ \s]+$/gm;
                var m;
                if ((m = KiTuVaSo.exec(vm.passwordInfo.newPassword)) == null) {
                    abp.notify.error("Mật khẩu phải chứa cả số và kí tự (hoặc các kí tự đặc biệt ! @ # $ & *)");
                    vm.saving = false;
                    return;
                }
                if ((m = daucach.exec(vm.passwordInfo.newPassword)) != null) {
                    abp.notify.error("Mật khẩu không được chứa dấu cách ở đầu và cuối");
                    vm.saving = false;
                    return;
                }
                if (vm.passwordInfo.newPassword.length > 10 || vm.passwordInfo.newPassword.length < 6) {
                    abp.notify.error("Mật khẩu phải có độ dài tối thiểu 6 kí tự, tối đa 10 kí tự");
                    vm.saving = false;
                    return;
                }
                //(\w +|\d +\d +\w +|\w +\d +)
                profileService.changePassword(vm.passwordInfo)
                    .then(function (result) {
                        abp.notify.info(app.localize('YourPasswordHasChangedSuccessfully'));
                        $uibModalInstance.close();
                    }).catch(function (err) {
                        console.log(err, 'err');
                        //if (err.data.code == 0) {
                        //    abp.notify.error("")
                        //}
                        //abp.notify.error(err.data.message);
                    })
                    .finally(function () {
                        vm.saving = false;
                    });
            };

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };

            init();
        }
    ]);
})();