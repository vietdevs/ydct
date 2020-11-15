(function () {
    //app.thuTucHienHanh khai bao trong layout.cshtml
    angular.forEach(app.thuTucHienHanh, function (it) {
        let maTT = it < 10 ? "0" + it : "" + it;
        appModule.directive('app.directives.dirpages.motcuaphancong' + maTT, ['$compile', '$templateRequest',
            function ($compile, $templateRequest) {
                return {
                    restrict: 'EA',
                    scope: {},
                    link: function (scope, elem, attr, ctrl) {
                        $templateRequest("/App/quanlyhoso/thutuc" + maTT + "/views/2_motcuaphancong/index.cshtml").then(function (html) {
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