(function () {
    appModule.controller('website.views.cauhinhchung.index', [
        '$rootScope', '$scope', '$uibModal', '$stateParams', 'uiGridConstants', 'abp.services.app.cauHinhChung', 'appSession',
        function ($rootScope, $scope, $uibModal, $stateParams, uiGridConstants, cauHinhChungService, appSession) {
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
            vm.closeTab = function (_cauHinhChunglabel) {
                $rootScope.closeTab(_cauHinhChunglabel);
            };

            //controll
            vm.cauHinhChungColumns = [
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
                        '<li><a ng-click="vm.suaCauHinhChung(this.dataItem)">' + "Sửa" + '</a></li>' +
                        '<li><a ng-click="vm.xoaCauHinhChung(this.dataItem)">' + "Xóa" + '</a></li>' +
                        '    </ul>' +
                        '  </div>' +
                        '</div>',
                    width: 150
                },
                {
                    field: "settingKey",
                    title: "Từ khóa"
                },
                {
                    field: "description",
                    title: "Mô tả"
                },
                {
                    field: "isActive",
                    title: "Trạng Thái",
                    template: `<p style="margin: 0;text-align:center;"><i ng-if="#: isActive # == 1" class="fa fa-check fa-3 font-green-meadow" aria-hidden="true"></i><i ng-if="#: isActive # == 0" class="fa fa-times fa-3 font-red"></i></p>`,
                    width: 120,
                }

            ];

            vm.cauHinhChungGridOptions = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        debugger
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;
                        vm.loading = true;
                        cauHinhChungService.getAllServerPaging($.extend({ filter: vm.filter.Filter }, vm.requestParams))
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
                vm.cauHinhChungGridOptions.read();
            };

            vm.taoCauHinhChung = function () {
                openCreateOrEditCauHinhChungModal(null);
            };

            vm.suaCauHinhChung = function (cauHinhChung) {
                console.log(cauHinhChung);
                var cauHinhChungCopy = angular.copy(cauHinhChung);
                if (cauHinhChungCopy.settingType == '1') {
                    cauHinhChungCopy.string = cauHinhChung.settingValue;
                }
                if (cauHinhChungCopy.settingType == '2') {
                    cauHinhChungCopy.image = cauHinhChung.settingValue;
                }
                if (cauHinhChungCopy.settingType == '3') {
                    cauHinhChungCopy.text = cauHinhChung.settingValue;
                }
                openCreateOrEditCauHinhChungModal(cauHinhChungCopy);
            };

            vm.xoaCauHinhChung = function (cauHinhChung) {
                abp.message.confirm(
                    app.localize(cauHinhChung.tenCauHinhChung),
                    "Bạn chắc chắn muốn xóa?",
                    function (isConfirmed) {
                        if (isConfirmed) {
                            cauHinhChungService.delete(cauHinhChung.id).then(function () {
                                vm.getGridData();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                        }
                    }
                );
            };

            function openCreateOrEditCauHinhChungModal(cauHinhChung) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/website/views/cauHinhChung/createOrEditModal.cshtml',
                    controller: 'website.views.cauhinhchung.createOrEditModal as vm',
                    backdrop: 'static',
                    resolve: {
                        cauHinhChung: cauHinhChung
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