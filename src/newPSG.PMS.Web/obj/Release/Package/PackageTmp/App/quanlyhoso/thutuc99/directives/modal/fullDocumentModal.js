(function () {
    appModule.controller('quanlyhoso.thutuc99.directives.modal.fullDocumentModal', [
        '$uibModalInstance', 'modalData',
        function ($uibModalInstance, modalData) {
            var vm = this;
            vm.title = modalData.title;
            vm.dataHtml = modalData.dataHtml;
            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };
        }
    ]);
})();