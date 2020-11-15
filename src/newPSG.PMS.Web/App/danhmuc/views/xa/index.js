(function () {
    appModule.controller('danhmuc.views.xa.index', [
        '$scope', '$uibModal', '$stateParams', 'uiGridConstants', 'abp.services.app.xa', 'abp.services.app.huyen', 'abp.services.app.tinh', 'appSession',
        function ($scope, $uibModal, $stateParams, uiGridConstants, xaService, huyenService, tinhService, appSession) {
            //variable
            var vm = this;
            vm.loading = false;
            vm.requestParams = {
                skipCount: 0,
                maxResultCount: 10,
                sorting: null
            };
            vm.filter = {
                page: 1,
                pageSize: 10,
                Filter: null,
                tinhId: null,
                huyenId: null,
            };

            //controll
            vm.tinhOptions = {
                dataSource: {
                    transport: {
                        read: function (options) {
                            tinhService.getAllToDDL().then(function (result) {
                                options.success(result.data);
                            });
                        }
                    }
                },
                dataValueField: "id",
                dataTextField: "name",
                filter: "startswith",
                optionLabel: "Chọn tỉnh thành",
            }

            vm.huyenOptions = {
                dataSource: {
                    transport: {
                        read: function (options) {
                            vm.huyenCallBack = options;
                            if (vm.filter.tinhId) {
                                huyenService.getAllToDDL(vm.filter.tinhId).then(function (result) {
                                    options.success(result.data);
                                });
                            }
                        }
                    }
                },
                dataValueField: "id",
                dataTextField: "name",
                filter: "startswith",
                optionLabel: "Chọn quận huyện",
            }

            vm.changeTinh = function () {
                vm.huyenOptions.dataSource.transport.read(vm.huyenCallBack);
            }

            vm.xaColumns = [
                {
                    field: "STT",
                    title: app.localize('STT'),
                    width: "50px",
                    template: "<div align='center'>{{this.dataItem.STT}}</div>"
                },
                {
                    field: "",
                    title: "",
                    template: '<div class=\"ui-grid-cell-contents\">' +
                        '  <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>' +
                        '    <button class="btn btn-xs green-meadow" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><i class="fa fa-cog"></i> ' + "Thao Tác" + ' <span class="caret"></span></button>' +
                        '    <ul uib-dropdown-menu>' +
                        '<li><a ng-click="vm.suaXa(this.dataItem)">' + "Sửa" + '</a></li>' +
                        '<li><a ng-click="vm.xoaXa(this.dataItem)">' + "Xóa" + '</a></li>' +
                        '    </ul>' +
                        '  </div>' +
                        '</div>',
                    width: 150
                },
                {
                    field: "id",
                    title: "Mã",
                    width: "100px",
                    template: "<div align='center'>{{this.dataItem.id}}</div>"
                },
                {
                    field: "ten",
                    title: "Tên phường xã",
                },
                {
                    field: "strHuyen",
                    title: "Quận / Huyện",
                },
                {
                    field: "strTinh",
                    title: "Tỉnh thành",
                },
                {
                    field: "isActive",
                    title: "Trạng Thái",
                    template: '<p style="margin: 0;text-align:center;"><i ng-if="#: isActive # == 1" class="fa fa-check fa-3 font-green-meadow" aria-hidden="true"></i><i ng-if="#: isActive # == 0" class="fa fa-times fa-3 font-red"></i></p>',
                    width: 120,
                }

            ];

            vm.xaGridOptions = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;
                        vm.loading = true;
                        xaService.getAllServerPaging($.extend({ filter: vm.filter.Filter, tinhId: vm.filter.tinhId, huyenId: vm.filter.huyenId }, vm.requestParams))
                            .then(function (result) {
                                var i = 1;
                                result.data.items.forEach(function (item) {
                                    item.STT = i;
                                    i++;
                                });
                                vm.optionCallback = options;
                                options.success(result.data);
                            }).finally(function () {
                                vm.loading = false;
                            });
                    }
                },
                pageSize: 10,
                serverPaging: true,
                serverSorting: true,
                scrollable: true,
                sortable: true,
                pageable: {
                    refresh: true,
                    pageSizes: 10,
                    buttonCount: 5
                },
                schema: {
                    data: "items",
                    total: "totalCount"
                }
            });

            vm.getGridData = function () {
                vm.xaGridOptions.read();
            };

            vm.initGridHoSo = function () {
                vm.filter = {
                    page: 1,
                    pageSize: 10,
                    Filter: null,
                    tinhId: null,
                    huyenId: null,
                };
                vm.xaGridOptions.read();
            }

            vm.taoXa = function () {
                vm.openCreateOrEditXaModal(null);
            };

            vm.suaXa = function (xa) {
                var xaCopy = angular.copy(xa);
                vm.openCreateOrEditXaModal(xaCopy);
            }

            vm.xoaXa = function (xa) {
                abp.message.confirm(
                    app.localize('DeleteXaWarningMessage', xa.ten),
                    "Bạn chắc chắn muốn xóa?",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            xaService.delete(xa.id).then(function () {
                                vm.getGridData();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                        }
                    }
                );
            };

            vm.openCreateOrEditXaModal = (xa) => {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/danhmuc/views/xa/createOrEditModal.cshtml',
                    controller: 'danhmuc.views.xa.createOrEditModal as vm',
                    backdrop: 'static',
                    resolve: {
                        xa: xa
                    }
                });

                modalInstance.result.then(function (result) {
                    vm.getGridData();
                });
            }

            $scope.$on('$viewContentLoaded', function () {
                App.initAjax();
            });
        }
    ]);
})()