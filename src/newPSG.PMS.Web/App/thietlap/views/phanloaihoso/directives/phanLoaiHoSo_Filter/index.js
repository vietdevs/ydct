(function () {
    appModule.controller('thietlap.views.phanloaihoso.directives.phanloaihosofilter.index', [
        '$scope', '$uibModal', '$uibModalInstance', 'abp.services.app.phanLoaiHoSo_Filter', 'dataItem', 'baseService',
        function ($scope, $uibModal, $uibModalInstance, phanLoaiHoSo_FilterService, dataItem, baseService) {

            var vm = this;

            var initVar = () => {
                vm.setting_LoaiHoSo = dataItem || {};
                vm.dataIsChange = false;
            };

            var initService = () => {
                abp.ui.setBusy();
                let pram = {
                    phanLoaiHoSoId: vm.setting_LoaiHoSo.id
                }
                phanLoaiHoSo_FilterService.getLoaiHoSo_Filter(pram).then(function (result) {
                    result.data.forEach(function (item) {
                        item.filter = JSON.parse(item.jsonFilter);
                    });
                    vm.listSetting_LoaiHoSo_Filter = result.data;
                }).finally(function () {
                    abp.ui.clearBusy();
                });
            };

            var initFun = () => {
                vm.openModal = function (dataItem) {
                    let dataSent = dataItem ? dataItem :
                        {
                            phanLoaiHoSoId: vm.setting_LoaiHoSo.id,
                            filter: vm.setting_LoaiHoSo.filter
                        };

                    var modalInstance = $uibModal.open({
                        templateUrl: '~/App/thietlap/views/phanloaihoso/directives/phanLoaiHoSo_Filter/filterEditModal.cshtml',
                        controller: 'thietlap.views.phanloaihoso.directives.phanloaihosofilter.filterEditModal as vm',
                        backdrop: 'static',
                        size: "lg",
                        resolve: {
                            dataItem: dataSent
                        }
                    });

                    modalInstance.result.then(function (result) {
                        initService();
                    });
                }

                vm.delete = function (dataItem) {
                    abp.message.confirm(
                        "", "Chắc chắn xóa?",
                        function (isConfirmed) {
                            if (isConfirmed) {
                                phanLoaiHoSo_FilterService.delete(dataItem.id).then(function (result) {
                                    abp.notify.success(app.localize('SuccessfullyDeleted'));
                                    initService();
                                });
                            }
                        }
                    )
                };
           
                vm.cancel = function () {
                    $uibModalInstance.close(vm.dataIsChange);
                };
            };

            var init = () => {
                initVar();
                initService();
                initFun();
            };
            init();
        }
    ]);
})();