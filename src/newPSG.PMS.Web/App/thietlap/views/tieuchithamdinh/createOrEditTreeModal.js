(function () {
    appModule.controller('thietlap.views.tieuchithamdinh.createOrEditTreeModal', [
        '$scope', '$uibModalInstance', 'abp.services.app.tieuChiThamDinh', 'dataItem', 'baseService', 'appSession',
        function ($scope, $uibModalInstance, tieuChiThamDinhService, dataItem, baseService, appSession) {
            var vm = this;
            vm.saving = false;
            vm.ROLE_LEVEL = app.ROLE_LEVEL;
            vm.dataItem = dataItem || { listNoiDung: [] };

            vm.isAdmin = true;
            if (appSession.user.roleLevel != null && appSession.user.roleLevel != app.ROLE_LEVEL.SA) {
                vm.isAdmin = false;
                vm.dataItem.roleLevel = appSession.user.roleLevel;
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
                    item.pId = vm.dataItem.id;
                });

                tieuChiThamDinhService.createOrUpdate(vm.dataItem.listNoiDung).then(function (result) {
                    abp.notify.success(app.localize('SavedSuccessfully'));
                    $uibModalInstance.close();
                }).finally(function () {
                    vm.saving = false;
                });
            };

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

            var init = () => {
                if (vm.dataItem.id > 0) {
                    tieuChiThamDinhService.getTieuChiThamDinhFormPId(vm.dataItem.id).then(function (result) {
                        vm.dataItem.listNoiDung = result.data;
                    });
                }
            }
            init();

            vm.add = function () {
                let stt = vm.dataItem.stt + "." + (vm.dataItem.listNoiDung.length + 1);
                vm.dataItem.listNoiDung.push({
                    stt: stt,
                    level: vm.dataItem.level + 1,
                    isValidate: true,
                    isTieuDe: false
                });
            }

            vm.remove = function (item) {
                if (vm.dataItem.listNoiDung.length > 0) {
                    var index = vm.dataItem.listNoiDung.indexOf(item);
                    vm.dataItem.listNoiDung.splice(index, 1);
                }
                vm.dataItem.listNoiDung.forEach((item, idx) => {
                    item.stt = idx + 1;
                });
            }

            vm.cancel = function () {
                $uibModalInstance.dismiss();
            };
        }
    ]);
})();