(function () {
    appModule.controller('thietlap.views.phanloaihoso.directives.phanloaihosofilter.filterEditModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.phanLoaiHoSo_Filter', 'dataItem', 'baseService',
        function ($scope, $uibModalInstance, phanLoaiHoSo_FilterService, dataItem, baseService) {

            var vm = this;
            vm.dataItem = dataItem;
            if (!vm.dataItem.filter) {
                vm.dataItem.filter = {
                    phanLoaiHoSo: {},
                    phanLoaiHinhThucSanXuat: {},
                };
            }

            vm.save = function () {
                if (!app.checkValidateForm("#createOrEditFormModal")) {
                    abp.notify.error("Xin vui lòng xem lại", "Dữ liệu nhập chưa đúng!");
                    vm.saving = false;
                    return;
                }
                if (!vm.dataItem.phanLoaiHoSoId) {
                    abp.notify.error("phanLoaiHoSoId==null");
                    vm.saving = false;
                    return;
                }

                vm.saving = true;
                phanLoaiHoSo_FilterService.createOrUpdate(vm.dataItem).then(function (result) {
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