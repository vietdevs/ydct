(function () {
    appModule.controller('danhmuc.views.thutuc.index', [
        '$scope', '$uibModal', '$stateParams', 'uiGridConstants', 'abp.services.app.thuTuc', 'appSession',
        function ($scope, $uibModal, $stateParams, uiGridConstants, thuTucService, appSession) {
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
            vm.loaiHoSoColumns = [
                {
                    field: "STT",
                    title: app.localize('STT'),
                    width: 50,
                    template: "<div align='center'>{{this.dataItem.STT}}</div>"
                },
                {
                    field: "",
                    title: "",
                    template: `<div class=\"ui-grid-cell-contents\">
                          <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>
                            <button class="btn btn-xs green-meadow" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><i class="fa fa-cog"></i>Thao Tác<span class="caret"></span></button>
                            <ul uib-dropdown-menu>
                                <li><a ng-click="vm.suaThuTuc(this.dataItem)">Sửa</a></li>
                                <li><a ng-click="vm.xoaThuTuc(this.dataItem)">Xóa</a></li>
                            </ul>
                          </div>
                        </div>`,
                    width: 120
                },
                {
                    field: "maThuTuc",
                    title: "Mã thủ tục",
                    width: 100
                },
                {
                    field: "tenThuTuc",
                    title: "Tên thủ tục",
                    width: 250
                },
                {
                    field: "moTa",
                    title: "Mô tả",
                },
                //{
                //    field: "soNgayXuLy",
                //    title: "Số ngày xử lý",
                //    width: 120,
                //    template: "<div align='center'>{{this.dataItem.soNgayXuLy}}</div>"
                //},
                //{
                //    field: "phiXuLy",
                //    title: "Phí",
                //    width: 110,
                //    template: "<div align='center' ng-if='this.dataItem.phiXuLy>0'>{{this.dataItem.phiXuLy | number}} VNĐ</div>"
                //},
                {
                    field: "isActive",
                    title: "Trạng Thái",
                    template: '<p style="margin: 0;text-align:center;"><i ng-if="#: isActive # == 1" class="fa fa-check fa-3 font-blue-steel" aria-hidden="true"></i><i ng-if="#: isActive # == 0" class="fa fa-times fa-3 font-red"></i></p>',
                    width: 120
                }
            ];

            vm.loaiHoSoGridOptions = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;
                        vm.loading = true;
                        thuTucService.getAllServerPaging($.extend({ filter: vm.filter.Filter }, vm.requestParams))
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
                vm.loaiHoSoGridOptions.read();
            };

            vm.taoLoaiHoSo = function () {
                openCreateOrEditLoaiHoSoModal(null);
            };

            vm.suaThuTuc = function (item) {
                var itemCopy = angular.copy(item);
                openCreateOrEditLoaiHoSoModal(itemCopy);
            };

            vm.xoaThuTuc = function (item) {
                abp.message.confirm(
                    app.localize('DeleteWarningMessage', item.tenThuTuc),
                    "Bạn chắc chắn muốn xóa?",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            thuTucService.delete(item.id).then(function () {
                                vm.getGridData();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                        }
                    }
                );
            };

            function openCreateOrEditLoaiHoSoModal(dataItem) {
                console.log('openCreateOrEditLoaiHoSoModal');

                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/danhmuc/views/thutuc/createOrEditModal.cshtml',
                    controller: 'danhmuc.views.thutuc.createOrEditModal as vm',
                    backdrop: 'static',
                    resolve: {
                        loaihoso: dataItem
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