(function () {
    appModule.controller('chuyenmucbaiviet.quantriduan.category.index', [
        '$rootScope', '$scope', '$uibModal', '$stateParams', 'uiGridConstants', 'abp.services.app.category', 'appSession',
        function ($rootScope, $scope, $uibModal, $stateParams, uiGridConstants, categoryService, appSession) {
            //variable
            var vm = this;
            vm.loading = false;
            vm.currentUser = angular.copy(appSession.user);

            vm.requestParams = {
                skipCount: 0,
                maxResultCount: 50,
                sorting: null
            };
            vm.filter = {
                filter: null,
                roleLevel: null
            };

            //controll
            vm.categoryColumns = [
                {
                    field: "STT",
                    title: app.localize('STT'),
                    width: "50px",
                    template: "<div align='center'>{{this.dataItem.STT}}</div>"
                },
                {
                    field: "",
                    title: "Xóa",
                    width: "50px",
                    template: `<a class='font-red-flamingo ablock text-center' ng-click='vm.xoaCategory(this.dataItem)'>
                                <i class='fa fa-trash'></i>
                            </a >`
                },
                {
                    field: "",
                    title: "Sửa",
                    width: "50px",
                    template: "<a class='font-green-meadow ablock text-center' ng-click='vm.suaCategory(this.dataItem)'><i class='fa fa-pencil'></i></a>"
                },
                {
                    field: "title",
                    title: "Tiêu đề",
                    template: function (dataItem) {
                        let space = "";
                        if (dataItem.level == 1)
                            space = "---";
                        else if (dataItem.level == 2)
                            space = "--- ---";
                        else if (dataItem.level == 3)
                            space = "--- --- ---";
                        else if (dataItem.level == 4)
                            space = "--- --- --- ---";
                        let _title = space + dataItem.title;
                        return `` + _title + ``;
                    }
                },
                {
                    field: "strRoleLevel",
                    title: "Đối tượng sử dụng",
                    width: 200
                },
                {
                    field: "file",
                    title: "Thao tác",
                    width: "180px",
                    template: `<p style="margin: 0;text-align:center;" ng-if="this.dataItem.file">
                                <a href='{{this.dataItem.file}}'><i class='fa fa-download'> Tải file</a>
                            </p>`
                },
                {
                    field: "isActive",
                    title: "Trạng Thái",
                    template: `<p style="margin: 0;text-align:center;">
                                <i ng-if="#: isActive # == 1" class="fa fa-check fa-3 font-green-meadow" aria-hidden="true"></i>
                                <i ng-if="#: isActive # == 0" class="fa fa-times fa-3 font-red"></i>
                            </p>`,
                    width: 120
                }

            ];

            vm.categoryGridOptions = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;

                        vm.loading = true;
                        categoryService.getAllServerPagingLevel($.extend(vm.filter, vm.requestParams))
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
                schema: {
                    data: "items",
                    total: "totalCount"
                }
            });

            //function
            vm.getGridData = function () {
                vm.categoryGridOptions.read();
            };

            vm.taoCategory = function () {
                openCreateOrEditCategoryModal(null);
            };

            vm.suaCategory = function (category) {
                var categoryCopy = angular.copy(category);
                categoryCopy.title = categoryCopy.title.replace('--- ', '');
                openCreateOrEditCategoryModal(categoryCopy);
            };

            vm.xoaCategory = function (category) {
                abp.message.confirm(
                    app.localize(category.tenCategory),
                    "Bạn chắc chắn muốn xóa?",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            categoryService.delete(category.id).then(function () {
                                vm.getGridData();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                        }
                    }
                );
            };

            function openCreateOrEditCategoryModal(category) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/chuyenmucbaiviet/quantriduan/category/createOrEditModal.cshtml',
                    controller: 'chuyenmucbaiviet.quantriduan.category.createOrEditModal as vm',
                    backdrop: 'static',
                    windowClass: 'full-screen-modal',
                    resolve: {
                        category: category
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