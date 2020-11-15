(function () {
    appModule.factory('quanlyhoso.common.services.appCustom', ['$rootScope',
        function ($rootScope) {

            var getStrRoleLevel = function (roleLevel) {
                let strRoleLevel = "";
                switch (roleLevel) {
                    case app.ROLE_LEVEL.CHUYEN_VIEN:
                        strRoleLevel = "Chuyên viên";
                        break
                    case app.ROLE_LEVEL.CHUYEN_GIA:
                        strRoleLevel = "Chuyên gia";
                        break;
                }
                return strRoleLevel;
            };

            var getStrTieuBan = function (tieuBanEnum) {
                let strTieuBan = "";
                switch (tieuBanEnum) {
                    case app.TIEU_BAN_TT3.TIEU_BAN_DIET_CON_TRUNG:
                        strTieuBan = "Tiểu ban diệt côn trùng";
                        break
                    case app.TIEU_BAN_TT3.TIEU_BAN_DIET_KHUAN:
                        strTieuBan = "Tiểu ban diệt khuẩn";
                        break;
                }
                return strTieuBan;
            };

            return {
                getStrRoleLevel: getStrRoleLevel,
                getStrTieuBan: getStrTieuBan
            };
        }
    ]);
})();