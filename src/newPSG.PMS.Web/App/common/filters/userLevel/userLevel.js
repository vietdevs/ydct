(function () {
    appModule.directive('common.filter.userlevel', [
        function () {
            var controller = ['$scope', '$http', 'baseService', 'appSession',
                function ($scope, $http, baseService, appSession) {
                    //variable
                    var vm = this;
                    vm.ROLE_LEVEL = app.ROLE_LEVEL;
                    vm.currentUser = appSession.user;

                    //ArrLevel
                    {
                        var arr_level = [];
                        vm.getArrLevel = function () {
                            arr_level = appSession.get_level();
                        };
                        vm.getArrLevel();
                        vm.arrLevel = arr_level;
                    }

                    vm.cbxOptions = {
                        dataSource: vm.arrLevel,
                        dataValueField: "id",
                        dataTextField: "name",
                        optionLabel: "Chọn ...",
                        filter: "startswith",
                        change: function (e) {
                            var dataItem = e.sender.dataItem();
                            if ($scope.sChange())
                                $scope.sChange()(dataItem);
                        }
                    };
                }
            ];

            return {
                restrict: 'EA',
                scope: {
                    sModel: '=?',
                    sDisabled: '=?',
                    sChange: '&',
                    formCase: '=?'
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                template: `<input style="width: 100%"
                                kendo-drop-down-list
                                ng-disabled="sDisabled"
                                ng-model="sModel"
                                k-options="vm.cbxOptions"
                                class="form-control" />`
            };
        }
    ]);
})();