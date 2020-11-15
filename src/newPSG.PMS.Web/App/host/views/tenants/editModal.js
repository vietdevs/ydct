(function () {
    appModule.controller('host.views.tenants.editModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.tenant', 'abp.services.app.commonLookup', 'tenantId', 'appSession',
        function ($scope, $uibModalInstance, tenantService, commonLookupService, tenantId, appSession) {
            var vm = this;

            vm.saving = false;
            vm.tenant = null;
            vm.currentConnectionString = null;
            vm.editions = [];

            vm.tinhOptions = {
                dataSource: appSession.get_tinh(),
                dataValueField: "id",
                dataTextField: "ten",
                optionLabel: "Chọn tỉnh",
                filter: "startswith",
                dataBound: function () {
                    //this.enable(false);
                }
            };

            vm.save = function () {
                if (vm.tenant.editionId == 0) {
                    vm.tenant.editionId = null;
                }

                vm.saving = true;
                tenantService.updateTenant(vm.tenant)
                    .then(function () {
                        abp.notify.info(app.localize('SavedSuccessfully'));
                        $uibModalInstance.close();
                    }).finally(function () {
                        vm.saving = false;
                    });
            };

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };

            vm.getEditionValue = function (item) {
                return parseInt(item.value);
            };

            function init() {
                commonLookupService.getEditionsForCombobox({}).then(function (result) {
                    vm.editions = result.data.items;
                    vm.editions.unshift({ value: "0", displayText: app.localize('NotAssigned') });
                });

                tenantService.getTenantForEdit({
                    id: tenantId
                }).then(function (result) {
                    vm.tenant = result.data;
                    vm.currentConnectionString = result.data.connectionString;
                    vm.tenant.editionId = vm.tenant.editionId || 0;
                });
            }

            init();
        }
    ]);
})();