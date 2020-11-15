(function () {
    appModule.controller('common.views.languages.index', [
        '$scope', '$state', '$uibModal', 'abp.services.app.language', 'uiGridConstants',
        function ($scope, $state, $uibModal, languageService, uiGridConstants) {
            var vm = this;

            $scope.$on('$viewContentLoaded', function () {
                App.initAjax();
            });

            vm.loading = false;
            vm.defaultLanguageName = null;
            vm.currentTenantId = abp.session.tenantId;

            vm.permissions = {
                create: abp.auth.hasPermission('Pages.Administration.Languages.Create'),
                edit: abp.auth.hasPermission('Pages.Administration.Languages.Edit'),
                changeTexts: abp.auth.hasPermission('Pages.Administration.Languages.ChangeTexts'),
                'delete': abp.auth.hasPermission('Pages.Administration.Languages.Delete')
            };

            vm.gridOptions = {
                enableHorizontalScrollbar: uiGridConstants.scrollbars.WHEN_NEEDED,
                enableVerticalScrollbar: uiGridConstants.scrollbars.WHEN_NEEDED,
                appScopeProvider: vm,
                columnDefs: [
                    {
                        name: app.localize('Actions'),
                        width: 120,
                        enableSorting: false,
                        cellTemplate:
                            '<div class=\"ui-grid-cell-contents\">' +
                            '  <div ng-if="grid.appScope.permissions.changeTexts || row.entity.tenantId == grid.appScope.currentTenantId" class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>' +
                            '    <button class="btn btn-xs green-meadow" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><i class="fa fa-cog"></i> ' + app.localize('Actions') + ' <span class="caret"></span></button>' +
                            '    <ul uib-dropdown-menu>' +
                            '      <li><a ng-if="grid.appScope.permissions.edit && row.entity.tenantId == grid.appScope.currentTenantId" ng-click="grid.appScope.editLanguage(row.entity)">' + app.localize('Edit') + '</a></li>' +
                            '      <li><a ng-if="grid.appScope.permissions.changeTexts" ng-click="grid.appScope.changeTexts(row.entity)">' + app.localize('ChangeTexts') + '</a></li>' +
                            '      <li><a ng-if="grid.appScope.permissions.edit && row.entity.name != grid.appScope.defaultLanguageName" ng-click="grid.appScope.setAsDefaultLanguage(row.entity)">' + app.localize('SetAsDefaultLanguage') + '</a></li>' +
                            '      <li><a ng-if="grid.appScope.permissions.delete && row.entity.tenantId == grid.appScope.currentTenantId" ng-click="grid.appScope.deleteLanguage(row.entity)">' + app.localize('Delete') + '</a></li>' +
                            '    </ul>' +
                            '  </div>' +
                            '</div>'
                    },
                    {
                        name: app.localize('Name'),
                        field: 'displayName',
                        minWidth: 120,
                        cellTemplate:
                            '<div class=\"ui-grid-cell-contents\">' +
                            '  <i ng-class="row.entity.icon"></i> &nbsp; ' +
                            '  <span ng-if="row.entity.name != grid.appScope.defaultLanguageName">{{row.entity.displayName}}</span>' +
                            '  <span ng-if="row.entity.name == grid.appScope.defaultLanguageName"><strong>{{row.entity.displayName}} (' + app.localize('Default') + ')</strong></span>' +
                            '</div>'
                    },
                    {
                        name: app.localize('Code'),
                        field: 'name',
                        minWidth: 120
                    },
                    {
                        name: app.localize('Default') + '*',
                        field: 'tenantId',
                        cellTemplate:
                            '<div class=\"ui-grid-cell-contents\">' +
                            '  <span ng-show="row.entity.tenantId != grid.appScope.currentTenantId" class="label label-default">' + app.localize('Yes') + '</span>' +
                            '  <span ng-show="row.entity.tenantId == grid.appScope.currentTenantId" class="label label-success">' + app.localize('No') + '</span>' +
                            '</div>',
                        width: 100
                    },
                    {
                        name: app.localize('CreationTime'),
                        field: 'creationTime',
                        cellFilter: 'momentFormat: \'L\'',
                        minWidth: 140
                    }
                ],
                data: []
            };

            vm.languageColumns = [
                {
                    field: "",
                    title: app.localize('Actions'),
                    template: '<div class=\"ui-grid-cell-contents\">' +
                        '<div class=\"ui-grid-cell-contents\">' +
                        '  <div ng-if="vm.permissions.changeTexts || this.dataItem.tenantId == vm.currentTenantId" class="btn-group dropdown" uib-dropdown="" dropdown-append-to-body>' +
                        '    <button class="btn btn-xs blue-steel" uib-dropdown-toggle="" aria-haspopup="true" aria-expanded="false"><i class="fa fa-cog"></i> ' + app.localize('Actions') + ' <span class="caret"></span></button>' +
                        '    <ul uib-dropdown-menu>' +
                        '      <li><a ng-if="vm.permissions.edit && this.dataItem.tenantId == vm.currentTenantId" ng-click="vm.editLanguage(this.dataItem)">' + app.localize('Edit') + '</a></li>' +
                        '      <li><a ng-if="vm.permissions.changeTexts" ng-click="vm.changeTexts(this.dataItem)">' + app.localize('ChangeTexts') + '</a></li>' +
                        '      <li><a ng-if="vm.permissions.edit && this.dataItem.name != vm.defaultLanguageName" ng-click="vm.setAsDefaultLanguage(this.dataItem)">' + app.localize('SetAsDefaultLanguage') + '</a></li>' +
                        '      <li><a ng-if="vm.permissions.delete && this.dataItem.tenantId == vm.currentTenantId" ng-click="vm.deleteLanguage(this.dataItem)">' + app.localize('Delete') + '</a></li>' +
                        '    </ul>' +
                        '  </div>' +
                        '</div>',
                    minWidth: 120,
                },
                {
                    field: 'displayName',
                    title: app.localize('Name'),
                    sortable: true,
                    template:
                        '<div class=\"ui-grid-cell-contents\">' +
                        '  <i ng-class="this.dataItem.icon"></i> &nbsp; ' +
                        '  <span ng-if="this.dataItem.name != vm.defaultLanguageName">{{this.dataItem.displayName}}</span>' +
                        '  <span ng-if="this.dataItem.name == vm.defaultLanguageName"><strong>{{this.dataItem.displayName}} (' + app.localize('Default') + ')</strong></span>' +
                        '</div>',
                    minWidth: 120,
                },
                {
                    title: app.localize('Code'),
                    field: 'name',
                    sortable: true,
                    minWidth: 120
                },
                {
                    title: app.localize('Default') + '*',
                    sortable: true,
                    field: 'tenantId',
                    template:
                        '<div class=\"ui-grid-cell-contents\">' +
                        '  <span ng-show="this.dataItem.tenantId != vm.currentTenantId" class="label label-default">' + app.localize('Yes') + '</span>' +
                        '  <span ng-show="this.dataItem.tenantId == vm.currentTenantId" class="label label-success">' + app.localize('No') + '</span>' +
                        '</div>',
                    width: 100
                },
                {
                    title: app.localize('CreationTime'),
                    field: 'creationTime',
                    cellFilter: 'momentFormat: \'L\'',
                    minWidth: 140
                }

            ];

            vm.languageGridOptions = new kendo.data.DataSource({
                transport: {
                    read: function (options) {
                        vm.loading = true;
                        languageService.getLanguages({}).then(function (result) {
                            options.success(result.data.items);
                            vm.defaultLanguageName = result.data.defaultLanguageName;
                            vm.callbackOption = options;
                        }).finally(function () {
                            vm.loading = false;
                        });
                    }
                },
                pageSize: 10,
                serverPaging: false,
                serverSorting: true,
                scrollable: true,
                sortable: true,
                pageable: false,
                //schema: {
                //    data: "items",
                //    total: "totalCount"
                //},
            });

            if (!vm.permissions.edit &&
                !vm.permissions.changeTexts &&
                !vm.permissions.delete) {
                vm.gridOptions.columnDefs.shift();
            }

            //No need to 'Default' field is this is a host user.
            if (!vm.currentTenantId) {
                vm.gridOptions.columnDefs.splice(vm.gridOptions.columnDefs.length - 2, 1);
            }

            vm.getLanguages = function () {
                vm.loading = true;
                languageService.getLanguages({}).then(function (result) {
                    vm.gridOptions.data = result.data.items;
                    vm.defaultLanguageName = result.data.defaultLanguageName;
                }).finally(function () {
                    vm.loading = false;
                });
            };

            vm.editLanguage = function (language) {
                openCreateOrEditLanguageModal(language.id); //TODO: CAN EDIT?
            };

            vm.deleteLanguage = function (language) {
                abp.message.confirm(
                    app.localize('LanguageDeleteWarningMessage', language.displayName),
                    function (isConfirmed) {
                        if (isConfirmed) {
                            languageService.deleteLanguage({
                                id: language.id
                            }).then(function () {
                                vm.getLanguages();
                                abp.notify.success(app.localize('SuccessfullyDeleted'));
                            });
                        }
                    }
                );
            };

            vm.createLanguage = function () {
                openCreateOrEditLanguageModal(null);
            };

            vm.setAsDefaultLanguage = function (language) {
                languageService.setDefaultLanguage({
                    name: language.name
                }).then(function () {
                    vm.getLanguages();
                    abp.notify.success(app.localize('SuccessfullySaved'));
                });
            };

            vm.changeTexts = function (language) {
                $state.go('languageTexts', {
                    languageName: language.name
                });
            }

            function openCreateOrEditLanguageModal(id) {
                var modalInstance = $uibModal.open({
                    templateUrl: '~/App/common/views/languages/createOrEditModal.cshtml',
                    controller: 'common.views.languages.createOrEditModal as vm',
                    backdrop: 'static',
                    resolve: {
                        languageId: function () {
                            return id;
                        }
                    }
                });

                modalInstance.result.then(function (result) {
                    vm.getLanguages();
                });
            }

            vm.getLanguages();
        }
    ]);
})();