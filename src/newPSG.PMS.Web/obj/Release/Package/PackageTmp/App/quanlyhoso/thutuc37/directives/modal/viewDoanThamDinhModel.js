(function () {
    appModule.controller('quanlyhoso.thutuc37.directives.modal.viewdoanthamdinh', [
        '$sce', '$uibModalInstance', 'modalData', 'abp.services.app.xuLyHoSoChuyenVien37',
        function ($sce, $uibModalInstance, modalData, xuLyHoSoChuyenVien37) {
            var vm = this;
            vm.dataItem = modalData;

            vm.init = function () {
                xuLyHoSoChuyenVien37.xemDoanThamDinh(vm.dataItem.id).then(function (result) {
                    if (result.data) {
                        vm.listDoanThamDinh = result.data;
                    }
                    
                })
            }
            vm.init();

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };

            //Function
            vm.trustSrc = function (src) {
                return $sce.trustAsResourceUrl(src);
            }
        }
    ]);
})();