(function () {
    appModule.controller('app.quanlyhoso.thutuc37.views.phongbanphancong.model.tralaihosomodel', [
        '$sce', '$rootScope', 'appSession', 'quanlyhoso.thutuc37.services.appChuKySo', '$uibModalInstance', 'dataItem', '$uibModal', '$filter', 'abp.services.app.xuLyHoSoPhanCong37',
        function ($sce, $rootScope, appSession, appChuKySo, $uibModalInstance, dataItem, $uibModal, $filter, xuLyHoSoPhanCongService) {

            var vm = this;
            vm.dataItem = dataItem;

            vm.save = function () {
                if (vm.lyDoTraLai == null || vm.lyDoTraLai == '') {
                    abp.notify.error('Lý do trả lại không được từ chối');
                    return;
                }
                abp.message.confirm('', 'Chắc chắn trả lại', function (isConfirmed) {
                    if (isConfirmed) {
                        xuLyHoSoPhanCongService.phongBanTraLaiLanhDaoCuc(vm.dataItem.id, vm.lyDoTraLai).then(function (result) {
                            abp.notify.info('Trả hồ sơ thành công');
                            vm.cancel(1);
                        })
                    }
                })
            }

            vm.cancel = function (status) {
                if (status) {
                    $uibModalInstance.close();
                }
                else {
                    $uibModalInstance.dismiss();
                }
            }

            var init = () => {
                if (dataItem.id > 0) {
                    vm.dataItem = dataItem
                }
            }
            init();
        }
    ])

})();