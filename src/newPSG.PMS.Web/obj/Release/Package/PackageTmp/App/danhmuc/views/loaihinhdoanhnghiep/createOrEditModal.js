(function () {
    appModule.controller('danhmuc.views.loaihinhdoanhnghiep.createOrEditModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.loaiHinhDoanhNghiep', 'loaihinhdoanhnghiep', 'baseService',
        function ($scope, $uibModalInstance, loaiHinhDoanhNghiepService, loaihinhdoanhnghiep, baseService) {
            var vm = this;
            vm.saving = false;
            vm.loaihinhdoanhnghiep = {
                isActive: true
            };

            vm.save = function () {
                baseService.ValidatorForm("#loaihinhdoanhnghiepCreateOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#loaihinhdoanhnghiepCreateOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (formValidation.isValid()) {
                    vm.saving = true;
                    loaiHinhDoanhNghiepService.createOrUpdate(vm.loaihinhdoanhnghiep).then(function (result) {
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

            var init = function () {
                if (loaihinhdoanhnghiep != null) {
                    vm.loaihinhdoanhnghiep = loaihinhdoanhnghiep;
                }
            }

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };

            init();
        }
    ]);
})();