(function () {
    appModule.controller('quanlychuky.views.createOrEditModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.chuKy', 'chuKy', 'abp.services.app.commonLookup', 'FileUploader', 'appSession', 'baseService',
        function ($scope, $uibModalInstance, chuKyService, chuKy, commonLookupService, fileUploader, appSession, baseService) {
            //variable
            var vm = this;
            vm.saving = false;
            vm.chuKy = {
                isActive: true,
                loaiChuKy: appSession.user.roleLevel == 1 ? 1 : null
            };
            vm.uploadedFileName = null;
            vm.ROLE_LEVEL = app.ROLE_LEVEL;
            vm.roleLv = appSession.user.roleLevel;
            vm.nameTile = '';

            //Controll
            vm.loaiChuKyOptions = {
                dataSource: {
                    transport: {
                        read: function (options) {
                            var level = appSession.user.roleLevel == null ? -1 : appSession.user.roleLevel;
                            commonLookupService.getLoaiChuKy(level).then(function (result) {
                                options.success(result.data);
                            });
                        }
                    }
                },
                dataValueField: "id",
                dataTextField: "name",
                optionLabel: app.localize('Chọn ...'),
                filter: "contains",
            }

            vm.save = function () {
                var checkRet = vm.checkMaxLength();
                if (checkRet == false) {
                    return;
                }
                vm.saving = true;
                baseService.ValidatorForm("#chuKyCreateOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#chuKyCreateOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (formValidation.isValid()) {
                    if (!vm.uploadedFileName && vm.chuKy.dataImage == null) {
                        abp.notify.error('Xin hãy chọn một ảnh ' + vm.nameTile);
                        vm.saving = false;
                        return;
                    }
                    vm.chuKy.userId = appSession.user.id;
                    vm.chuKy.loaiChuKy = (appSession.user.roleLevel == 1 ? 1 : vm.chuKy.loaiChuKy);
                    vm.chuKy.maChuKy = 'CK_' + vm.chuKy.loaiChuKy + "_" + appSession.user.roleLevel;
                    chuKyService.createOrUpdateChuKy(vm.chuKy).then(function (result) {
                        if (!baseService.isNullOrEmpty(vm.uploadedFileName)) {
                            chuKyService.updateChuKyImageUrl(result.data, vm.uploadedFileName, vm.chuKy.maChuKy).then(function (result) {
                                abp.notify.success(app.localize('SavedSuccessfully'));
                                $uibModalInstance.close();
                                vm.saving = false;
                            });
                        }
                        else {
                            abp.notify.success(app.localize('SavedSuccessfully'));
                            $uibModalInstance.close();
                            vm.saving = false;
                        }
                    }).finally(function () {
                    });
                }
                else {
                    vm.saving = false;
                }
            };

            vm.refreshValidate = function () {
                jQuery("#chuKyCreateOrEditForm").data('formValidation').resetForm();
            }

            vm.uploader = function () {
                var uploader = new fileUploader({
                    url: abp.appPath + 'File/UploadChuKy?maChuKy=' + 'CK_' + appSession.user.roleLevel,
                    headers: {
                        "X-XSRF-TOKEN": abp.security.antiForgery.getToken()
                    },
                    queueLimit: 1,
                    autoUpload: true,
                    removeAfterUpload: true,
                    filters: [{
                        name: 'chuKyFilter',
                        fn: function (item, options) {
                            //File type check
                            var type = '|' + item.type.slice(item.type.lastIndexOf('/') + 1) + '|';
                            if ('|png|'.indexOf(type) === -1) {
                                abp.message.error(app.localize('ProfilePicture_Warn_FileType'));
                                return false;
                            }

                            //File size check
                            if (item.size > 1048576) //1MB
                            {
                                abp.message.error(app.localize('ProfilePicture_Warn_SizeLimit'));
                                return false;
                            }

                            return true;
                        }
                    }]
                });
                uploader.onSuccessItem = function (fileItem, response, status, headers) {
                    if (response.success) {
                        var $profilePictureResize = $('#ChuKyResize');
                        var newCanvasHeight = response.result.height * $profilePictureResize.width() / response.result.width;
                        $profilePictureResize.height(newCanvasHeight + 'px');

                        var profileFilePath = abp.appPath + 'Temp/Downloads/' + response.result.fileName + '?v=' + new Date().valueOf();
                        vm.uploadedFileName = response.result.fileName;

                        $profilePictureResize.attr('src', profileFilePath);
                    } else {
                        abp.message.error(response.error.message);
                    }
                };
                return uploader;
            }

            vm.taiBoCai = function () {
                location.href = "/Temp/Downloads/TokenServices.zip"
            }
            vm.taiDauDemo = function () {
                location.href = "/Temp/Downloads/dau-mau.png"
            }
            vm.checkMaxLength = function () {
                if (vm.chuKy.tenChuKy != null && vm.chuKy.tenChuKy.length > 250) {
                    var msgString = "Xin vui lòng xem lại tên " + vm.nameTile;
                    abp.notify.error(msgString, "Tên " + vm.nameTile + " không được quá 250 ký tự");
                    return false;
                }
                if (vm.chuKy.moTa != null && vm.chuKy.moTa.length > 1000) {
                    var msgString = "Xin vui lòng xem lại mô tả";
                    abp.notify.error(msgString, "Mô tả không được quá 1000 ký tự");
                    return false;
                }
                if (vm.chuKy.chanChuKy != null && vm.chuKy.chanChuKy.length > 1000) {
                    var msgString = "Xin vui lòng xem lại chân chữ ký";
                    abp.notify.error(msgString, "Chân chữ ký không được quá 500 ký tự");
                    return false;
                }
            }

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };

            var init = function () {
                if (chuKy != null) {
                    vm.chuKy = chuKy;
                }
                vm.nameTile = vm.roleLv == vm.ROLE_LEVEL.DOANH_NGHIEP ? 'con dấu' : 'chữ ký';
                console.log(vm.ROLE_LEVEL);
            }

            init();
        }
    ]);
})();