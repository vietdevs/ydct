(function () {
    appModule.controller('website.views.cauhinhchung.common', [
        '$rootScope', '$scope', '$uibModal', '$stateParams', 'uiGridConstants', 'abp.services.app.cauHinhChung', 'appSession',
        function ($rootScope, $scope, $uibModal, $stateParams, uiGridConstants, cauHinhChungService, appSession) {
            //variable
            var vm = this;
            vm.cauHinhChungs = [];
            vm.group = [];
            var init = function () {
                abp.services.app.cauHinhChung.getAll()
                    .done(function (result) {
                        vm.cauHinhChungs = result;
                    });
                vm.group = [{ title: "Thông tin chung", value: 1 }, { title: "Thông tin liên hệ", value: 2 }, { title: "Cài đặt mã nhúng", value: 3 }, { title: "Cài đặt hiển thị", value: 4 }];
            }
            init();
            vm.ChooseImage = function (id) {
                var finder = new CKFinder();
                finder.selectActionFunction = function (fileUrl) {
                    $scope.$apply(function () {
                        document.getElementById(id).value = fileUrl;
                    })
                }
                finder.popup();
            }
            vm.save = function () {
                vm.saving = true;
                cauHinhChungService.updateAll(vm.cauHinhChungs).then(function (result) {
                    abp.notify.success(app.localize('SavedSuccessfully'));
                    $uibModalInstance.close();
                }).finally(function () {
                    vm.saving = false;
                });
            }
            vm.add = function () {
                openCreateOrEditCauHinhChungModal(null);
            };
            vm.changeTab = function (tab) {
                $('.tab-pane').removeClasss('active');
            }
            function openCreateOrEditCauHinhChungModal(cauHinhChung) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/website/views/cauhinhchung/createOrEditModal.cshtml',
                    controller: 'website.views.cauhinhchung.createOrEditModal as vm',
                    backdrop: 'static',
                    resolve: {
                        cauHinhChung: cauHinhChung
                    }
                });

                modalInstance.result.then(function (result) {
                    init();
                });
            }
            $scope.$on('$viewContentLoaded', function () {
                App.initAjax();
            });
        }
    ]);
})()