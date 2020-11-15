(function () {
    appLanhDaoCucModule.directive('app.layoutlanhdaocuc.pagecontent', ['$compile', '$timeout', '$templateRequest',
        function ($compile, $timeout, $templateRequest) {
            var controller = ['$scope', '$timeout', '$window',
                function ($scope, $timeout, $window) {
                    var vm = this;
                    var initVar = () => {
                    }

                    var mainFunc = () => {
                    }

                    var init = () => {
                        initVar();
                        mainFunc();
                    }
                    init();
                }];
            return {
                restrict: 'EA',
                scope: {
                    formview: '=?'
                },
                controller: controller,
                controllerAs: 'vm',
                bindToController: false,
                link: function (scope, elem, attr, ctrl) {
                    $templateRequest("/App/common/views/layoutLanhDaoCuc/pageContent.cshtml").then(function (html) {
                        var template = angular.element(html);
                        elem.append(template);
                        $compile(template)(scope);
                    });
                }
            };
        }
    ]);
})();