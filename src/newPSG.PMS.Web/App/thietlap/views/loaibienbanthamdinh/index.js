(function () {
    appModule.controller('thietlap.views.loaibienbanthamdinh.index', [
        '$scope', '$uibModal', '$stateParams', 'uiGridConstants', 'abp.services.app.loaiBienBanThamDinh', 'appSession', 'quanlyhoso.common.services.appCustom',
        function ($scope, $uibModal, $stateParams, uiGridConstants, loaiBienBanService, appSession, appCustom) {
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
            };

            //controll
            vm.loaiBienBanColumns = [
                {
                    field: "STT",
                    title: app.localize('STT'),
                    width: "50px",
                    template: "<div align='center'>{{this.dataItem.STT}}</div>"
                },
                {
                    title: "Thao tác",
                    template: '<div class=\"ui-grid-cell-contents\">' +
                        '  <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>' +
                        '    <button class="btn btn-xs blue-steel" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em> ' + "Thao Tác" + ' <span class="caret"></span></button>' +
                        '    <ul uib-dropdown-menu>' +
                        '<li><a ng-click="vm.suaLoaiBienBan(this.dataItem)">' + "Sửa" + '</a></li>' +
                        '<li><a ng-click="vm.xoaLoaiBienBan(this.dataItem)">' + "Xóa" + '</a></li>' +
                        '    </ul>' +
                        '  </div>' +
                        '</div>',
                    width: 130
                },
                {
                    field: "name",
                    title: "Tên loại biên bản",
                    width: 170,
                },
                {
                    field: "strRoleLevel",
                    title: "Vai trò",
                    width: 150,
                },
                {
                    field: "moTa",
                    title: "Mô tả"
                }
            ];

            vm.loaiBienBanGridOptions = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;
                        vm.loading = true;

                        loaiBienBanService.getAllServerPaging($.extend({ filter: vm.filter.Filter }, vm.requestParams))
                            .then(function (result) {
                                result.data.items.forEach(function (item, idx) {
                                    item.STT = idx + 1;
                                    item.strRoleLevel = appCustom.getStrRoleLevel(item.roleLevel);
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
                vm.loaiBienBanGridOptions.read();
            };

            vm.taoLoaiBienBan = function () {
                openCreateOrEditLoaiBienBanModal(null);
            };

            vm.suaLoaiBienBan = function (dataItem) {
                var dataItemCopy = angular.copy(dataItem);
                openCreateOrEditLoaiBienBanModal(dataItemCopy);
            }

            vm.xoaLoaiBienBan = function (dataItem) {
                abp.message.confirm(
                    app.localize('DeleteQuocGiaWarningMessage', dataItem.name),
                    "Bạn chắc chắn muốn xóa?",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            loaiBienBanService.delete(dataItem.id).then(function () {
                                vm.getGridData();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                        }
                    }
                );
            };

            function openCreateOrEditLoaiBienBanModal(dataItem) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/thietlap/views/loaibienbanthamdinh/createOrEditModal.cshtml',
                    controller: 'thietlap.views.loaibienbanthamdinh.createOrEditModal as vm',
                    backdrop: 'static',
                    resolve: {
                        dataItem: dataItem
                    }
                });

                modalInstance.result.then(function (result) {
                    //app.sessionStorage.remove("hhc_loaibienban");
                    vm.getGridData();
                });
            }

            $scope.$on('$viewContentLoaded', function () {
                App.initAjax();
            });
        }
    ]);
})()