(function () {
    angular.forEach(app.thuTucHienHanh, function (it) {
        let maTT = it < 10 ? "0" + it : "" + it;
        appModule.directive('app.directives.dirpages.lanhdaocucduyetthamdinh' + maTT, ['$compile', '$templateRequest',
            function ($compile, $templateRequest) {
                return {
                    restrict: 'EA',
                    scope: {},
                    link: function (scope, elem, attr, ctrl) {
                        $templateRequest("/App/quanlyhoso/thutuc" + maTT + "/views/51_lanhdaocucduyetthamdinh/index.cshtml").then(function (html) {
                            var template = angular.element(html);
                            elem.append(template);
                            $compile(template)(scope);
                        });
                    }
                };
            }
        ]);
    });
})();