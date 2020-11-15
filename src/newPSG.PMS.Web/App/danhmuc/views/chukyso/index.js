(function () {
    appModule.controller('quanlychuky.views.index', [
        '$scope', '$uibModal', '$stateParams', '$sce', '$compile', 'uiGridConstants', 'abp.services.app.chuKy', 'baseService', 'appSession',
        function ($scope, $uibModal, $stateParams, $sce, $compile, uiGridConstants, chuKyService, baseService, appSession) {
            //variable
            var vm = this;
            vm.loading = false;
            vm.ROLE_LEVEL = app.ROLE_LEVEL;
            vm.roleLv = appSession.user.roleLevel;

            vm.chuKys = [];
            vm.requestParams = {
                skipCount: 0,
                maxResultCount: 10,
                sorting: null
            };
            vm.filter = {
                page: 1,
                pageSize: 10,
                Filter: null,
                userId: null,
            };

            //controll
            vm.chuKyColumns = [
                {
                    field: "STT",
                    title: app.localize('STT'),
                    width: "50px",
                    template: "<div align='center'>{{this.dataItem.STT}}</div>"
                },
                {
                    field: "",
                    title: "Thao tác",
                    template: '<div class=\"ui-grid-cell-contents\">' +
                        '  <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>' +
                        '    <button class="btn btn-xs green-meadow" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><i class="fa fa-cog"></i> ' + "Thao Tác" + ' <span class="caret"></span></button>' +
                        '    <ul uib-dropdown-menu>' +
                        '<li><a ng-click="vm.suaChuKy(this.dataItem)">' + "Sửa" + '</a></li>' +
                        '<li><a ng-click="vm.xoaChuKy(this.dataItem)">' + "Xóa" + '</a></li>' +
                        '    </ul>' +
                        '  </div>' +
                        '</div>',
                    width: 150
                },
                {
                    field: "tenChuKy",
                    title: "Tên",
                },
                {
                    field: "maChuKy",
                    title: "Mã",
                },
                {
                    field: "dataImage",
                    title: vm.roleLv == vm.ROLE_LEVEL.DOANH_NGHIEP ? "Con dấu" : "Chữ ký",
                    template: "<div align='center'><img class='img-responsive' ng-src='data:image/JPEG;base64,{{this.dataItem.dataImage}}'></div>",
                    width: 120
                },
                {
                    field: "chanChuKy",
                    title: "Chân chữ ký",
                    template: function (dataItem) {
                        return baseService.isNullOrEmpty(dataItem.chanChuKy) ? "" : dataItem.chanChuKy;
                    }, hidden: vm.roleLv == vm.ROLE_LEVEL.DOANH_NGHIEP ? true : false
                },
                {
                    field: "moTa",
                    title: "Mô tả",
                }
            ];

            vm.chuKyGridOptions = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;

                        chuKyService.getAllChuKyServerPaging($.extend({ filter: vm.filter.Filter, userId: appSession.user.id }, vm.requestParams)).then(function (result) {
                            var i = 1;
                            vm.chuKys = angular.copy(result.data.items);
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
            vm.initGridHoSo = function () {
                vm.filter = {
                    page: 1,
                    pageSize: 10,
                    Filter: '',
                };
                vm.chuKyGridOptions.read();
            }

            vm.getGridData = function () {
                vm.chuKyGridOptions.transport.read(vm.optionCallback);
            };

            vm.taoChuKy = function () {
                openCreateOrEditChuKyModal(null);
            };

            vm.suaChuKy = function (chuKy) {
                var chuKyCopy = angular.copy(chuKy);
                openCreateOrEditChuKyModal(chuKyCopy);
            }

            vm.xoaChuKy = function (chuKy) {
                abp.message.confirm(
                    app.localize('DeleteWarningMessage', chuKy.tenChuKy),
                    "Bạn chắc chắn muốn xóa?",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            chuKyService.deleteChuKy(chuKy.id).then(function () {
                                vm.getGridData();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                        }
                    }
                );
            };

            vm.xemVideoHuongDanChuKy = function () {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/danhmuc/views/chukyso/xemVideoHuongDan.cshtml',
                    controller: 'quanlychuky.views.xemVideoHuongDan as vm',
                    backdrop: 'static',
                    size: 'lg'
                });
                modalInstance.result.then(function (result) {
                });
            }

            vm.xemKiemTraNenTang = function () {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/danhmuc/views/chukyso/xemVideoKiemTraNenTang.cshtml',
                    controller: 'quanlychuky.views.xemVideoKiemTraNenTang as vm',
                    backdrop: 'static',
                    size: 'lg'
                });
                modalInstance.result.then(function (result) {
                });
            }

            vm.xemKiemTraNenTang = function () {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/danhmuc/views/chukyso/xemVideoKiemTraNenTang.cshtml',
                    controller: 'quanlychuky.views.xemVideoKiemTraNenTang as vm',
                    backdrop: 'static',
                    size: 'lg'
                });
                modalInstance.result.then(function (result) {
                });
            }

            vm.taiNetFramework = function () {
                location.href = "/Temp/Downloads/dotNetFx40_Full_setup.zip"
            }

            function openCreateOrEditChuKyModal(chuKy) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/danhmuc/views/chukyso/createOrEditModal.cshtml',
                    controller: 'quanlychuky.views.createOrEditModal as vm',
                    backdrop: 'static',
                    resolve: {
                        chuKy: chuKy
                    }
                });

                modalInstance.result.then(function (result) {
                    vm.getGridData();
                });
            }

            vm.downloadBoCaiCA = function () {
                openDownLoadBoCaiModal();
            }

            function openDownLoadBoCaiModal() {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/danhmuc/views/chukyso/downLoadBoCaiModal.cshtml',
                    controller: 'quanlychuky.views.downLoadBoCaiModal as vm',
                    backdrop: 'static'
                });

                modalInstance.result.then(function (result) {
                    location.href = "/Temp/Downloads/TokenServices.zip"
                });
            }

            $scope.$on('$viewContentLoaded', function () {
                App.initAjax();
            });
        }
    ]);
})()