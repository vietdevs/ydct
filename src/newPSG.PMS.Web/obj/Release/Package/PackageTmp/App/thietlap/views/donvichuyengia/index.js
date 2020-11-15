(function () {
    appModule.controller('thietlap.views.setting.donvichuyengia.index', [
        '$scope', '$uibModal', '$interval', '$filter', 'abp.services.app.donViChuyenGia',
        function ($scope, $uibModal, $interval, $filter, donViChuyenGiaService) {
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
                        '    <button class="btn btn-xs green-jungle" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em> ' + "Thao Tác" + ' <span class="caret"></span></button>' +
                        '    <ul uib-dropdown-menu>' +
                        '<li><a ng-click="vm.showCreateOrEditModal(this.dataItem.id)">' + "Sửa" + '</a></li>' +
                        '<li><a ng-click="vm.delete(this.dataItem.id)">' + "Xóa" + '</a></li>' +
                        '    </ul>' +
                        '  </div>' +
                        '</div>',
                    width: 150
                },
                {
                    field: "name",
                    title: "Tên đơn vị",
                    width: 400
                },
                {
                    title: "Trưởng đơn vị",
                    template: `<div>{{this.dataItem.tenTruongDonVi}}</div>`,
                    width: 200
                },
                {
                    title: "Địa chỉ",
                    template: `<div>{{this.dataItem.strXa}}, {{this.dataItem.strHuyen}}, {{this.dataItem.strTinh}}</div>`,
                    width: 400,
                },
            ];

            vm.getDataGrids = function () {
                vm.gridDataSource = new kendo.data.DataSource({
                    transport: {
                        read: function (options) {
                            vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                            vm.requestParams.maxResultCount = options.data.pageSize;

                            donViChuyenGiaService.getAllServerPaging($.extend(vm.filter, vm.requestParams))
                                .then(function (result) {
                                    console.log(result.data);
                                    vm.gvCallBack = options;
                                    vm.dataSource = result.data;
                                    var i = 1;
                                    result.data.items.forEach(function (item) {
                                        item.STT = i;
                                        i++;
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

            vm.showCreateOrEditModal = function (id) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/thietlap/views/donvichuyengia/createOrEditModal.cshtml',
                    controller: 'thietlap.views.donvichuyengia.createOrEditModal as vm',
                    backdrop: 'static',
                    resolve: {
                        data: {
                            id: id
                        }
                    }
                });

                modalInstance.result.then(function (result) {
                    console.log(result);
                    if (result)
                        vm.refreshGrid();
                });
            }

            vm.delete = function (id) {
                abp.message.confirm(
                    "", "Chắc chắn xóa?",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            donViChuyenGiaService.delete({ id: id }).then(function (result) {
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