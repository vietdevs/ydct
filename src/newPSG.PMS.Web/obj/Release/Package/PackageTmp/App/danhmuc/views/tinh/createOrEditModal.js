(function () {
    appModule.controller('danhmuc.views.tinh.createOrEditModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.tinh', 'tinh', 'baseService',
        function ($scope, $uibModalInstance, tinhService, tinh, baseService) {
            //variable
            var vm = this;
            vm.saving = false;
            vm.tinh = {
                isActive: true
            };

            //function
            vm.save = function () {
                baseService.ValidatorForm("#tinhCreateOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#tinhCreateOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (formValidation.isValid()) {
                    vm.saving = true;
                    tinhService.createOrUpdate(vm.tinh).then(function (result) {
                        abp.notify.success(app.localize('SavedSuccessfully'));
                        $uibModalInstance.close();
                    }).finally(function () {
                        vm.saving = false;
                    });
                }
            };

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };

            var init = function () {
                if (tinh != null) {
                    vm.tinh = tinh;
                    //vm.isUpdate = isUpdate;
                }
            }

            init();
        }
    ]);
})();