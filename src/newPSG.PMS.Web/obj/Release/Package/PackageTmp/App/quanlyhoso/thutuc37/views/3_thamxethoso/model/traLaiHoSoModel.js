(function () {
    appModule.controller('app.quanlyhoso.thutuc37.views.thamxethoso.model.tralaihosomodel', [
        '$sce', '$rootScope', 'appSession', 'quanlyhoso.thutuc37.services.appChuKySo', '$uibModalInstance', 'dataItem', '$uibModal', '$filter', 'abp.services.app.xuLyHoSoChuyenVien37',
        function ($sce, $rootScope, appSession, appChuKySo, $uibModalInstance, dataItem, $uibModal, $filter, xuLyHoSoChuyenVienService) {

            var vm = this;
            vm.dataItem = dataItem;

            vm.save = function () {
                if (vm.lyDoTraLai == null || vm.lyDoTraLai == '') {
                    abp.notify.error('Lý do trả lại không được từ chối');
                    return;
                }
                abp.message.confirm('', 'Chắc chắn trả lại', function (isConfirmed) {
                    if (isConfirmed) {
                        xuLyHoSoChuyenVienService.chuyenVienTraLaiPhongBan(vm.dataItem.id, vm.lyDoTraLai).then(function (result) {
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
                $uibModalInstance.dismiss();
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