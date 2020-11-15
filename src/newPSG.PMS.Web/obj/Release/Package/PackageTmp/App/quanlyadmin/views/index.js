(function () {
    appModule.controller('quanlyadmin.views.index', [
        '$scope', '$uibModal', '$stateParams', 'abp.services.app.user', 'appSession', 'abp.services.app.customTennant',
        function ($scope, $uibModal, $stateParams, userService, appSession, customTennantService) {
            var vm = this;
            vm.tenant = appSession.tenant;
            vm.loading = false;
            vm.filter = {
                Filter: '',
                TenantId: null
            }
            vm.currentUserId = abp.session.userId;

            vm.requestParams = {
                permission: '',
                role: '',
                skipCount: 0,
                maxResultCount: 10,
                sorting: null
            };

            vm.tenantOptions = {
                dataSource: {
                    transport: {
                        read: function (options) {
                            customTennantService.getAllTenant().then(function (result) {
                                options.success(result.data);
                            });
                        }
                    }
                },
                dataValueField: "id",
                dataTextField: "name",
                optionLabel: app.localize('Chọn đơn vị ...'),
                filter: "contains",
            }

            vm.adminColumns = [
                {
                    field: "",
                    title: "Thao Tác",
                    width: 150,
                    template: '<div class=\"ui-grid-cell-contents\">' +
                        '  <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>' +
                        '    <button class="btn btn-xs blue-steel" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><i class="fa fa-cog"></i> ' + 'Thao Tác' + ' <span class="caret"></span></button>' +
                        '    <ul uib-dropdown-menu>' +
                        '      <li><a ng-click="vm.unlockAdmin(this.dataItem)">' + app.localize('Unlock') + '</a></li>' +
                        '      <li><a ng-click="vm.changePasswordAdmin(this.dataItem)">' + app.localize('ChangePassword') + '</a></li>' +
                        '    </ul>' +
                        '  </div>' +
                        '</div>',
                },
                {
                    field: 'userName',
                    title: app.localize('UserName'),
                    template:
                        '<div class=\"ui-grid-cell-contents\">' +
                        '  <img ng-if="this.dataItem.profilePictureId" ng-src="' + abp.appPath + 'Profile/GetProfilePictureById?id={{this.dataItem.profilePictureId}}" width="22" height="22" class="img-rounded img-profile-picture-in-grid" />' +
                        '  <img ng-if="!this.dataItem.profilePictureId" src="' + abp.appPath + 'Common/Images/default-profile-picture.png" width="22" height="22" class="img-rounded" /><text ng-if="this.dataItem.tenancyName!=\'Default\'">{{this.dataItem.tenancyName}}/</text>#:userName#' +
                        '</div>',
                },
                {
                    title: "Đơn vị",
                    field: 'tenantName',
                },
                {
                    title: app.localize('EmailAddress'),
                    field: 'emailAddress',
                    minWidth: 200
                },
                {
                    title: app.localize('LastLoginTime'),
                    field: 'lastLoginTime',
                    template: "{{this.dataItem.lastLoginTime | date:'dd/MM/yyyy HH:mm:ss'}}",
                    minWidth: 100
                },
                {
                    title: app.localize('CreationTime'),
                    field: 'creationTime',
                    template: "{{this.dataItem.creationTime | date:'dd/MM/yyyy HH:mm:ss'}}",
                    minWidth: 100
                }
            ];

            vm.adminGridOptions = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        vm.loading = true;
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;
                        userService.getAdmins($.extend(vm.filter, vm.requestParams))
                            .then(function (result) {
                                options.success(result.data);
                            }).finally(function () {
                                vm.loading = false;
                            });
                    }
                },
                pageSize: 10,
                autoBind: false,
                serverPaging: true,
                serverSorting: true,
                scrollable: true,
                sortable: true,
                pageable: {
                    pageSizes: [5, 10, 50, "Tất cả"],
                    refresh: true,
                    buttonCount: 5
                },
                schema: {
                    data: "items",
                    total: "totalCount"
                },
            });
            vm.getAdmins = function () {
                vm.adminGridOptions.read();
            };

            vm.changePasswordAdmin = function (_data) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/quanlyadmin/views/modal/changePasswordModal.cshtml',
                    controller: 'quanlyadmin.views.modal.changePasswordModal as vm',
                    backdrop: 'static',
                    resolve: {
                        detailData: _data
                    },
                });
                modalInstance.result.then(function (result) {
                    vm.getAdmins();
                });
            }

            vm.unlockAdmin = function (user) {
                userService.unlockAdmin({
                    id: user.id
                })
                    .then(function () {
                        abp.notify.success(app.localize('UnlockedTheUser', user.userName));
                    });
            };

            vm.getAdmins();
        }]);
})();