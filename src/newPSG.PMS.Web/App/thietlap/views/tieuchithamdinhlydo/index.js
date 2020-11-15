(function () {
    appModule.controller('thietlap.views.setting.tieuchithamdinhlydo.index', [
        '$scope', '$uibModal', '$interval', '$filter', 'abp.services.app.tieuChiThamDinhLyDo', 'quanlyhoso.common.services.appCustom',
        function ($scope, $uibModal, $interval, $filter, tieuChiThamDinhLyDoService, appCustom) {
            var vm = this;
            vm.saving = false;
            vm.filterDefault = {
                page: 1,
                pageSize: 10,
                filter: "",
            };
            vm.filter = angular.copy(vm.filterDefault);

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
                        display: "Tổng {2} hồ sơ",
                        itemsPerPage: "Hồ sơ mỗi trang"
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
                    width: "60px",
                    template: "<div align='center'>{{this.dataItem.STT}}</div>"
                },
                {
                    field: "",
                    title: "",
                    template: '<div class=\"text-center\">' +
                        '  <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>' +
                        '    <button class="btn btn-xs blue-steel" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em> ' + "Thao Tác" + ' <span class="caret"></span></button>' +
                        '    <ul uib-dropdown-menu>' +
                        '<li><a ng-click="vm.view(this.dataItem)">' + "Xem nội dung" + '</a></li>' +
                        '<li><a ng-click="vm.showCreateOrEditModal(this.dataItem)">' + "Sửa" + '</a></li>' +
                        '<li><a ng-click="vm.delete(this.dataItem)">' + "Xóa" + '</a></li>' +
                        '    </ul>' +
                        '  </div>' +
                        '</div>',
                    width: 150
                },
                {
                    title: "Thủ tục",
                    width: 90,
                    template: "TT - {{this.dataItem.thuTucId}}"
                },
                {
                    field: "strRoleLevel",
                    title: "Vai trò",
                    width: 150,
                },
                {
                    field: "strTieuBan",
                    title: "Tiểu ban",
                    width: 170,
                },
                {
                    field: "tieuDeThamDinh",
                    title: "Tiêu chí lý do",
                    width: 170,
                },
                {
                    title: "Tiêu chí lý do",
                    template: `
                            <div>
                                <ul style="list-style-type: square;">
                                    <li ng-repeat="item in this.dataItem.listLyDoSplice" ng-bind-html="item"></li>
                                </ul>
                            </div>`
                },
            ];

            vm.getDataGrids = function () {
                vm.gridDataSource = new kendo.data.DataSource({
                    transport: {
                        read: function (options) {
                            vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                            vm.requestParams.maxResultCount = options.data.pageSize;

                            tieuChiThamDinhLyDoService.getAllServerPaging($.extend(vm.filter, vm.requestParams))
                                .then(function (result) {
                                    result.data.items.forEach(function (item, idx) {
                                        item.STT = idx + 1;

                                        item.listLyDoSplice = [];
                                        item.listLyDo.forEach((noiDung, idxND) => {
                                            if (idxND < 3) {
                                                item.listLyDoSplice.push("<em class='fa fa-caret-right'></em> " + noiDung.lyDo);
                                            }
                                        });
                                        if (item.listLyDoSplice.length > 0) {
                                            item.listLyDoSplice.push("  ...");
                                        }
                                        item.strRoleLevel = appCustom.getStrRoleLevel(item.roleLevel);
                                        item.strTieuBan = appCustom.getStrTieuBan(item.tieuBanEnum);
                                    });
                                    vm.gvCallBack = options;
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
            }

            var init = function () {
                vm.getDataGrids();
            }
            init();

            vm.initGridHoSo = function () {
                vm.filter = angular.copy(vm.filterDefault);
                vm.getDataGrids();
            }
            vm.refreshGrid = function () {
                vm.gridDataSource.transport.read(vm.gvCallBack);
            }

            vm.showCreateOrEditModal = function (dataItem) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/thietlap/views/tieuchithamdinhlydo/createOrEditModal.cshtml',
                    controller: 'thietlap.views.setting.tieuchithamdinhlydo.createOrEditModal as vm',
                    backdrop: 'static',
                    resolve: {
                        data: dataItem
                    }
                });

                modalInstance.result.then(function (result) {
                    vm.refreshGrid();
                });
            }

            vm.view = function (dataItem) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/thietlap/views/tieuchithamdinhlydo/_viewModal.cshtml',
                    controller: 'thietlap.views.tieuchithamdinhlydo.viewModal as vm',
                    backdrop: 'static',
                    resolve: {
                        dataItem: dataItem
                    }
                });
            };

            vm.delete = function (dataItem) {
                abp.message.confirm(
                    "", "Chắc chắn xóa?",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            tieuChiThamDinhLyDoService.delete(dataItem.tieuChiThamDinhId).then(function (result) {
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                                vm.refreshGrid();
                            });
                        }
                    }
                )
            };
        }
    ]);
})();