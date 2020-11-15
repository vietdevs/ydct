(function () {
    appModule.controller('quanlyhoso.thutuc37.directives.modal.viewHistoryChiTietModal', [
        '$uibModalInstance', 'modalData', 'baseService',
        function ($uibModalInstance, modalData, baseService) {
            var vm = this;            
            vm.itemYKien = modalData.yKien;
            if (vm.itemYKien) {
                vm.itemYKien.noiDungYKien = baseService.formatBreakLineTextareaToHTML(vm.itemYKien.noiDungYKien);
                vm.itemYKien.lyDoChuyenNhanh = baseService.formatBreakLineTextareaToHTML(vm.itemYKien.lyDoChuyenNhanh);
            }
            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };
            
        }
    ]);
})();