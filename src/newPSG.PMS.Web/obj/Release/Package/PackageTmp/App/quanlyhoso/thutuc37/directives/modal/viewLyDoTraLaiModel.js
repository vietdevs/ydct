(function () {
    appModule.controller('quanlyhoso.thutuc37.directives.modal.viewLyDoTraLaiModel', [
        '$sce', '$uibModalInstance', 'modalData',
        function ($sce, $uibModalInstance, modalData) {
            var vm = this;

            vm.dataItem = modalData;
            
            vm.init = function () {
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