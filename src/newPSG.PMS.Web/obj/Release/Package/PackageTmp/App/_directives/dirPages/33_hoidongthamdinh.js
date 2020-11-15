(function () {
    angular.forEach(app.thuTucHienHanh, function (it) {
        let maTT = it < 10 ? "0" + it : "" + it;
        appModule.directive('app.directives.dirpages.hoidongthamdinh' + maTT, ['$compile', '$templateRequest', '$state',
            function ($compile, $templateRequest, $state) {
                return {
                    restrict: 'EA',
                    scope: {},
                    link: function (scope, elem, attr, ctrl, state) {
                        $templateRequest("/App/quanlyhoso/thutuc" + maTT + "/views/33_hoidongthamdinh/index.cshtml").then(function (html) {
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