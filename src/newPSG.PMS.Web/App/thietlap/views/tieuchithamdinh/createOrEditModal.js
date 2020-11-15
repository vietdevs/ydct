(function () {
    appModule.controller('thietlap.views.tieuchithamdinh.createOrEditModal', [
        '$scope', '$uibModalInstance', '$uibModal', 'abp.services.app.tieuChiThamDinh', 'dataItem', 'baseService', 'appSession',
        function ($scope, $uibModalInstance, $uibModal, tieuChiThamDinhService, dataItem, baseService, appSession) {
            var vm = this;
            vm.saving = false;
            vm.ROLE_LEVEL = app.ROLE_LEVEL;
            vm.dataItem = dataItem || { listNoiDung: [] };

            vm.isAdmin = true;
            if (appSession.user.roleLevel != null && appSession.user.roleLevel != app.ROLE_LEVEL.SA) {
                vm.isAdmin = false;
                vm.dataItem.roleLevel = appSession.user.roleLevel;
                vm.controlLoaiBienBan.setDataSource(vm.dataItem.roleLevel);
            }

            vm.save = function () {
                let frmValidatorForm = app.checkValidateForm("#dataItemCreateOrEditForm");
                if (!frmValidatorForm || vm.dataItem.listNoiDung.length <= 0) {
                    abp.notify.error('Mời bạn nhập dữ liệu !!!');
                    return;
                }

                vm.saving = true;

                vm.dataItem.listNoiDung.forEach((item, idx) => {
                    item.thuTucId = vm.dataItem.thuTucId;
                    item.roleLevel = vm.dataItem.roleLevel;
                    item.tieuBanEnum = vm.dataItem.tieuBanEnum;
                    item.loaiBienBanThamDinhId = vm.dataItem.loaiBienBanThamDinhId;
                });

                tieuChiThamDinhService.createOrUpdate(vm.dataItem.listNoiDung).then(function (result) {
                    abp.notify.success(app.localize('SavedSuccessfully'));
                    // $uibModalInstance.close();
                    resetTable();
                }).finally(function () {
                    //vm.saving = false;
                });
            };

            var resetTable = () => {
                let pram = {
                    thuTucId: vm.dataItem.thuTucId,
                    roleLevel: vm.dataItem.roleLevel,
                    tieuBanEnum: vm.dataItem.tieuBanEnum,
                    loaiBienBanThamDinhId: vm.dataItem.loaiBienBanThamDinhId,
                };
                tieuChiThamDinhService.getTieuChiThamDinhCustom(pram).then(function (result) {
                    vm.dataItem.listNoiDung = app.orderByNoiDungThamDinh(result.data);
                }).finally(function () {
                    vm.saving = false;
                });
            }
            
            if (vm.dataItem.listNoiDung.length > 0) {
                vm.dataItem.listNoiDung = app.orderByNoiDungThamDinh(vm.dataItem.listNoiDung);
            }

            vm.changeRole = (dataItem) => {
                vm.controlLoaiBienBan.setDataSource(dataItem.id);
            }

            vm.ROLE_LEVELOptions = {
                dataSource: {
                    data: [
                        {
                            id: 1,
                            name: "Cấp 1"
                        },
                        {
                            id: 2,
                            name: "Cấp 2"
                        },
                        {
                            id: 3,
                            name: "Cấp 3"
                        },
                        {
                            id: 4,
                            name: "Cấp 4"
                        },
                        {
                            id: 5,
                            name: "Cấp 5"
                        }
                    ]
                },
                dataValueField: "id",
                dataTextField: "name",
                filter: "contains",
                select: function (e) {
                    var dataItem = this.dataItem(e.item);
                }
            };

            vm.addTree = (item) => {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/thietlap/views/tieuchithamdinh/createOrEditTreeModal.cshtml',
                    controller: 'thietlap.views.tieuchithamdinh.createOrEditTreeModal as vm',
                    backdrop: 'static',
                    resolve: {
                        dataItem: angular.copy(item)
                    }
                });

                modalInstance.result.then(function (result) {
                    resetTable();
                });
            }

            vm.add = function () {
                var level_1 = vm.dataItem.listNoiDung.filter(x => x.level == 1);
                vm.dataItem.listNoiDung.push({
                    stt: level_1.length + 1,
                    isValidate: true,
                    isTieuDe: false
                });
            }

            vm.remove = function (item) {
                if (vm.dataItem.listNoiDung.length > 0) {
                    var index = vm.dataItem.listNoiDung.indexOf(item);
                    vm.dataItem.listNoiDung.splice(index, 1);
                }
                vm.dataItem.listNoiDung = app.orderByNoiDungThamDinh(vm.dataItem.listNoiDung);
            }

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };
        }
    ]);
})();