(function () {
    appModule.controller('common.views.users.createOrEditModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.user', 'userId',
        'abp.services.app.commonLookup', 'abp.services.app.phongBan', 'baseService', 'appSession', 'FileUploader',
        function ($scope, $uibModalInstance, userService, userId,
            commonLookupService, phongBanService, baseService, appSession, fileUploader) {
            var vm = this;
            vm.ROLE_LEVEL = app.ROLE_LEVEL;

            vm.saving = false;
            vm.user = null;
            vm.profilePictureId = null;
            vm.roles = [];
            vm.setRandomPassword = (userId == null);
            vm.sendActivationEmail = (userId == null);
            vm.canChangeUserName = true;
            vm.isTwoFactorEnabled = abp.setting.getBoolean("Abp.Zero.UserManagement.TwoFactorLogin.IsEnabled");
            vm.isLockoutEnabled = abp.setting.getBoolean("Abp.Zero.UserManagement.UserLockOut.IsEnabled");
            console.log(userId, 'userId');
            vm.tenant = appSession.tenant;
            console.log(vm.tenant, 'vm.tenant');

            vm.save = function () {
                var assignedRoleNames = _.map(
                    _.where(vm.roles, { isAssigned: true }), //Filter assigned roles
                    function (role) {
                        return role.roleName; //Get names
                    });

                if (vm.setRandomPassword) {
                    vm.user.password = null;
                }

                //if (vm.user.isDonViTrucThuoc != true) {
                //    vm.user.huyenId = null;
                //    vm.user.xaId = null;
                //}

                vm.saving = true;
                userService.createOrUpdateUser({
                    user: vm.user,
                    assignedRoleNames: assignedRoleNames,
                    sendActivationEmail: vm.sendActivationEmail,
                    setRandomPassword: vm.setRandomPassword
                }).then(function (result) {
                    console.log(result, "result");
                    if (baseService.isNullOrEmpty(result.data)) {
                        abp.notify.info(app.localize('SavedSuccessfully'));
                        $uibModalInstance.close();
                    }
                    else {
                        abp.notify.error(result.data);
                    }
                }).finally(function () {
                    vm.saving = false;
                });
            };

            vm.refreshValidate = function () {
                baseService.ValidatorForm("#userCreateOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#userCreateOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (!formValidation.isValid()) {
                    jQuery("#userCreateOrEditForm").data('formValidation').resetForm();
                }
            }

            vm.changeRoleLevel = function () {
                vm.refreshValidate();
            }

            vm.deactiveAllRoles = function () {
                _.map(
                    _.where(vm.roles, { isAssigned: true }), //Filter roles by role level
                    function (role) {
                        role.isAssigned = false; //Assign role active true
                    });
            }

            //Customs
            {
                vm.RoleLevelOptions = {
                    dataSource: {
                        transport: {
                            read: function (options) {
                                commonLookupService.getRoleLevel().then(function (result) {
                                    console.log(result.data,"role_level");
                                    options.success(result.data);
                                });
                            }
                        }
                    },
                    dataValueField: "id",
                    dataTextField: "name",
                    filter: "startswith",
                    optionLabel: "Chọn chức vụ",
                }

                vm.boNganhIdOptions = {
                    dataSource: {
                        transport: {
                            read: function (options) {
                                commonLookupService.getListDonViTrucThuoc().then(function (result) {
                                    options.success(result.data);
                                });
                            }
                        }
                    },
                    dataValueField: "id",
                    dataTextField: "name",
                    filter: "startswith",
                    optionLabel: "Bộ nghành quản lý",
                }

                vm.phongBanOptions = {
                    dataSource: {
                        transport: {
                            read: function (options) {
                                phongBanService.getAllToDDL().then(function (result) {
                                    options.success(result.data);
                                });
                            }
                        }
                    },
                    dataValueField: "id",
                    dataTextField: "name",
                    filter: "startswith",
                    optionLabel: "Chọn phòng ban",
                }

                vm.tieuBanOptions = {
                    dataSource: {
                        transport: {
                            read: function (options) {
                                commonLookupService.getListTieuBanChuyenGia().then(function (result) {
                                    options.success(result.data);
                                });
                            }
                        }
                    },
                    dataValueField: "id",
                    dataTextField: "name",
                    filter: "startswith",
                    optionLabel: "Chọn tiểu ban",
                }

                vm.changePhongBan = function () {
                    vm.refreshValidate();
                }

                vm.tinhOptions = {
                    dataSource: appSession.get_tinh(),
                    dataValueField: "id",
                    dataTextField: "ten",
                    optionLabel: "Chọn tỉnh",
                    filter: "startswith",
                    dataBound: function () {
                        //this.enable(false);
                    }
                };

                vm.huyenOptions = {
                    dataSource: appSession.get_huyen(),
                    dataValueField: "id",
                    dataTextField: "ten",
                    optionLabel: "Tất cả",
                    filter: "startswith",
                    dataBound: function () {
                        //this.enable(false);
                    }
                };

                vm.xaOptions = {
                    dataSource: appSession.get_xa(),
                    dataValueField: "id",
                    dataTextField: "ten",
                    optionLabel: "Tất cả",
                    filter: "startswith",
                    dataBound: function () {
                        //this.enable(false);
                    }
                };
            }

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };

            vm.getAssignedRoleCount = function () {
                return _.where(vm.roles, { isAssigned: true }).length;
            };

            vm.getCurrentRole = function () {
                _.map(
                    _.where(vm.roles, { isAssigned: true }),
                    function (role) {
                        //role.isAssigned = false; //Assign role active true
                    });
            }

            function init() {
                userService.getUserForEdit({
                    id: userId
                }).then(function (result) {
                    vm.user = result.data.user;
                    vm.profilePictureId = result.data.profilePictureId;
                    vm.user.passwordRepeat = vm.user.password;
                    vm.roles = result.data.roles;
                    vm.getCurrentRole();
                    if (vm.user) {
                        if (vm.user.tinhId) {
                        } else if (vm.tenant) {
                            vm.user.tinhId = vm.tenant.tinhId;
                        }
                    }
                    console.log(vm.user, 'vm.user');

                    vm.canChangeUserName = vm.user.userName != app.consts.userManagement.defaultAdminUserName;
                    $timeout(function () {
                        var ddlRoleLevel = $('#ddlRoleLevel').data("kendoDropDownList");
                        ddlRoleLevel.setDataSource(vm.RoleLevelDataSource);
                        ddlRoleLevel.dataSource.read();
                    });

                    var $profilePictureResize = $('#ChuKyResize');
                    //// var newCanvasHeight = response.result.height * $profilePictureResize.width() / response.result.width;
                    // //$profilePictureResize.height(newCanvasHeight + 'px');
                    vm.uploadedFileName = vm.user.urlImageChuKyNhay;
                    var profileFilePath = "~/ChuKy/_can-bo/8/8_KN.png";//vm.user.urlImageChuKyNhay;///abp.appPath + 'Temp/Downloads/' + response.result.fileName + '?v=' + new Date().valueOf();
                    $profilePictureResize.attr('src', profileFilePath);
                });
            }

            init();
            vm.uploadedFileName = '';
            vm.uploader = function () {
                var uploader = new fileUploader({
                    url: abp.appPath + 'File/UploadChuKy?maChuKy=' + 'KN_',
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
                        vm.user.urlImageChuKyNhay = vm.uploadedFileName;
                        vm.user.isNew = true;

                        $profilePictureResize.attr('src', profileFilePath);
                    } else {
                        abp.message.error(response.error.message);
                    }
                };
                return uploader;
            }
        }
    ]);
})();