(function () {
    appModule.controller('quanlyhoso.thutuc37.views.dangkyhoso.modal.selectLoaiHoSoModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.user', 'abp.services.app.commonLookup', 'abp.services.app.loaiHoSo', 'baseService',
        function ($scope, $uibModalInstance, userService, commonLookupService, loaiHoSoService, baseService) {
            var vm = this;
            vm.loaiHoSo = [];
            vm.loaiHoSoEnum = [];
            vm.selectNhom = function (_nhom) {
                $uibModalInstance.close(_nhom);
            }

            vm.init = function () {
                var thuTucEnum = 37;
                loaiHoSoService.getLoaiHoSoByThuTucToDDL(thuTucEnum).then(function (result) {
                    if (result.data) {
                        abp.ui.clearBusy();
                        vm.loaiHoSo = result.data;
                        console.log(vm.loaiHoSo, "vm.loaiHoSo");
                    }
                });
            };
            vm.init();

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };
        }
    ]);
})();