(function () {
    appModule.controller('danhmuc.views.loaihinhdoanhnghiep.index', [
        '$scope', '$uibModal', '$stateParams', 'uiGridConstants', 'abp.services.app.loaiHinhDoanhNghiep', 'appSession',
        function ($scope, $uibModal, $stateParams, uiGridConstants, loaiHinhDoanhNghiepService, appSession) {
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
            vm.loaiHinhDoanhNghiepColumns = [
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
                        '<li><a ng-click="vm.suaLoaiHinhDoanhNghiep(this.dataItem)">' + "Sửa" + '</a></li>' +
                        '<li><a ng-click="vm.xoaLoaiHinhDoanhNghiep(this.dataItem)">' + "Xóa" + '</a></li>' +
                        '    </ul>' +
                        '  </div>' +
                        '</div>',
                    width: 150
                },
                {
                    field: "tenLoaiHinh",
                    title: "Tên loại hình",
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

            vm.loaiHinhDoanhNghiepGridOptions = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;
                        vm.loading = true;
                        loaiHinhDoanhNghiepService.getAllServerPaging($.extend({ filter: vm.filter.Filter }, vm.requestParams))
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
                vm.loaiHinhDoanhNghiepGridOptions.read();
            };

            vm.taoLoaiHinhDoanhNghiep = function () {
                openCreateOrEditLoaiHinhDoanhNghiepModal(null);
            };

            vm.suaLoaiHinhDoanhNghiep = function (loaihinhdoanhnghiep) {
                var loaihinhdoanhnghiepCopy = angular.copy(loaihinhdoanhnghiep);
                openCreateOrEditLoaiHinhDoanhNghiepModal(loaihinhdoanhnghiepCopy);
            }

            vm.xoaLoaiHinhDoanhNghiep = function (loaihinhdoanhnghiep) {
                abp.message.confirm(
                    app.localize('DeleteChucDanhWarningMessage', loaihinhdoanhnghiep.tenLoaiHinh),
                    "Bạn chắc chắn muốn xóa?",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            loaiHinhDoanhNghiepService.delete(loaihinhdoanhnghiep.id).then(function () {
                                vm.getGridData();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                        }
                    }
                );
            };

            function openCreateOrEditLoaiHinhDoanhNghiepModal(loaihinhdoanhnghiep) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/danhmuc/views/loaihinhdoanhnghiep/createOrEditModal.cshtml',
                    controller: 'danhmuc.views.loaihinhdoanhnghiep.createOrEditModal as vm',
                    backdrop: 'static',
                    resolve: {
                        loaihinhdoanhnghiep: loaihinhdoanhnghiep
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