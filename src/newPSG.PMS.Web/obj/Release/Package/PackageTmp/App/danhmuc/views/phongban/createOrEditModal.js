(function () {
    appModule.controller('danhmuc.views.phongban.createOrEditModal', [
        '$scope', '$uibModalInstance', '_model', 'abp.services.app.phongBan', 'abp.services.app.commonLookup', 'appSession', 'baseService',
        function ($scope, $uibModalInstance, _model, phongBanService, commonLookupService, appSession, baseService) {
            //variable
            var vm = this;
            vm.saving = false;
            vm.model = {
                isActive: true,
                arrLoaiHoSo: []
            };
            vm.loaiHoSo = appSession.get_loaihoso();
            vm.arrLoaiHoSoIdChon = [];

            //controll
            vm.loaiOptions = {
                dataSource: appSession.get_loaihoso(),
                dataValueField: "id",
                dataTextField: "name",
                filter: "contains",
                optionLabel: "Chọn loại hồ sơ",
            }

            vm.quiTrinhOptions = {
                dataSource: appSession.get_quytrinhthamdinh(),
                dataValueField: "id",
                dataTextField: "name",
                filter: "contains",
                optionLabel: "Chọn qui trình",
            }

            //function
            vm.save = function () {
                baseService.ValidatorForm("#phongBanCreateOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#phongBanCreateOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (formValidation.isValid()) {
                    vm.model.arrLoaiHoSo = [];
                    vm.arrLoaiHoSoIdChon.forEach(function (_item) {
                        vm.model.arrLoaiHoSo.push(_item);
                    });
                    vm.saving = true;
                    phongBanService.createOrUpdate(vm.model).then(function (result) {
                        abp.notify.success(app.localize('SavedSuccessfully'));
                        $uibModalInstance.close();
                    }).finally(function () {
                        vm.saving = false;
                    });
                }
            };

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };

            var init = function () {
                vm.arrLoaiHoSoIdChon = [];
                if (_model != null) {
                    vm.model = _model;
                    if (vm.model.arrLoaiHoSo && vm.model.arrLoaiHoSo.length > 0) {
                        vm.model.arrLoaiHoSo.forEach(function (item) {
                            vm.arrLoaiHoSoIdChon.push(item);
                        });
                    }
                }
            }
            init();
        }
    ]);
})();