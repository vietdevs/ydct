(function () {
    appModule.controller('quanlychuky.views.xemVideoKiemTraNenTang', [
        '$scope', '$uibModalInstance',
        function ($scope, $uibModalInstance) {
            var vm = this;
            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };
        }
    ]);
})();