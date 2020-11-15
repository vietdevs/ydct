(function () {
    appModule.controller('quanlydoanhnghiep.views.danhmucdoanhnghiep.detailModal', [
        '$scope', '$uibModal', '$location', '$state', '$uibModalInstance', 'detailData', 'appSession', 'abp.services.app.doanhNghiep', 'abp.services.app.user',
        function ($scope, $uibModal, $location, $state, $uibModalInstance, detailData, appSession, doanhNghiepService, userService) {
            var vm = this;
            vm.duyetDoanhNghiep = true;
            vm.lyDoKhongDuyet = '';
            vm.dataForm = angular.copy(detailData);
            vm.tinhOptions = {
                dataSource: appSession.get_tinh(),
                dataValueField: "id",
                dataTextField: "ten",
                optionLabel: "Chọn ...",
                filter: "startswith",
                dataBound: function () {
                    this.enable(false);
                }
            };
            vm.huyenOptions = {
                dataSource: appSession.get_huyen(),
                dataValueField: "id",
                dataTextField: "ten",
                optionLabel: "Chọn ...",
                filter: "contains",
                dataBound: function () {
                    this.enable(false);
                }
            };
            vm.xaOptions = {
                dataSource: appSession.get_xa(),
                dataValueField: "id",
                dataTextField: "ten",
                optionLabel: "Chọn ...",
                filter: "contains",
                dataBound: function () {
                    this.enable(false);
                }
            }
            vm.loaiHinhOptions = {
                dataSource: appSession.get_loaihinh(),
                dataValueField: "id",
                dataTextField: "tenLoaiHinh",
                optionLabel: "Chọn ...",
                filter: "contains",
            }

            vm.chucVuOptions = {
                dataSource: appSession.get_chucvu(),
                dataValueField: "id",
                dataTextField: "name",
                optionLabel: "Chọn ...",
                filter: "contains",
            }

            vm.moTaiKhoan = function (data) {
                vm.saving = true;
                doanhNghiepService.moTaiKhoanDoanhNghiep(data.id).then(function (result) {
                    vm.saving = false;
                    abp.notify.success(app.localize('SavedSuccessfully'));
                    $uibModalInstance.close();
                });
            }

            vm.khongChapNhan = function (data) {
                vm.saving = true;
                var input = {
                    Id: data.id,
                    LyDoKhongDuyet: vm.lyDoKhongDuyet
                }
                doanhNghiepService.khongChapNhanDangKyDoanhNghiep(input).then(function (result) {
                    vm.saving = false;
                    abp.notify.info(app.localize('SavedSuccessfully'));
                    $uibModalInstance.close();
                });
            }

            vm.save = function (data) {
                var flag = vm.duyetDoanhNghiep || (vm.duyetDoanhNghiep == false && vm.lyDoKhongDuyet != null && vm.lyDoKhongDuyet != '');
                if (!flag) {
                    abp.notify.error('Hãy nhập dữ liệu');
                }
                else {
                    if (vm.duyetDoanhNghiep == true) {
                        vm.moTaiKhoan(data);
                    }
                    else {
                        vm.khongChapNhan(data);
                    }
                }
            }

            vm.xemPDF = function (data) {
                var viewData = {
                    pathPDF: data.duongDanTep,
                    title: data.moTaTep
                }
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/quanlyhoso/_common/modal/viewFilePDFModal.cshtml',
                    controller: 'quanlyhoso.common.modal.viewFilePDFModal as vm',
                    backdrop: 'static',
                    resolve: {
                        modalData: viewData
                    },
                });
                modalInstance.result.then(function (result) {
                });
            }

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };
        }
    ]);
})();