(function () {
    appModule.controller('danhmuc.views.quocgia.createOrEditModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.quocGia', 'quocgia', 'baseService',
        function ($scope, $uibModalInstance, quocGiaService, quocgia, baseService) {
            //variable
            var vm = this;
            vm.saving = false;
            vm.quocgia = {
                isActive: true
            };

            //function
            vm.save = function () {
                baseService.ValidatorForm("#quocgiaCreateOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#quocgiaCreateOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (formValidation.isValid()) {
                    vm.saving = true;
                    quocGiaService.createOrUpdate(vm.quocgia).then(function (result) {
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
                if (quocgia != null) {
                    vm.quocgia = quocgia;
                }
            }
            init();
        }
    ]);
})();