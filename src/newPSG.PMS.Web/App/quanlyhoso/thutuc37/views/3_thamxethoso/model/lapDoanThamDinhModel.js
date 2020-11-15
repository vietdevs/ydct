(function () {
    appModule.controller('app.quanlyhoso.thutuc37.views.thamxethoso.model.lapdoanthamdinh', [
        '$sce', '$rootScope', 'appSession', 'quanlyhoso.thutuc37.services.appChuKySo', '$uibModalInstance', 'dataItem', '$uibModal', '$filter', 'abp.services.app.xuLyHoSoChuyenVien37',
        function ($sce, $rootScope, appSession, appChuKySo, $uibModalInstance, dataItem, $uibModal, $filter, xuLyHoSoChuyenVien37) {

            var vm = this;
            vm.thanhVienDoanThamDinh = [{
                userId: null
            }];
            vm.vaiTroThamDinhEnum = {
                TRUONG_DOAN: 0,
                THU_KY: 1,
                THANH_VIEN: 2
            }
            vm.listDoanThamDinh = {
                dataSource: {
                    transport: {
                        read: function (options) {
                            xuLyHoSoChuyenVien37.getAllThanhVienDoanThamDinh().then(function (result) {
                                options.success(result.data);
                            });
                        }
                    }
                },
                dataValueField: "id",
                dataTextField: "hoTen",
                optionLabel: app.localize('Chọn ...'),
                filter: "contains"
            };

            vm.vaiTroThamDinhOptions = {
                dataSource: {
                    transport: {
                        read: function (options) {
                            xuLyHoSoChuyenVien37.getVaiTroThamDinhToDDL().then(function (result) {
                                options.success(result.data);
                            });
                        }
                    }
                },
                dataValueField: "id",
                dataTextField: "name",
                optionLabel: app.localize('Chọn ...'),
                filter: "contains"
            };

            vm.onChangeTruongDoan = function (dataItem) {
                if (dataItem > 0) {
                    vm.thuKyFilter();
                    vm.thanhVienFilter();
                }
            };

            vm.onChangeThuKy = function (dataItem) {
                if (dataItem > 0) {
                    vm.truongDoanFilter();
                    vm.thanhVienFilter();
                }
            };

            vm.truongDoanFilter = function () {
                if (!vm.doanThamDinh) {
                    return;
                }
                var dataSourceFilter = vm.doanThamDinh.filter(function (element) {
                    return element.id != vm.thuKyId;
                });
                var selected = [];
                var controls = angular.element("select.thanhvien");
                if (controls && controls.length > 0) {
                    for (var i = 0; i < controls.length; i++) {
                        var ele = controls[i];
                        if (ele.value && ele.value > 0) {
                            selected.push(parseInt(ele.value)); //selected chưa list giá trị đã được chọn
                        }
                    }
                    dataSourceFilter = $filter('filter')(dataSourceFilter, function (el) {
                        var check = (selected.indexOf(el.id) == -1);
                        if (check) {
                            return el;
                        }
                    });
                }
                var truongDoanSource = $("#dropdownlisttruongdoan").data("kendoDropDownList");
                truongDoanSource.setDataSource(dataSourceFilter);
            };

            vm.thuKyFilter = function () {
                if (!vm.doanThamDinh) {
                    return;
                }

                var dataSourceFilter = vm.doanThamDinh.filter(function (element) {
                    return element.id != vm.truongDoanId;
                });

                var selected = [];
                var controls = angular.element("select.thanhvien");
                if (controls && controls.length > 0) {
                    for (var i = 0; i < controls.length; i++) {
                        var ele = controls[i];
                        if (ele.value && ele.value > 0) {
                            selected.push(parseInt(ele.value)); //selected chưa list giá trị đã được chọn
                        }
                    }
                    dataSourceFilter = $filter('filter')(dataSourceFilter, function (el) {
                        var check = (selected.indexOf(el.id) == -1);
                        if (check) {
                            return el;
                        }
                    });
                }

                var thuKySource = $("#dropdownlistthuky").data("kendoDropDownList");
                thuKySource.setDataSource(dataSourceFilter);
            };

            vm.onChangeThanhVien = function () {
                vm.thanhVienFilter();
                vm.truongDoanFilter();
                vm.thuKyFilter();
            };

            vm.thanhVienFilter = function () {
                if (!vm.doanThamDinh) {
                    return;
                }

                var dataSourceFilter = vm.doanThamDinh.filter(function (element) {
                    return element.id != vm.truongDoanId && element.id != vm.thuKyId;
                });
                var selected = [];
                var controls = angular.element("select.thanhvien");
                if (controls && controls.length > 0) {
                    for (var i = 0; i < controls.length; i++) {
                        var ele = controls[i];
                        if (ele.value && ele.value > 0) {
                            selected.push(parseInt(ele.value)); //selected chưa list giá trị đã được chọn
                        }
                    }
                    for (var j = 0; j < controls.length; j++) {
                        var e = controls[j];
                        var controlSource = $filter('filter')(dataSourceFilter, function (el) { //Loại bỏ các giá trị đã chọn ra khỏi datasource
                            var check = (selected.indexOf(el.id) == -1) || (el.id == e.value);
                            if (check) {
                                return el;
                            }
                        });
                        var control = $("#" + e.id); //gán datasource cho controll
                        var controlData = control.data("kendoDropDownList");
                        controlData.setDataSource(controlSource);
                    }
                }
            };

            vm.getAllListThanVienThamDinh = () => {
                xuLyHoSoChuyenVien37.getAllThanhVienDoanThamDinh().then(function (result) {
                    vm.doanThamDinh = result.data;
                });
            };
            vm.getAllListThanVienThamDinh();

            // function 

            vm.themThanhVien = function () {
                vm.thanhVienDoanThamDinh.push({ userId: null });
            };

            vm.removeThanhVienDoan = function (index) {
                vm.thanhVienDoanThamDinh.splice(index, 1);
            };

            vm.checkValidate = function () {
                if (!app.checkValidateForm("#FormLapDoanThamDinh")) {
                    abp.notify.error("Xin vui lòng xem lại", "Dữ liệu nhập chưa đúng!");
                    vm.saving = false;
                    return false;
                }
                
                return true;
            };

            vm.save = function () {
                var validated = vm.checkValidate();
                if (!validated) {
                    return;
                }
                let listHoSoDoanThamDinhObj = [
                    {
                        hoSoId: vm.dataItem.id,
                        userId: vm.thuKyId,
                        vaiTroEnum: vm.vaiTroThamDinhEnum.THU_KY
                    },
                    {
                        hoSoId: vm.dataItem.id,
                        userId: vm.truongDoanId,
                        vaiTroEnum: vm.vaiTroThamDinhEnum.TRUONG_DOAN
                    }
                ];
                
                vm.thanhVienDoanThamDinh.forEach(function (item) {
                    listHoSoDoanThamDinhObj.push({
                        hoSoId: vm.dataItem.id,
                        userId: item.userId,
                        vaiTroEnum: vm.vaiTroThamDinhEnum.THANH_VIEN
                    });
                });

                let input = {
                    hoSoId: vm.dataItem.id,
                    listHoSoDoanThamDinh: listHoSoDoanThamDinhObj
                }
                console.log(input, 'inputinput');
                //return;
                xuLyHoSoChuyenVien37.thanhLapDoanThamDinh(input).then(function (result) {
                    abp.notify.info(app.localize('Lập đoàn thẩm định thành công'));
                    $uibModalInstance.close();
                })
            }

            vm.cancel = function () {
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