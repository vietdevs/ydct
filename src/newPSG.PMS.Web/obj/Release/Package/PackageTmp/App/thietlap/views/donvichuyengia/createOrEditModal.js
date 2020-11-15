(function () {
    appModule.controller('thietlap.views.donvichuyengia.createOrEditModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.donViChuyenGia', 'abp.services.app.commonLookup', 'data', 'baseService','appSession',
        function ($scope, $uibModalInstance, donViChuyenGiaService, commonLookupService, data, baseService, appSession) {
            var vm = this;
            //Variable
            vm.saving = false;
            vm.dataModal = {
                isActive: true
            };

            vm.dataId = data ? data.id : undefined;

            //controll
            {
                vm.tinhOptions = {
                    dataSource: appSession.get_tinh(),
                    dataValueField: "id",
                    dataTextField: "ten",
                    optionLabel: "Chọn ...",
                    filter: "startswith",
                    dataBound: function () {
                    }
                };
                vm.huyenOptions = {
                    dataSource: appSession.get_huyen(),
                    dataValueField: "id",
                    dataTextField: "ten",
                    optionLabel: "Chọn ...",
                    filter: "contains"

                };
                vm.xaOptions = {
                    dataSource: appSession.get_xa(),
                    dataValueField: "id",
                    dataTextField: "ten",
                    optionLabel: "Chọn ...",
                    filter: "contains",
                    dataBound: function () {
                    }
                };
                vm.thuTucOptions = {
                    dataSource: {
                        transport: {
                            read: function (options) {
                                commonLookupService.getThuTuc().then(function (result) {
                                    options.success(result.data);
                                });
                            }
                        }
                    },
                    dataValueField: "id",
                    dataTextField: "name",
                    filter: "contains",
                    optionLabel: "Chọn thủ tục",
                }
            }

            //Function
            vm.save = function () {
                if (!app.checkValidateForm("#createOrEditForm")) {
                    abp.notify.error("Xin vui lòng xem lại", "Dữ liệu nhập chưa đúng!");
                    vm.saving = false;
                    return;
                }
                vm.saving = true;
                baseService.ValidatorForm("#createOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#createOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();

                if (formValidation.isValid()) {
                    donViChuyenGiaService.createOrUpdate(vm.dataModal).then(function (result) {
                        abp.notify.success(app.localize('SavedSuccessfully'));
                        $uibModalInstance.close(true);
                    }).finally(function () {
                        vm.saving = false;
                    });
                }
                else {
                    vm.saving = false;
                }
            };

            //vm.refreshValidate = function () {
            //    jQuery("#chucdanhCreateOrEditForm").data('formValidation').resetForm();
            //};

            var init = function () {
                if (vm.dataId) {
                    donViChuyenGiaService.getForEdit({ id: vm.dataId }).then(function (result) {
                        if (result.data)
                            vm.dataModal = result.data;
                    }).finally(function () {
                    });
                }
            };

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };

            init();
        }
    ]);
})();