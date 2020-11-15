(function () {
    appModule.controller('danhmuc.views.phongban.index', ['$scope', '$uibModal', '$stateParams', 'uiGridConstants', 'abp.services.app.phongBan', 'abp.services.app.commonLookup', 'appSession',
        function ($scope, $uibModal, $stateParams, uiGridConstants, phongBanService, commonLookupService, appSession) {
            //variable
            var vm = this;
            vm.loading = false;
            vm.requestParams = {
                skipCount: 0,
                maxResultCount: 10,
                sorting: null
            };
            vm.filter = {
                Filter: '',
                NhomSanPhamId: null,
                IsActive: true
            };

            //controll
            vm.nhomSanPhamOptions = {
                dataSource: appSession.nhomSanPham,
                dataValueField: "id",
                dataTextField: "name",
                filter: "contains",
                optionLabel: "Chọn nhóm sản phẩm",
            }

            vm.gridColumns = [
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
                        '<li><a ng-click="vm.editPhongBan(this.dataItem)">' + "Sửa" + '</a></li>' +
                        '<li><a ng-click="vm.deletePhongBan(this.dataItem)">' + "Xóa" + '</a></li>' +
                        '    </ul>' +
                        '  </div>' +
                        '</div>',
                    width: 150
                },
                {
                    field: "tenPhongBan",
                    title: "Tên phòng ban",
                },
                {
                    field: "isActive",
                    title: "Trạng Thái",
                    template: '<p style="margin: 0;text-align:center;"><i ng-if="#: isActive # == 1" class="fa fa-check fa-3 font-green-meadow" aria-hidden="true"></i><i ng-if="#: isActive # == 0" class="fa fa-times fa-3 font-red"></i></p>',
                    width: 120,
                }

            ];

            vm.gridDataSource = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;
                        vm.loading = true;
                        phongBanService.getAllServerPaging($.extend(vm.filter, vm.requestParams))
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
                schema: {
                    data: "items",
                    total: "totalCount"
                }
            });

            vm.gridOptions = {
                pageable:
                {
                    "refresh": true,
                    "pageSizes": true,
                    messages: {
                        empty: "Không có dữ liệu",
                        display: "Hiển thị từ {0} đến {1}/{2} bản ghi",
                        itemsPerPage: "bản ghi mỗi trang"
                    }
                },
                resizable: true,
                scrollable: true
            }

            //function
            vm.loadGridDataSource = function () {
                vm.gridDataSource.read();
            };

            function openCreateOrEditModal(item) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/danhmuc/views/phongban/createOrEditModal.cshtml',
                    controller: 'danhmuc.views.phongban.createOrEditModal as vm',
                    backdrop: 'static',
                    size: 'lg',
                    resolve: {
                        _model: item
                    }
                });

                modalInstance.result.then(function (result) {
                    vm.loadGridDataSource();
                });
            }

            vm.addPhongBan = function () {
                openCreateOrEditModal(null);
            };

            vm.editPhongBan = function (item) {
                var _item = angular.copy(item);
                openCreateOrEditModal(_item);
            }

            vm.deletePhongBan = function (item) {
                abp.message.confirm(
                    app.localize('DeleteWarningMessage', item.tenPhongBan),
                    "Bạn chắc chắn muốn xóa?",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            phongBanService.delete(item.id).then(function () {
                                vm.loadGridDataSource();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                        }
                    }
                );
            };
        }
    ]);
})()