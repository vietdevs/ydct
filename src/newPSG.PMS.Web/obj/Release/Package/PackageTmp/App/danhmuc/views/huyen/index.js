(function () {
    appModule.controller('danhmuc.views.huyen.index', [
        '$scope', '$uibModal', '$stateParams', 'uiGridConstants', 'abp.services.app.huyen', 'abp.services.app.tinh', 'appSession',
        function ($scope, $uibModal, $stateParams, uiGridConstants, huyenService, tinhService, appSession) {
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
                dataTextField: "ten",
                filter: "startswith",
                optionLabel: "Chọn tỉnh thành",
            }

            vm.initGridHoSo = function () {
                vm.filter = {
                    page: 1,
                    pageSize: 10,
                    Filter: null,
                    tinhId: null,
                };
                vm.huyenGridOptions.read();
            }

            vm.huyenColumns = [
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
                        '<li><a ng-click="vm.suaHuyen(this.dataItem)">' + "Sửa" + '</a></li>' +
                        '<li><a ng-click="vm.xoaHuyen(this.dataItem)">' + "Xóa" + '</a></li>' +
                        '    </ul>' +
                        '  </div>' +
                        '</div>',
                    width: 150
                },
                {
                    field: "niisId",
                    title: "Mã Huyện",
                    width: "100px",
                },
                {
                    field: "ten",
                    title: "Huyện",
                },
                {
                    field: "strTinh",
                    title: "Tỉnh/Thành phố",
                },
                {
                    field: "isActive",
                    title: "Trạng Thái",
                    template: '<p style="margin: 0;text-align:center;"><i ng-if="#: isActive # == 1" class="fa fa-check fa-3 font-green-meadow" aria-hidden="true"></i><i ng-if="#: isActive # == 0" class="fa fa-times fa-3 font-red"></i></p>',
                    width: 120,
                }

            ];

            vm.huyenGridOptions = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;
                        vm.loading = true;
                        huyenService.getAllServerPaging($.extend({ filter: vm.filter.Filter, tinhId: vm.filter.tinhId }, vm.requestParams))
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
                vm.huyenGridOptions.read();
            };

            vm.taoHuyen = function () {
                openCreateOrEditHuyenModal(null, false);
            };

            vm.suaHuyen = function (huyen) {
                var huyenCopy = angular.copy(huyen);
                openCreateOrEditHuyenModal(huyenCopy, true
                );
            }

            vm.xoaHuyen = function (huyen) {
                abp.message.confirm(
                    app.localize('DeleteHuyenWarningMessage', huyen.ten),
                    "Chắc chắn xóa?",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            huyenService.delete(huyen.id).then(function () {
                                vm.getGridData();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                        }
                    }
                );
            };

            function openCreateOrEditHuyenModal(huyen, isUpdate) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/danhmuc/views/huyen/createOrEditModal.cshtml',
                    controller: 'danhmuc.views.huyen.createOrEditModal as vm',
                    backdrop: 'static',
                    resolve: {
                        huyen: huyen,
                        isUpdate: isUpdate
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