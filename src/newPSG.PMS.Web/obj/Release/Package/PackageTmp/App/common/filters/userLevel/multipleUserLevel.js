(function () {
    appModule.directive('common.filter.multiuserlevel', [
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
                }
            ];
            return {
                restrict: 'EA',
                scope: {
                    ngModel: '=?',
                    ngDisabled: '=?',
                    formCase: '='
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                template: `<select multi-select includeSelectAllOption="true" maxheight="300"
                                allSelectedText='Chọn tất cả'
                                nSelectedText='Loại hình cơ sở y tế '
                                buttonWidth="100%"
                                enableFiltering="true" selectAllText="Chọn tất cả" nonSelectedText='Chọn Loại hình cơ sở y tế ...'
                                enableClickableOptGroups="true" enableCollapsibleOptGroups="true" multiple
                                ng-disabled="ngDisabled"
                                ng-model="ngModel"
                                ng-options="item.id as item.name for item in vm.arrLevel"></select>`
            };
        }
    ]);
})();