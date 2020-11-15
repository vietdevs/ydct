(function () {
    appModule.controller('thietlap.views.tieuchithamdinhlydo.viewModal', [
        '$scope', '$uibModalInstance', 'dataItem', 'baseService', 'appSession',
        function ($scope, $uibModalInstance , dataItem, baseService, appSession) {
            var vm = this;
            vm.saving = false;
            vm.dataItem = dataItem || { listLyDo: [] };

            vm.noiDungThamDinhOptions = {
                dataSource: {
                    transport: {
                        read: function (options) {
                            abp.services.app.tieuChiThamDinh.getAll().then(function (result) {
                                options.success(result);
                            });
                        }
                    }
                },
                dataValueField: "id",
                dataTextField: "tieuDeThamDinh",
                filter: "contains",
                optionLabel: "Chọn ...",
            }

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };
        }
    ]);
})();