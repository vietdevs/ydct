(function () {
    appModule.controller('danhmuc.views.phongban.createOrEditModalv2', [
        '$scope', '$uibModalInstance', '_model',
        'abp.services.app.phongBan', 'abp.services.app.commonLookup', 'appSession', 'baseService',
        function ($scope, $uibModalInstance, _model,
            phongBanService, commonLookupService, appSession, baseService) {
            var vm = this;
            vm.saving = false;
            vm.isChonQuiTrinh = false;
            vm.model = {
                isActive: true,
                arrLoaiHoSoQuiTrinh: []
            };

            vm.loaiHoSo = appSession.get_loaihoso();
            vm.arrLoaiHoSoIdChon = [];

            vm.loaiHoSoOptions = {
                dataSource: appSession.get_loaihoso(),
                dataValueField: "id",
                dataTextField: "name",
                filter: "contains",
                optionLabel: "Chọn loại hồ sơ",
            }

            vm.quiTrinhOptions = {
                dataSource: appSession.quitrinh,
                dataValueField: "id",
                dataTextField: "name",
                filter: "contains",
                optionLabel: "Chọn qui trình",
            }

            vm.refreshValidate = function () {
                jQuery("#_CreateOrEditForm").data('formValidation').resetForm();
            }

            vm.checkXuLy = function (loaiHoSo) {
                if (loaiHoSo.isXuLy == true) {
                    loaiHoSo.quiTrinh = 1;
                }
                else {
                    loaiHoSo.quiTrinh = null;
                }
            }

            var init = function () {
                if (_model != null) {
                    vm.model = _model;
                }
            }
            init();

            vm.checkChonQuiTrinh = function () {
                for (var i = 0; i < vm.model.arrLoaiHoSoQuiTrinh.length; i++) {
                    if (vm.model.arrLoaiHoSoQuiTrinh[i].isXuLy == true && !vm.model.arrLoaiHoSoQuiTrinh[i].quiTrinh) {
                        abp.notify.error('Hãy chọn quy trình để xử lý loại hồ sơ ' + vm.model.arrLoaiHoSoQuiTrinh[i].tenLoaiHoSo);
                        return false;
                    }
                }
            }

            vm.save = function () {
                if (vm.checkChonQuiTrinh() == false) {
                    return;
                }
                baseService.ValidatorForm("#_CreateOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#_CreateOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (formValidation.isValid()) {
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
        }
    ]);
})();