(function () {
    appModule.controller('host.views.tenants.createModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.tenant', 'abp.services.app.commonLookup', 'appSession',
        function ($scope, $uibModalInstance, tenantService, commonLookupService, appSession) {
            var vm = this;

            vm.saving = false;
            vm.setRandomPassword = true;
            vm.useHostDb = true;
            vm.editions = [];
            vm.tenant = {
                isActive: true,
                shouldChangePasswordOnNextLogin: true,
                sendActivationEmail: true,
                editionId: 0,
                tinhId: null
            };

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

            vm.getEditionValue = function (item) {
                return parseInt(item.value);
            };

            vm.save = function () {
                if (vm.setRandomPassword) {
                    vm.tenant.adminPassword = null;
                }

                if (vm.tenant.editionId == 0) {
                    vm.tenant.editionId = null;
                }

                vm.saving = true;
                tenantService.createTenant(vm.tenant)
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

            commonLookupService.getEditionsForCombobox({}).then(function (result) {
                vm.editions = result.data.items;
                vm.editions.unshift({ value: "0", displayText: app.localize('NotAssigned') });

                commonLookupService.getDefaultEditionName({}).then(function (result) {
                    var defaultEdition = _.where(vm.editions, { displayText: result.data });
                    if (defaultEdition && defaultEdition.length) {
                        vm.tenant.editionId = parseInt(_.where(vm.editions, { displayText: 'Standard' })[0].value);
                    }
                });
            });
        }
    ]);
})();