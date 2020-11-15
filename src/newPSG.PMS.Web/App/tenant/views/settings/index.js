(function () {
    appModule.controller('tenant.views.settings.index',
        [
            '$scope', 'abp.services.app.tenantSettings', 'appSession', 'FileUploader', 'abp.services.app.callApiATTP',
            function ($scope, tenantSettingsService, appSession, fileUploader, callApiATTPService) {
                var vm = this;

                var usingDefaultTimeZone = false;
                var initialTimeZone = null;

                vm.logoUploader = null;
                vm.customCssUploader = null;

                $scope.$on('$viewContentLoaded',
                    function () {
                        App.initAjax();
                    });

                vm.isMultiTenancyEnabled = abp.multiTenancy.isEnabled;
                vm.showTimezoneSelection = abp.clock.provider.supportsMultipleTimezone;
                vm.activeTabIndex = (!vm.isMultiTenancyEnabled || vm.showTimezoneSelection) ? 0 : 1;
                vm.loading = false;
                vm.settings = null;
                vm.tenant = appSession.tenant;

                vm.logoUploader = undefined;
                vm.customCssUploader = undefined;

                vm.getSettings = function () {
                    vm.loading = true;
                    tenantSettingsService.getAllSettings()
                        .then(function (result) {
                            vm.settings = result.data;
                            console.log(vm.settings, 'vm.settings');
                            initialTimeZone = vm.settings.general.timezone;
                            usingDefaultTimeZone = vm.settings.general.timezoneForComparison ===
                                abp.setting.values["Abp.Timing.TimeZone"];
                        }).finally(function () {
                            vm.loading = false;
                        });
                };

                function initUploaders() {
                    vm.logoUploader = createUploader(
                        "TenantCustomization/UploadLogo",
                        [{
                            name: 'imageFilter',
                            fn: function (item, options) {
                                var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
                                if ('|jpg|jpeg|png|gif|'.indexOf(type) === -1) {
                                    abp.message.error(app.localize('UploadLogo_Info'));
                                    return false;
                                }

                                if (item.size > 30720) //30KB
                                {
                                    abp.message.error(app.localize('UploadLogo_Info'));
                                    return false;
                                }

                                return true;
                            }
                        }],
                        function (result) {
                            appSession.tenant.logoFileType = result.fileType;
                            appSession.tenant.logoId = result.id;
                            $('#LogoFileInput').val(null);
                        }
                    );

                    vm.customCssUploader = createUploader(
                        "TenantCustomization/UploadCustomCss",
                        null,
                        function (result) {
                            appSession.tenant.customCssId = result.id;
                            $('#TenantCustomCss').remove();
                            $('head').append('<link id="TenantCustomCss" href="' + abp.appPath + 'TenantCustomization/GetCustomCss?id=' + appSession.tenant.customCssId + '" rel="stylesheet"/>');
                            $('#CustomCssFileInput').val(null);
                        }
                    );
                }

                function createUploader(url, filters, success) {
                    var uploader = new fileUploader({
                        url: abp.appPath + url,
                        headers: {
                            "X-XSRF-TOKEN": abp.security.antiForgery.getToken()
                        },
                        queueLimit: 1,
                        removeAfterUpload: true
                    });

                    if (filters) {
                        uploader.filters = filters;
                    }

                    uploader.onSuccessItem = function (item, ajaxResponse, status) {
                        if (ajaxResponse.success) {
                            abp.notify.info(app.localize('SavedSuccessfully'));
                            success && success(ajaxResponse.result);
                        } else {
                            abp.message.error(ajaxResponse.error.message);
                        }
                    };

                    return uploader;
                }

                vm.uploadLogo = function () {
                    vm.logoUploader.uploadAll();
                };

                vm.uploadCustomCss = function () {
                    vm.customCssUploader.uploadAll();
                };

                vm.clearLogo = function () {
                    tenantSettingsService.clearLogo().then(function () {
                        appSession.tenant.logoFileType = null;
                        appSession.tenant.logoId = null;
                        abp.notify.info(app.localize('ClearedSuccessfully'));
                        $('#LogoFileInput').val(null);
                    });
                };

                vm.clearCustomCss = function () {
                    tenantSettingsService.clearCustomCss().then(function () {
                        appSession.tenant.customCssId = null;
                        $('#TenantCustomCss').remove();
                        abp.notify.info(app.localize('ClearedSuccessfully'));
                        $('#CustomCssFileInput').val(null);
                    });
                };

                vm.showValidate = false;
                vm.checkTaiKhoanLienThong = function () {
                    vm.showValidate = true;
                    app.checkValidateForm("#CauHinhLienThong");
                    if (vm.settings.lienThong.userLienThong === null || vm.settings.lienThong.userLienThong === ''
                        //|| vm.settings.lienThong.tenantLienThong == null || vm.settings.lienThong.tenantLienThong == ''
                        || vm.settings.lienThong.passLienThong === null || vm.settings.lienThong.passLienThong === ''
                        || vm.settings.lienThong.domainLienThong === null || vm.settings.lienThong.domainLienThong === ''
                       /* || !vm.validateUrl(vm.settings.lienThong.domainLienThong)*/) {
                        let msgString = "Xin vui lòng nhập đủ 'Thông tin cấu hình liên thông'!";
                        abp.notify.error(msgString, "Dữ liệu đang bị trống");
                        return false;
                    }
                    else {
                        vm.getToken();
                    }
                };

                vm.getToken = function () {
                    vm.settings.lienThong.tenantLienThong = 'lienthong';
                    vm.saving = true;
                    var rep = {
                        tenancy_name: vm.settings.lienThong.tenantLienThong,
                        usr: vm.settings.lienThong.userLienThong,
                        pwd: vm.settings.lienThong.passLienThong,
                        domainLienThong: vm.settings.lienThong.domainLienThong,
                    };

                    callApiATTPService.getToken(rep).then(function (result) {
                        if (result && result.data && result.data.success && result.data.result !== null && result.data.result !== '') {
                            vm.settings.lienThong.tokenLienThong = result.data.result;
                            let msgString = "Thông tin tài khoản hợp lệ";
                            abp.notify.success(msgString, "Kiểm tra thành công!");
                        }
                        else {
                            vm.settings.lienThong.tokenLienThong = null;
                            let msgString = "Vui lòng xem lại thông tin !";
                            abp.notify.error(msgString, "Kiểm tra thất bại!");
                        }
                    }).finally(function () {
                        vm.saving = false;
                    });
                };

                vm.saveAll = function () {
                    vm.saving = true;
                    tenantSettingsService.updateAllSettings(vm.settings).then(function () {
                        abp.notify.info(app.localize('SavedSuccessfully'));

                        if (abp.clock.provider.supportsMultipleTimezone &&
                            usingDefaultTimeZone &&
                            initialTimeZone !== vm.settings.general.timezone) {
                            abp.message.info(app.localize('TimeZoneSettingChangedRefreshPageNotification'))
                                .done(function () {
                                    window.location.reload();
                                });
                        }
                    }).finally(function () {
                        vm.saving = false;
                    });
                };

                vm.getSettings();
                initUploaders();

                vm.validateUrl = function (value) {
                    return /^(?:(?:(?:https?|ftp):)?\/\/)(?:\S+(?::\S*)?@)?(?:(?!(?:10|127)(?:\.\d{1,3}){3})(?!(?:169\.254|192\.168)(?:\.\d{1,3}){2})(?!172\.(?:1[6-9]|2\d|3[0-1])(?:\.\d{1,3}){2})(?:[1-9]\d?|1\d\d|2[01]\d|22[0-3])(?:\.(?:1?\d{1,2}|2[0-4]\d|25[0-5])){2}(?:\.(?:[1-9]\d?|1\d\d|2[0-4]\d|25[0-4]))|(?:(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)(?:\.(?:[a-z\u00a1-\uffff0-9]-*)*[a-z\u00a1-\uffff0-9]+)*(?:\.(?:[a-z\u00a1-\uffff]{2,})))(?::\d{2,5})?(?:[/?#]\S*)?$/i.test(value);
                };
            }
        ]);
})();