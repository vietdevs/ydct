(function () {
    appModule.controller('danhmuc.views.huyen.createOrEditModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.huyen', 'abp.services.app.tinh', 'huyen', 'isUpdate', 'baseService',
        function ($scope, $uibModalInstance, huyenService, tinhService, huyen, isUpdate, baseService) {
            //variable
            var vm = this;
            vm.saving = false;
            vm.huyen = {
                isActive: true
            };

            vm.save = function () {
                vm.saving = true;
                baseService.ValidatorForm("#huyenCreateOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#huyenCreateOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (formValidation.isValid()) {
                    huyenService.createOrUpdate(vm.huyen).then(function (result) {
                        abp.notify.success(app.localize('SavedSuccessfully'));
                        $uibModalInstance.close();
                    }).finally(function () {
                        vm.saving = false;
                    });
                }
                else {
                    vm.saving = false;
                }
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
                optionLabel: "Chọn tỉnh",
            }

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };

            var init = function () {
                if (huyen != null) {
                    vm.huyen = huyen;
                }
            }

            init();
        }
    ]);
})();