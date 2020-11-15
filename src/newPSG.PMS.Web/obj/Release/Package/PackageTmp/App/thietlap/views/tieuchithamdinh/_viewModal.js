(function () {
    appModule.controller('thietlap.views.tieuchithamdinh.viewModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.tieuChiThamDinh', 'dataItem', 'baseService', 'appSession',
        function ($scope, $uibModalInstance, tieuChiThamDinhService, dataItem, baseService, appSession) {
            var vm = this;
            vm.saving = false;
            vm.ROLE_LEVEL = app.ROLE_LEVEL;
            vm.dataItem = dataItem || { listNoiDung: [] };

            if (vm.dataItem.listNoiDung.length > 0) {
                vm.dataItem.listNoiDung = app.orderByNoiDungThamDinh(vm.dataItem.listNoiDung);
            }

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };
        }
    ]);
})();