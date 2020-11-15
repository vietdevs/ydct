(function () {
    appModule.controller('thietlap.views.phanloaihoso.directives.phanloaihoso.index', [
        '$scope', '$uibModalInstance', 'abp.services.app.phanLoaiHoSo', 'dataItem', 'baseService',
        function ($scope, $uibModalInstance, phanLoaiHoSoService, dataItem, baseService) {

            var vm = this;
            vm.dataItem = dataItem || {};

            vm.save = function () {
                if (!app.checkValidateForm("#createOrEditForm")) {
                    abp.notify.error("Xin vui lòng xem lại", "Dữ liệu nhập chưa đúng!");
                    return;
                }

                vm.saving = true;
                phanLoaiHoSoService.createOrUpdate(vm.dataItem).then(function (result) {
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