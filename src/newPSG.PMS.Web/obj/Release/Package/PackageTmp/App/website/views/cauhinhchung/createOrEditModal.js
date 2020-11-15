(function () {
    appModule.controller('website.views.cauhinhchung.createOrEditModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.cauHinhChung', 'cauHinhChung', 'baseService',
        function ($scope, $uibModalInstance, cauHinhChungService, cauHinhChung, baseService) {
            var vm = this;
            vm.saving = false;
            vm.cauHinhChung = {
                isActive: true
            };
            vm.group = [];
            vm.type = [];
            vm.ChooseImageAdd = function (id) {
                var finder = new CKFinder();
                finder.selectActionFunction = function (fileUrl) {
                    $scope.$apply(function () {
                        vm.cauHinhChung.image = fileUrl;
                    })
                }
                finder.popup();
            }
            vm.save = function () {
                vm.saving = true;
                baseService.ValidatorForm("#CauHinhChungCreateOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#CauHinhChungCreateOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (formValidation.isValid()) {
                    if (vm.cauHinhChung.kieuCauHinh == '1') {
                        vm.cauHinhChung.giaTri = vm.cauHinhChung.string;
                    }
                    if (vm.cauHinhChung.kieuCauHinh == '2') {
                        vm.cauHinhChung.giaTri = vm.cauHinhChung.image;
                    }
                    if (vm.cauHinhChung.kieuCauHinh == '3') {
                        vm.cauHinhChung.giaTri = vm.cauHinhChung.text;
                    }
                    cauHinhChungService.createOrUpdate(vm.cauHinhChung).then(function (result) {
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
                vm.group = [{ title: "Thông tin chung", value: 1 }, { title: "Thông tin liên hệ", value: 2 }, { title: "Cài đặt mã nhúng", value: 3 }, { title: "Cài đặt hiển thị", value: 4 }];
                vm.type = [{ title: "Text", value: 1 }, { title: "Ảnh", value: 2 }, { title: "Văn bản", value: 3 }];
                if (cauHinhChung != null) {
                    vm.cauHinhChung = cauHinhChung;
                }
            }

            init();
        }
    ]);
})();