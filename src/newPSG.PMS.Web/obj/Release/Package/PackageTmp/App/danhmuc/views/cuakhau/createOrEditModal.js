(function () {
    appModule.controller('danhmuc.views.cuakhau.createOrEditModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.cuaKhau', 'cuakhau', 'baseService',
        function ($scope, $uibModalInstance, chucDanhService, cuakhau, baseService) {
            var vm = this;
            vm.saving = false;
            vm.chucdanh = {
                isActive: true
            };

            vm.save = function () {
                vm.saving = true;
                baseService.ValidatorForm("#chucdanhCreateOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#chucdanhCreateOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (formValidation.isValid()) {
                    chucDanhService.createOrUpdate(vm.chucdanh).then(function (result) {
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

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };

            init = function () {
                if (chucdanh != null) {
                    vm.chucdanh = chucdanh;
                }
            }

            init();
        }
    ]);
})();