(function () {
    appModule.directive('kendoGridRowDblClick', kendoGridRowDblClick);
    function kendoGridRowDblClick() {
        return {
            link: function (scope, element, attrs) {
                scope.$on("kendoWidgetCreated", function (event, widget) {
                    if (widget !== element.getKendoGrid())
                        return;
                    attachDblClickToRows(scope, element, attrs);
                    element.data("kendoGrid").bind('dataBound', function () {
                        attachDblClickToRows(scope, element, attrs);
                    });
                });
            }
        };
        function attachDblClickToRows(scope, element, attrs) {
            element.find('tbody tr').on('dblclick', function (event) {
                var rowScope = angular.element(event.currentTarget).scope();
                scope.$eval(attrs.kendoGridRowDblClick, { rowData: rowScope.dataItem });
            });
        }
    }
})();