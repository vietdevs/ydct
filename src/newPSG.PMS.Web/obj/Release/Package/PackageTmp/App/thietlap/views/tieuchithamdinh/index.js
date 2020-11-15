(function () {
    appModule.controller('thietlap.views.tieuchithamdinh.index', [
        '$scope', '$uibModal', '$stateParams', 'uiGridConstants', 'abp.services.app.tieuChiThamDinh', 'appSession', 'quanlyhoso.common.services.appCustom',
        function ($scope, $uibModal, $stateParams, uiGridConstants, tieuChiThamDinhService, appSession, appCustom) {
            //variable
            var vm = this;
            vm.loading = false;
            vm.ROLE_LEVEL = app.ROLE_LEVEL;
            vm.requestParams = {
                skipCount: 0,
                maxResultCount: 10,
                sorting: null
            };
            vm.filter = {
                page: 1,
                pageSize: 10,
                thuTucId: null,
                loaiBienBanThamDinhId: null,
                roleLevel: null
            };
            vm.filterObj = angular.copy(vm.filter);

            if (appSession.user.roleLevel != app.ROLE_LEVEL.SA) {
                vm.isAdmin = true;
                vm.filter.roleLevel = appSession.user.roleLevel;
            }

            vm.columns = [
                {
                    field: "STT",
                    title: "#",
                    width: 50,
                    template: "<div align='center'>{{this.dataItem.STT}}</div>"
                },
                {
                    title: "Thao tác",
                    template: '<div class=\"ui-grid-cell-contents\">' +
                        '  <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>' +
                        '    <button class="btn btn-xs blue-steel" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em> ' + "Thao Tác" + ' <span class="caret"></span></button>' +
                        '    <ul uib-dropdown-menu>' +
                        '<li><a ng-click="vm.view(this.dataItem)">' + "Xem nội dung" + '</a></li>' +
                        '<li><a ng-click="vm.edit(this.dataItem)">' + "Sửa" + '</a></li>' +
                        '<li><a ng-click="vm.delete(this.dataItem)">' + "Xóa" + '</a></li>' +
                        '    </ul>' +
                        '  </div>' +
                        '</div>',
                    width: 120
                },
                {
                    title: "Thủ tục",
                    width: 90,
                    template: "TT - {{this.dataItem.thuTucId}}"
                },
                {
                    field: "strRoleLevel",
                    title: "Vai trò",
                    width: 150,
                },
                {
                    field: "strTieuBan",
                    title: "Tiểu ban",
                    width: 170,
                },
                {
                    field: "strLoaiBienBan",
                    title: "Loại biên bản",
                    width: 120
                },
                {
                    title: "Nội dung thẩm định",
                    template: `
                            <div>
                                <ul style="list-style-type: square;">
                                    <li ng-repeat="item in this.dataItem.listNoiDungSplice track by $index" ng-bind-html="item"></li>
                                </ul>
                            </div>`
                },
            ];

            vm.gridOptions = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;
                        vm.loading = true;
                        tieuChiThamDinhService.getAllServerPaging($.extend(vm.filter, vm.requestParams))
                            .then(function (result) {
                                result.data.items.forEach(function (item, idx) {
                                    item.STT = idx + 1;
                                    item.listNoiDungSplice = [];
                                    item.listNoiDung.forEach((noiDung, idxND) => {
                                        if (idxND < 3) {
                                            item.listNoiDungSplice.push("<em class='fa fa-caret-right'></em> " + noiDung.tieuDeThamDinh);
                                        }
                                    });
                                    if (item.listNoiDungSplice.length > 0) {
                                        item.listNoiDungSplice.push("  ...");
                                    }
                                    item.strRoleLevel = appCustom.getStrRoleLevel(item.roleLevel);
                                    item.strTieuBan = appCustom.getStrTieuBan(item.tieuBanEnum);
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
                vm.gridOptions.read();
            };

            vm.search = () => {
                vm.getGridData();
            };

            vm.refresh = () => {
                vm.filter = angular.copy(vm.filterObj);
                vm.getGridData();
            };

            vm.add = function () {
                openModal(null);
            };

            vm.edit = function (dataItem) {
                openModal(dataItem);
            }

            vm.delete = function (dataItem) {
                abp.message.confirm("",
                    "Bạn chắc chắn muốn xóa?",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            tieuChiThamDinhService.deleteList(dataItem.listNoiDung).then(function () {
                                vm.getGridData();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                        }
                    }
                );
            };

            function openModal(dataItem) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/thietlap/views/tieuchithamdinh/createOrEditModal.cshtml',
                    controller: 'thietlap.views.tieuchithamdinh.createOrEditModal as vm',
                    backdrop: 'static',
                    resolve: {
                        dataItem: angular.copy(dataItem)
                    }
                });

                modalInstance.result.then(function (result) {
                    vm.getGridData();
                });
            }

            vm.view = function (dataItem) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/thietlap/views/tieuchithamdinh/_viewModal.cshtml',
                    controller: 'thietlap.views.tieuchithamdinh.viewModal as vm',
                    backdrop: 'static',
                    resolve: {
                        dataItem: dataItem
                    }
                });
            };

            $scope.$on('$viewContentLoaded', function () {
                App.initAjax();
            });
        }
    ]);
})()