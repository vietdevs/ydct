(function () {
    appModule.controller('thietlap.views.phanloaihoso.directives.phanloaihosophancong.index', [
        '$scope', '$uibModalInstance', 'baseService', 'abp.services.app.phanLoaiHoSo_PhanCong', 'dataItem',
        function ($scope, $uibModalInstance, baseService, phanLoaiHoSo_PhanCongService, dataItem) {

            var vm = this;

            var initVar = () => {
                vm.ROLE_LEVEL = app.ROLE_LEVEL;
                vm.listSetting_LoaiHoSo_BienBan = [];
                vm.dataItem = dataItem ? dataItem : {};
            };

            var initService = () => {
                if (vm.dataItem.listLoaiHoSo_BienBan) {
                    if (vm.dataItem.listLoaiHoSo_BienBan.length > 0) {
                        vm.listSetting_LoaiHoSo_BienBan = vm.dataItem.listLoaiHoSo_BienBan;
                        vm.listSetting_LoaiHoSo_BienBan.forEach((item, idx) => {
                            vm.setDataSourceLoaiBienBan(idx);
                        });
                    }
                } else if (vm.dataItem.roleLevel != vm.ROLE_LEVEL.CHUYEN_GIA) {
                    vm.addRow();
                }
            };

            var initFun = () => {
                vm.deleteRow = function (index) {
                    vm.listSetting_LoaiHoSo_BienBan.splice(index, 1)
                };

                vm.addRow = function () {
                    let data = {
                        loaiHoSoPCId: vm.dataItem.id
                    }
                    vm.listSetting_LoaiHoSo_BienBan.push(data);

                    let index = vm.listSetting_LoaiHoSo_BienBan.length - 1;
                    vm.setDataSourceLoaiBienBan(index);
                }

                vm.setDataSourceLoaiBienBan = function (index) {
                    setTimeout(() => {
                        vm.controlLoaiBienBan.setDataSource(vm.dataItem.roleLevel, index);
                    }, 700);
                };

                vm.save = function () {
                    if (!app.checkValidateForm("#createOrEditForm")) {
                        abp.notify.error("Xin vui lòng xem lại", "Dữ liệu nhập chưa đúng!");
                        vm.saving = false;
                        return;
                    }

                    vm.saving = true;
                    let pram = {
                        phanLoaiHoSoId: vm.dataItem.id,
                        listPhanLoaiHoSo_PhanCong: vm.listSetting_LoaiHoSo_BienBan
                    };
                    phanLoaiHoSo_PhanCongService.createOrUpdate(pram).then(function (result) {
                        console.log(result.data);
                        if (result.data.severity == 1) {
                            abp.notify.success(result.data.message);
                            $uibModalInstance.close();
                        } else {
                            abp.notify.warn(result.data.message);
                        }
                    });
                }

                vm.cancel = function () {
                    $uibModalInstance.dismiss();
                };
            };

            var init = () => {
                initVar();
                initFun();
                initService();
                abp.ui.clearBusy();
            };
            init();
        }
    ]);
})();