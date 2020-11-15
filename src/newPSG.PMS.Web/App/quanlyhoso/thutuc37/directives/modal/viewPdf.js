(function () {
    appModule.controller('app.quanlyhoso.thutuc37.directives.modal.viewpdf', [
        '$scope', '$uibModalInstance', 'base64', 'baseService', 'appSession',
        function ($scope, $uibModalInstance, base64, baseService, appSession) {
            //variable
            var vm = this;
            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };
            $scope.onViewShow = function () {
                console.log("show Viwe");
                var obj = document.createElement('object');
                obj.style.width = '100%';
                obj.style.height = '100%';
                obj.type = 'application/pdf';
                obj.data = 'data:application/pdf;base64,' + base64;

                var objzzz = angular.element(document.getElementById('viewPdf'))
                objzzz[0].appendChild(obj);
            }
            this.$onInit = function () {

                //obj.type = 'application/pdf';
                //obj.data = 'data:application/pdf;base64,' + base64;
                //document.body.appendChild(base64);
                //if (dataSent) {
                //    deNghiCapThuocService.getForEdit({ id: dataSent }).then(function (result) {
                //        vm.dataModal = result.data;
                //    }).finally(function () {
                //    });
                //}
            };

        }
    ]);
})();