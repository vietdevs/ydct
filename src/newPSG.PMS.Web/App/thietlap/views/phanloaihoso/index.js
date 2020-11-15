(function () {
    appModule.controller('thietlap.views.phanloaihoso.index', [
        '$scope', '$uibModal', '$interval', '$filter', 'abp.services.app.phanLoaiHoSo', 'quanlyhoso.common.services.appCustom',
        function ($scope, $uibModal, $interval, $filter, phanLoaiHoSoService, appCustom) {
            var vm = this;
            vm.saving = false;
            vm.filterDefault = {
                page: 1,
                pageSize: 10,
                filter: null,
                thuTucId: null,
            };
            vm.filter = angular.copy(vm.filterDefault);
            vm.ROLE_LEVEL = app.ROLE_LEVEL;

            vm.requestParams = {
                skipCount: 0,
                maxResultCount: 10,
                sorting: null
            };

            vm.gridOptions = {
                pageable:
                {
                    "refresh": true,
                    "pageSizes": true,
                    messages: {
                        empty: "Không có dữ liệu",
                        display: "Tổng {2} loại hồ sơ",
                        itemsPerPage: "Loại hồ sơ mỗi trang"
                    }
                },
                resizable: true,
                scrollable: true,
            }
            vm.columnGrid = [
                {
                    field: "STT",
                    title: app.localize('STT'),
                    attributes: { class: "text-center" },
                    headerAttributes: { style: "text-align: center;" },
                    width: 60,
                    template: "<div align='center'>{{this.dataItem.STT}}</div>"
                },
                {
                    field: "",
                    title: "Thao tác",
                    template: `
                        <div class=\"text-center\">
                            <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                                <button class="btn btn-xs blue-steel" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em>Thao Tác<span class="caret"></span></button>
                                <ul uib-dropdown-menu>
                                    <li><a ng-click="vm.openLoaiHoSoModal(this.dataItem)">Sửa</a></li>
                                    <li><a ng-click="vm.deleteLoaiHoSo(this.dataItem.id)">Xóa</a></li>
                                    <li><a ng-click="vm.openLoaiHoSo_FilterModal(this.dataItem)">Thiết lập điều kiện lọc hồ sơ</a></li>
                                    <li><a ng-click="vm.openLoaiHoSo_BienBanModal(this.dataItem)">Thiết lập loại biên bản, số người thẩm định</a></li>
                                </ul>
                            </div>
                        </div>`,
                    width: 130
                },
                {
                    field: "strThuTuc",
                    title: "Thủ tục",
                    width: 100
                },
                {
                    field: "strRoleLevel",
                    title: "Vai trò",
                    width: 150,
                },
                {
                    field: "name",
                    title: "Loại hồ sơ",
                    width: 200
                },
                {
                    field: "",
                    title: "Loại biên bản, số người thẩm định",
                    template: `
                         <div ng-if="this.dataItem.listLoaiHoSo_BienBan && this.dataItem.listLoaiHoSo_BienBan.length > 0"> 
                            <table class="table" style="width:100%;" ng-if="this.dataItem.roleLevel == vm.ROLE_LEVEL.CHUYEN_GIA">
                                <tr ng-repeat="item in this.dataItem.listLoaiHoSo_BienBan">
                                    <td>
                                        <a href="javascript:;" class="primary-link">LBB:</a> {{item.strLoaiBienBanThamDinh}}
                                    </td>
                                    <td>{{item.strTieuBan}}</td>
                                    <td style="width:140px;">
                                        <a href="javascript:;" class="primary-link">SL:</a>  {{item.soLuong}} {{this.dataItem.strRoleLevel}}
                                        </td>
                                    </tr>
                            </table>
                            <table class="table" style="width:100%;" ng-if="this.dataItem.roleLevel != vm.ROLE_LEVEL.CHUYEN_GIA">
                                <tr ng-repeat="item in this.dataItem.listLoaiHoSo_BienBan">
                                    <td>
                                        <a href="javascript:;" class="primary-link">LBB:</a> {{item.strLoaiBienBanThamDinh}}
                                    </td>
                                    <td style="width:140px;">
                                        <a href="javascript:;" class="primary-link">SL:</a>  {{item.soLuong}} {{this.dataItem.strRoleLevel}}
                                        </td>
                                    </tr>
                                </tr>
                            </table>
                        </div>
                    `,
                },
                {
                    field: "",
                    title: "Điều kiện lọc thỏa mãn",
                    template: `
                         <div ng-if="this.dataItem.listLoaiHoSo_Filter && this.dataItem.listLoaiHoSo_Filter.length > 0"> 
                            <table class="table" style="width:100%;">
                                <tr ng-repeat="item in this.dataItem.listLoaiHoSo_Filter">
                                    <td>{{item.name}}</td>
                                </tr>
                            </table>
                        </div>
                    `,
                    width: 220
                },
            ];

            vm.gridDataSource = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;
                        phanLoaiHoSoService.getAllServerPaging($.extend(vm.filter, vm.requestParams))
                            .then(function (result) {
                                vm.gvCallBack = options;
                                var i = vm.requestParams.skipCount + 1;
                                result.data.items.forEach(function (item) {
                                    item.STT = i;
                                    i++;

                                    item.strRoleLevel = appCustom.getStrRoleLevel(item.roleLevel);

                                    if (item.listLoaiHoSo_BienBan) {
                                        item.listLoaiHoSo_BienBan.forEach(function (bienBan) {
                                            bienBan.strTieuBan = appCustom.getStrTieuBan(bienBan.tieuBanEnum);
                                        });
                                    }

                                });
                                options.success(result.data);
                            }).finally(function () {
                                abp.ui.clearBusy();
                            });
                    }
                },
                pageSize: 10,
                serverPaging: true,
                sortable: true,
                selectable: false,
                schema: {
                    data: "items",
                    total: "totalCount"
                },
            });

            vm.initGridHoSo = function () {
                vm.filter = angular.copy(vm.filterDefault);
                vm.refreshGrid();
            }
            vm.refreshGrid = function () {
                vm.gridDataSource.transport.read(vm.gvCallBack);
            }

            vm.deleteLoaiHoSo = function (id) {
                abp.message.confirm(
                    "", "Chắc chắn xóa?",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            phanLoaiHoSoService.delete(id).then(function (result) {
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                                vm.refreshGrid();
                            });
                        }
                    }
                )
            };

            vm.openLoaiHoSoModal = function (dataItem) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/thietlap/views/phanloaihoso/directives/phanLoaiHoSo/index.cshtml',
                    controller: 'thietlap.views.phanloaihoso.directives.phanloaihoso.index as vm',
                    backdrop: 'static',
                    resolve: {
                        dataItem: dataItem
                    }
                });

                modalInstance.result.then(function (result) {
                    vm.refreshGrid();
                });
            }

            vm.openLoaiHoSo_FilterModal = function (dataItem) {
                abp.ui.setBusy();
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/thietlap/views/phanloaihoso/directives/phanLoaiHoSo_Filter/index.cshtml',
                    controller: 'thietlap.views.phanloaihoso.directives.phanloaihosofilter.index as vm',
                    backdrop: 'static',
                    size: "lg",
                    resolve: {
                        dataItem: dataItem
                    }
                });

                modalInstance.result.then(function (result) {
                    vm.refreshGrid();
                });
            }

            vm.openLoaiHoSo_BienBanModal = function (dataItem) {
                abp.ui.setBusy();
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/thietlap/views/phanloaihoso/directives/phanLoaiHoSo_PhanCong/index.cshtml',
                    controller: 'thietlap.views.phanloaihoso.directives.phanloaihosophancong.index as vm',
                    backdrop: 'static',
                    size: "md",
                    resolve: {
                        dataItem: dataItem
                    }

                });

                modalInstance.result.then(function (result) {
                    vm.refreshGrid();
                });
            }
        }
    ]);
})();