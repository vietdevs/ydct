(function () {
    appModule.controller('quanlyhoso.thutuc99.views.dangkyhoso.thanhToanModal', [
        '$scope', '$uibModalInstance', '$window', '$uibModal', 'dataItem',
        'abp.services.app.loaiHoSo', 'abp.services.app.xuLyHoSoDoanhNghiep99', 'appSession',
        function ($uibModalInstance, $window, $uibModal, dataItem,
            loaiHoSoService, xuLyHoSoDoanhNghiep,appSession) {

            var vm = this;
            vm.doanhnghiep = angular.copy(appSession.doanhNghiepInfo.doanhNghiep);
            vm.hoSo = angular.copy(dataItem);
            vm.loaiHoSo = {};

            //Nộp hồ sơ mới trực tiếp
            {
                vm.nopHoSoMoi = function () {
                    if (vm.hoSo && vm.hoSo.id > 0) {
                        vm.saving = true;
                        xuLyHoSoDoanhNghiep.chuyenHoSoKhongCanThanhToan(vm.hoSo.id)
                            .then(function (result) {
                                if (result) {
                                    abp.notify.success(app.localize('SavedSuccessfully'));
                                }
                                else {
                                    abp.notify.error(app.localize('Thanh toán thất bại'));
                                }
                                $uibModalInstance.close();
                            }).finally(function () {
                                vm.saving = false;
                            });
                    }
                };
            }

            //Xử lý thanh toán
            {

                vm.chuyenThanhToanViettel = function () {

                };

                vm.chuyenThanhToanKeypay = function () {
                    if (vm.hoSo && vm.hoSo.id > 0) {
                        var input = {
                            HoSoId: vm.hoSo.id
                        };
                        vm.saving = true;
                        xuLyHoSoDoanhNghiep.chuyenThanhToanKeyPay(input).then(function (res) {
                            if (res.data && res.data.statusCode == 1) {
                                if (res.data.url == '' || res.data.url == null) {
                                    abp.notify.error("Tài khoản cục/chi cục chưa được cấu hình thanh toán. Vui lòng liên hệ với cục/chi cục");
                                }
                                else {
                                    $window.location.href = res.data.url;
                                }
                            }
                            else {
                                abp.notify.error(res.data.description);
                            }
                            //if (result.result != '') {
                            //    var data = result.data;
                            //    var objectjson = JSON.parse(data);
                            //    if (objectjson.StatusCode == "2") {
                            //        $window.open(objectjson.Description, '_blank');
                            //    }
                            //    else {
                            //        abp.notify.success(app.localize(objectjson.Description));
                            //    }
                            //}
                            $uibModalInstance.close();
                        }).finally(function () {
                            vm.saving = false;
                        });
                    }                    
                };

                vm.chuyenKhoanModal = function (data) {
                    var modalInstance = $uibModal.open({
                        templateUrl: '~/App/quanlyhoso/thutuc99/views/1_dangkyhoso/modal/chuyenKhoanModal.cshtml',
                        controller: 'quanlyhoso.thutuc99.views.dangkyhoso.modal.chuyenKhoanModal as vm',
                        backdrop: 'static',
                        size: 'lg',
                        resolve: {
                            dataItem: data
                        }
                    });
                    modalInstance.result.then(function (result) {
                        $uibModalInstance.close();
                    });
                };
            }

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };

            var init = function () {
                if (vm.hoSo.loaiHoSoId) {
                    loaiHoSoService.getById(vm.hoSo.loaiHoSoId).then(function (result) {
                        if (result.data) {
                            vm.loaiHoSo = result.data;
                        }
                    });
                }
                else {
                    console.error(vm.hoSo.loaiHoSoId, 'vm.hoSo.loaiHoSoId');
                }
            };
            init();
        }
    ]);
})();