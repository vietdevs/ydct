(function () {
    appModule.controller('website.views.lienhe.index', [
        '$rootScope', '$scope', '$uibModal', '$stateParams', 'uiGridConstants', 'abp.services.app.lienHe', 'appSession',
        function ($rootScope, $scope, $uibModal, $stateParams, uiGridConstants, lienheService, appSession) {
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
            };
            vm.closeTab = function (_menulabel) {
                $rootScope.closeTab(_menulabel);
            };

            //controll
            vm.lienheColumns = [
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
                        '<li><a ng-click="vm.suaLienHe(this.dataItem)">' + "Sửa" + '</a></li>' +
                        '<li><a ng-click="vm.xoaLienHe(this.dataItem)">' + "Xóa" + '</a></li>' +
                        '    </ul>' +
                        '  </div>' +
                        '</div>',
                    width: 150
                },
                {
                    field: "hoten",
                    title: "Tên người gửi",
                },
                {
                    field: "dienthoai",
                    title: "Điện thoại",
                },
                {
                    field: "tieude",
                    title: "Tiêu đề",
                },
                {
                    field: "loinhan",
                    title: "Lời nhắn",
                },
                //{
                //    field: "isRead",
                //    title: "Đã đọc",
                //    template: `<p style="margin: 0;text-align:center;"><i ng-if="#: isRead # == 1" class="fa fa-check fa-3 font-green-meadow" aria-hidden="true"></i><i ng-if="#: isRead # == 0" class="fa fa-times fa-3 font-red"></i></p>`,
                //    width: 120,
                //}

            ];

            vm.lienheGridOptions = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;
                        vm.loading = true;
                        lienheService.getAllServerPaging($.extend({ filter: vm.filter.Filter }, vm.requestParams))
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
                vm.lienheGridOptions.read();
            };

            //vm.taoLienHe = function () {
            //    openCreateOrEditLienHeModal(null);
            //};

            vm.suaLienHe = function (lienhe) {
                var lienheCopy = angular.copy(lienhe);
                openCreateOrEditLienHeModal(lienheCopy);
            };

            vm.xoaLienHe = function (lienhe) {
                abp.message.confirm(
                    app.localize(lienhe.tenLienHe),
                    "Bạn chắc chắn muốn xóa?",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            lienheService.delete(lienhe.id).then(function () {
                                vm.getGridData();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                        }
                    }
                );
            };

            function openCreateOrEditLienHeModal(lienhe) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/website/views/lienhe/createOrEditModal.cshtml',
                    controller: 'website.views.lienhe.createOrEditModal as vm',
                    backdrop: 'static',
                    resolve: {
                        lienhe: lienhe
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