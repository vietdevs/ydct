(function () {
    appModule.controller('danhmuc.views.chucdanh.index', [
        '$scope', '$uibModal', '$stateParams', 'uiGridConstants', 'abp.services.app.chucVu', 'appSession',
        function ($scope, $uibModal, $stateParams, uiGridConstants, chucVuService, appSession) {
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
                Filter: '',
            };

            //controll
            vm.chucVuColumns = [
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
                        '<li><a ng-click="vm.suaChucDanh(this.dataItem)">' + "Sửa" + '</a></li>' +
                        '<li><a ng-click="vm.xoaChucDanh(this.dataItem)">' + "Xóa" + '</a></li>' +
                        '    </ul>' +
                        '  </div>' +
                        '</div>',
                    width: 150
                },
                {
                    field: "tenChucVu",
                    title: "Tên chức vụ",
                    width: "300px",
                },
                {
                    field: "moTa",
                    title: "Mô tả",
                },
                {
                    field: "isActive",
                    title: "Trạng Thái",
                    template: '<p style="margin: 0;text-align:center;"><i ng-if="#: isActive # == 1" class="fa fa-check fa-3 font-green-meadow" aria-hidden="true"></i><i ng-if="#: isActive # == 0" class="fa fa-times fa-3 font-red"></i></p>',
                    width: 120,
                }
            ];

            vm.chucVuGridOptions = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;
                        vm.loading = true;
                        chucVuService.getAllServerPaging($.extend({ filter: vm.filter.Filter }, vm.requestParams))
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

            //function
            vm.getGridData = function () {
                vm.chucVuGridOptions.read();
            };

            vm.taoChucDanh = function () {
                openCreateOrEditChucVuModal(null);
            };

            vm.suaChucDanh = function (chucdanh) {
                var chucdanhCopy = angular.copy(chucdanh);
                openCreateOrEditChucVuModal(chucdanhCopy);
            }

            vm.xoaChucDanh = function (chucdanh) {
                abp.message.confirm(
                    app.localize('DeleteChucDanhWarningMessage', chucdanh.tenChucVu),
                    "Chắc chắn xóa?",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            chucVuService.delete(chucdanh.id).then(function () {
                                vm.getGridData();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                        }
                    }
                );
            };

            function openCreateOrEditChucVuModal(chucdanh) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/danhmuc/views/chucdanh/createOrEditModal.cshtml',
                    controller: 'danhmuc.views.chucdanh.createOrEditModal as vm',
                    backdrop: 'static',
                    resolve: {
                        chucdanh: chucdanh
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