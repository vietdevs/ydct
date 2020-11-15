(function () {

    appModule.controller('quanlytaikhoan.soytevkn.index', [
        '$scope', '$uibModal', '$stateParams', 'uiGridConstants', 'abp.services.app.quanLyTaiKhoan', 'appSession',
        function ($scope, $uibModal, $stateParams, uiGridConstants, quanLyTK, appSession) {
            var vm = this;
            vm.tenant = appSession.tenant;
            vm.user = appSession.user;
            vm.filterDefault = {
                page: 1,
                pageSize: 10,
                filter: null,
                loaiTaiKhoan: app.LOAI_TAI_KHOAN.TAI_KHOAN_SO_Y_TE 
            };
            vm.filter = angular.copy(vm.filterDefault);

            vm.loading = false;
            vm.advancedFiltersAreShown = false;
            vm.filterText = $stateParams.filterText || '';
            vm.currentUserId = abp.session.userId;

            vm.requestParams = {
                skipCount: 0,
                maxResultCount: app.consts.grid.defaultPageSize,
                sorting: null
            };
            
            vm.userColumns = [
                {
                    field: "STT",
                    title: app.localize('STT'),
                    attributes: { class: "text-center" },
                    headerAttributes: { style: "text-align: center;" },
                    width: "60px",
                    template: "<div align='center'>{{this.dataItem.STT}}</div>"
                },
                {
                    field: "",
                    title: "Thao Tác",
                    width: 150,
                    template: '<div class=\"ui-grid-cell-contents\">' +
                    '  <div class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>' +
                    '    <button class="btn btn-xs blue-steel" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><em class="fa fa-cog"></em> ' + 'Thao Tác' + ' <span class="caret"></span></button>' +
                    '    <ul uib-dropdown-menu>' +
                    '      <li><a  ng-click="vm.editUser(this.dataItem)">' + app.localize('Edit') + '</a></li>' +
                    '      <li><a  ng-click="vm.deleteUser(this.dataItem)">' + app.localize('Delete') + '</a></li>' +
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
                    '  <img ng-if="!this.dataItem.profilePictureId" src="' + abp.appPath + 'Common/Images/default-profile-picture.png" width="22" height="22" class="img-rounded" /> #:userName#' +
                    '</div>',
                },
                {
                    title: "Tên",
                    field: 'fullName',
                    minWidth: 120
                },
                //{
                //    title: "Phòng ban",
                //    field: 'strPhongBan',
                //    minWidth: 150
                //},
                {
                    title: app.localize('Active'),
                    field: 'isActive',
                    template:
                    '<div class=\"ui-grid-cell-contents\">' +
                    '  <span ng-show="this.dataItem.isActive" class="label label-success">' + app.localize('Yes') + '</span>' +
                    '  <span ng-show="!this.dataItem.isActive" class="label label-default">' + app.localize('No') + '</span>' +
                    '</div>',
                    minWidth: 80
                },
                {
                    title: app.localize('LastLoginTime'),
                    //name: app.localize('LastLoginTime'),
                    field: 'lastLoginTime',
                    cellFilter: 'momentFormat: \'L\'',
                    minWidth: 100
                },
            ];
            vm.userGridOptions = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        vm.loading = true;
                        vm.requestParams.skipCount = (options.data.page - 1) * options.data.pageSize;
                        vm.requestParams.maxResultCount = options.data.pageSize;
                        quanLyTK.getAllServerPaging($.extend(vm.filter, vm.requestParams))
                            .then(function (result) {
                                let i = 1;
                                result.data.items.forEach(function (item) {
                                    item.STT = i;
                                    i++;
                                });
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
            vm.getUsers = function () {
                vm.userGridOptions.read();
            };


            vm.editUser = function (user) {
                openCreateOrEditUserModal(user.id);
            };

            vm.createUser = function () {
                openCreateOrEditUserModal(null);
            };

            vm.deleteUser = function (user) {
                abp.message.confirm(
                    app.localize('UserDeleteWarningMessage', user.userName),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            quanLyTK.delete({
                                id: user.id
                            }).then(function () {
                                vm.getUsers();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                        }
                    }
                );
            };

            vm.unlockUser = function (user) {
                quanLyTK.unlockUser({
                        id: user.id
                    })
                    .then(function() {
                        abp.notify.success(app.localize('UnlockedTheUser', user.userName));
                    });
            };

         

            function openCreateOrEditUserModal(userId) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/quanlytaikhoan/soyte-vkn/createOrEditModal.cshtml',
                    controller: 'quanlytaikhoan.soytevkn.createOrEditModal as vm',
                    backdrop: 'static',
                    resolve: {
                        data: {
                            userId: userId,
                            loaiTaiKhoan: vm.filterDefault.loaiTaiKhoan
                        }
                    }
                });
                modalInstance.result.then(function (result) {
                    vm.getUsers();
                });
            }
        }]);
})();