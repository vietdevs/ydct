(function () {
    appModule.controller('quanlyhoso.thutuc37.directives.modal.viewHoSoModal', [
        '$sce', '$uibModalInstance', 'modalData',
        function ($sce, $uibModalInstance, modalData) {
            var vm = this;

            vm.title = modalData.title;
            vm.pathPDF = "/Report37/GoToViewHoSo?hoSoId=" + modalData.id;

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