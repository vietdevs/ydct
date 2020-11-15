(function () {
    appModule.controller('thietlap.views.loaibienbanthamdinh.createOrEditModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.loaiBienBanThamDinh', 'dataItem', 'baseService',
        function ($scope, $uibModalInstance, loaiBienBanService , dataItem, baseService) {
            //variable
            var vm = this;
            vm.saving = false;
            vm.dataItem = dataItem || {};
            vm.ROLE_LEVEL = app.ROLE_LEVEL;
            
            vm.save = function () {
                if (!app.checkValidateForm("#dataItemCreateOrEditForm")) {
                    abp.notify.error("Vui lòng nhập đầy đủ thông tin!");
                    return false;
                }

                vm.saving = true;
                loaiBienBanService.createOrUpdate(vm.dataItem).then(function (result) {
                    abp.notify.success(app.localize('SavedSuccessfully'));
                    $uibModalInstance.close();
                }).finally(function () {
                    vm.saving = false;
                });
            };
            
            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };
        }
    ]);
})();