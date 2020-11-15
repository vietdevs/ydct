(function () {
    appModule.controller('thietlap.views.setting.tieuchithamdinhlydo.createOrEditModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.tieuChiThamDinhLyDo', 'data', 'baseService', 'appSession',
        function ($scope, $uibModalInstance, tieuChiThamDinhLyDoService, data, baseService, appSession) {
            var vm = this;
            //Variable
            vm.saving = false;
            vm.dataModal = data || { listLyDo: [] };

            vm.noiDungThamDinhOptions = {
                dataSource: {
                    transport: {
                        read: function (options) {
                            abp.services.app.tieuChiThamDinh.getAll().then(function (result) {
                                vm.tieuChiThamDinh = result;
                                options.success(result);
                            });
                        }
                    }
                },
                dataValueField: "id",
                dataTextField: "tieuDeThamDinh",
                filter: "contains",
                optionLabel: "Chọn ...",
            }

            vm.save = function () {
                if (!app.checkValidateForm("#createOrEditForm")) {
                    abp.notify.error("Xin vui lòng xem lại", "Dữ liệu nhập chưa đúng!");
                    return;
                }

                vm.saving = true;
                vm.dataModal.listLyDo.forEach((item) => {
                    item.tieuChiThamDinhId = vm.dataModal.tieuChiThamDinhId;
                });

                tieuChiThamDinhLyDoService.createOrUpdate(vm.dataModal.listLyDo).then(function (result) {
                    abp.notify.success(app.localize('SavedSuccessfully'));
                    $uibModalInstance.close();
                }).finally(function () {
                    vm.saving = false;
                });
            };

            var init = function () {

            };

            vm.add = function () {
                vm.dataModal.listLyDo.push({
                    lyDo: null
                });
            }

            vm.remove = function (item) {
                if (vm.dataModal.listLyDo.length > 0) {
                    var index = vm.dataModal.listLyDo.indexOf(item);
                    vm.dataModal.listLyDo.splice(index, 1);
                }
            }

            vm.timKiem = () => {
                if (vm.tieuChiThamDinh.length > 0) {
                    let dataSource = angular.copy(vm.tieuChiThamDinh);
                    if (vm.filter.thuTucId) {
                        dataSource = dataSource.filter(x => x.thuTucId == vm.filter.thuTucId);
                    }
                    if (vm.filter.roleLevel) {
                        dataSource = dataSource.filter(x => x.roleLevel == vm.filter.roleLevel);
                    }
                    if (vm.filter.tieuBanEnum) {
                        dataSource = dataSource.filter(x => x.tieuBanEnum == vm.filter.tieuBanEnum);
                    }
                    if (vm.filter.loaiBienBanThamDinhId) {
                        dataSource = dataSource.filter(x => x.loaiBienBanThamDinhId == vm.filter.loaiBienBanThamDinhId);
                    }

                    var ddl = $("#TieuChiThamDinh").data("kendoDropDownList");
                    if (ddl) {
                        ddl.setDataSource(dataSource);
                    }
                }
            }

            vm.refesh = () => {
                vm.filter.thuTucId = null;
                vm.filter.roleLevel = null;
                vm.filter.tieuBanEnum = null;
                vm.filter.loaiBienBanThamDinhId = null;
                vm.timKiem();
            }

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };

            vm.isCollapsed = true;
            vm.checkCollapse = function () {
                vm.isCollapsed = angular.element("#nangcao").hasClass('collapse in');
            };

            init();
        }
    ]);
})();