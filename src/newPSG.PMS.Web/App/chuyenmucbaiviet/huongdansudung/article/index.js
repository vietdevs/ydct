(function () {
    appModule.controller('chuyenmucbaiviet.huongdansudung.article.index', [
        '$rootScope', '$scope', '$uibModal', '$stateParams', 'uiGridConstants', 'abp.services.app.article', 'appSession',
        function ($rootScope, $scope, $uibModal, $stateParams, uiGridConstants, articleService, appSession) {
            //variable
            var vm = this;
            vm.loading = false;
            vm.requestParams = {
                skipCount: 0,
                maxResultCount: 50,
                sorting: null
            };
            vm.filter = {
                filter: null,
                categoryId: 0,
                roleLevel: 0
            };
            vm.closeTab = function (_menulabel) {
                $rootScope.closeTab(_menulabel);
            };
            vm.categoryOptions = {
                dataSource: new kendo.data.DataSource({
                    transport: {
                        read: function (options) {
                            abp.services.app.category.allCategoryToDDL().done(function (result) {
                                options.success(result);
                            });
                        }
                    }
                }),
                dataValueField: "id",
                dataTextField: "title",
                optionLabel: app.localize('Chọn danh mục ...'),
                filter: "contains"
            };

            //controll
            vm.articleColumns = [
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
                    template: "<a class='font-red-flamingo ablock text-center' ng-click='vm.xoaarticle(this.dataItem)'><i class='fa fa-trash'></i></a>"
                },
                {
                    field: "",
                    title: "Sửa",
                    width: "50px",
                    template: "<a class='font-green-meadow ablock text-center' ng-click='vm.suaarticle(this.dataItem)'><i class='fa fa-pencil'></i></a>"
                },
                {
                    field: "title",
                    title: "Tiêu đề"
                },
                {
                    field: "categoryName",
                    title: "Danh mục"
                },
                {
                    field: "isActive",
                    title: "Trạng Thái",
                    template: `<p style="margin: 0;text-align:center;"><i ng-if="#: isActive # == 1" class="fa fa-check fa-3 font-green-meadow" aria-hidden="true"></i><i ng-if="#: isActive # == 0" class="fa fa-times fa-3 font-red"></i></p>`,
                    width: 120
                }
            ];

            vm.articleGridOptions = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;
                        vm.loading = true;
                        articleService.getAllServerPaging($.extend({ filter: vm.filter.Filter, categoryId: vm.filter.CategoryId, roleLevel: vm.filter.RoleLevel }, vm.requestParams))
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
                vm.articleGridOptions.read();
            };

            vm.taoarticle = function () {
                openCreateOrEditarticleModal(null);
            };

            vm.suaarticle = function (article) {
                var articleCopy = angular.copy(article);

                openCreateOrEditarticleModal(articleCopy);
            };

            vm.xoaarticle = function (article) {
                abp.message.confirm(
                    app.localize(article.tenarticle),
                    "Bạn chắc chắn muốn xóa?",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            articleService.delete(article.id).then(function () {
                                vm.getGridData();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                        }
                    }
                );
            };

            function openCreateOrEditarticleModal(article) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/chuyenmucbaiviet/huongdansudung/article/createOrEditModal.cshtml',
                    controller: 'chuyenmucbaiviet.huongdansudung.article.createOrEditModal as vm',
                    backdrop: 'static',
                    resolve: {
                        article: article
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