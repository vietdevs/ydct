(function () {
    appModule.controller('danhmuc.views.lichlamviec.createOrEditModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.ngayNghi', 'ngaynghi', 'lichlamviec', 'baseService',
        function ($scope, $uibModalInstance, ngayNghiService, ngaynghi, lichlamviec, baseService) {
            //variable
            var vm = this;
            vm.saving = false;
            vm.ngaynghi = {
                isActive: true
            };
            vm.ngayNghiModel = {
                startDate: null,
                endDate: null,
            };

            //controll
            vm.ngayNghiDateRangePickerOptions = app.createDateRangePickerOptions();
            vm.ngayNghiDateRangePickerOptions.min = moment(lichlamviec.ngayBatDau);
            vm.ngayNghiDateRangePickerOptions.minDate = moment(lichlamviec.ngayBatDau);
            vm.ngayNghiDateRangePickerOptions.max = moment(lichlamviec.ngayKetThuc);
            vm.ngayNghiDateRangePickerOptions.maxDate = moment(lichlamviec.ngayKetThuc);

            //function
            vm.save = function () {
                vm.saving = true;
                baseService.ValidatorForm("#ngaynghiCreateOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#ngaynghiCreateOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (formValidation.isValid()) {
                    var ngayBatDau = new Date(vm.ngayNghiModel.startDate);
                    var ngayKetThuc = new Date(vm.ngayNghiModel.endDate);
                    vm.ngaynghi.ngayBatDau = new Date(ngayBatDau.getFullYear(), ngayBatDau.getMonth(), ngayBatDau.getDate(), 0, 0, 0);
                    vm.ngaynghi.ngayKetThuc = new Date(ngayKetThuc.getFullYear(), ngayKetThuc.getMonth(), ngayKetThuc.getDate(), 23, 59, 59);

                    ngayNghiService.createOrUpdate(vm.ngaynghi).then(function (result) {
                        abp.notify.success(app.localize('SavedSuccessfully'));
                        $uibModalInstance.close();
                    }).finally(function () {
                        vm.saving = false;
                    });
                }
                else {
                    vm.saving = false;
                }
            };

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };

            var init = function () {
                if (ngaynghi != null) {
                    vm.ngaynghi = ngaynghi;
                    vm.ngayNghiModel = {
                        startDate: vm.ngaynghi.ngayBatDau,
                        endDate: vm.ngaynghi.ngayKetThuc,
                    };
                }
            }

            init();
        }
    ]);
})();