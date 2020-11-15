(function () {
    appModule.controller('quanlyhoso.thutuc99.views.dangkyhoso.modal.xemVideoHuongDan', [
        '$scope', '$uibModalInstance', 
        function ($scope, $uibModalInstance) {
            var vm = this;
            
            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };
            
        }
    ]);
})();