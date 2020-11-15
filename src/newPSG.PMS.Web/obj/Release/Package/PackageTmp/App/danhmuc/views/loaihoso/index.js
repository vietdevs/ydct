(function () {
    appModule.controller('danhmuc.views.loaihoso.index', [
        '$scope', '$uibModal', '$stateParams', 'uiGridConstants', 'abp.services.app.loaiHoSo', 'appSession',
        function ($scope, $uibModal, $stateParams, uiGridConstants, loaiHoSoService, appSession) {
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
                    template: '<div class=\"ui-grid-cell-contents\">' +
                        '  <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>' +
                        '    <button class="btn btn-xs green-meadow" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><i class="fa fa-cog"></i> ' + "Thao Tác" + ' <span class="caret"></span></button>' +
                        '    <ul uib-dropdown-menu>' +
                        '<li><a ng-click="vm.suaLoaiHoSo(this.dataItem)">' + "Sửa" + '</a></li>' +
                        '<li><a ng-click="vm.xoaLoaiHoSo(this.dataItem)">' + "Xóa" + '</a></li>' +
                        '<li><a ng-click="vm.cauHinhLoaiHoSo(this.dataItem)">' + "Cấu hình quá hạn" + '</a></li>' +
                        '    </ul>' +
                        '  </div>' +
                        '</div>',
                    width: 150
                },
                {
                    field: "tenLoaiHoSo",
                    title: "Tên loại hồ sơ",
                },
                {
                    field: "tenThuTuc",
                    title: "Thủ tục áp dụng",
                },
                {
                    field: "dataImage",
                    title: "Ảnh đại diện",
                    template: "<div align='center' ng-if='this.dataItem.dataImage!=null'><img ng-src='data:image/JPEG;base64,{{this.dataItem.dataImage}}' style='width: 100px; height: 100px;'></div>",
                    width: 120
                },
                {
                    field: "niisId",
                    title: "Mã kết nối cục ATTP",
                    width: "180px",
                    template: "<div align='center'>{{this.dataItem.niisId}}</div>"
                },
                {
                    field: "moTa",
                    title: "Mô tả",
                },
                {
                    field: "soNgayXuLy",
                    title: "Số ngày xử lý",
                    width: 120,
                    template: "<div align='center'>{{this.dataItem.soNgayXuLy}}</div>"
                },
                {
                    field: "phiXuLy",
                    title: "Phí",
                    width: 110,
                    template: "<div align='center' ng-if='this.dataItem.phiXuLy>0'>{{this.dataItem.phiXuLy | number}} VNĐ</div>"
                },
                {
                    field: "isActive",
                    title: "Trạng Thái",
                    template: '<p style="margin: 0;text-align:center;"><i ng-if="#: isActive # == 1" class="fa fa-check fa-3 font-green-meadow" aria-hidden="true"></i><i ng-if="#: isActive # == 0" class="fa fa-times fa-3 font-red"></i></p>',
                    width: 120,
                }
            ];

            vm.loaiHoSoGridOptions = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;
                        vm.loading = true;
                        loaiHoSoService.getAllServerPaging($.extend({ filter: vm.filter.Filter }, vm.requestParams))
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

            vm.suaLoaiHoSo = function (loaihoso) {
                var loaihosoCopy = angular.copy(loaihoso);
                openCreateOrEditLoaiHoSoModal(loaihosoCopy);
            }
            vm.cauHinhLoaiHoSo = function (loaihoso) {
                var loaihosoCopy = angular.copy(loaihoso);
                openCreateOrEditQuaHanModal(loaihosoCopy);
            }

            vm.xoaLoaiHoSo = function (loaihoso) {
                abp.message.confirm(
                    app.localize('DeleteWarningMessage', loaihoso.tenLoaiHoSo),
                    "Bạn chắc chắn muốn xóa?",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            loaiHoSoService.delete(loaihoso.id).then(function () {
                                vm.getGridData();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                        }
                    }
                );
            };

            function openCreateOrEditLoaiHoSoModal(loaihoso) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/danhmuc/views/loaihoso/createOrEditModal.cshtml',
                    controller: 'danhmuc.views.loaihoso.createOrEditModal as vm',
                    backdrop: 'static',
                    resolve: {
                        loaihoso: loaihoso
                    }
                });
                modalInstance.result.then(function (result) {
                    vm.getGridData();
                });
            }
            function openCreateOrEditQuaHanModal(loaihoso) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/danhmuc/views/loaihoso/createOrEditQuaHanModal.cshtml',
                    controller: 'danhmuc.views.loaihoso.createOrEditQuaHanModal as vm',
                    backdrop: 'static',
                    resolve: {
                        loaihoso: loaihoso
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