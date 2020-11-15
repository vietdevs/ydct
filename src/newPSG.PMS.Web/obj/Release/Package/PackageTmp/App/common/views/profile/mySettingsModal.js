(function () {
    appModule.controller('common.views.profile.mySettingsModal', [
        '$scope', 'appSession', '$uibModalInstance', 'abp.services.app.profile',
        function ($scope, appSession, $uibModalInstance, profileService) {
            var vm = this;

            var initialTimezone = null;

            vm.saving = false;
            vm.user = null;
            vm.canChangeUserName = true;
            vm.showTimezoneSelection = abp.clock.provider.supportsMultipleTimezone;

            vm.save = function () {
                vm.saving = true;
                profileService.updateCurrentUserProfile(vm.user)
                    .then(function (result) {
                        if (result.data == 'ok') {
                            appSession.user.name = vm.user.name;
                            appSession.user.surname = vm.user.surname;
                            appSession.user.userName = vm.user.userName;
                            appSession.user.emailAddress = vm.user.emailAddress;

                            abp.notify.info(app.localize('SavedSuccessfully'));

                            $uibModalInstance.close();

                            if (abp.clock.provider.supportsMultipleTimezone && initialTimezone !== vm.user.timezone) {
                                abp.message.info(app.localize('TimeZoneSettingChangedRefreshPageNotification')).done(function () {
                                    window.location.reload();
                                });
                            }
                        }
                        else if (result.data == 'email_da_co') {
                            abp.notify.error('Email đã có người sử dụng');
                        }
                        else if (result.data = 'ten_dang_nhap_da_co') {
                            abp.notify.error('Tên đăng nhập đã có người sử dụng');
                        }
                    }).finally(function () {
                        vm.saving = false;
                    });
            };

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };

            function init() {
                profileService.getCurrentUserProfileForEdit({
                    id: appSession.user.id
                }).then(function (result) {
                    vm.user = result.data;
                    vm.canChangeUserName = vm.user.userName != app.consts.userManagement.defaultAdminUserName;
                    initialTimezone = vm.user.timezone;
                });
            }

            init();
        }
    ]);
})();