(function () {
    appModule.controller('quanlytaikhoan.chuyengia.createOrEditModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.quanLyTaiKhoan', 'abp.services.app.phongBan', 'data',
        'abp.services.app.commonLookup', 'baseService', 'appSession',
        function ($scope, $uibModalInstance, quanLyTK, phongBanService, data,
            commonLookupService, baseService, appSession) {
            var vm = this;
            vm.user = {};
            vm.roles = [];
            vm.saving = false;
            vm.setRandomPassword = (data.userId == null);
            vm.save = function () {
                var assignedRoleNames = _.map(
                    _.where(vm.roles, { isAssigned: true }), //Filter assigned roles
                    function (role) {
                        return role.roleName; //Get names
                    });
                if (!app.checkValidateForm("#createOrEditForm")) {
                    abp.notify.error("Xin vui lòng xem lại", "Dữ liệu nhập chưa đúng!");
                    vm.saving = false;
                    return;
                }
                if (assignedRoleNames.length <= 0) {
                    abp.notify.error("Chưa thiết lập vai trò!");
                    vm.saving = false;
                    return;
                }
                vm.saving = true;
                baseService.ValidatorForm("#createOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#createOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (formValidation.isValid()) {
                    if (vm.setRandomPassword) {
                        vm.user.password = null;
                    }
                    if (!vm.user.isTrongCuc) {
                        vm.user.phongBanId = null;
                        vm.user.tieuBanId = null;
                    } 
                    quanLyTK.createOrUpdate({
                        user: vm.user,
                        assignedRoleNames: assignedRoleNames,
                        loaiTaiKhoan: data.loaiTaiKhoan,
                        setRandomPassword: vm.setRandomPassword
                    }).then(function (result) {
                        if (baseService.isNullOrEmpty(result.data)) {
                            abp.notify.info(app.localize('SavedSuccessfully'));
                            $uibModalInstance.close();
                        }
                        else {
                            abp.notify.warn(result.data);
                        }
                        },function (ex) {
                            // Handle error here
                            abp.notify.warn(ex.data.message);
                            vm.saving = false;
                        }
                        ).finally(function () {
                            vm.saving = false;
                    });
                }
            };
            {
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
                //vm.tieuBanOptions = {
                //    dataSource: {
                //        transport: {
                //            read: function (options) {
                //                commonLookupService.getListTieuBanChuyenGia().then(function (result) {
                //                    options.success(result.data);
                //                });
                //            }
                //        }
                //    },
                //    dataValueField: "id",
                //    dataTextField: "name",
                //    filter: "startswith",
                //    optionLabel: "Chọn tiểu ban",
                //}

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
            vm.dvqlChuyenGiaChange = function (dataItem) {
                vm.user.isTrongCuc = dataItem.isTrongCuc;
            }
            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };

            function init() {
                quanLyTK.getForEdit({ id: data.userId, loaiTaiKhoan: data.loaiTaiKhoan }).then(function (result) {
                    console.log(result.data.user, "result.data.user");
                    vm.user = result.data.user;
                    vm.roles = result.data.roles;
                    vm.user.passwordRepeat = vm.user.password;
                });
            }
            init();
        }
    ]);
})();