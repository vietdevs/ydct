(function () {
    appModule.controller('website.views.bothutuc.createOrEditModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.boThuTuc', 'bothutuc', 'baseService',
        function ($scope, $uibModalInstance, boThuTucService, bothutuc, baseService) {
            //variable
            var vm = this;
            vm.saving = false;
            vm.bothutuc = {
                isActive: true,
                isHome: false
            };
            //$scope.ckDetails = {
            //    languague: 'vi',
            //    height: '320px'
            //}
            vm.hanhChinhCap = [];
            vm.GetSeo = function () {
                vm.bothutuc.duongDan = app.locdau(vm.bothutuc.tenThuTuc);
            }
            vm.ChooseImage = function () {
                var finder = new CKFinder();
                finder.selectActionFunction = function (fileUrl) {
                    $scope.$apply(function () {
                        vm.bothutuc.anh = fileUrl;
                    })
                }
                finder.popup();
            }
            vm.save = function () {
                app.validatorForm("#bothutucCreateOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#bothutucCreateOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (formValidation.isValid()) {
                    vm.saving = true;
                    boThuTucService.createOrUpdate(vm.bothutuc).then(function (result) {
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
                vm.hanhChinhCap = [{ title: "Cấp Trung ương", value: 1 }, { title: "Cấp tỉnh", value: 2 }];
                if (bothutuc != null) {
                    vm.bothutuc = bothutuc;
                }
            };
            init();
        }
    ]);
})();