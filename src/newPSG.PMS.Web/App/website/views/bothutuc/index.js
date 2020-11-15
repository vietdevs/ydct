(function () {
    appModule.controller('website.views.bothutuc.index', [
        '$rootScope', '$scope', '$uibModal', '$stateParams', 'uiGridConstants', 'abp.services.app.boThuTuc', 'appSession',
        function ($rootScope, $scope, $uibModal, $stateParams, uiGridConstants, boThuTucService, appSession) {
            //variable
            var vm = this;
            vm.loading = false;
            vm.requestParams = {
                skipCount: 0,
                maxResultCount: 50,
                sorting: null
            };
            vm.filter = {
                page: 1,
                pageSize: 50,
                Filter: null,
                CategoryId: 0
            };
            vm.closeTab = function (_menulabel) {
                $rootScope.closeTab(_menulabel);
            };

            //controll
            vm.bothutucColumns = [
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
                        '<li><a ng-click="vm.suaBoThuTuc(this.dataItem)">' + "Sửa" + '</a></li>' +
                        '<li><a ng-click="vm.xoaBoThuTuc(this.dataItem)">' + "Xóa" + '</a></li>' +
                        '    </ul>' +
                        '  </div>' +
                        '</div>',
                    width: 150
                },
                {
                    field: "soHoSo",
                    title: "Số hồ sơ",
                    width: 200,
                },
                {
                    field: "tenThuTuc",
                    title: "Tên thủ tục"
                },
                {
                    field: "coQuanThucHien",
                    title: "Cơ Quan",
                    width: 240,
                },
                {
                    field: "isActive",
                    title: "Trạng Thái",
                    template: `<p style="margin: 0;text-align:center;"><i ng-if="#: isActive # == 1" class="fa fa-check fa-3 font-green-meadow" aria-hidden="true"></i><i ng-if="#: isActive # == 0" class="fa fa-times fa-3 font-red"></i></p>`,
                    width: 120,
                }

            ];

            vm.bothutucGridOptions = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;
                        vm.loading = true;
                        boThuTucService.getAllServerPaging($.extend({ filter: vm.filter.Filter, categoryId: vm.filter.CategoryId }, vm.requestParams))
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
                pageSize: 50,
                serverPaging: true,
                serverSorting: true,
                scrollable: true,
                sortable: true,
                pageable: {
                    refresh: true,
                    pageSizes: 50,
                    buttonCount: 5
                },
                schema: {
                    data: "items",
                    total: "totalCount"
                }
            });

            //function
            vm.getGridData = function () {
                vm.bothutucGridOptions.read();
            };

            vm.taoBoThuTuc = function () {
                openCreateOrEditBoThuTucModal(null);
            };

            vm.suaBoThuTuc = function (bothutuc) {
                var bothutucCopy = angular.copy(bothutuc);

                openCreateOrEditBoThuTucModal(bothutucCopy);
            };

            vm.xoaBoThuTuc = function (bothutuc) {
                abp.message.confirm(
                    app.localize(bothutuc.tenBoThuTuc),
                    "Bạn chắc chắn muốn xóa?",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            boThuTucService.delete(bothutuc.id).then(function () {
                                vm.getGridData();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                        }
                    }
                );
            };

            function openCreateOrEditBoThuTucModal(bothutuc) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/website/views/bothutuc/createOrEditModal.cshtml',
                    controller: 'website.views.bothutuc.createOrEditModal as vm',
                    backdrop: 'static',
                    resolve: {
                        bothutuc: bothutuc
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