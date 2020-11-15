(function () {
    appModule.controller('danhmuc.views.xa.createOrEditModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.xa', 'abp.services.app.huyen', 'abp.services.app.tinh', 'xa', 'baseService',
        function ($scope, $uibModalInstance, xaService, huyenService, tinhService, xa, baseService) {
            //variable
            var vm = this;
            vm.saving = false;
            vm.xa = {
                isActive: true
            };

            //controll
            vm.tinhOptions = {
                dataSource: {
                    transport: {
                        read: function (options) {
                            tinhService.getAllToDDL().then(function (result) {
                                options.success(result.data);
                            });
                        }
                    }
                },
                dataValueField: "id",
                dataTextField: "ten",
                filter: "startswith",
                optionLabel: "Chọn tỉnh thành",
            }

            vm.huyenOptions = {
                dataSource: {
                    transport: {
                        read: function (options) {
                            vm.huyenCallBack = options;
                            if (vm.xa.tinhId) {
                                huyenService.getAllToDDL(vm.xa.tinhId).then(function (result) {
                                    options.success(result.data);
                                });
                            }
                        }
                    }
                },
                dataValueField: "id",
                dataTextField: "name",
                filter: "startswith",
                optionLabel: "Chọn quận huyện",
            }

            //function
            vm.save = function () {
                baseService.ValidatorForm("#xaCreateOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#xaCreateOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (formValidation.isValid()) {
                    vm.saving = true;
                    xaService.createOrUpdate(vm.xa).then(function (result) {
                        abp.notify.success(app.localize('SavedSuccessfully'));
                        $uibModalInstance.close();
                    }).finally(function () {
                        vm.saving = false;
                    });
                }
            };

            vm.changeTinh = function () {
                vm.huyenOptions.dataSource.transport.read(vm.huyenCallBack);
            }

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };

            var init = function () {
                if (xa != null) {
                    vm.xa = xa;
                    // vm.isUpdate = isUpdate;
                }
            }

            init();
        }
    ]);
})();