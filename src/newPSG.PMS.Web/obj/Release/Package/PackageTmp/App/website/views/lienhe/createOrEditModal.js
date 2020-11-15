(function () {
    appModule.controller('website.views.lienhe.createOrEditModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.lienHe', 'lienhe', 'baseService',
        function ($scope, $uibModalInstance, lienheService, lienhe, baseService) {
            //variable
            var vm = this;
            vm.saving = false;
            vm.lienhe = {
                isActive: true
            };

            //function
            vm.save = function () {
                app.validatorForm("#lienheCreateOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#lienheCreateOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (formValidation.isValid()) {
                    vm.saving = true;
                    lienheService.createOrUpdate(vm.lienhe).then(function (result) {
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
                if (lienhe != null) {
                    vm.lienhe = lienhe;
                }
            };
            init();
        }
    ]);
})();