(function () {
    appModule.controller('website.views.thongbao.createOrEditModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.thongBao', 'thongbao', 'baseService',
        function ($scope, $uibModalInstance, thongBaoService, thongbao, baseService) {
            //variable
            var vm = this;
            vm.saving = false;
            vm.thongbao = {
                isActive: true,
                isHome: false
            };
            //$scope.ckDetails = {
            //    languague: 'vi',
            //    height: '320px'
            //}
            vm.hanhChinhCap = [];
            vm.GetSeo = function () {
                vm.thongbao.duongDan = app.locdau(vm.thongbao.tenThongBao);
            }
            vm.ChooseImage = function () {
                var finder = new CKFinder();
                finder.selectActionFunction = function (fileUrl) {
                    $scope.$apply(function () {
                        vm.thongbao.anh = fileUrl;
                    })
                }
                finder.popup();
            }
            vm.ChooseFile = function () {
                var finder = new CKFinder();
                finder.selectActionFunction = function (fileUrl) {
                    $scope.$apply(function () {
                        vm.thongbao.taiLieu = fileUrl;
                    })
                }
                finder.popup();
            }
            vm.save = function () {
                app.validatorForm("#thongbaoCreateOrEditForm");
                var frmValidatorForm = angular.element(document.querySelector('#thongbaoCreateOrEditForm'));
                var formValidation = frmValidatorForm.data('formValidation').validate();
                if (formValidation.isValid()) {
                    vm.saving = true;
                    thongBaoService.createOrUpdate(vm.thongbao).then(function (result) {
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

                if (thongbao != null) {
                    vm.thongbao = thongbao;
                }
            };
            init();
        }
    ]);
})();