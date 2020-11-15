(function () {
    appModule.controller('quanlychuky.views.xemVideoHuongDan', [
        '$scope', '$uibModalInstance',
        function ($scope, $uibModalInstance) {
            var vm = this;

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };
        }
    ]);
})();