(function () {
    appModule.controller('quanlychuky.views.downLoadBoCaiModal', [
        '$scope', '$uibModalInstance',
        function ($scope, $uibModalInstance) {
            var vm = this;
            vm.downloading = false;

            vm.download = function () {
                $uibModalInstance.close();
            };

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };
        }
    ]);
})();