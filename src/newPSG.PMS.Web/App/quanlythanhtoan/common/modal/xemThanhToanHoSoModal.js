(function () {
    appModule.controller('quanlythanhtoan.common.modal.xemThanhToanHoSoModal',
        ['$rootScope', '$scope', '$location', '$state', '$uibModalInstance', 'hoSo', 'abp.services.app.commonLookup',
            'abp.services.app.thanhToan', 'baseService', 'appSession',
            function ($rootScope, $scope, $location, $state, $uibModalInstance, hoSo, commonLookupAppService,
                thanhToanService, baseService, appSession) {
                var vm = this;
                var ROLE_DOANH_NGHIEP = 1;
                vm.choPhepXacNhanThanhToan = (appSession.user.roleLevel != ROLE_DOANH_NGHIEP);
                vm.loading = false;

                vm.BAT_DAU_GIAO_DICH = 1;
                vm.GIAO_DICH_DANG_CHO_KET_QUA = 2;
                vm.GIAO_DICH_THANH_CONG = 3;

                vm.KENH_THANH_TOAN = {
                    KENH_VIETTEL: 1,
                    KENH_KEYPAY: 2,
                    HINH_THUC_CHUYEN_KHOAN: 3,
                };
                vm.hoSo = angular.copy(hoSo);
                vm.saving = false;

                vm.cancel = function () {
                    $uibModalInstance.dismiss();
                };

                vm.arrThanhToan = [];
                vm.LoadThanhToan = function () {
                    var _thuTucId = vm.hoSo.thuTucId;
                    var _hoSoId = vm.hoSo.id;
                    vm.loading = true;
                    thanhToanService.getListThanhToanByHoSoId(_thuTucId, _hoSoId).then(function (result) {
                        if (result && result.data) {
                            vm.arrThanhToan = result.data;
                        }
                    }).finally(function () {
                        vm.loading = false;
                    });
                }

                function init() {
                    vm.LoadThanhToan();
                }
                init();

                vm.QueryBildToKeypay = function (thanhtoan) {
                    if (thanhtoan && thanhtoan.id > 0) {
                        try {
                            thanhToanService.queryBildToKeypayV2(thanhtoan.id).then(function (result) {
                                if (result) {
                                    if (result.data && result.data.status == 2) {
                                        abp.notify.warn("Hồ sơ đã được thanh toán!");
                                    } else if (result.data && result.data.status == 1) {
                                        if (result.data.statusCode == "00")
                                            abp.notify.success("Cập nhật thanh toán thành công!");
                                        else
                                            abp.notify.warn("Giao dịch đang trong quá trình xử lý hoặc giao dịch lỗi!");
                                        $rootScope.$broadcast('refreshGridHoSo', 'ok');
                                        $uibModalInstance.close();
                                    } else {
                                        abp.notify.error('Không lấy được dữ liệu thanh toán!');
                                    }
                                }
                            })
                                .finally(function () {
                                });
                        }
                        catch (ex) {
                            abp.notify.error('Có lỗi xảy ra, vui lòng xem console.log');
                            console.log(ex, 'ex');
                        }
                    }
                }
            }
        ]);
})();